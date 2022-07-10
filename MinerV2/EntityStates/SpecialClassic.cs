using EntityStates;
using RoR2;
using UnityEngine;
using KinematicCharacterController;

namespace EntityStates.Digger
{
    public class ToTheStarsClassic : BaseSkillState
    {
        public float baseDuration = 0.45f;
        public static float damageCoefficient = 3.6f;

        private float duration;
        public GameObject hitEffectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/impacteffects/MissileExplosionVFX");
        public GameObject tracerEffectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/tracers/tracerembers");
        public GameObject smokeEffectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashLoader");
        public GameObject flashEffectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashfire");

        private Quaternion major = Quaternion.FromToRotation(Vector3.forward, Vector3.down);

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = this.baseDuration;

            if (base.isAuthority)
            {
                base.characterMotor.velocity *= 0.2f;
                base.characterMotor.rootMotion.y += 0.5f;
                base.characterMotor.velocity.y = 25;

                base.gameObject.layer = LayerIndex.fakeActor.intVal;
                base.characterMotor.Motor.RebuildCollidableLayers();
            }

            base.PlayAnimation("FullBody, Override", "ToTheStarsStart", "ToTheStars.playbackRate", 1f);
            Util.PlayAttackSpeedSound("Play_commando_shift", base.gameObject, 1.8f);
        }

        public override void OnExit()
        {
            if (base.cameraTargetParams)
            {
                base.cameraTargetParams.fovOverride = -1f;
            }

            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();

                Vector3 aimer = Vector3.down;

                bool isCrit = base.RollCrit();
                BulletAttack bulletAttack = new BulletAttack
                {
                    owner = base.gameObject,
                    weapon = base.gameObject,
                    muzzleName = "Chest",
                    origin = aimRay.origin,
                    aimVector = aimer,
                    minSpread = 0f,
                    maxSpread = 0f,
                    radius = 0.7f,
                    bulletCount = 1U,
                    procCoefficient = 1f,
                    damage = base.characterBody.damage * ToTheStarsClassic.damageCoefficient,
                    force = 0f,
                    falloffModel = BulletAttack.FalloffModel.None,
                    tracerEffectPrefab = this.tracerEffectPrefab,
                    hitEffectPrefab = this.hitEffectPrefab,
                    isCrit = isCrit,
                    HitEffectNormal = false,
                    smartCollision = true,
                    maxDistance = 300f
                };
                bulletAttack.aimVector = Vector3.down;
                bulletAttack.Fire();

                Vector3 forwardDirection = aimRay.direction;
                forwardDirection.y = 0f;
                forwardDirection.Normalize();

                //Fire the edges of the star
                for (int i = 0; i < 5; i++)
                {

                }

                EffectData effectData = new EffectData();
                effectData.origin = aimRay.origin + (1 * Vector3.down);
                effectData.scale = 8;
                effectData.rotation = major;

                EffectManager.SpawnEffect(smokeEffectPrefab, effectData, true);
                effectData.scale = 16;
                EffectManager.SpawnEffect(flashEffectPrefab, effectData, true);
            }

            Util.PlaySound(DiggerPlugin.Sounds.ToTheStars, base.gameObject);

            base.gameObject.layer = LayerIndex.defaultLayer.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();

            base.PlayAnimation("FullBody, Override", "ToTheStarsEnd");

            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.cameraTargetParams)
            {
                base.cameraTargetParams.fovOverride = Mathf.Lerp(Commando.DodgeState.dodgeFOV, 60f, base.fixedAge / this.duration);
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
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
