using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Miner
{
    public class DrillBreak : BaseSkillState
    {
        public static float damageCoefficient = 8f;
        public float baseDuration = 0.4f;

        private float duration;
        private OverlapAttack attack;
        private StyleSystem.StyleComponent styleComponent;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = this.baseDuration;
            Ray aimRay = base.GetAimRay();
            this.styleComponent = base.GetComponent<StyleSystem.StyleComponent>();
            base.characterMotor.disableAirControlUntilCollision = false;

            base.PlayAnimation("FullBody, Override", "DrillChargeShort");

            if (NetworkServer.active) base.characterBody.AddBuff(BuffIndex.HiddenInvincibility);

            Util.PlaySound(MinerPlugin.Sounds.DrillCharge, base.gameObject);
            base.characterMotor.velocity += 75 * aimRay.direction;

            HitBoxGroup hitBoxGroup = null;
            Transform modelTransform = base.GetModelTransform();

            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Charge");
            }

            this.attack = new OverlapAttack();
            this.attack.attacker = base.gameObject;
            this.attack.inflictor = base.gameObject;
            this.attack.teamIndex = base.GetTeam();
            this.attack.damage = DrillBreak.damageCoefficient * this.damageStat;
            this.attack.procCoefficient = 1;
            this.attack.hitEffectPrefab = MinerPlugin.Assets.hitFX;
            this.attack.forceVector = Vector3.zero;
            this.attack.pushAwayForce = 1f;
            this.attack.hitBoxGroup = hitBoxGroup;
            this.attack.isCrit = base.RollCrit();

            EffectData effectData = new EffectData();
            effectData.origin = base.transform.position;
            effectData.scale = 8;

            EffectManager.SpawnEffect(MinerPlugin.MinerPlugin.backblastEffect, effectData, true);
        }

        public override void OnExit()
        {
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(BuffIndex.HiddenInvincibility);
                base.characterBody.AddTimedBuff(BuffIndex.HiddenInvincibility, 0.5f);
            }

            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                if (!base.characterMotor.disableAirControlUntilCollision) base.characterMotor.velocity *= 0.4f;
                base.PlayAnimation("FullBody, Override", "DrillChargeShortEnd");
                this.outer.SetNextStateToMain();
                return;
            }

            if (base.isAuthority)
            {
                if (this.attack.Fire())
                {
                    Util.PlayScaledSound(MinerPlugin.Sounds.Hit, base.gameObject, 0.5f);

                    base.characterMotor.velocity = Vector3.zero;

                    if (base.characterMotor && !base.characterMotor.isGrounded)
                    {
                        base.SmallHop(base.characterMotor, 16f);
                    }

                    base.PlayAnimation("FullBody, Override", "DrillChargeVertiEnd");

                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
