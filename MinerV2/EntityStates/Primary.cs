using RoR2;
using UnityEngine;
using System;

namespace EntityStates.Digger
{
    public class Crush : BaseSkillState
    {
        public static float damageCoefficient = DiggerPlugin.DiggerPlugin.crushDamage.Value;
        public float baseDuration = 0.69f;
        public static float attackRecoil = 0.75f;
        public static float hitHopVelocity = 5f;
        public static float styleCoefficient = 0.7f;

        private float duration;
        public GameObject explodePrefab = Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniExplosionVFX");
        public GameObject slashPrefab = Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniImpactVFXSlash");
        public GameObject swingPrefab = Resources.Load<GameObject>("prefabs/effects/lemurianbitetrail");
        private bool hasFired;
        private float hitPauseTimer;
        private OverlapAttack attack;
        private bool inHitPause;
        private bool hasHopped;
        private float stopwatch;
        private Animator animator;
        private BaseState.HitStopCachedState hitStopCachedState;
        //private StyleSystem.StyleComponent styleComponent;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = this.baseDuration / this.attackSpeedStat;
            this.hasFired = false;
            this.animator = base.GetModelAnimator();
            //this.styleComponent = base.GetComponent<StyleSystem.StyleComponent>();
            base.StartAimMode(0.5f + this.duration, false);
            base.PlayCrossfade("Gesture, Override", "Crush", "Crush.playbackRate", this.duration, 0.05f);

            HitBoxGroup hitBoxGroup = null;
            Transform modelTransform = base.GetModelTransform();

            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Crush");
            }

            this.attack = new OverlapAttack();
            this.attack.attacker = base.gameObject;
            this.attack.inflictor = base.gameObject;
            this.attack.teamIndex = base.GetTeam();
            this.attack.damage = Crush.damageCoefficient * this.damageStat;
            this.attack.procCoefficient = 1;
            this.attack.hitEffectPrefab = Loader.SwingChargedFist.overchargeImpactEffectPrefab;
            this.attack.forceVector = Vector3.zero;
            this.attack.pushAwayForce = 1f;
            this.attack.hitBoxGroup = hitBoxGroup;
            this.attack.isCrit = base.RollCrit();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public void FireAttack() 
        {

            float speedScale = 0.7f * (Mathf.Sqrt(3.5f * base.attackSpeedStat));
            float attackRadius = 1.1f * Mathf.Sqrt(speedScale - 0.31f);

            if (base.isAuthority)
            {

                base.GetModelChildLocator().FindChild("SwingCenter").transform.localScale = Vector3.one * attackRadius;

                if (this.attack.Fire()) 
                {
                    //if (this.styleComponent) this.styleComponent.AddStyle(Crush.styleCoefficient);

                    if (!this.hasHopped) 
                    {
                        if (base.characterMotor && !base.characterMotor.isGrounded) 
                        {
                            base.SmallHop(base.characterMotor, Crush.hitHopVelocity);
                        }

                        this.hasHopped = true;
                    }

                    if (!this.inHitPause) 
                    {
                        this.hitStopCachedState = base.CreateHitStopCachedState(base.characterMotor, this.animator, "Crush.playbackRate");
                        this.hitPauseTimer = (0.6f * Merc.GroundLight.hitPauseDuration) / this.attackSpeedStat;
                        this.inHitPause = true;
                    }
                }

            }


            if (!this.hasFired) 
            {
                this.hasFired = true;
                Util.PlaySound(DiggerPlugin.Sounds.Crush, base.gameObject);

                base.AddRecoil(-1f * Crush.attackRecoil * speedScale, 
                               -2f * Crush.attackRecoil * speedScale, 
                               -0.5f * Crush.attackRecoil * speedScale, 
                               0.5f * Crush.attackRecoil * speedScale); 
                if (base.isAuthority) 
                {
                    Ray aimRay = base.GetAimRay();

                    //float theta = Vector3.Angle(new Vector3(0, -1, 0), aimRay.direction);
                    //theta = Mathf.Min(theta, 90);
                    //Vector3 theSpot = aimRay.origin + ((1 + (theta / 30)) * aimRay.direction);

                    //nerdy math written by a nerd
                    //Vector2 move = new Vector2(characterMotor.moveDirection.x, characterMotor.moveDirection.z);
                    //Vector2 aim = new Vector2(aimRay.direction.x, aimRay.direction.z);
                    //float forward = Vector2.Dot(move, aim.normalized);
                    //Vector2 aimO = new Vector2(aimRay.direction.z, -1 * aimRay.direction.x);
                    //float right = Vector2.Dot(move, aimO.normalized);

                    //BlastAttack blastAttack = new BlastAttack();
                    //blastAttack.radius = attackRadius;
                    //blastAttack.procCoefficient = 1f;
                    //blastAttack.position = theSpot;
                    //blastAttack.attacker = base.gameObject;
                    //blastAttack.crit = this.RollCrit();
                    //blastAttack.baseDamage = base.characterBody.damage * Crush.damageCoefficient;
                    //blastAttack.falloffModel = BlastAttack.FalloffModel.SweetSpot;
                    //blastAttack.baseForce = 3f;
                    //blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
                    //blastAttack.damageType = DamageType.Generic;
                    //blastAttack.attackerFiltering = AttackerFiltering.NeverHit;
                    //BlastAttack.Result result = blastAttack.Fire();

                    Transform swingCenter = base.GetModelChildLocator().FindChild("CrushMuzzleExplosion").transform;

                    EffectData effectData = new EffectData();
                    effectData.origin = swingCenter.position;
                    effectData.scale = attackRadius * 2.69f;
                    EffectManager.SpawnEffect(explodePrefab, effectData, false);

                    effectData.scale = 0.1f;
                    Vector3 left = Vector3.Cross(aimRay.direction, Vector3.up).normalized;
                    effectData.origin = aimRay.origin - (0.5f * left);
                    EffectManager.SpawnEffect(swingPrefab, effectData, false);
                    effectData.origin = aimRay.origin + left;
                    EffectManager.SpawnEffect(swingPrefab, effectData, false);

                    //I have been defeated
                    //fuck you
                    //base.GetModelChildLocator().FindChild("CrushMuzzleExplosion").transform.localScale = Vector3.one * attackRadius;

                    //EffectManager.SimpleMuzzleFlash(DiggerPlugin.DiggerPlugin.crushExplosionEffect, base.gameObject, "CrushMuzzleExplosion", true);
                    //GameObject effectPrefab = DiggerPlugin.Assets.crushFX; 
                    //EffectManager.SimpleMuzzleFlash(effectPrefab, base.gameObject, "CrushMuzzleLeft", true);
                    //EffectManager.SimpleMuzzleFlash(effectPrefab, base.gameObject, "CrushMuzzleRight", true);

                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.hitPauseTimer -= Time.fixedDeltaTime;

            if (this.hitPauseTimer <= 0f && this.inHitPause)
            {
                base.ConsumeHitStopCachedState(this.hitStopCachedState, base.characterMotor, this.animator);
                this.inHitPause = false;
            }

            if (!this.inHitPause)
            {
                this.stopwatch += Time.fixedDeltaTime;
            }
            else
            {
                if (base.characterMotor) base.characterMotor.velocity = Vector3.zero;
                if (this.animator) this.animator.SetFloat("Crush.playbackRate", 0f);
            }

            if (this.stopwatch >= this.duration * 0.3f && (!this.hasFired || this.stopwatch <= this.duration * 0.59f))
            {
                this.FireAttack();
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
