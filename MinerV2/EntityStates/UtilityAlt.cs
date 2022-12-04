using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EntityStates.Digger
{
    public class CaveIn : BaseSkillState
    {
        public static GameObject effectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2SmokeBomb.prefab").WaitForCompletion();
        public static float damageCoefficient = 0f;
        public float baseDuration = 0.35f;
        public static float blastRadius = 25f;
        public static float succForce = 7f;

        private float duration;
        private ChildLocator childLocator;

        public override void OnEnter()
        {
            base.OnEnter();
            Ray aimRay = base.GetAimRay();
            this.duration = this.baseDuration;
            this.childLocator = base.GetModelChildLocator();

            Util.PlaySound(DiggerPlugin.Sounds.Backblast, base.gameObject);
            base.StartAimMode(0.8f, true);

            base.characterMotor.disableAirControlUntilCollision = false;

            base.PlayAnimation("FullBody, Override", "CaveIn");

            if (NetworkServer.active) base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);

            Vector3 theSpot = aimRay.origin + 2 * aimRay.direction;

            if (base.isAuthority)
            {

                BlastAttack blastAttack = new BlastAttack();
                blastAttack.radius = CaveIn.blastRadius;
                blastAttack.procCoefficient = 1f;
                blastAttack.position = theSpot;
                blastAttack.attacker = base.gameObject;
                blastAttack.crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
                blastAttack.baseDamage = base.characterBody.damage * CaveIn.damageCoefficient;
                blastAttack.falloffModel = BlastAttack.FalloffModel.None;
                blastAttack.baseForce = 0f;
                blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
                blastAttack.damageType = DamageType.Stun1s;
                blastAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;
                blastAttack.Fire();

                EffectData effectData = new EffectData();
                effectData.origin = theSpot;
                effectData.scale = 15;

                EffectManager.SpawnEffect(CaveIn.effectPrefab, effectData, false);

                base.characterMotor.velocity = -80 * aimRay.direction;

            }
            //succ
            if (NetworkServer.active)
            {
                RootPulse();
            }
        }

        //Based on EntityStates.TreeBot.TreebotFlower.TreebotFlower2Projectile.RootPulse
        private void RootPulse()
        {
            List<CharacterBody> rootedBodies = new List<CharacterBody>();
            Vector3 position = base.transform.position;
            foreach (HurtBox hurtBox in new SphereSearch
            {
                origin = position,
                radius = CaveIn.blastRadius,
                mask = LayerIndex.entityPrecise.mask
            }.RefreshCandidates().FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(base.GetTeam())).OrderCandidatesByDistance().FilterCandidatesByDistinctHurtBoxEntities().GetHurtBoxes())
            {
                CharacterBody body = hurtBox.healthComponent.body;
                if (!rootedBodies.Contains(body))
                {
                    rootedBodies.Add(body);
                    Vector3 a = hurtBox.transform.position - position;
                    float magnitude = a.magnitude;
                    Vector3 a2 = a / magnitude;
                    Rigidbody component = hurtBox.healthComponent.GetComponent<Rigidbody>();
                    float num = component ? component.mass : 1f;
                    float num2 = magnitude - 6f;    //REX yankIdealDistance = 6f
                    float num3 = EntityStates.Treebot.TreebotFlower.TreebotFlower2Projectile.yankSuitabilityCurve.Evaluate(num);
                    Vector3 vector = component ? component.velocity : Vector3.zero;
                    if (HGMath.IsVectorNaN(vector))
                    {
                        vector = Vector3.zero;
                    }
                    Vector3 a3 = -vector;
                    if (num2 > 0f)
                    {
                        a3 = a2 * -Trajectory.CalculateInitialYSpeedForHeight(num2, -body.acceleration);
                    }
                    Vector3 force = a3 * (num * num3);
                    DamageInfo damageInfo = new DamageInfo
                    {
                        attacker = base.gameObject,
                        inflictor = base.gameObject,
                        crit = false,
                        damage = 0f,
                        damageColorIndex = DamageColorIndex.Default,
                        damageType = DamageType.NonLethal | DamageType.Silent,
                        force = force,
                        position = hurtBox.transform.position,
                        procChainMask = default,
                        procCoefficient = 0f
                    };
                    hurtBox.healthComponent.TakeDamage(damageInfo);
                }
            }
        }

        public override void OnExit()
        {
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
                base.characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 0.5f);

                //no succ
                Ray aimRay = base.GetAimRay();
                Vector3 theSpot = aimRay.origin + 2 * aimRay.direction;

                Collider[] array = Physics.OverlapSphere(theSpot, CaveIn.blastRadius + 8f, LayerIndex.defaultLayer.mask);
                for (int i = 0; i < array.Length; i++)
                {
                    HealthComponent healthComponent = array[i].GetComponent<HealthComponent>();
                    if (healthComponent)
                    {
                        TeamComponent component2 = healthComponent.GetComponent<TeamComponent>();
                        if (component2.teamIndex != TeamIndex.Player)
                        {
                            var charb = healthComponent.body;
                            if (charb)
                            {
                                var motor = charb.characterMotor;
                                var rb = charb.rigidbody;

                                if (motor) motor.velocity *= 0.1f;
                                if (rb) rb.velocity *= 0.1f;
                            }
                        }
                    }
                }
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

    public class VaultState : BaseSkillState
    {
        public float baseDuration = 0.3f;
        private float duration;
        //public GameObject effectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/omnieffect/OmniExplosionVFX");
        public GameObject slashPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/omnieffect/OmniImpactVFXSlash");

        BlastAttack blastAttack;
        List<CharacterBody> victimBodyList = new List<CharacterBody>();
        Ray aimRay;
        public override void OnEnter()
        {
            base.OnEnter();
            aimRay = base.GetAimRay();
            this.duration = this.baseDuration;
            if (base.isAuthority)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);

                blastAttack = new BlastAttack();
                blastAttack.radius = 25f;
                blastAttack.procCoefficient = 1f;
                blastAttack.position = aimRay.origin;
                blastAttack.attacker = base.gameObject;
                blastAttack.crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
                blastAttack.baseDamage = 0.1f;
                blastAttack.falloffModel = BlastAttack.FalloffModel.None;
                blastAttack.baseForce = 3f;
                blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
                blastAttack.damageType = DamageType.Stun1s;
                blastAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;
                blastAttack.Fire();

                EffectData effectData = new EffectData();
                effectData.origin = aimRay.origin;
                effectData.scale = 15;

                EffectManager.SpawnEffect(slashPrefab, effectData, false);

                Util.PlaySound("Backblast", base.gameObject);

                getHitList(blastAttack);
                victimBodyList.ForEach(Suck);

                base.characterMotor.velocity = -60 * aimRay.direction;
            }
        }
        public override void OnExit()
        {
            base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
            base.characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 0.5f * 0.5f);

            blastAttack.radius = 5f;
            blastAttack.Fire();

            victimBodyList.Clear();
            getHitList(blastAttack);
            victimBodyList.ForEach(Stop);

            base.characterMotor.velocity.x *= 0.1f;
            base.characterMotor.velocity.y *= 0.1f;
            base.characterMotor.velocity.z *= 0.1f;

            base.OnExit();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if ((base.fixedAge >= this.duration && base.isAuthority))
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        private void getHitList(BlastAttack ba)
        {
            Collider[] array = Physics.OverlapSphere(ba.position, ba.radius, LayerIndex.defaultLayer.mask);
            int num = 0;
            int num2 = 0;
            while (num < array.Length && num2 < 12)
            {
                HealthComponent component = array[num].GetComponent<HealthComponent>();
                if (component)
                {
                    TeamComponent component2 = component.GetComponent<TeamComponent>();
                    if (component2.teamIndex != TeamIndex.Player)
                    {
                        this.AddToList(component.gameObject);
                        num2++;
                    }
                }
                num++;
            }
        }

        private void AddToList(GameObject affectedObject)
        {
            CharacterBody component = affectedObject.GetComponent<CharacterBody>();
            if (!this.victimBodyList.Contains(component))
            {
                this.victimBodyList.Add(component);
            }
        }

        void Suck(CharacterBody charb)
        {
            if (charb.characterMotor)
            {
                charb.characterMotor.velocity += (aimRay.origin - charb.corePosition) * 3;
            }
            else
            {
                Rigidbody component2 = charb.GetComponent<Rigidbody>();
                if (component2)
                {
                    component2.velocity += (aimRay.origin - charb.corePosition) * 3;
                }
            }
        }

        void Stop(CharacterBody charb)
        {
            if (charb.characterMotor)
            {
                charb.characterMotor.velocity *= 0.1f;
            }
            else
            {
                Rigidbody component2 = charb.GetComponent<Rigidbody>();
                if (component2)
                {
                    component2.velocity *= 0.1f;
                }
            }

        }
    }
}
