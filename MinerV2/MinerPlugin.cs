using System;
using System.Collections.Generic;
using BepInEx;
using R2API;
using R2API.Utils;
using EntityStates;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;
using KinematicCharacterController;
using EntityStates.Miner;
using RoR2.UI;
using BepInEx.Configuration;

namespace MinerPlugin
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, "MinerUnearthed", "1.2.0")]
    [R2APISubmoduleDependency(new string[]
    {
        "PrefabAPI",
        "SurvivorAPI",
        "LoadoutAPI",
        "BuffAPI",
        "LanguageAPI",
        "SoundAPI",
        "EffectAPI",
        "UnlockablesAPI",
        "ResourcesAPI"
    })]

    public class MinerPlugin : BaseUnityPlugin
    {
        public const string MODUID = "com.rob.MinerUnearthed";

        public const string characterName = "Miner";
        public const string characterSubtitle = "Destructive Drug Addict";
        public const string characterOutro = "..and so he left, adrenaline still rushing through his veins.";
        public const string characterLore = "\nAmong the cold steel of the freighter's ruins and the air of the freezing night, the dim blue light of an audio journal catches your eye. The journal lies in utter disrepair among the remains of the cargo, and yet it persists, remaining functional. The analog buttons on the device's casing pulse slowly, as if inviting anybody to listen to their warped entries, ever-decaying in coherency..." +
                "\n\n<color=#8990A7><CLICK></color>" +
                "\n\n<color=#8990A7>/ / - - L  O  G     1 - - / /</color>" +
                "\n''First log. Date, uhhh... Oh-five, oh-one, twenty-fifty-five. Brass decided to pull us out of the mining job. After months of digging, after months of blood, swe<color=#8990A7>-///////-</color>nd millions spent on this operation, we're just dropping it? Bull<color=#8990A7>-//-</color>.''" +
                "\n\n<color=#8990A7>/ / - - L  O  G     5 - - / /</color>" +
                "\n''<color=#8990A7>-////////-</color>nter of the asteroid. I've never seen anything like it. It was like a dr<color=#8990A7>-////////-</color>nd fractal... The inside is almost like a kaleidoscope. It's gotta be worth<color=#8990A7>-/////-</color>nd brass wants us gone, ASAP. Well, they can kiss m<color=#8990A7>-////-</color>. And I'm going out there tonight to dig the rest of it out... If I can fence this, the next thousand paychecks won't hold a candle. I've been waiting for an opportunity like this.''" +
                "\n\n<color=#8990A7>/ / - - L  O  G     7 - - / /</color>" +
                "\n''Pretty sure they know everything. They're questioning people now. I stashed the artifact in a vent in<color=#8990A7>-//////-</color>, should be safe. Freighter called the UE<color=#8990A7>-////-</color>t's docking in a few months. I just have to hold out and sneak on.''" +
                "\n\n<color=#8990A7>/ / - - L  O  G     15 - - / /</color>" +
                "\n''They're looking for me. Not long before they find me. Hiding in the walls of the ship like a f<color=#8990A7>-//////-</color>rat. Freighter's off schedule. I gave everything for this. I gave <i>everything</i> for this.''" +
                "\n\n<color=#8990A7>/ / - - L  O  G     16 - - / /</color>" +
                "\n''Somewhere in the hull. Nobody comes back here. Not even maintenance. I should be safe until the ship arrives.''" +
                "\n\n<color=#8990A7>/ / - - L  O  G     18 - - / /</color>" +
                "\n''Ship still on hold. Can't get an idea of when it won't be.'' <color=#8990A7><Groan>.</color> ''This is ago<color=#8990A7>-//-</color>zing.''" +
                "\n\n <color=#8990A7>/ / - - L  O  G     29 - - / /</color>" +
                "\n''<color=#8990A7>-//////////-</color>'s here. Finally. Finally. Have to get the artifact on board. Security everywhere, though. Mercs...'' <color=#8990A7><Distant voices have an indiscernible conversation.></color> ''Okay. Shut up. Shut up and go. I'm going. Going. Tonight.''" +
                "\n\n <color=#8990A7>/ / - - L  O  G     31 - - / /</color>" +
                "\n''<color=#8990A7>-//////-</color>omething's happening to the ship. Cargo flying out. Lost the artifact. Lost everything. . . . Lost everythi<color=#8990A7>-//-</color>.''" +
                "\n\nThe audio journal's screen sparks and pops, leaving you in complete darkness, complemented by the deafening silence brought about by the ominous last words of the miner.";

        public static MinerPlugin instance;

        public static GameObject characterPrefab;
        public static GameObject characterDisplay;

        public GameObject doppelganger;

        public static event Action awake;
        public static event Action start;

        public static BuffIndex goldRush;

        public static readonly Color characterColor = new Color(0.956862745f, 0.874509803f, 0.603921568f);

        public SkillLocator skillLocator;

        public static float adrenalineCap;

        public static GameObject backblastEffect;

        public static ConfigEntry<float> maxAdrenaline;
        public static ConfigEntry<bool> extraSkins;
        public static ConfigEntry<bool> styleUI;

        public MinerPlugin()
        {
            awake += MinerPlugin_Load;
            start += MinerPlugin_LoadStart;
        }

        private void MinerPlugin_Load()
        {
            ConfigShit();
            Assets.PopulateAssets();
            CreateDisplayPrefab();
            CreatePrefab();
            RegisterCharacter();
            Skins.RegisterSkins();
            ItemDisplays.RegisterDisplays();
            Unlockables.RegisterUnlockables();
            RegisterEffects();
            RegisterBuffs();
            CreateDoppelganger();

            //the il is broken and idk how to fix, sorry
            //ILHook();
            Hook();
        }

        private void MinerPlugin_LoadStart()
        {
        }

        public void Awake()
        {
            Action awake = MinerPlugin.awake;
            if (awake == null)
            {
                return;
            }
            awake();
        }
        public void Start()
        {
            Action start = MinerPlugin.start;
            if (start == null)
            {
                return;
            }
            start();
        }

        private void ConfigShit()
        {
            maxAdrenaline = base.Config.Bind<float>(new ConfigDefinition("01 - General Settings", "Adrenaline Cap"), 50, new ConfigDescription("Max Adrenaline stacks allowed", null, Array.Empty<object>()));
            adrenalineCap = maxAdrenaline.Value - 1;
            extraSkins = base.Config.Bind<bool>(new ConfigDefinition("01 - General Settings", "Extra Skins"), false, new ConfigDescription("Enables a bunch of extra skins", null, Array.Empty<object>()));
            styleUI = base.Config.Bind<bool>(new ConfigDefinition("01 - General Settings", "Style Rank"), false, new ConfigDescription("Enables a style ranking system taken from Devil May Cry", null, Array.Empty<object>()));
        }

        private void RegisterEffects()
        {
            backblastEffect = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniExplosionVFX"), "MinerBackblastEffect", true);

            if (!backblastEffect.GetComponent<EffectComponent>()) backblastEffect.AddComponent<EffectComponent>();
            if (!backblastEffect.GetComponent<VFXAttributes>()) backblastEffect.AddComponent<VFXAttributes>();
            if (!backblastEffect.GetComponent<NetworkIdentity>()) backblastEffect.AddComponent<NetworkIdentity>();

            backblastEffect.GetComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;

            EffectAPI.AddEffect(backblastEffect);
        }

        private void RegisterBuffs()
        {
            BuffDef goldRushDef = new BuffDef
            {
                name = "Gold Rush",
                iconPath = "Textures/BuffIcons/texBuffOnFireIcon",
                buffColor = Color.yellow,
                canStack = true,
                isDebuff = false,
                eliteIndex = EliteIndex.None
            };
            CustomBuff goldRush = new CustomBuff(goldRushDef);
            MinerPlugin.goldRush = BuffAPI.Add(goldRush);
        }

        private void Hook()
        {
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            if (styleUI.Value) On.RoR2.UI.HUD.OnEnable += HUDAwake;
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
            if (self)
            {
                if (self.HasBuff(goldRush))
                {
                    int count = self.GetBuffCount(goldRush);
                    Reflection.SetPropertyValue<float>(self, "attackSpeed", self.attackSpeed + (count * 0.1f));
                    Reflection.SetPropertyValue<float>(self, "moveSpeed", self.moveSpeed + (count * 0.15f));
                    Reflection.SetPropertyValue<float>(self, "regen", self.regen + (count * 0.25f));
                }
            }
        }

        internal static void HUDAwake(On.RoR2.UI.HUD.orig_OnEnable orig, HUD self)
        {
            orig(self);

            if (self.gameObject) self.gameObject.AddComponent<StyleSystem.ExtraHUD>();
        }

        private void ILHook()
        {
            /*IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                    x => x.MatchLdloc(50),
                    x => x.MatchLdloc(51)
                    );
                //x => x.MatchLdloc(52),
                //x => x.MatchDiv(),
                //x => x.MatchMul()
                //);
                c.Index += 2;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<CharacterBody, float>>((charBody) =>
                {
                    float output = 0f;
                    if (charBody.HasBuff(goldRush))
                    {
                        output = 0.4f;
                    }
                    return output;
                });
                c.Emit(OpCodes.Add);
            };

            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                    x => x.MatchMul(),
                    x => x.MatchAdd(),
                    x => x.MatchStloc(58)
                    );
                c.Index += 2;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<CharacterBody, float>>((charBody) =>
                {
                    float output = 0f;
                    if (charBody.HasBuff(goldRush))
                    {
                        output = charBody.GetBuffCount(goldRush) * 0.12f;
                    }
                    return output;
                });
                c.Emit(OpCodes.Add);
            };

            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                    x => x.MatchMul(),
                    x => x.MatchLdloc(43),
                    x => x.MatchMul(),
                    x => x.MatchStloc(47)
                    );
                c.Index += 3;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<CharacterBody, float>>((charBody) =>
                {
                    float output = 0f;
                    if (charBody.HasBuff(goldRush))
                    {
                        output = charBody.GetBuffCount(goldRush) * 1;
                    }
                    return output;
                });
                c.Emit(OpCodes.Add);
            };*/
        }

        private static GameObject CreateModel(GameObject main, int index)
        {
            Destroy(main.transform.Find("ModelBase").gameObject);
            Destroy(main.transform.Find("CameraPivot").gameObject);
            Destroy(main.transform.Find("AimOrigin").gameObject);

            GameObject model = null;

            if (index == 0) model = Assets.MainAssetBundle.LoadAsset<GameObject>("mdlMiner");
            else if (index == 1) model = Assets.MainAssetBundle.LoadAsset<GameObject>("MinerDisplay");

            return model;
        }

        private static void CreateDisplayPrefab()
        {
            //spaghetti incoming
            GameObject tempDisplay = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody"), "MinerDisplay");

            GameObject model = CreateModel(tempDisplay, 1);

            GameObject gameObject = new GameObject("ModelBase");
            gameObject.transform.parent = tempDisplay.transform;
            gameObject.transform.localPosition = new Vector3(0f, -0.81f, 0f);
            gameObject.transform.localRotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(1f, 1f, 1f);

            GameObject gameObject2 = new GameObject("CameraPivot");
            gameObject2.transform.parent = gameObject.transform;
            gameObject2.transform.localPosition = new Vector3(0f, 1.6f, 0f);
            gameObject2.transform.localRotation = Quaternion.identity;
            gameObject2.transform.localScale = Vector3.one;

            GameObject gameObject3 = new GameObject("AimOrigin");
            gameObject3.transform.parent = gameObject.transform;
            gameObject3.transform.localPosition = new Vector3(0f, 1.8f, 0f);
            gameObject3.transform.localRotation = Quaternion.identity;
            gameObject3.transform.localScale = Vector3.one;

            Transform transform = model.transform;
            transform.parent = gameObject.transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            ModelLocator modelLocator = tempDisplay.GetComponent<ModelLocator>();
            modelLocator.modelTransform = transform;
            modelLocator.modelBaseTransform = gameObject.transform;

            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            CharacterModel characterModel = model.AddComponent<CharacterModel>();
            characterModel.body = null;
            characterModel.baseRendererInfos = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    defaultMaterial = model.GetComponentInChildren<SkinnedMeshRenderer>().material,
                    renderer = model.GetComponentInChildren<SkinnedMeshRenderer>(),
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false
                }
            };
            characterModel.autoPopulateLightInfos = true;
            characterModel.invisibilityCount = 0;
            characterModel.temporaryOverlays = new List<TemporaryOverlay>();

            characterModel.SetFieldValue("mainSkinnedMeshRenderer", characterModel.baseRendererInfos[0].renderer.gameObject.GetComponent<SkinnedMeshRenderer>());

            characterDisplay = PrefabAPI.InstantiateClone(tempDisplay.GetComponent<ModelLocator>().modelBaseTransform.gameObject, "MinerDisplay", true);

            characterDisplay.AddComponent<MenuSound>();
        }

        private static void CreatePrefab()
        {
            characterPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody"), "MinerBody");

            characterPrefab.GetComponent<NetworkIdentity>().localPlayerAuthority = true;

            GameObject model = CreateModel(characterPrefab, 0);

            GameObject gameObject = new GameObject("ModelBase");
            gameObject.transform.parent = characterPrefab.transform;
            gameObject.transform.localPosition = new Vector3(0f, -0.81f, 0f);
            gameObject.transform.localRotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(1f, 1f, 1f);

            GameObject gameObject2 = new GameObject("CameraPivot");
            gameObject2.transform.parent = gameObject.transform;
            gameObject2.transform.localPosition = new Vector3(0f, 1.6f, 0f);
            gameObject2.transform.localRotation = Quaternion.identity;
            gameObject2.transform.localScale = Vector3.one;

            GameObject gameObject3 = new GameObject("AimOrigin");
            gameObject3.transform.parent = gameObject.transform;
            gameObject3.transform.localPosition = new Vector3(0f, 1.5f, 0f);
            gameObject3.transform.localRotation = Quaternion.identity;
            gameObject3.transform.localScale = Vector3.one;

            Transform transform = model.transform;
            transform.parent = gameObject.transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            CharacterDirection characterDirection = characterPrefab.GetComponent<CharacterDirection>();
            characterDirection.moveVector = Vector3.zero;
            characterDirection.targetTransform = gameObject.transform;
            characterDirection.overrideAnimatorForwardTransform = null;
            characterDirection.rootMotionAccumulator = null;
            characterDirection.modelAnimator = model.GetComponentInChildren<Animator>();
            characterDirection.driveFromRootRotation = false;
            characterDirection.turnSpeed = 720f;

            CharacterBody bodyComponent = characterPrefab.GetComponent<CharacterBody>();
            bodyComponent.bodyIndex = -1;
            bodyComponent.name = "MinerBody";
            bodyComponent.baseNameToken = "MINER_NAME";
            bodyComponent.subtitleNameToken = "MINER_SUBTITLE";
            bodyComponent.bodyFlags = CharacterBody.BodyFlags.ImmuneToExecutes;
            bodyComponent.rootMotionInMainState = false;
            bodyComponent.mainRootSpeed = 0;
            bodyComponent.baseMaxHealth = 120;
            bodyComponent.levelMaxHealth = 48;
            bodyComponent.baseRegen = 0.5f;
            bodyComponent.levelRegen = 0.25f;
            bodyComponent.baseMaxShield = 0;
            bodyComponent.levelMaxShield = 0;
            bodyComponent.baseMoveSpeed = 7;
            bodyComponent.levelMoveSpeed = 0;
            bodyComponent.baseAcceleration = 80;
            bodyComponent.baseJumpPower = 15;
            bodyComponent.levelJumpPower = 0;
            bodyComponent.baseDamage = 12;
            bodyComponent.levelDamage = 2.4f;
            bodyComponent.baseAttackSpeed = 1;
            bodyComponent.levelAttackSpeed = 0;
            bodyComponent.baseCrit = 1;
            bodyComponent.levelCrit = 0;
            bodyComponent.baseArmor = 20;
            bodyComponent.levelArmor = 0;
            bodyComponent.baseJumpCount = 1;
            bodyComponent.sprintingSpeedMultiplier = 1.45f;
            bodyComponent.wasLucky = false;
            bodyComponent.hideCrosshair = false;
            bodyComponent.crosshairPrefab = Resources.Load<GameObject>("Prefabs/Crosshair/SimpleDotCrosshair");
            bodyComponent.aimOriginTransform = gameObject3.transform;
            bodyComponent.hullClassification = HullClassification.Human;
            bodyComponent.portraitIcon = Assets.charPortrait;
            bodyComponent.isChampion = false;
            bodyComponent.currentVehicle = null;
            bodyComponent.skinIndex = 0U;

            LoadoutAPI.AddSkill(typeof(MinerMain));

            var stateMachine = bodyComponent.GetComponent<EntityStateMachine>();
            stateMachine.mainStateType = new SerializableEntityStateType(typeof(MinerMain));

            CharacterMotor characterMotor = characterPrefab.GetComponent<CharacterMotor>();
            characterMotor.walkSpeedPenaltyCoefficient = 1f;
            characterMotor.characterDirection = characterDirection;
            characterMotor.muteWalkMotion = false;
            characterMotor.mass = 100f;
            characterMotor.airControl = 0.25f;
            characterMotor.disableAirControlUntilCollision = false;
            characterMotor.generateParametersOnAwake = true;

            CameraTargetParams cameraTargetParams = characterPrefab.GetComponent<CameraTargetParams>();
            cameraTargetParams.cameraParams = Resources.Load<GameObject>("Prefabs/CharacterBodies/MercBody").GetComponent<CameraTargetParams>().cameraParams;
            cameraTargetParams.cameraPivotTransform = null;
            cameraTargetParams.aimMode = CameraTargetParams.AimType.Standard;
            cameraTargetParams.recoil = Vector2.zero;
            cameraTargetParams.idealLocalCameraPos = Vector3.zero;
            cameraTargetParams.dontRaycastToPivot = false;

            ModelLocator modelLocator = characterPrefab.GetComponent<ModelLocator>();
            modelLocator.modelTransform = transform;
            modelLocator.modelBaseTransform = gameObject.transform;

            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            CharacterModel characterModel = model.AddComponent<CharacterModel>();
            characterModel.body = bodyComponent;
            characterModel.baseRendererInfos = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    defaultMaterial = model.GetComponentInChildren<SkinnedMeshRenderer>().material,
                    renderer = model.GetComponentInChildren<SkinnedMeshRenderer>(),
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false
                }
            };

            characterModel.autoPopulateLightInfos = true;
            characterModel.invisibilityCount = 0;
            characterModel.temporaryOverlays = new List<TemporaryOverlay>();

            characterModel.SetFieldValue("mainSkinnedMeshRenderer", characterModel.baseRendererInfos[0].renderer.gameObject.GetComponent<SkinnedMeshRenderer>());

            TeamComponent teamComponent = null;
            if (characterPrefab.GetComponent<TeamComponent>() != null) teamComponent = characterPrefab.GetComponent<TeamComponent>();
            else teamComponent = characterPrefab.GetComponent<TeamComponent>();
            teamComponent.hideAllyCardDisplay = false;
            teamComponent.teamIndex = TeamIndex.None;

            HealthComponent healthComponent = characterPrefab.GetComponent<HealthComponent>();
            healthComponent.shield = 0f;
            healthComponent.barrier = 0f;
            healthComponent.magnetiCharge = 0f;
            healthComponent.body = null;
            healthComponent.dontShowHealthbar = false;
            healthComponent.globalDeathEventChanceCoefficient = 1f;

            characterPrefab.GetComponent<Interactor>().maxInteractionDistance = 3f;
            characterPrefab.GetComponent<InteractionDriver>().highlightInteractor = true;

            CharacterDeathBehavior characterDeathBehavior = characterPrefab.GetComponent<CharacterDeathBehavior>();
            characterDeathBehavior.deathStateMachine = characterPrefab.GetComponent<EntityStateMachine>();
            //characterDeathBehavior.deathState = new SerializableEntityStateType(typeof(GenericCharacterDeath));

            SfxLocator sfxLocator = characterPrefab.GetComponent<SfxLocator>();
            //sfxLocator.deathSound = Sounds.DeathSound;
            sfxLocator.barkSound = "";
            sfxLocator.openSound = "";
            sfxLocator.landingSound = "Play_char_land";
            sfxLocator.fallDamageSound = "Play_char_land_fall_damage";
            sfxLocator.aliveLoopStart = "";
            sfxLocator.aliveLoopStop = "";

            Rigidbody rigidbody = characterPrefab.GetComponent<Rigidbody>();
            rigidbody.mass = 100f;
            rigidbody.drag = 0f;
            rigidbody.angularDrag = 0f;
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
            rigidbody.interpolation = RigidbodyInterpolation.None;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            rigidbody.constraints = RigidbodyConstraints.None;

            CapsuleCollider capsuleCollider = characterPrefab.GetComponent<CapsuleCollider>();
            capsuleCollider.isTrigger = false;
            capsuleCollider.material = null;
            capsuleCollider.center = new Vector3(0f, 0f, 0f);
            capsuleCollider.radius = 0.5f;
            capsuleCollider.height = 1.82f;
            capsuleCollider.direction = 1;

            KinematicCharacterMotor kinematicCharacterMotor = characterPrefab.GetComponent<KinematicCharacterMotor>();
            kinematicCharacterMotor.CharacterController = characterMotor;
            kinematicCharacterMotor.Capsule = capsuleCollider;
            kinematicCharacterMotor.Rigidbody = rigidbody;

            kinematicCharacterMotor.DetectDiscreteCollisions = false;
            kinematicCharacterMotor.GroundDetectionExtraDistance = 0f;
            kinematicCharacterMotor.MaxStepHeight = 0.2f;
            kinematicCharacterMotor.MinRequiredStepDepth = 0.1f;
            kinematicCharacterMotor.MaxStableSlopeAngle = 55f;
            kinematicCharacterMotor.MaxStableDistanceFromLedge = 0.5f;
            kinematicCharacterMotor.PreventSnappingOnLedges = false;
            kinematicCharacterMotor.MaxStableDenivelationAngle = 55f;
            kinematicCharacterMotor.RigidbodyInteractionType = RigidbodyInteractionType.None;
            kinematicCharacterMotor.PreserveAttachedRigidbodyMomentum = true;
            kinematicCharacterMotor.HasPlanarConstraint = false;
            kinematicCharacterMotor.PlanarConstraintAxis = Vector3.up;
            kinematicCharacterMotor.StepHandling = StepHandlingMethod.None;
            kinematicCharacterMotor.LedgeHandling = true;
            kinematicCharacterMotor.InteractiveRigidbodyHandling = true;
            kinematicCharacterMotor.SafeMovement = false;

            HurtBoxGroup hurtBoxGroup = model.AddComponent<HurtBoxGroup>();

            HurtBox mainHurtbox = model.transform.Find("MainHurtbox").gameObject.AddComponent<HurtBox>();
            mainHurtbox.gameObject.layer = LayerIndex.entityPrecise.intVal;
            mainHurtbox.healthComponent = healthComponent;
            mainHurtbox.isBullseye = true;
            mainHurtbox.damageModifier = HurtBox.DamageModifier.Normal;
            mainHurtbox.hurtBoxGroup = hurtBoxGroup;
            mainHurtbox.indexInGroup = 0;

            hurtBoxGroup.hurtBoxes = new HurtBox[]
            {
                mainHurtbox
            };

            hurtBoxGroup.mainHurtBox = mainHurtbox;
            hurtBoxGroup.bullseyeCount = 1;

            HitBoxGroup hitBoxGroup = model.AddComponent<HitBoxGroup>();

            GameObject crushHitbox = new GameObject("CrushHitbox");
            crushHitbox.transform.parent = childLocator.FindChild("SwingLeft");
            crushHitbox.transform.localPosition = new Vector3(0f, 0f, 0f);
            crushHitbox.transform.localRotation = Quaternion.identity;
            crushHitbox.transform.localScale = Vector3.one * 0.08f;

            HitBox hitBox = crushHitbox.AddComponent<HitBox>();
            crushHitbox.layer = LayerIndex.projectile.intVal;

            hitBoxGroup.hitBoxes = new HitBox[]
            {
                hitBox
            };

            hitBoxGroup.groupName = "Crush";

            HitBoxGroup chargeHitBoxGroup = model.AddComponent<HitBoxGroup>();

            GameObject chargeHitbox = new GameObject("ChargeHitbox");
            chargeHitbox.transform.parent = childLocator.FindChild("SwingLeft");
            chargeHitbox.transform.localPosition = new Vector3(0f, 0f, 0f);
            chargeHitbox.transform.localRotation = Quaternion.identity;
            chargeHitbox.transform.localScale = Vector3.one * 0.08f;

            HitBox chargeHitBox = chargeHitbox.AddComponent<HitBox>();
            chargeHitbox.layer = LayerIndex.projectile.intVal;

            chargeHitBoxGroup.hitBoxes = new HitBox[]
            {
                chargeHitBox
            };

            chargeHitBoxGroup.groupName = "Charge";

            FootstepHandler footstepHandler = model.AddComponent<FootstepHandler>();
            footstepHandler.baseFootstepString = "Play_player_footstep";
            footstepHandler.sprintFootstepOverrideString = "";
            footstepHandler.enableFootstepDust = true;
            footstepHandler.footstepDustPrefab = Resources.Load<GameObject>("Prefabs/GenericFootstepDust");

            RagdollController ragdollController = model.GetComponent<RagdollController>();

            PhysicMaterial physicMat = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponentInChildren<RagdollController>().bones[1].GetComponent<Collider>().material;

            foreach (Transform i in ragdollController.bones)
            {
                if (i)
                {
                    i.gameObject.layer = LayerIndex.ragdoll.intVal;
                    Collider j = i.GetComponent<Collider>();
                    if (j)
                    {
                        j.material = physicMat;
                        j.sharedMaterial = physicMat;
                    }
                    Rigidbody k = i.GetComponent<Rigidbody>();
                    if (k) k.drag = 0.1f;
                }
            }

            AimAnimator aimAnimator = model.AddComponent<AimAnimator>();
            aimAnimator.directionComponent = characterDirection;
            aimAnimator.pitchRangeMax = 60f;
            aimAnimator.pitchRangeMin = -60f;
            aimAnimator.yawRangeMin = -90f;
            aimAnimator.yawRangeMax = 90f;
            aimAnimator.pitchGiveupRange = 30f;
            aimAnimator.yawGiveupRange = 10f;
            aimAnimator.giveupDuration = 3f;
            aimAnimator.inputBank = characterPrefab.GetComponent<InputBankTest>();

            if (styleUI.Value) characterPrefab.AddComponent<StyleSystem.StyleComponent>();

            GameObject particlesObject = childLocator.FindChild("AdrenalineFire").gameObject;
            if (particlesObject) {
                characterPrefab.AddComponent<AdrenalineParticleTimer>().init(particlesObject);
            } else {
                Debug.LogWarning("no object found for adrenaline particle system");
            }
        }

        private void RegisterCharacter()
        {
            string desc = "The Miner is a fast paced and highly mobile melee survivor who prioritizes getting long kill combos to build stacks of his passive.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Once you get a good number of stacks of Adrenaline, Crush will be your best source of damage." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Note that charging Drill Charge only affects damage dealt. Aim at the ground or into enemies to deal concentrated damage." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > You can tap Backblast to travel a short distance. Hold it to go further." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > To The Stars when used low to the ground is great at dealing high amounts of damage to enemies with large hitboxes." + Environment.NewLine + Environment.NewLine;

            string outro = characterOutro;

            LanguageAPI.Add("MINER_NAME", characterName);
            LanguageAPI.Add("MINER_DESCRIPTION", desc);
            LanguageAPI.Add("MINER_SUBTITLE", characterSubtitle);
            LanguageAPI.Add("MINER_LORE", characterLore);
            LanguageAPI.Add("MINER_OUTRO_FLAVOR", outro);

            characterDisplay.AddComponent<NetworkIdentity>();

            string unlockString = "";//"MINER_CHARACTERUNLOCKABLE_REWARD_ID"

            SurvivorDef survivorDef = new SurvivorDef
            {
                name = "MINER_NAME",
                unlockableName = unlockString,
                descriptionToken = "MINER_DESCRIPTION",
                primaryColor = characterColor,
                bodyPrefab = characterPrefab,
                displayPrefab = characterDisplay,
                outroFlavorToken = "MINER_OUTRO_FLAVOR"
            };

            SurvivorAPI.AddSurvivor(survivorDef);

            SkillSetup();

            BodyCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(characterPrefab);
            };

            characterPrefab.tag = "Player";
        }

        private void CreateDoppelganger()
        {
            doppelganger = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterMasters/MercMonsterMaster"), "MinerMonsterMaster");

            MasterCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(doppelganger);
            };

            CharacterMaster component = doppelganger.GetComponent<CharacterMaster>();
            component.bodyPrefab = characterPrefab;
        }

        private void SkillSetup()
        {
            foreach (GenericSkill obj in characterPrefab.GetComponentsInChildren<GenericSkill>())
            {
                BaseUnityPlugin.DestroyImmediate(obj);
            }

            skillLocator = characterPrefab.GetComponent<SkillLocator>();

            PassiveSetup();
            PrimarySetup();
            SecondarySetup();
            UtilitySetup();
            SpecialSetup();
        }

        private void PassiveSetup()
        {
            LanguageAPI.Add("MINER_PASSIVE_NAME", "Gold Rush");
            LanguageAPI.Add("MINER_PASSIVE_DESCRIPTION", "Gain <style=cIsHealth>ADRENALINE</style> on receiving gold, increasing <style=cIsDamage>attack speed</style>, <style=cIsUtility>movement speed</style>, and <style=cIsHealing>health regen</style>. <style=cIsUtility>Any increase in gold refreshes all stacks.</style>");

            skillLocator.passiveSkill.enabled = true;
            skillLocator.passiveSkill.skillNameToken = "MINER_PASSIVE_NAME";
            skillLocator.passiveSkill.skillDescriptionToken = "MINER_PASSIVE_DESCRIPTION";
            skillLocator.passiveSkill.icon = Assets.iconP;
        }

        private void PrimarySetup()
        {
            LoadoutAPI.AddSkill(typeof(Crush));

            string desc = "<style=cIsUtility>Agile.</style> Crush nearby enemies for <style=cIsDamage>" + 100f * Crush.damageCoefficient + "% damage</style>. <style=cIsUtility>Range increases with attack speed</style>.";

            LanguageAPI.Add("MINER_PRIMARY_CRUSH_NAME", "Crush");
            LanguageAPI.Add("MINER_PRIMARY_CRUSH_DESCRIPTION", desc);

            SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            mySkillDef.activationState = new SerializableEntityStateType(typeof(Crush));
            mySkillDef.activationStateMachineName = "Weapon";
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
            mySkillDef.shootDelay = 0.5f;
            mySkillDef.stockToConsume = 1;
            mySkillDef.icon = Assets.icon1;
            mySkillDef.skillDescriptionToken = "MINER_PRIMARY_CRUSH_DESCRIPTION";
            mySkillDef.skillName = "MINER_PRIMARY_CRUSH_NAME";
            mySkillDef.skillNameToken = "MINER_PRIMARY_CRUSH_NAME";
            mySkillDef.keywordTokens = new string[] {
                "KEYWORD_AGILE"
            };

            LoadoutAPI.AddSkillDef(mySkillDef);

            skillLocator.primary = characterPrefab.AddComponent<GenericSkill>();
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


            LoadoutAPI.AddSkill(typeof(Gouge));

            desc = "<style=cIsUtility>Agile.</style> Wildly swing at nearby enemies for <style=cIsDamage>" + 100f * Gouge.damageCoefficient + "% damage</style>.";

            LanguageAPI.Add("MINER_PRIMARY_GOUGE_NAME", "Gouge");
            LanguageAPI.Add("MINER_PRIMARY_GOUGE_DESCRIPTION", desc);

            mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            mySkillDef.activationState = new SerializableEntityStateType(typeof(Gouge));
            mySkillDef.activationStateMachineName = "Weapon";
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
            mySkillDef.shootDelay = 0.5f;
            mySkillDef.stockToConsume = 1;
            mySkillDef.icon = Assets.icon1;
            mySkillDef.skillDescriptionToken = "MINER_PRIMARY_GOUGE_DESCRIPTION";
            mySkillDef.skillName = "MINER_PRIMARY_GOUGE_NAME";
            mySkillDef.skillNameToken = "MINER_PRIMARY_GOUGE_NAME";
            mySkillDef.keywordTokens = new string[] {
                "KEYWORD_AGILE"
            };

            LoadoutAPI.AddSkillDef(mySkillDef);

            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = mySkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
            };
        }

        private void SecondarySetup()
        {
            LoadoutAPI.AddSkill(typeof(DrillChargeStart));
            LoadoutAPI.AddSkill(typeof(DrillCharge));

            string desc = "Charge for 1 second. Dash into enemies for up to <style=cIsDamage>12x" + 100f * DrillCharge.damageCoefficient + "% damage</style>. <style=cIsUtility>You cannot be hit during and following the dash.</style>";

            LanguageAPI.Add("MINER_SECONDARY_CHARGE_NAME", "Drill Charge");
            LanguageAPI.Add("MINER_SECONDARY_CHARGE_DESCRIPTION", desc);

            SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            mySkillDef.activationState = new SerializableEntityStateType(typeof(DrillChargeStart));
            mySkillDef.activationStateMachineName = "Weapon";
            mySkillDef.baseMaxStock = 1;
            mySkillDef.baseRechargeInterval = 7f;
            mySkillDef.beginSkillCooldownOnSkillEnd = true;
            mySkillDef.canceledFromSprinting = false;
            mySkillDef.fullRestockOnAssign = true;
            mySkillDef.interruptPriority = InterruptPriority.Skill;
            mySkillDef.isBullets = false;
            mySkillDef.isCombatSkill = true;
            mySkillDef.mustKeyPress = true;
            mySkillDef.noSprint = false;
            mySkillDef.forceSprintDuringState = true;
            mySkillDef.rechargeStock = 1;
            mySkillDef.requiredStock = 1;
            mySkillDef.shootDelay = 0f;
            mySkillDef.stockToConsume = 1;
            mySkillDef.icon = Assets.icon2;
            mySkillDef.skillDescriptionToken = "MINER_SECONDARY_CHARGE_DESCRIPTION";
            mySkillDef.skillName = "MINER_SECONDARY_CHARGE_NAME";
            mySkillDef.skillNameToken = "MINER_SECONDARY_CHARGE_NAME";

            LoadoutAPI.AddSkillDef(mySkillDef);

            skillLocator.secondary = characterPrefab.AddComponent<GenericSkill>();
            SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
            newFamily.variants = new SkillFamily.Variant[1];
            LoadoutAPI.AddSkillFamily(newFamily);
            skillLocator.secondary.SetFieldValue("_skillFamily", newFamily);
            SkillFamily skillFamily = skillLocator.secondary.skillFamily;

            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = mySkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
            };

            LoadoutAPI.AddSkill(typeof(DrillBreak));

            desc = "Dash forward, exploding for <style=cIsDamage>" + 100f * DrillBreak.damageCoefficient + "% damage</style> on contact with an enemy. <style=cIsUtility>You cannot be hit during and following the dash.</style>";

            LanguageAPI.Add("MINER_SECONDARY_BREAK_NAME", "Crack Hammer");
            LanguageAPI.Add("MINER_SECONDARY_BREAK_DESCRIPTION", desc);

            mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            mySkillDef.activationState = new SerializableEntityStateType(typeof(DrillBreak));
            mySkillDef.activationStateMachineName = "Weapon";
            mySkillDef.baseMaxStock = 1;
            mySkillDef.baseRechargeInterval = 7f;
            mySkillDef.beginSkillCooldownOnSkillEnd = true;
            mySkillDef.canceledFromSprinting = false;
            mySkillDef.fullRestockOnAssign = true;
            mySkillDef.interruptPriority = InterruptPriority.Skill;
            mySkillDef.isBullets = false;
            mySkillDef.isCombatSkill = true;
            mySkillDef.mustKeyPress = true;
            mySkillDef.noSprint = false;
            mySkillDef.forceSprintDuringState = true;
            mySkillDef.rechargeStock = 1;
            mySkillDef.requiredStock = 1;
            mySkillDef.shootDelay = 0f;
            mySkillDef.stockToConsume = 1;
            mySkillDef.icon = Assets.icon2B;
            mySkillDef.skillDescriptionToken = "MINER_SECONDARY_BREAK_DESCRIPTION";
            mySkillDef.skillName = "MINER_SECONDARY_BREAK_NAME";
            mySkillDef.skillNameToken = "MINER_SECONDARY_BREAK_NAME";

            LoadoutAPI.AddSkillDef(mySkillDef);

            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = mySkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
            };
        }

        private void UtilitySetup()
        {
            LoadoutAPI.AddSkill(typeof(BackBlast));

            LanguageAPI.Add("MINER_UTILITY_BACKBLAST_NAME", "Backblast");
            LanguageAPI.Add("MINER_UTILITY_BACKBLAST_DESCRIPTION", "<style=cIsUtility>Stunning.</style> Blast backwards a variable distance, hitting all enemies in a large radius for <style=cIsDamage>" + 100f * BackBlast.damageCoefficient + "% damage</style>. <style=cIsUtility>You cannot be hit while dashing.</style>");

            SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            mySkillDef.activationState = new SerializableEntityStateType(typeof(BackBlast));
            mySkillDef.activationStateMachineName = "Weapon";
            mySkillDef.baseMaxStock = 1;
            mySkillDef.baseRechargeInterval = 5;
            mySkillDef.beginSkillCooldownOnSkillEnd = true;
            mySkillDef.canceledFromSprinting = false;
            mySkillDef.fullRestockOnAssign = true;
            mySkillDef.interruptPriority = InterruptPriority.Skill;
            mySkillDef.isBullets = false;
            mySkillDef.isCombatSkill = true;
            mySkillDef.mustKeyPress = false;
            mySkillDef.noSprint = false;
            mySkillDef.rechargeStock = 1;
            mySkillDef.requiredStock = 1;
            mySkillDef.shootDelay = 0f;
            mySkillDef.stockToConsume = 1;
            mySkillDef.icon = Assets.icon3;
            mySkillDef.skillDescriptionToken = "MINER_UTILITY_BACKBLAST_DESCRIPTION";
            mySkillDef.skillName = "MINER_UTILITY_BACKBLAST_NAME";
            mySkillDef.skillNameToken = "MINER_UTILITY_BACKBLAST_NAME";
            mySkillDef.keywordTokens = new string[] {
                "KEYWORD_STUNNING"
            };

            LoadoutAPI.AddSkillDef(mySkillDef);

            skillLocator.utility = characterPrefab.AddComponent<GenericSkill>();
            SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
            newFamily.variants = new SkillFamily.Variant[1];
            LoadoutAPI.AddSkillFamily(newFamily);
            skillLocator.utility.SetFieldValue("_skillFamily", newFamily);
            SkillFamily skillFamily = skillLocator.utility.skillFamily;

            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = mySkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
            };

            LoadoutAPI.AddSkill(typeof(CaveIn));

            LanguageAPI.Add("MINER_UTILITY_CAVEIN_NAME", "Cave In");
            LanguageAPI.Add("MINER_UTILITY_CAVEIN_DESCRIPTION", "<style=cIsUtility>Stunning.</style> Blast backwards a short distance, <style=cIsUtility>pulling</style> together all enemies in a large radius. You cannot be hit while dashing.");

            mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            mySkillDef.activationState = new SerializableEntityStateType(typeof(CaveIn));
            mySkillDef.activationStateMachineName = "Weapon";
            mySkillDef.baseMaxStock = 1;
            mySkillDef.baseRechargeInterval = 5;
            mySkillDef.beginSkillCooldownOnSkillEnd = true;
            mySkillDef.canceledFromSprinting = false;
            mySkillDef.fullRestockOnAssign = true;
            mySkillDef.interruptPriority = InterruptPriority.Skill;
            mySkillDef.isBullets = false;
            mySkillDef.isCombatSkill = true;
            mySkillDef.mustKeyPress = false;
            mySkillDef.noSprint = false;
            mySkillDef.rechargeStock = 1;
            mySkillDef.requiredStock = 1;
            mySkillDef.shootDelay = 0f;
            mySkillDef.stockToConsume = 1;
            mySkillDef.icon = Assets.icon3B;
            mySkillDef.skillDescriptionToken = "MINER_UTILITY_CAVEIN_DESCRIPTION";
            mySkillDef.skillName = "MINER_UTILITY_CAVEIN_NAME";
            mySkillDef.skillNameToken = "MINER_UTILITY_CAVEIN_NAME";
            mySkillDef.keywordTokens = new string[] {
                "KEYWORD_STUNNING"
            };

            LoadoutAPI.AddSkillDef(mySkillDef);

            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = mySkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
            };
        }

        private void SpecialSetup()
        {
            LoadoutAPI.AddSkill(typeof(ToTheStars));

            LanguageAPI.Add("MINER_SPECIAL_TOTHESTARS_NAME", "To The Stars!");
            LanguageAPI.Add("MINER_SPECIAL_TOTHESTARS_DESCRIPTION", "Jump into the air, shooting a wide spray of projectiles downwards for <style=cIsDamage>39x" + 100f * ToTheStars.damageCoefficient + "% damage</style> total.");

            SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            mySkillDef.activationState = new SerializableEntityStateType(typeof(ToTheStars));
            mySkillDef.activationStateMachineName = "Weapon";
            mySkillDef.baseMaxStock = 1;
            mySkillDef.baseRechargeInterval = 6f;
            mySkillDef.beginSkillCooldownOnSkillEnd = false;
            mySkillDef.canceledFromSprinting = false;
            mySkillDef.fullRestockOnAssign = true;
            mySkillDef.interruptPriority = InterruptPriority.PrioritySkill;
            mySkillDef.isBullets = false;
            mySkillDef.isCombatSkill = true;
            mySkillDef.mustKeyPress = false;
            mySkillDef.noSprint = true;
            mySkillDef.rechargeStock = 1;
            mySkillDef.requiredStock = 1;
            mySkillDef.shootDelay = 0f;
            mySkillDef.stockToConsume = 1;
            mySkillDef.icon = Assets.icon4;
            mySkillDef.skillDescriptionToken = "MINER_SPECIAL_TOTHESTARS_DESCRIPTION";
            mySkillDef.skillName = "MINER_SPECIAL_TOTHESTARS_NAME";
            mySkillDef.skillNameToken = "MINER_SPECIAL_TOTHESTARS_NAME";

            LoadoutAPI.AddSkillDef(mySkillDef);

            skillLocator.special = characterPrefab.AddComponent<GenericSkill>();
            SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
            newFamily.variants = new SkillFamily.Variant[1];
            LoadoutAPI.AddSkillFamily(newFamily);
            skillLocator.special.SetFieldValue("_skillFamily", newFamily);
            SkillFamily skillFamily = skillLocator.special.skillFamily;

            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = mySkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
            };
        }
    }

    public class MenuSound : MonoBehaviour
    {
        private uint playID;

        private void OnEnable()
        {
            this.playID = Util.PlaySound(Sounds.Select, base.gameObject);
        }

        private void OnDestroy()
        {
            AkSoundEngine.StopPlayingID(this.playID);
        }
    }

    public static class Sounds
    {
        public static readonly string Crush = "Crush";
        public static readonly string DrillChargeStart = "DrillCharging";
        public static readonly string DrillCharge = "DrillCharge";
        public static readonly string Backblast = "Backblast";
        public static readonly string ToTheStars = "ToTheStars";

        public static readonly string Swing = "MinerSwing";
        public static readonly string Hit = "MinerHit";
        public static readonly string Select = "MinerSelect";
    }
}