using EntityStates;
using RoR2;
using UnityEngine;
using KinematicCharacterController;

namespace EntityStates.Digger
{
    public class ToTheStars : BaseSkillState
    {
        public float baseDuration = 0.45f;
        public static float damageCoefficient = 1.8f;   //was 0.9

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

                BulletAttack bulletAttack = new BulletAttack
                {
                    owner = base.gameObject,
                    weapon = base.gameObject,
                    muzzleName = "Chest",
                    origin = aimRay.origin,
                    aimVector = aimer,
                    minSpread = 0f,
                    maxSpread = base.characterBody.spreadBloomAngle,
                    radius = 0.5f,  //was 0.35
                    bulletCount = 1U,
                    procCoefficient = .5f,
                    damage = base.characterBody.damage * ToTheStars.damageCoefficient,
                    force = 3,
                    falloffModel = BulletAttack.FalloffModel.None,
                    tracerEffectPrefab = this.tracerEffectPrefab,
                    hitEffectPrefab = this.hitEffectPrefab,
                    isCrit = base.RollCrit(),
                    HitEffectNormal = false,
                    stopperMask = LayerIndex.world.mask,
                    smartCollision = true,
                    maxDistance = 300f
                };

                for (int j = 0; j < 3; j++)
                {
                    for (int i = 0; i <= 4; i++)    //was 9
                    {
                        float theta = Random.Range(0.0f, 6.28f);
                        float x = Mathf.Cos(theta);
                        float z = Mathf.Sin(theta);
                        float c = i * 0.3777f;
                        c *= (1f / 12f);
                        aimer.x += c * x;
                        aimer.z += c * z;
                        bulletAttack.aimVector = aimer;
                        bulletAttack.Fire();
                        aimer = Vector3.down;
                    }
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
