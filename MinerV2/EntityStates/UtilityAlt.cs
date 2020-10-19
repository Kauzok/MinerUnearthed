using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Miner
{
    public class CaveIn : BaseSkillState
    {
        public static float damageCoefficient = 0f;
        public float baseDuration = 0.25f;
        public static float blastRadius = 18f;
        public static float succForce = 2.4f;

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
                blastAttack.baseForce = 3f;
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
                                    Vector3 pushForce = (aimRay.origin - charb.corePosition) * CaveIn.succForce;
                                    var motor = charb.GetComponent<CharacterMotor>();
                                    var rb = charb.GetComponent<Rigidbody>();

                                    if (motor) pushForce *= motor.mass;
                                    else if (rb) pushForce *= rb.mass;

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

                Collider[] array = Physics.OverlapSphere(theSpot, CaveIn.blastRadius + 4f, LayerIndex.defaultLayer.mask);
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
}
