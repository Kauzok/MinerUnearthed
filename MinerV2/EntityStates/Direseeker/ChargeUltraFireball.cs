using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Direseeker
{
    public class ChargeUltraFireball : BaseState
    {
        public static float baseDuration = 2f;

        private float duration;
        private GameObject chargeInstance;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = ChargeUltraFireball.baseDuration / this.attackSpeedStat;
            UnityEngine.Object modelAnimator = base.GetModelAnimator();
            Transform modelTransform = base.GetModelTransform();
            Util.PlayAttackSpeedSound(EntityStates.LemurianBruiserMonster.ChargeMegaFireball.attackString, base.gameObject, this.attackSpeedStat);

            if (modelTransform)
            {
                ChildLocator component = modelTransform.GetComponent<ChildLocator>();
                if (component)
                {
                    Transform transform = component.FindChild("MuzzleMouth");
                    if (transform && EntityStates.LemurianBruiserMonster.ChargeMegaFireball.chargeEffectPrefab)
                    {
                        this.chargeInstance = UnityEngine.Object.Instantiate<GameObject>(EntityStates.LemurianBruiserMonster.ChargeMegaFireball.chargeEffectPrefab, transform.position, transform.rotation);
                        this.chargeInstance.transform.parent = transform;

                        ScaleParticleSystemDuration component2 = this.chargeInstance.GetComponent<ScaleParticleSystemDuration>();
                        if (component2)
                        {
                            component2.newDuration = this.duration;
                        }
                    }
                }
            }

            if (modelAnimator)
            {
                base.PlayCrossfade("Gesture, Additive", "ChargeMegaFireball", "ChargeMegaFireball.playbackRate", this.duration, 0.1f);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (this.chargeInstance)
            {
                EntityState.Destroy(this.chargeInstance);
            }
        }

        public override void Update()
        {
            base.Update();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                FireUltraFireball nextState = new FireUltraFireball();
                this.outer.SetNextState(nextState);
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
