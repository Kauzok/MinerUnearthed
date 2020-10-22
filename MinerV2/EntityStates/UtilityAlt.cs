using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Miner
{
    public class CaveIn : BaseSkillState
    {
        public static float damageCoefficient = 0f;
        public float baseDuration = 0.15f;
        public static float blastRadius = 25f;
        public static float succForce = 8f;

        private float duration;
        private ChildLocator childLocator;

        public override void OnEnter()
        {
            base.OnEnter();
            Ray aimRay = base.GetAimRay();
            this.duration = this.baseDuration;
            this.childLocator = base.GetModelChildLocator();

            Util.PlaySound(MinerPlugin.Sounds.Backblast, base.gameObject);
            base.StartAimMode(0.8f, true);

            base.characterMotor.disableAirControlUntilCollision = false;

            base.PlayAnimation("FullBody, Override", "CaveIn");

            if (NetworkServer.active) base.characterBody.AddBuff(BuffIndex.HiddenInvincibility);

            if (base.isAuthority)
            {
                Vector3 theSpot = aimRay.origin + 2 * aimRay.direction;

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
                blastAttack.attackerFiltering = AttackerFiltering.NeverHit;
                blastAttack.Fire();

                EffectData effectData = new EffectData();
                effectData.origin = theSpot;
                effectData.scale = 15;

                EffectManager.SpawnEffect(MinerPlugin.MinerPlugin.backblastEffect, effectData, false);

                base.characterMotor.velocity = -80 * aimRay.direction;

                //succ
                if (NetworkServer.active)
                {
                    Collider[] array = Physics.OverlapSphere(theSpot, CaveIn.blastRadius, LayerIndex.defaultLayer.mask);
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
                                    Vector3 pushForce = (theSpot - charb.corePosition) * CaveIn.succForce;
                                    var motor = charb.GetComponent<CharacterMotor>();
                                    var rb = charb.GetComponent<Rigidbody>();

                                    float mass = 1;
                                    if (motor) mass = motor.mass;
                                    else if (rb) mass = rb.mass;
                                    if (mass < 1) mass = 1;

                                    pushForce *= mass;

                                    DamageInfo info = new DamageInfo
                                    {
                                        attacker = base.gameObject,
                                        inflictor = base.gameObject,
                                        damage = 0,
                                        damageColorIndex = DamageColorIndex.Default,
                                        damageType = DamageType.Generic,
                                        crit = false,
                                        dotIndex = DotController.DotIndex.None,
                                        force = pushForce,
                                        position = base.transform.position,
                                        procChainMask = default(ProcChainMask),
                                        procCoefficient = 0
                                    };

                                    charb.healthComponent.TakeDamageForce(info, true, true);
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void OnExit()
        {
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(BuffIndex.HiddenInvincibility);
                base.characterBody.AddTimedBuff(BuffIndex.HiddenInvincibility, 0.5f);

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
    }

    public class VaultState : BaseSkillState
    {
        public float baseDuration = 0.3f;
        private float duration;
        //public GameObject effectPrefab = Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniExplosionVFX");
        public GameObject slashPrefab = Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniImpactVFXSlash");

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
                base.characterBody.AddBuff(BuffIndex.HiddenInvincibility);

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
                blastAttack.attackerFiltering = AttackerFiltering.NeverHit;
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
            base.characterBody.RemoveBuff(BuffIndex.HiddenInvincibility);
            base.characterBody.AddTimedBuff(BuffIndex.HiddenInvincibility, 0.5f * 0.5f);

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
