using MinerPlugin;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Direseeker
{
    public class FireUltraFireball : BaseState
    {
        public static int projectileCount = 15;
        public static float totalYawSpread = 25f;
        public static float baseDuration = 1.5f;
        public static float baseFireDuration = 0.25f;
        public static float damageCoefficient = 2.5f;
        public static float projectileSpeed = 80f;
        public static float force = 1200f;

        private float duration;
        private float fireDuration;
        private int projectilesFired;
        private DireseekerController direController;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = FireUltraFireball.baseDuration / this.attackSpeedStat;
            this.fireDuration = FireUltraFireball.baseFireDuration / this.attackSpeedStat;
            this.direController = base.GetComponent<DireseekerController>();

            if (this.direController) this.direController.FlameBurst();

            base.PlayAnimation("Gesture, Additive", "FireMegaFireball", "FireMegaFireball.playbackRate", this.duration);
            Util.PlaySound(EntityStates.LemurianBruiserMonster.FireMegaFireball.attackString, base.gameObject);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            string muzzleName = "MuzzleMouth";

            if (base.isAuthority)
            {
                int num = Mathf.FloorToInt(base.fixedAge / this.fireDuration * (float)FireUltraFireball.projectileCount);

                if (this.projectilesFired <= num && this.projectilesFired < FireUltraFireball.projectileCount)
                {
                    if (EntityStates.LemurianBruiserMonster.FireMegaFireball.muzzleflashEffectPrefab)
                    {
                        EffectManager.SimpleMuzzleFlash(EntityStates.LemurianBruiserMonster.FireMegaFireball.muzzleflashEffectPrefab, base.gameObject, muzzleName, false);
                    }

                    Ray aimRay = base.GetAimRay();

                    float speedOverride = FireUltraFireball.projectileSpeed;
                    float bonusYaw = (float)Mathf.FloorToInt((float)this.projectilesFired - (float)(FireUltraFireball.projectileCount - 1) / 2f) / (float)(FireUltraFireball.projectileCount - 1) * FireUltraFireball.totalYawSpread;

                    Vector3 forward = Util.ApplySpread(aimRay.direction, 0f, 0f, 1f, 1f, bonusYaw, 0f);

                    ProjectileManager.instance.FireProjectile(MinerPlugin.Direseeker.fireballPrefab, aimRay.origin, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * FireUltraFireball.damageCoefficient, FireUltraFireball.force, base.RollCrit(), DamageColorIndex.Default, null, speedOverride);
                    this.projectilesFired++;
                }
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
