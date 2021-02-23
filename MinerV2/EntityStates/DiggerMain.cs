using DiggerPlugin;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Digger
{
    public class DiggerMain : GenericCharacterMain
    {
        //public static float passiveStyleCoefficient = 0.4f;
        public static float maxEmission = 25f;
        public static float minEmission = 0f;

        private float adrenalineGainBuffer;
        private bool isMainSkin;
        private float adrenalineCap;
        private Material bodyMat;
        //private StyleSystem.StyleComponent styleComponent;
        private Animator animator;
        private AdrenalineParticleTimer adrenalineParticles;
        private float adrenalineSmooth;
        private int moneyTracker;
        private float residue;
        private int buffCounter;
        private float[] secretTimers = new float[6];

        public static event Action<float> rallypoint = delegate { };
        public static event Action<Run> SecretAchieved = delegate { };
        public static event Action<bool> JunkieAchieved = delegate { };
        private bool gotJunkie;

        public LocalUser localUser;

        public override void OnEnter()
        {
            base.OnEnter();
            this.isMainSkin = false;
            this.animator = base.GetModelAnimator();
            this.adrenalineGainBuffer = 0.3f;
            this.moneyTracker = (int)base.characterBody.master.money;
            this.gotJunkie = false;
            this.localUser = LocalUserManager.readOnlyLocalUsersList[0];

            if (base.characterBody)
            {
                Transform modelTransform = base.GetModelTransform();
                if (modelTransform)
                {
                    this.bodyMat = modelTransform.GetComponent<CharacterModel>().baseRendererInfos[0].defaultMaterial;
                }

                if (base.characterBody.skinIndex == 0) this.isMainSkin = true;
                else if (base.characterBody.skinIndex == 2) this.isMainSkin = true;
                else if (base.characterBody.skinIndex == 4) this.isMainSkin = true;
            }

            this.adrenalineCap = DiggerPlugin.DiggerPlugin.adrenalineCap;
            //this.styleComponent = base.GetComponent<StyleSystem.StyleComponent>();
            this.adrenalineParticles = base.GetComponent<AdrenalineParticleTimer>();

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "frozenwall")
            {
                rallypoint?.Invoke(Run.instance.time);
            }
        }

        public override void Update()
        {
            base.Update();

            if (base.isAuthority && base.characterMotor.isGrounded)
            {
                if (!this.localUser.isUIFocused)
                {
                    if (Input.GetKeyDown(DiggerPlugin.DiggerPlugin.restKeybind.Value))
                    {
                        this.outer.SetInterruptState(EntityState.Instantiate(new SerializableEntityStateType(typeof(Rest))), InterruptPriority.Any);
                        return;
                    }
                    else if (Input.GetKeyDown(DiggerPlugin.DiggerPlugin.tauntKeybind.Value))
                    {
                        this.outer.SetInterruptState(EntityState.Instantiate(new SerializableEntityStateType(typeof(Taunt))), InterruptPriority.Any);
                        return;
                    }
                    else if (Input.GetKeyDown(DiggerPlugin.DiggerPlugin.jokeKeybind.Value))
                    {
                        this.outer.SetInterruptState(EntityState.Instantiate(new SerializableEntityStateType(typeof(Joke))), InterruptPriority.Any);
                        return;
                    }
                }
            }

            if (base.isAuthority)
            {
                this.HandleSecret();
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.adrenalineGainBuffer -= Time.fixedDeltaTime;
            if (this.adrenalineGainBuffer <= 0 && NetworkServer.active) this.UpdatePassiveBuff();
            else this.buffCounter = base.GetBuffCount(DiggerPlugin.DiggerPlugin.goldRush);

            if (this.animator)
            {
                this.adrenalineSmooth = Mathf.Lerp(this.adrenalineSmooth, this.buffCounter, 1.5f * Time.fixedDeltaTime);
                this.animator.SetFloat("adrenaline", Util.Remap(this.adrenalineSmooth, 0, this.adrenalineCap, 0, 1));
            }

            if (!this.gotJunkie)
            {
                if (this.buffCounter >= this.adrenalineCap)
                {
                    if (JunkieAchieved != null)
                    {
                        this.gotJunkie = true;
                        JunkieAchieved(true);
                    }
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void UpdatePassiveBuff()
        {
            int currentCount = base.characterBody.GetBuffCount(DiggerPlugin.DiggerPlugin.goldRush);
            int newMoney = (int)base.characterBody.master.money;

            if (this.moneyTracker < newMoney)
            {
                this.RefreshExistingStacks(currentCount);
                this.GiveNewStacks(newMoney);
            }
            this.moneyTracker = newMoney;

            this.HandleStackDecay(currentCount);
        }

        private void RefreshExistingStacks(int currentCount)
        {
            base.characterBody.ClearTimedBuffs(DiggerPlugin.DiggerPlugin.goldRush);
            for (int i = 0; i < currentCount; i++)
            {
                if (base.characterBody.GetBuffCount(DiggerPlugin.DiggerPlugin.goldRush) <= this.adrenalineCap) base.characterBody.AddTimedBuff(DiggerPlugin.DiggerPlugin.goldRush, 5);
            }
        }

        private void GiveNewStacks(int newMoney)
        {
            float baseReward = (newMoney - this.moneyTracker) / Mathf.Pow(Run.instance.difficultyCoefficient, 1.25f);
            this.residue = (baseReward + this.residue) % 5;
            float numStacks = (baseReward + this.residue) / 5;

            for (int i = 1; i <= numStacks; i++)
            {
                if (base.characterBody.GetBuffCount(DiggerPlugin.DiggerPlugin.goldRush) <= this.adrenalineCap) base.characterBody.AddTimedBuff(DiggerPlugin.DiggerPlugin.goldRush, 5);
            }

            //if (this.styleComponent) this.styleComponent.AddStyle(DiggerMain.passiveStyleCoefficient);

            if (this.adrenalineParticles)
            {
                adrenalineParticles.updateAdrenaline(numStacks, true);
            }

            this.UpdateEmission();
        }

        private void HandleStackDecay(int currentCount)
        {
            if (this.buffCounter != 0 && currentCount == 0)
            {
                for (int i = 1; i < buffCounter * .5; i++)
                {
                    if (base.characterBody.GetBuffCount(DiggerPlugin.DiggerPlugin.goldRush) <= this.adrenalineCap) base.characterBody.AddTimedBuff(DiggerPlugin.DiggerPlugin.goldRush, 1);
                }
            }

            this.buffCounter = base.characterBody.GetBuffCount(DiggerPlugin.DiggerPlugin.goldRush);

            if (this.adrenalineParticles)
            {
                adrenalineParticles.updateAdrenaline(buffCounter);
            }

            this.UpdateEmission();
        }

        private void UpdateEmission()
        {
            if (this.isMainSkin && this.bodyMat)
            {
                float emValue = Util.Remap(this.adrenalineSmooth, 0, this.adrenalineCap, DiggerMain.minEmission, DiggerMain.maxEmission);
                Color emColor = Color.white;

                if (this.adrenalineSmooth <= 0.5f * this.adrenalineCap)
                {
                    float colorValue = Util.Remap(this.adrenalineSmooth, 0, 0.5f * this.adrenalineCap, 0f, 1f);
                    emColor = new Color(colorValue, colorValue, colorValue);
                }
                else
                {
                    float startValue = this.adrenalineCap * 0.5f;
                    float colorValue = Util.Remap(this.adrenalineSmooth - startValue, 0, this.adrenalineCap - startValue, 1f, 0f);
                    emColor = new Color(1, colorValue, colorValue);
                }

                this.bodyMat.SetFloat("_EmPower", emValue);
                this.bodyMat.SetColor("_EmColor", emColor);
            }
        }

        private void HandleSecret()
        {
            if (Input.GetKeyDown(KeyCode.G)) { secretTimers[0] = Time.fixedTime; }
            if (Input.GetKeyDown(KeyCode.N)) { secretTimers[1] = Time.fixedTime; }
            if (Input.GetKeyDown(KeyCode.O)) { secretTimers[2] = Time.fixedTime; }
            if (Input.GetKeyDown(KeyCode.M)) { secretTimers[3] = Time.fixedTime; }
            if (Input.GetKeyDown(KeyCode.E)) { secretTimers[4] = Time.fixedTime; }

            bool successful = true;
            for (int i = 0; i < 5; i++)
            {
                successful = successful && (Time.fixedTime - secretTimers[i]) < 0.5;
            }

            if (successful && (Time.fixedTime - secretTimers[5]) > 3)
            {
                Chat.AddMessage("Gnome loves you");

                SecretAchieved(Run.instance);

                secretTimers[5] = Time.fixedTime;
            }
        }
    }
}