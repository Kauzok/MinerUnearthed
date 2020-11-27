using MinerPlugin;
using RoR2;
using RoR2.Skills;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Miner
{
    public class MinerMain : GenericCharacterMain
    {
        //public static float passiveStyleCoefficient = 0.4f;
        public static float maxEmission = 25f;
        public static float minEmission = 0f;

        private bool isMainSkin;
        private float adrenalineCap;
        private Material bodyMat;
        //private StyleSystem.StyleComponent styleComponent;
        private Animator animator;
        private AdrenalineParticleTimer adrenalineParticles;
        private int moneyTracker;
        private float residue;
        private int buffCounter;

        public override void OnEnter()
        {
            base.OnEnter();
            this.isMainSkin = false;
            this.animator = base.GetModelAnimator();

            if (base.characterBody)
            {
                Transform modelTransform = base.GetModelTransform();
                if (modelTransform)
                {
                    this.bodyMat = modelTransform.GetComponent<CharacterModel>().baseRendererInfos[0].defaultMaterial;
                }

                if (base.characterBody.skinIndex == 0) this.isMainSkin = true;
            }

            this.adrenalineCap = MinerPlugin.MinerPlugin.adrenalineCap;
            //this.styleComponent = base.GetComponent<StyleSystem.StyleComponent>();
            this.adrenalineParticles = base.GetComponent<AdrenalineParticleTimer>();
        }

        public override void Update()
        {
            base.Update();

            if (base.isAuthority && base.characterMotor.isGrounded)
            {
                if (Input.GetKeyDown(MinerPlugin.MinerPlugin.restKeybind.Value))
                {
                    this.outer.SetInterruptState(EntityState.Instantiate(new SerializableEntityStateType(typeof(Rest))), InterruptPriority.Any);
                    return;
                }
                else if (Input.GetKeyDown(MinerPlugin.MinerPlugin.tauntKeybind.Value))
                {
                    this.outer.SetInterruptState(EntityState.Instantiate(new SerializableEntityStateType(typeof(Taunt))), InterruptPriority.Any);
                    return;
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (NetworkServer.active) this.UpdatePassiveBuff();

            this.animator.SetFloat("adrenaline", Util.Remap(base.GetBuffCount(MinerPlugin.MinerPlugin.goldRush), 0, adrenalineCap, 0, 1));
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void UpdatePassiveBuff()
        {
            int currentCount = base.characterBody.GetBuffCount(MinerPlugin.MinerPlugin.goldRush);
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
            base.characterBody.ClearTimedBuffs(MinerPlugin.MinerPlugin.goldRush);
            for (int i = 0; i < currentCount; i++)
            {
                if (base.characterBody.GetBuffCount(MinerPlugin.MinerPlugin.goldRush) <= this.adrenalineCap) base.characterBody.AddTimedBuff(MinerPlugin.MinerPlugin.goldRush, 5);
            }
        }

        private void GiveNewStacks(int newMoney)
        {
            float baseReward = (newMoney - this.moneyTracker) / Mathf.Pow(Run.instance.difficultyCoefficient, 1.25f);
            this.residue = (baseReward + this.residue) % 5;
            float numStacks = (baseReward + this.residue) / 5;

            for (int i = 1; i <= numStacks; i++)
            {
                if (base.characterBody.GetBuffCount(MinerPlugin.MinerPlugin.goldRush) <= this.adrenalineCap) base.characterBody.AddTimedBuff(MinerPlugin.MinerPlugin.goldRush, 5);
            }

            //if (this.styleComponent) this.styleComponent.AddStyle(MinerMain.passiveStyleCoefficient);

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
                    if (base.characterBody.GetBuffCount(MinerPlugin.MinerPlugin.goldRush) <= this.adrenalineCap) base.characterBody.AddTimedBuff(MinerPlugin.MinerPlugin.goldRush, 1);
                }
            }

            this.buffCounter = base.characterBody.GetBuffCount(MinerPlugin.MinerPlugin.goldRush);

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
                float emValue = Util.Remap(this.buffCounter, 0, this.adrenalineCap, MinerMain.minEmission, MinerMain.maxEmission);
                Color emColor = Color.white;

                if (this.buffCounter <= 0.5f * this.adrenalineCap)
                {
                    float colorValue = Util.Remap(this.buffCounter, 0, 0.5f * this.adrenalineCap, 0f, 1f);
                    emColor = new Color(colorValue, colorValue, colorValue);
                }
                else
                {
                    float startValue = this.adrenalineCap * 0.5f;
                    float colorValue = Util.Remap(this.buffCounter, 0, this.adrenalineCap - startValue, 1f, 0f);
                    emColor = new Color(1, colorValue, colorValue);
                }

                this.bodyMat.SetFloat("_EmPower", emValue);
                this.bodyMat.SetColor("_EmColor", emColor);
            }
        }
    }
}