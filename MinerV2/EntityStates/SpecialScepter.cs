using EntityStates;
using RoR2;
using UnityEngine;
using KinematicCharacterController;
using UnityEngine.Networking;

namespace EntityStates.Digger
{
    public class FallingComet : BaseSkillState
    {
        public float baseDuration = 0.45f;
        public static float damageCoefficient = 0.9f;
        public static float blastDamageCoefficient = 15f;

        private bool hasFallen;
        private float duration;
        public GameObject hitEffectPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/MissileExplosionVFX");
        public GameObject tracerEffectPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerEmbers");
        public GameObject smokeEffectPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/MuzzleFlashes/MuzzleFlashLoader");
        public GameObject flashEffectPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/MuzzleFlashes/MuzzleFlashFire");

        private Quaternion major = Quaternion.FromToRotation(Vector3.forward, Vector3.down);
        private ParticleSystem cometParticle;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = this.baseDuration;
            this.hasFallen = false;
            this.cometParticle = base.GetModelChildLocator().FindChild("CometEffect").GetComponentInChildren<ParticleSystem>();

            base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;

            if (base.isAuthority)
            {
                base.characterMotor.velocity *= 0.2f;
                base.characterMotor.rootMotion.y += 0.5f;
                base.characterMotor.velocity.y = 35;

                base.gameObject.layer = LayerIndex.fakeActor.intVal;
                base.characterMotor.Motor.RebuildCollidableLayers();
            }

            Transform modelTransform = base.GetModelTransform();
            if (modelTransform)
            {
                TemporaryOverlay temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = this.duration;
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matDoppelganger");
                temporaryOverlay.AddToCharacerModel(modelTransform.GetComponent<CharacterModel>());
            }

            base.PlayAnimation("FullBody, Override", "FallingCometStart", "ToTheStars.playbackRate", 1f);
            Util.PlayAttackSpeedSound(Croco.Leap.leapSoundString, base.gameObject, 0.75f);
        }

        public override void OnExit()
        {
            if (base.cameraTargetParams)
            {
                base.cameraTargetParams.fovOverride = -1f;
            }

            this.FireBlast();

            if (this.cometParticle) this.cometParticle.Stop();

            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;

            base.characterMotor.velocity *= 0.1f;

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
                this.FireProjectiles();

                base.characterMotor.velocity.y = -100f;
            }

            if (base.fixedAge >= this.duration && base.isAuthority && base.isGrounded)
            {
                this.FireBlast();
                this.outer.SetNextStateToMain();
                return;
            }
        }

        private void FireBlast()
        {
            base.PlayAnimation("FullBody, Override", "BufferEmpty");
            Util.PlayAttackSpeedSound(DiggerPlugin.Sounds.ToTheStarsExplosion, base.gameObject, 0.5f);

            if (base.isAuthority)
            {
                BlastAttack blastAttack = new BlastAttack();
                blastAttack.radius = 20f;
                blastAttack.procCoefficient = 1f;
                blastAttack.position = base.characterBody.footPosition;
                blastAttack.attacker = base.gameObject;
                blastAttack.crit = base.RollCrit();
                blastAttack.baseDamage = base.characterBody.damage * FallingComet.blastDamageCoefficient;
                blastAttack.falloffModel = BlastAttack.FalloffModel.Linear;
                blastAttack.baseForce = 2000f;
                blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
                blastAttack.damageType = DamageType.PercentIgniteOnHit;
                blastAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;
                blastAttack.Fire();

                EffectData effectData = new EffectData();
                effectData.origin = base.characterBody.footPosition;
                effectData.scale = 20;

                EffectManager.SpawnEffect(DiggerPlugin.DiggerPlugin.backblastEffect, effectData, false);
            }
        }

        private void FireProjectiles()
        {
            if (!this.hasFallen)
            {
                this.hasFallen = true;

                if (this.cometParticle) this.cometParticle.Play();

                Util.PlayAttackSpeedSound(DiggerPlugin.Sounds.ToTheStars, base.gameObject, 0.5f);

                base.gameObject.layer = LayerIndex.defaultLayer.intVal;
                base.characterMotor.Motor.RebuildCollidableLayers();

                base.PlayAnimation("FullBody, Override", "FallingCometEnd");

                Transform modelTransform = base.GetModelTransform();
                if (modelTransform)
                {
                    TemporaryOverlay temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay.duration = 2.5f;
                    temporaryOverlay.animateShaderAlpha = true;
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlay.destroyComponentOnEnd = true;
                    temporaryOverlay.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matOnFire");
                    temporaryOverlay.AddToCharacerModel(modelTransform.GetComponent<CharacterModel>());
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
                        radius = 0.35f,
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
                        for (int i = 0; i <= 9; i++)
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
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
