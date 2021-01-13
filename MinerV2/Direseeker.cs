using EntityStates;
using KinematicCharacterController;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Navigation;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace DiggerPlugin
{
    public static class Direseeker
    {
        public static GameObject bodyPrefab;
        public static GameObject survivorPrefab;
        public static GameObject masterPrefab;

        public static GameObject fireballPrefab;
        public static GameObject fireballGroundPrefab;
        public static GameObject fireTrailPrefab;
        public static GameObject fireSegmentPrefab;

        public static void PerroGrande()
        {
            GameObject fatAcridPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterBodies/CrocoBody"), "DireseekerBody");

            CharacterBody bodyComponent = fatAcridPrefab.GetComponent<CharacterBody>();

            LanguageAPI.Add("FATACRID_BODY_NAME", "Perro Grande");
            LanguageAPI.Add("FATACRID_BODY_SUBTITLE", "(Forma Gigante)");

            bodyComponent.name = "FatAcridBody";
            bodyComponent.baseNameToken = "FATACRID_BODY_NAME";
            bodyComponent.subtitleNameToken = "FATACRID_BODY_SUBTITLE";
            bodyComponent.baseMoveSpeed = 3f;
            bodyComponent.baseMaxHealth = 3800f;
            bodyComponent.levelMaxHealth = 1140f;
            bodyComponent.baseDamage = 4f;
            bodyComponent.levelDamage = 0.8f;
            bodyComponent.isChampion = true;

            //resize

            fatAcridPrefab.GetComponent<ModelLocator>().modelBaseTransform.gameObject.transform.localScale *= 6f;
            //fatAcridPrefab.GetComponent<ModelLocator>().modelBaseTransform.gameObject.transform.Translate(new Vector3(0f, 5.6f, 0f));

            foreach (KinematicCharacterMotor kinematicCharacterMotor in fatAcridPrefab.GetComponentsInChildren<KinematicCharacterMotor>())
            {
                kinematicCharacterMotor.SetCapsuleDimensions(kinematicCharacterMotor.Capsule.radius * 6f, kinematicCharacterMotor.Capsule.height * 6f, 6f);
            }

            //

            CharacterModel model = fatAcridPrefab.GetComponentInChildren<CharacterModel>();

            Material newMat = UnityEngine.Object.Instantiate<Material>(model.baseRendererInfos[0].defaultMaterial);
            newMat.SetTexture("_MainTex", Assets.mainAssetBundle.LoadAsset<Material>("matDireseeker").GetTexture("_MainTex"));
            newMat.SetTexture("_EmTex", Assets.mainAssetBundle.LoadAsset<Material>("matDireseeker").GetTexture("_EmissionMap"));
            newMat.SetFloat("_EmPower", 80f);

            model.baseRendererInfos[0].defaultMaterial = newMat;

            GameObject acridMasterPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterMasters/LemurianBruiserMaster"), "FatAcridMaster");

            CharacterMaster master = acridMasterPrefab.GetComponent<CharacterMaster>();

            master.bodyPrefab = fatAcridPrefab;
            master.isBoss = true;

            BodyCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(fatAcridPrefab);
            };

            MasterCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(acridMasterPrefab);
            };

            CharacterSpawnCard characterSpawnCard = ScriptableObject.CreateInstance<CharacterSpawnCard>();
            characterSpawnCard.name = "cscFatAcrid";
            characterSpawnCard.prefab = acridMasterPrefab;
            characterSpawnCard.sendOverNetwork = true;
            characterSpawnCard.hullSize = HullClassification.BeetleQueen;
            characterSpawnCard.nodeGraphType = MapNodeGroup.GraphType.Ground;
            characterSpawnCard.requiredFlags = NodeFlags.None;
            characterSpawnCard.forbiddenFlags = NodeFlags.TeleporterOK;
            characterSpawnCard.directorCreditCost = 2000;
            characterSpawnCard.occupyPosition = false;
            characterSpawnCard.loadout = new SerializableLoadout();
            characterSpawnCard.noElites = true;
            characterSpawnCard.forbiddenAsBoss = false;

            DirectorCard card = new DirectorCard
            {
                spawnCard = characterSpawnCard,
                selectionWeight = 1,
                allowAmbushSpawn = false,
                preventOverhead = false,
                minimumStageCompletions = 5,
                requiredUnlockable = "",
                forbiddenUnlockable = "",
                spawnDistance = DirectorCore.MonsterSpawnDistance.Close
            };

            DirectorAPI.DirectorCardHolder fatAcridCard = new DirectorAPI.DirectorCardHolder
            {
                Card = card,
                MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                InteractableCategory = DirectorAPI.InteractableCategory.None
            };

            DirectorAPI.MonsterActions += delegate (List<DirectorAPI.DirectorCardHolder> list, DirectorAPI.StageInfo stage)
            {
                if (stage.stage == DirectorAPI.Stage.AbandonedAqueduct || stage.stage == DirectorAPI.Stage.DistantRoost || stage.stage == DirectorAPI.Stage.RallypointDelta || stage.stage == DirectorAPI.Stage.SkyMeadow || stage.stage == DirectorAPI.Stage.VoidCell || stage.stage == DirectorAPI.Stage.WetlandAspect)
                {
                    if (!list.Contains(fatAcridCard))
                    {
                        list.Add(fatAcridCard);
                    }
                }
            };
        }

        public static void CreateProjectiles()
        {
            fireballPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/ArchWispCannon"), "DireseekerFireball", true);
            fireballGroundPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/ArchWispGroundCannon"), "DireseekerGroundFireball", true);

            var controller = fireballPrefab.GetComponent<ProjectileController>();
            controller.ghostPrefab = Resources.Load<GameObject>("Prefabs/Projectiles/LemurianBigFireball").GetComponent<ProjectileController>().ghostPrefab;
            controller.startSound = "Play_lemurianBruiser_m1_shoot";

            GameObject explosion = Resources.Load<GameObject>("Prefabs/Projectiles/LemurianBigFireball").GetComponent<ProjectileImpactExplosion>().impactEffect;

            var impact = fireballPrefab.GetComponent<ProjectileImpactExplosion>();
            impact.childrenProjectilePrefab = fireballGroundPrefab;
            impact.GetComponent<ProjectileImpactExplosion>().impactEffect = explosion;
            impact.falloffModel = BlastAttack.FalloffModel.SweetSpot;
            impact.blastDamageCoefficient = 1f;
            impact.blastProcCoefficient = 1f;

            fireballGroundPrefab.GetComponent<ProjectileController>().ghostPrefab = Resources.Load<GameObject>("Prefabs/Projectiles/MagmaOrbProjectile").GetComponent<ProjectileController>().ghostPrefab;
            fireballGroundPrefab.GetComponent<ProjectileImpactExplosion>().impactEffect = explosion;

            fireTrailPrefab = PrefabAPI.InstantiateClone(fireballGroundPrefab.GetComponent<ProjectileDamageTrail>().trailPrefab, "DireseekerFireTrail", true);
            fireTrailPrefab.AddComponent<NetworkIdentity>();

            fireSegmentPrefab = PrefabAPI.InstantiateClone(fireTrailPrefab.GetComponent<DamageTrail>().segmentPrefab, "DireseekerFireSegment", true);
            fireSegmentPrefab.AddComponent<NetworkIdentity>();

            fireSegmentPrefab.GetComponent<ParticleSystemRenderer>().material = explosion.transform.GetChild(9).GetComponent<ParticleSystemRenderer>().material;
            fireTrailPrefab.GetComponent<DamageTrail>().segmentPrefab = fireSegmentPrefab;
            fireballGroundPrefab.GetComponent<ProjectileDamageTrail>().trailPrefab = fireTrailPrefab;

            ProjectileCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(fireballPrefab);
                list.Add(fireballGroundPrefab);
            };
        }

        public static void SkillSetup(GameObject sx)
        {
            foreach (GenericSkill obj in sx.GetComponentsInChildren<GenericSkill>())
            {
                DiggerPlugin.DestroyImmediate(obj);
            }

            SkillLocator skillLocator = sx.GetComponentInChildren<SkillLocator>();

            SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            mySkillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.Direseeker.ChargeUltraFireball));
            mySkillDef.activationStateMachineName = "Weapon";
            mySkillDef.baseMaxStock = 2;
            mySkillDef.baseRechargeInterval = 8f;
            mySkillDef.beginSkillCooldownOnSkillEnd = true;
            mySkillDef.canceledFromSprinting = false;
            mySkillDef.fullRestockOnAssign = true;
            mySkillDef.interruptPriority = InterruptPriority.Any;
            mySkillDef.isBullets = false;
            mySkillDef.isCombatSkill = true;
            mySkillDef.mustKeyPress = false;
            mySkillDef.noSprint = false;
            mySkillDef.rechargeStock = 1;
            mySkillDef.requiredStock = 1;
            mySkillDef.shootDelay = 0f;
            mySkillDef.stockToConsume = 1;

            LoadoutAPI.AddSkillDef(mySkillDef);

            skillLocator.primary = sx.AddComponent<GenericSkill>();
            SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
            newFamily.variants = new SkillFamily.Variant[1];
            LoadoutAPI.AddSkillFamily(newFamily);
            skillLocator.primary.SetFieldValue("_skillFamily", newFamily);
            SkillFamily skillFamily = skillLocator.primary.skillFamily;

            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = mySkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
            };

            mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            mySkillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.LemurianBruiserMonster.Flamebreath));
            mySkillDef.activationStateMachineName = "Body";
            mySkillDef.baseMaxStock = 1;
            mySkillDef.baseRechargeInterval = 0f;
            mySkillDef.beginSkillCooldownOnSkillEnd = false;
            mySkillDef.canceledFromSprinting = false;
            mySkillDef.fullRestockOnAssign = true;
            mySkillDef.interruptPriority = InterruptPriority.Any;
            mySkillDef.isBullets = false;
            mySkillDef.isCombatSkill = true;
            mySkillDef.mustKeyPress = false;
            mySkillDef.noSprint = false;
            mySkillDef.rechargeStock = 1;
            mySkillDef.requiredStock = 1;
            mySkillDef.shootDelay = 0f;
            mySkillDef.stockToConsume = 0;

            LoadoutAPI.AddSkillDef(mySkillDef);

            skillLocator.secondary = sx.AddComponent<GenericSkill>();
            newFamily = ScriptableObject.CreateInstance<SkillFamily>();
            newFamily.variants = new SkillFamily.Variant[1];
            LoadoutAPI.AddSkillFamily(newFamily);
            skillLocator.secondary.SetFieldValue("_skillFamily", newFamily);
            skillFamily = skillLocator.secondary.skillFamily;

            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = mySkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
            };
        }



        public static void CreateDireseeker()
        {
            if (DiggerPlugin.direseekerInstalled) AddUnlockComponent();

            if (DiggerPlugin.fatAcrid.Value) PerroGrande();

            CreateProjectiles();

            LanguageAPI.Add("DIRESEEKER_BODY_NAME", "Direseeker");
            LanguageAPI.Add("DIRESEEKER_BODY_SUBTITLE", "Track and Kill");
            //LanguageAPI.Add("DIRESEEKER_BODY_LORE", "Direseeker\n\nDireseeker is a giant Elder Lemurian that acts as a boss in the Stage 4 area Magma Barracks. Upon defeating it, the player will unlock the Miner character for future playthroughs. The path leading to Direseeker's location only appears in one of the three variants of the level, and even then Direseeker may or may not spawn with random chance. Completing the teleporter event will also prevent it from spawning.\nNote that in online co-op the boss may spawn for the Host, but not others, although they can still damage it.\nActivating the Artifact of Kin does not prevent it from appearing.\n\nCategories: Enemies | Bosses | Unlisted Enemies\n\nLanguages: Español");
            LanguageAPI.Add("DIRESEEKER_BODY_LORE", "Legends tell of a monstrous beast that once roamed the underground barracks of Petrichor V.\n\nFeared by the bravest of survivors and the nastiest of monsters, the massive beast was unrivaled. It donned blood-red scales, tempered by hellfire. It had burning yellow eyes, with a glare so intense it made the largest of creatures stop dead in their tracks. It had smoldering breath, hot enough to melt metal in an instant.\n\nOnly once stopped by a survivor strong enough to slay Providence himself, it was believed that the beast had finally met its match.\n\n<style=cIsHealth>Until it showed its terrifying face once again.</style>");
            LanguageAPI.Add("DIRESEEKER_BODY_OUTRO_FLAVOR", "..and so it left, in search of new prey.");

            //skills and states
            LoadoutAPI.AddSkill(typeof(EntityStates.Direseeker.SpawnState));
            LoadoutAPI.AddSkill(typeof(EntityStates.Direseeker.ChargeUltraFireball));
            LoadoutAPI.AddSkill(typeof(EntityStates.Direseeker.FireUltraFireball));

            if (!DiggerPlugin.direseekerInstalled)
            {
                bodyPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterBodies/LemurianBruiserBody"), "DireseekerBody");

                CharacterBody bodyComponent = bodyPrefab.GetComponent<CharacterBody>();

                bodyComponent.name = "DireseekerBody";
                bodyComponent.baseNameToken = "DIRESEEKER_BODY_NAME";
                bodyComponent.subtitleNameToken = "DIRESEEKER_BODY_SUBTITLE";
                bodyComponent.baseMoveSpeed = 11f;
                bodyComponent.baseMaxHealth = 2800f;
                bodyComponent.levelMaxHealth = 840f;
                bodyComponent.baseDamage = 20f;
                bodyComponent.levelDamage = 4f;
                bodyComponent.isChampion = true;
                bodyComponent.portraitIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texDireseekerIcon").texture;

                SkillSetup(bodyPrefab);

                var stateMachine = bodyComponent.GetComponentInChildren<EntityStateMachine>();
                if (stateMachine) stateMachine.initialStateType = new SerializableEntityStateType(typeof(EntityStates.Direseeker.SpawnState));

                //resize

                bodyPrefab.GetComponent<ModelLocator>().modelBaseTransform.gameObject.transform.localScale *= 1.5f;
                //bodyPrefab.GetComponent<ModelLocator>().modelBaseTransform.gameObject.transform.Translate(new Vector3(0f, 5.6f, 0f));

                foreach (KinematicCharacterMotor kinematicCharacterMotor in bodyPrefab.GetComponentsInChildren<KinematicCharacterMotor>())
                {
                    kinematicCharacterMotor.SetCapsuleDimensions(kinematicCharacterMotor.Capsule.radius * 1.5f, kinematicCharacterMotor.Capsule.height * 1.5f, 1.5f);
                }

                //

                CharacterModel model = bodyPrefab.GetComponentInChildren<CharacterModel>();

                Material newMat = UnityEngine.Object.Instantiate<Material>(model.baseRendererInfos[0].defaultMaterial);
                newMat.SetTexture("_MainTex", Assets.mainAssetBundle.LoadAsset<Material>("matDireseeker").GetTexture("_MainTex"));
                newMat.SetTexture("_EmTex", Assets.mainAssetBundle.LoadAsset<Material>("matDireseeker").GetTexture("_EmissionMap"));
                newMat.SetFloat("_EmPower", 50f);

                model.baseRendererInfos[0].defaultMaterial = newMat;

                GameObject horn1 = Assets.mainAssetBundle.LoadAsset<GameObject>("DireHorn").InstantiateClone("DireseekerHorn", false);
                GameObject horn2 = Assets.mainAssetBundle.LoadAsset<GameObject>("DireHornBroken").InstantiateClone("DireseekerHornBroken", false);
                //GameObject rageFlame = Assets.mainAssetBundle.LoadAsset<GameObject>("DireseekerRageFlame").InstantiateClone("DireseekerRageFlame", false);
                GameObject burstFlame = Assets.mainAssetBundle.LoadAsset<GameObject>("DireseekerBurstFlame").InstantiateClone("DireseekerBurstFlame", false);

                ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();

                horn1.transform.SetParent(childLocator.FindChild("Head"));
                horn1.transform.localPosition = new Vector3(-2.5f, 1, -0.5f);
                horn1.transform.localRotation = Quaternion.Euler(new Vector3(45, 0, 90));
                horn1.transform.localScale = new Vector3(100, 100, 100);

                horn2.transform.SetParent(childLocator.FindChild("Head"));
                horn2.transform.localPosition = new Vector3(2.5f, 1, -0.5f);
                horn2.transform.localRotation = Quaternion.Euler(new Vector3(45, 0, 90));
                horn2.transform.localScale = new Vector3(100, -100, 100);

                /*rageFlame.transform.SetParent(childLocator.FindChild("Head"));
                rageFlame.transform.localPosition = new Vector3(0, 1, 0);
                rageFlame.transform.localRotation = Quaternion.Euler(new Vector3(270, 180, 0));
                rageFlame.transform.localScale = new Vector3(5, 5, 5);*/

                burstFlame.transform.SetParent(childLocator.FindChild("Head"));
                burstFlame.transform.localPosition = new Vector3(0, 1, 0);
                burstFlame.transform.localRotation = Quaternion.Euler(new Vector3(270, 180, 0));
                burstFlame.transform.localScale = new Vector3(5, 5, 5);

                bodyPrefab.AddComponent<DireseekerController>().burstFlame = burstFlame.GetComponent<ParticleSystem>();

                Shader hotpoo = Resources.Load<Shader>("Shaders/Deferred/hgstandard");

                Material hornMat = horn1.GetComponentInChildren<MeshRenderer>().material;
                hornMat.shader = hotpoo;

                //add horns

                CharacterModel.RendererInfo[] infos = model.baseRendererInfos;
                CharacterModel.RendererInfo[] newInfos = new CharacterModel.RendererInfo[]
                {
                infos[0],
                new CharacterModel.RendererInfo
                {
                    renderer = horn1.GetComponentInChildren<MeshRenderer>(),
                    defaultMaterial = hornMat,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = true
                },
                new CharacterModel.RendererInfo
                {
                    renderer = horn2.GetComponentInChildren<MeshRenderer>(),
                    defaultMaterial = hornMat,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = true
                }
                };

                model.baseRendererInfos = newInfos;

                masterPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterMasters/LemurianBruiserMaster"), "DireseekerMaster");

                CharacterMaster master = masterPrefab.GetComponent<CharacterMaster>();

                master.bodyPrefab = bodyPrefab;
                master.isBoss = false;

                BodyCatalog.getAdditionalEntries += delegate (List<GameObject> list)
                {
                    list.Add(bodyPrefab);
                };

                MasterCatalog.getAdditionalEntries += delegate (List<GameObject> list)
                {
                    list.Add(masterPrefab);
                };

                if (DiggerPlugin.enableDireseeker.Value && !DiggerPlugin.direseekerInstalled)
                {
                    CharacterSpawnCard characterSpawnCard = ScriptableObject.CreateInstance<CharacterSpawnCard>();
                    characterSpawnCard.name = "cscDireseeker";
                    characterSpawnCard.prefab = masterPrefab;
                    characterSpawnCard.sendOverNetwork = true;
                    characterSpawnCard.hullSize = HullClassification.BeetleQueen;
                    characterSpawnCard.nodeGraphType = MapNodeGroup.GraphType.Ground;
                    characterSpawnCard.requiredFlags = NodeFlags.None;
                    characterSpawnCard.forbiddenFlags = NodeFlags.TeleporterOK;
                    characterSpawnCard.directorCreditCost = 800;
                    characterSpawnCard.occupyPosition = false;
                    characterSpawnCard.loadout = new SerializableLoadout();
                    characterSpawnCard.noElites = true;
                    characterSpawnCard.forbiddenAsBoss = false;

                    DirectorCard card = new DirectorCard
                    {
                        spawnCard = characterSpawnCard,
                        selectionWeight = 1,
                        allowAmbushSpawn = false,
                        preventOverhead = false,
                        minimumStageCompletions = 2,
                        requiredUnlockable = "",
                        forbiddenUnlockable = "",
                        spawnDistance = DirectorCore.MonsterSpawnDistance.Close
                    };

                    DirectorAPI.DirectorCardHolder direseekerCard = new DirectorAPI.DirectorCardHolder
                    {
                        Card = card,
                        MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                        InteractableCategory = DirectorAPI.InteractableCategory.None
                    };

                    DirectorAPI.MonsterActions += delegate (List<DirectorAPI.DirectorCardHolder> list, DirectorAPI.StageInfo stage)
                    {
                        if (stage.stage == DirectorAPI.Stage.AbyssalDepths)
                        {
                            if (!list.Contains(direseekerCard))
                            {
                                list.Add(direseekerCard);
                            }
                        }
                    };
                }
            }

            if (DiggerPlugin.enableDireseekerSurvivor.Value)
            {
                survivorPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterBodies/LemurianBruiserBody"), "DireseekerPlayerBody");

                CharacterBody bodyComponent2 = survivorPrefab.GetComponent<CharacterBody>();

                bodyComponent2.name = "DireseekerPlayerBody";
                bodyComponent2.baseNameToken = "DIRESEEKER_BODY_NAME";
                bodyComponent2.subtitleNameToken = "DIRESEEKER_BODY_SUBTITLE";
                bodyComponent2.baseMoveSpeed = 11f;
                bodyComponent2.baseMaxHealth = 2200f;
                bodyComponent2.levelMaxHealth = 800f;
                bodyComponent2.baseRegen = 0.5f;
                bodyComponent2.levelRegen = 0.2f;
                bodyComponent2.baseDamage = 20f;
                bodyComponent2.levelDamage = 4f;
                bodyComponent2.isChampion = false;
                bodyComponent2.portraitIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texDireseekerPlayerIcon").texture;
                bodyComponent2.hideCrosshair = true;

                SkillSetup(survivorPrefab);

                var stateMachine = survivorPrefab.GetComponentInChildren<EntityStateMachine>();
                if (stateMachine) stateMachine.initialStateType = new SerializableEntityStateType(typeof(EntityStates.Direseeker.SpawnState));

                //resize

                survivorPrefab.GetComponent<ModelLocator>().modelBaseTransform.gameObject.transform.localScale *= 0.75f;
                survivorPrefab.transform.GetChild(0).localPosition = new Vector3(0, -2.75f, 0);
                survivorPrefab.transform.GetChild(2).localPosition = new Vector3(0, 0.8f, 1.5f);

                foreach (KinematicCharacterMotor kinematicCharacterMotor in survivorPrefab.GetComponentsInChildren<KinematicCharacterMotor>())
                {
                    kinematicCharacterMotor.SetCapsuleDimensions(kinematicCharacterMotor.Capsule.radius * 0.75f, kinematicCharacterMotor.Capsule.height * 0.75f, 0.75f);
                }

                //

                CharacterModel model2 = survivorPrefab.GetComponentInChildren<CharacterModel>();

                Material newMat = UnityEngine.Object.Instantiate<Material>(model2.baseRendererInfos[0].defaultMaterial);
                newMat.SetTexture("_MainTex", Assets.mainAssetBundle.LoadAsset<Material>("matDireseeker").GetTexture("_MainTex"));
                newMat.SetTexture("_EmTex", Assets.mainAssetBundle.LoadAsset<Material>("matDireseeker").GetTexture("_EmissionMap"));
                newMat.SetFloat("_EmPower", 50f);

                model2.baseRendererInfos[0].defaultMaterial = newMat;

                GameObject horn1b = Assets.mainAssetBundle.LoadAsset<GameObject>("DireHorn").InstantiateClone("DireseekerHorn", false);
                GameObject horn2b = Assets.mainAssetBundle.LoadAsset<GameObject>("DireHornBroken").InstantiateClone("DireseekerHornBroken", false);
                //GameObject rageFlame = Assets.mainAssetBundle.LoadAsset<GameObject>("DireseekerRageFlame").InstantiateClone("DireseekerRageFlame", false);
                GameObject burstFlame2 = Assets.mainAssetBundle.LoadAsset<GameObject>("DireseekerBurstFlame").InstantiateClone("DireseekerBurstFlame", false);

                ChildLocator childLocator2 = survivorPrefab.GetComponentInChildren<ChildLocator>();

                horn1b.transform.SetParent(childLocator2.FindChild("Head"));
                horn1b.transform.localPosition = new Vector3(-2.5f, 1, -0.5f);
                horn1b.transform.localRotation = Quaternion.Euler(new Vector3(45, 0, 90));
                horn1b.transform.localScale = new Vector3(100, 100, 100);

                horn2b.transform.SetParent(childLocator2.FindChild("Head"));
                horn2b.transform.localPosition = new Vector3(2.5f, 1, -0.5f);
                horn2b.transform.localRotation = Quaternion.Euler(new Vector3(45, 0, 90));
                horn2b.transform.localScale = new Vector3(100, -100, 100);

                /*rageFlame.transform.SetParent(childLocator.FindChild("Head"));
                rageFlame.transform.localPosition = new Vector3(0, 1, 0);
                rageFlame.transform.localRotation = Quaternion.Euler(new Vector3(270, 180, 0));
                rageFlame.transform.localScale = new Vector3(5, 5, 5);*/

                burstFlame2.transform.SetParent(childLocator2.FindChild("Head"));
                burstFlame2.transform.localPosition = new Vector3(0, 1, 0);
                burstFlame2.transform.localRotation = Quaternion.Euler(new Vector3(270, 180, 0));
                burstFlame2.transform.localScale = new Vector3(5, 5, 5);

                survivorPrefab.AddComponent<DireseekerController>().burstFlame = burstFlame2.GetComponent<ParticleSystem>();

                Shader hotpoo = Resources.Load<Shader>("Shaders/Deferred/hgstandard");

                Material hornMat = horn1b.GetComponentInChildren<MeshRenderer>().material;
                hornMat.shader = hotpoo;

                //add horns

                CharacterModel.RendererInfo[] infos2 = model2.baseRendererInfos;
                CharacterModel.RendererInfo[] newInfos2 = new CharacterModel.RendererInfo[]
                {
                    infos2[0],
                    new CharacterModel.RendererInfo
                    {
                        renderer = horn1b.GetComponentInChildren<MeshRenderer>(),
                        defaultMaterial = hornMat,
                        defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                        ignoreOverlays = true
                    },
                    new CharacterModel.RendererInfo
                    {
                        renderer = horn2b.GetComponentInChildren<MeshRenderer>(),
                        defaultMaterial = hornMat,
                        defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                        ignoreOverlays = true
                    }
                };

                model2.baseRendererInfos = newInfos2;

                survivorPrefab.GetComponent<DeathRewards>().logUnlockableName = "";
                survivorPrefab.GetComponent<Interactor>().maxInteractionDistance = 5f;

                survivorPrefab.tag = "Player";

                SkinSetup();

                BodyCatalog.getAdditionalEntries += delegate (List<GameObject> list)
                {
                    list.Add(survivorPrefab);
                };

                GameObject displayPrefab = PrefabAPI.InstantiateClone(survivorPrefab.GetComponent<ModelLocator>().modelTransform.gameObject, "DireseekerDisplay", true);
                displayPrefab.AddComponent<NetworkIdentity>();
                displayPrefab.transform.localScale *= 0.5f;

                SurvivorDef survivorDef = new SurvivorDef
                {
                    name = "DIRESEEKER_BODY_NAME",
                    unlockableName = "",
                    descriptionToken = "MINER_DESCRIPTION",
                    primaryColor = Color.red,
                    bodyPrefab = survivorPrefab,
                    displayPrefab = displayPrefab,
                    outroFlavorToken = "DIRESEEKER_BODY_OUTRO_FLAVOR"
                };

                SurvivorAPI.AddSurvivor(survivorDef);
            }
        }

        public static void LateSetup()
        {
            if (DiggerPlugin.sivsItemsInstalled)
            {
                AddBossPickup();
            }
        }

        private static void SkinSetup()
        {
            GameObject model = survivorPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            if (model.GetComponent<ModelSkinController>()) DiggerPlugin.Destroy(model.GetComponent<ModelSkinController>());

            ModelSkinController skinController = model.AddComponent<ModelSkinController>();
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            SkinnedMeshRenderer mainRenderer = Reflection.GetFieldValue<SkinnedMeshRenderer>(characterModel, "mainSkinnedMeshRenderer");

            LanguageAPI.Add("DIRESEEKER_BODY_DEFAULT_SKIN_NAME", "Default");

            LoadoutAPI.SkinDefInfo skinDefInfo = default(LoadoutAPI.SkinDefInfo);
            skinDefInfo.BaseSkins = Array.Empty<SkinDef>();
            skinDefInfo.ProjectileGhostReplacements = new SkinDef.ProjectileGhostReplacement[0];
            skinDefInfo.MinionSkinReplacements = new SkinDef.MinionSkinReplacement[0];

            skinDefInfo.GameObjectActivations = Array.Empty<SkinDef.GameObjectActivation>();

            skinDefInfo.Icon = Assets.mainAssetBundle.LoadAsset<Sprite>("texMoltenAchievement");
            skinDefInfo.MeshReplacements = new SkinDef.MeshReplacement[0];
            /*{
                new SkinDef.MeshReplacement
                {
                    renderer = mainRenderer,
                    mesh = mainRenderer.sharedMesh
                }
            };*/
            skinDefInfo.Name = "DIRESEEKER_BODY_DEFAULT_SKIN_NAME";
            skinDefInfo.NameToken = "DIRESEEKER_BODY_DEFAULT_SKIN_NAME";
            skinDefInfo.RendererInfos = characterModel.baseRendererInfos;
            skinDefInfo.RootObject = model;
            skinDefInfo.UnlockableName = "";

            SkinDef defaultSkin = LoadoutAPI.CreateNewSkinDef(skinDefInfo);

            skinController.skins = new SkinDef[1]
            {
                defaultSkin
            };
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void AddBossPickup()
        {
            if (DiggerPlugin.enableDireseeker.Value)
            {
                var pickup = new SerializablePickupIndex();
                pickup.pickupName = PickupCatalog.FindPickupIndex("FlameGland").ToString();
                bodyPrefab.GetComponent<DeathRewards>().bossPickup = new SerializablePickupIndex();

                //Debug.Log("direseeker drop is" + (PickupIndex)pickup);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void AddUnlockComponent()
        {
            DireseekerMod.Modules.Prefabs.bodyPrefab.AddComponent<DiggerUnlockComponent>();
        }
    }

    public class DireseekerController : MonoBehaviour
    {
        public ParticleSystem burstFlame;
        public ParticleSystem rageFlame;

        public void StartRageMode()
        {
            if (rageFlame) rageFlame.Play();
        }

        public void FlameBurst()
        {
            if (burstFlame) burstFlame.Play();
        }
    }
}
