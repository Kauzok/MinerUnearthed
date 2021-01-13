using EntityStates;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Digger
{
    public class BackBlast : BaseSkillState
    {
        public static float damageCoefficient = 6f;
        public float baseDuration = 0.35f;
        private float duration;
        public static event Action<int> Compacted;

        public override void OnEnter()
        {
            base.OnEnter();
            Ray aimRay = base.GetAimRay();
            this.duration = this.baseDuration;

            Util.PlaySound(DiggerPlugin.Sounds.Backblast, base.gameObject);
            base.StartAimMode(0.6f, true);

            base.characterMotor.disableAirControlUntilCollision = false;

            float angle = Vector3.Angle(new Vector3(0, -1, 0), aimRay.direction);
            if (angle < 60)
            {
                base.PlayAnimation("FullBody, Override", "BackblastUp");
            }
            else if (angle > 120)
            {
                base.PlayAnimation("FullBody, Override", "BackblastDown");
            }
            else
            {
                base.PlayAnimation("FullBody, Override", "Backblast");
            }

            if (NetworkServer.active) base.characterBody.AddBuff(BuffIndex.HiddenInvincibility);

            if (base.isAuthority)
            {
                Vector3 theSpot = aimRay.origin + 2 * aimRay.direction;

                BlastAttack blastAttack = new BlastAttack();
                blastAttack.radius = 14f;
                blastAttack.procCoefficient = 1f;
                blastAttack.position = theSpot;
                blastAttack.attacker = base.gameObject;
                blastAttack.crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
                blastAttack.baseDamage = base.characterBody.damage * BackBlast.damageCoefficient;
                blastAttack.falloffModel = BlastAttack.FalloffModel.None;
                blastAttack.baseForce = 500f;
                blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
                blastAttack.damageType = DamageType.Stun1s;
                blastAttack.attackerFiltering = AttackerFiltering.NeverHit;
                BlastAttack.Result result = blastAttack.Fire();

                EffectData effectData = new EffectData();
                effectData.origin = theSpot;
                effectData.scale = 15;

                EffectManager.SpawnEffect(DiggerPlugin.DiggerPlugin.backblastEffect, effectData, false);

                base.characterMotor.velocity = -80 * aimRay.direction;

                Compacted?.Invoke(result.hitCount);
            }
        }

        public override void OnExit()
        {
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(BuffIndex.HiddenInvincibility);
                base.characterBody.AddTimedBuff(BuffIndex.HiddenInvincibility, 0.5f);
            }

            base.characterMotor.velocity *= 0.1f;

            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if ((base.fixedAge >= this.duration && base.isAuthority) || (!base.IsKeyDownAuthority()))
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
