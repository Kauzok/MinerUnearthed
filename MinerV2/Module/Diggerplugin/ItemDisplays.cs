using UnityEngine;
using R2API;
using RoR2;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DiggerPlugin
{
    public static class ItemDisplays {
        internal static ItemDisplayRuleSet itemDisplayRuleSet;
        internal static List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules;

        private static Dictionary<string, GameObject> itemDisplayPrefabs = new Dictionary<string, GameObject>();

        internal static void InitializeItemDisplays() {
            PopulateDisplayPrefabs();

            CharacterModel characterModel = DiggerPlugin.characterPrefabModel;// DiggerPlugin.characterPrefab.GetComponentInChildren<CharacterModel>();

            itemDisplayRuleSet = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
            itemDisplayRuleSet.name = "idrsMiner";

            characterModel.itemDisplayRuleSet = itemDisplayRuleSet;

        }

        internal static void PopulateDisplayPrefabs() {
            PopulateFromBody("Commando");
            PopulateFromBody("Croco");
        }

        private static void PopulateFromBody(string bodyName) {
            ItemDisplayRuleSet itemDisplayRuleSet = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/" + bodyName + "Body").GetComponent<ModelLocator>().modelTransform.GetComponent<CharacterModel>().itemDisplayRuleSet;

            ItemDisplayRuleSet.KeyAssetRuleGroup[] item = itemDisplayRuleSet.keyAssetRuleGroups;

            for (int i = 0; i < item.Length; i++) {
                ItemDisplayRule[] rules = item[i].displayRuleGroup.rules;

                for (int j = 0; j < rules.Length; j++) {
                    GameObject followerPrefab = rules[j].followerPrefab;
                    if (followerPrefab) {
                        string name = followerPrefab.name;
                        string key = (name != null) ? name.ToLower() : null;
                        if (!itemDisplayPrefabs.ContainsKey(key)) {
                            itemDisplayPrefabs[key] = followerPrefab;
                        }
                    }
                }
            }
        }

        private static GameObject LoadDisplay(string name) {
            if (itemDisplayPrefabs.ContainsKey(name.ToLower())) {
                if (itemDisplayPrefabs[name.ToLower()]) return itemDisplayPrefabs[name.ToLower()];
            }
            return null;
        }

        internal static void SetItemDisplays() {

            //how come I couldn't get characterPrefab.GetComponent<CharacterModel>() huh?
            //Debug.LogWarning("norg" + DiggerPlugin.characterPrefabModel.itemDisplayRuleSet.name);// DiggerPlugin.characterPrefab.GetComponent<CharacterModel>().itemDisplayRuleSet.name);
            itemDisplayRules = new List<ItemDisplayRuleSet.KeyAssetRuleGroup>();

            //add item displays here
            SetVanillaDisplays();

            try {
                if (DiggerPlugin.ancientScepterInstalled)
                    fixFuckinScepterDisplay();
            }
            catch (System.Exception e) {
                //DiggerPlugin.logger.LogWarning($"could not load fixed displays for ancientScepter\nWARNING: DISPLAY WILL BE HUGE\n{e}\n");
            }
            try {
                if (DiggerPlugin.goldenCoastInstalled)
                    fixFuckinGoaldCoastDisplays();
            }
            catch (System.Exception e) {
                //DiggerPlugin.logger.LogWarning($"could not load fixed displays for goldenCoast\nWARNING: DISPLAYS WILL BE HUGE\n{e}\n");
            }

            try {
                if (DiggerPlugin.supplyDropInstalled)
                    AddSupplyDropDisplays();
            }
            catch (System.Exception e) {
                //DiggerPlugin.logger.LogWarning($"could not load displays for supplydrop\n{e}\n");
            }

            try {
                if (DiggerPlugin.sivsItemsInstalled)
                    AddSivsItemsDisplays();
            }
            catch (System.Exception e) {
                //DiggerPlugin.logger.LogWarning($"could not load displays for sivsitems\n{e}\n");
            }

            //try {
            //    if (DiggerPlugin.aetheriumInstalled)
            //        AddAetheriumDisplays();
            //}
            //catch (System.Exception e) {
            //    DiggerPlugin.logger.LogWarning($"could not load displays for aetherium\n{e}\n");
            //}


            //apply displays here

            itemDisplayRuleSet.keyAssetRuleGroups = itemDisplayRules.ToArray();
            itemDisplayRuleSet.GenerateRuntimeValues();
        }

        private static void SetVanillaDisplays() {

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Equipment.Jetpack,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBugWings"),
                            childName = "Chest",
                            localPos = new Vector3(0, 0.004f, -0.002f),
                            localAngles = new Vector3(45, 0, 0),
                            localScale = new Vector3(0.001f, 0.001f, 0.001f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Equipment.GoldGat,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGoldGat"),
                            childName = "Chest",
                            localPos = new Vector3(0.0024f, 0.0075f, 0),
                            localAngles = new Vector3(0, 90, -45),
                            localScale = new Vector3(0.0015f, 0.0015f, 0.0015f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Equipment.BFG,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBFG"),
                            childName = "Chest",
                            localPos = new Vector3(0.002f, 0.004f, 0),
                            localAngles = new Vector3(0, 0, 330),
                            localScale = new Vector3(0.003f, 0.003f, 0.003f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.CritGlasses,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGlasses"),
                            childName = "Head",
                            localPos = new Vector3(0, 0.0016f, -0.0014f),
                            localAngles = new Vector3(0, 180, 0),
                            localScale = new Vector3(0.003f, 0.003f, 0.001f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = DLC1Content.Items.CritGlassesVoid,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGlassesVoid"),
                            childName = "Head",
                            localPos = new Vector3(0, 0.0016f, -0.0014f),
                            localAngles = new Vector3(0, 180, 0),
                            localScale = new Vector3(0.003f, 0.003f, 0.001f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.Syringe,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySyringeCluster"),
                            childName = "Chest",
                            localPos = new Vector3(0.002f, 0.0048f, 0),
                            localAngles = new Vector3(30, 90, 0),
                            localScale = new Vector3(0.0015f, 0.0015f, 0.0015f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.Behemoth,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBehemoth"),
                            childName = "Pelvis",
                            localPos = new Vector3(0, -0.002f, 0.0015f),
                            localAngles = new Vector3(90, 180, 0),
                            localScale = new Vector3(0.001f, 0.001f, 0.001f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.Missile,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMissileLauncher"),
                            childName = "Chest",
                            localPos = new Vector3(-0.002f, 0.008f, 0),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.001f, 0.001f, 0.001f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = DLC1Content.Items.MissileVoid,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMissileLauncherVoid"),
                            childName = "Chest",
                            localPos = new Vector3(-0.002f, 0.008f, 0),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.001f, 0.001f, 0.001f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.Dagger,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDagger"),
                            childName = "Chest",
                            localPos = new Vector3(0, 0.005f, 0),
                            localAngles = new Vector3(0, 0, 45),
                            localScale = new Vector3(0.01f, 0.01f, 0.01f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.Hoof,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayHoof"),
                            childName = "KneeL",
                            localPos = new Vector3(0, 0.002f, -0.0006f),
                            localAngles = new Vector3(70, 0, 0),
                            localScale = new Vector3(0.0011f, 0.0014f, 0.0008f),
                            limbMask = LimbFlags.None
                        },

                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.LimbMask,
                            childName = "KneeL",
                            limbMask = LimbFlags.LeftLeg
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.ChainLightning,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayUkulele"),
                            childName = "PickR",
                            localPos = new Vector3(-0.003f, 0.001f, 0),
                            localAngles = new Vector3(0, 0, 270),
                            localScale = new Vector3(0.008f, 0.008f, 0.008f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = DLC1Content.Items.ChainLightningVoid,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayUkuleleVoid"),
                            childName = "PickR",
                            localPos = new Vector3(-0.003f, 0.001f, 0),
                            localAngles = new Vector3(0, 0, 270),
                            localScale = new Vector3(0.008f, 0.008f, 0.008f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.GhostOnKill,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMask"),
                            childName = "Head",
                            localPos = new Vector3(0, 0.0014f, -0.0012f),
                            localAngles = new Vector3(0, 180, 0),
                            localScale = new Vector3(0.006f, 0.006f, 0.006f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.Mushroom,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMushroom"),
                            childName = "ShoulderR",
                            localPos = new Vector3(0, 0, 0),
                            localAngles = new Vector3(0, 0, 130),
                            localScale = new Vector3(0.001f, 0.001f, 0.001f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.AttackSpeedOnCrit,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWolfPelt"),
                            childName = "KneeR",
                            localPos = new Vector3(0.00023F, 0.00035F, -0.0005F),
                            localAngles = new Vector3(304.9414F, 175.5463F, 185.4277F),
                            localScale = new Vector3(0.00395F, 0.0038F, 0.0038F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.BleedOnHit,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayTriTip"),
                            childName = "Chest",
                            localPos = new Vector3(0, 0.0025f, 0.0025f),
                            localAngles = new Vector3(0, 180, 0),
                            localScale = new Vector3(0.01f, 0.01f, 0.01f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = DLC1Content.Items.BleedOnHitVoid,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayTriTipVoid"),
                            childName = "Chest",
                            localPos = new Vector3(0, 0.0025f, 0.0025f),
                            localAngles = new Vector3(0, 180, 0),
                            localScale = new Vector3(0.01f, 0.01f, 0.01f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.WardOnLevel,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWarbanner"),
                            childName = "Chest",
                            localPos = new Vector3(0.0003f, 0,-0.0015f),
                            localAngles = new Vector3(0, 0, 90),
                            localScale = new Vector3(0.005f, 0.005f, 0.005f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.HealOnCrit,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayScythe"),
                            childName = "PickR",
                            localPos = new Vector3(0, 0.0015f, 0),
                            localAngles = new Vector3(0, 90, 270),
                            localScale = new Vector3(0.005f, 0.005f, 0.005f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.HealWhileSafe,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySnail"),
                            childName = "ShoulderL",
                            localPos = new Vector3(0, -0.0015f, -0.0005f),
                            localAngles = new Vector3(45, 180, 180),
                            localScale = new Vector3(0.001f, 0.001f, 0.001f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.Clover,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayClover"),
                            childName = "ShoulderL",
                            localPos = new Vector3(0, 0.0025f, 0.0005f),
                            localAngles = new Vector3(90, 0, 0),
                            localScale = new Vector3(0.005f, 0.005f, 0.005f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = DLC1Content.Items.CloverVoid,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayCloverVoid"),
                            childName = "ShoulderL",
                            localPos = new Vector3(0, 0.0025f, 0.0005f),
                            localAngles = new Vector3(90, 0, 0),
                            localScale = new Vector3(0.005f, 0.005f, 0.005f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.BarrierOnOverHeal,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAegis"),
                            childName = "ElbowL",
                            localPos = new Vector3(0, 0, 0.001f),
                            localAngles = new Vector3(90, 180, 0),
                            localScale = new Vector3(0.0035f, 0.0035f, 0.0035f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.GoldOnHit,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBoneCrown"),
                            childName = "Head",
                            localPos = new Vector3(0, 0.002f, 0),
                            localAngles = new Vector3(0, 180, 0),
                            localScale = new Vector3(0.01f, 0.01f, 0.01f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.WarCryOnMultiKill,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayPauldron"),
                            childName = "ShoulderL",
                            localPos = new Vector3(0, 0, 0.001f),
                            localAngles = new Vector3(110, 0, 0),
                            localScale = new Vector3(0.01f, 0.01f, 0.01f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.SprintArmor,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBuckler"),
                            childName = "ElbowR",
                            localPos = new Vector3(0, 0.0015f, 0.001f),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.002f, 0.002f, 0.002f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.IceRing,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayIceRing"),
                            childName = "PickL",
                            localPos = new Vector3(-0.0016f, 0.0013f, -0.0001f),
                            localAngles = new Vector3(0, 90, 0),
                            localScale = new Vector3(0.005f, 0.005f, 0.005f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.FireRing,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFireRing"),
                            childName = "PickR",
                            localPos = new Vector3(0.0014f, 0.00122f, -0.0003f),
                            localAngles = new Vector3(0, 90, 0),
                            localScale = new Vector3(0.005f, 0.005f, 0.005f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.UtilitySkillMagazine,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAfterburnerShoulderRing"),
                            childName = "ShoulderL",
                            localPos = new Vector3(0, 0, -0.002f),
                            localAngles = new Vector3(-90, 0, 0),
                            localScale = new Vector3(0.01f, 0.01f, 0.01f),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAfterburnerShoulderRing"),
                            childName = "ShoulderR",
                            localPos = new Vector3(0, 0, -0.002f),
                            localAngles = new Vector3(-90, 0, 0),
                            localScale = new Vector3(0.01f, 0.01f, 0.01f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.JumpBoost,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWaxBird"),
                            childName = "Head",
                            localPos = new Vector3(0, -0.0035f, 0),
                            localAngles = new Vector3(0, 180, 0),
                            localScale = new Vector3(0.01f, 0.01f, 0.01f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.ArmorReductionOnHit,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWarhammer"),
                            childName = "PickR",
                            localPos = new Vector3(0.005f, 0.0015f, 0),
                            localAngles = new Vector3(0, 90, 90),
                            localScale = new Vector3(0.005f, 0.005f, 0.005f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.NearbyDamageBonus,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDiamond"),
                            childName = "PickL",
                            localPos = new Vector3(-0.0024f, 0.0013f, 0),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.0015f, 0.0015f, 0.0015f),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDiamond"),
                            childName = "PickR",
                            localPos = new Vector3(0.002f, 0.0013f, -0.0005f),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.0015f, 0.0015f, 0.0015f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.ArmorPlate,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayRepulsionArmorPlate"),
                            childName = "LegL",
                            localPos = new Vector3(-0.0008f, 0.002f, 0),
                            localAngles = new Vector3(90, 90, 0),
                            localScale = new Vector3(0.002f, 0.002f, 0.002f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Equipment.CommandMissile,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMissileRack"),
                            childName = "Chest",
                            localPos = new Vector3(0, 0.004f, -0.002f),
                            localAngles = new Vector3(90, 180, 0),
                            localScale = new Vector3(0.005f, 0.005f, 0.005f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.Feather,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFeather"),
                            childName = "ElbowL",
                            localPos = new Vector3(0, 0.0015f, 0),
                            localAngles = new Vector3(-90, -90, 0),
                            localScale = new Vector3(0.0004f, 0.0004f, 0.0004f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.Crowbar,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayCrowbar"),
                            childName = "PickL",
                            localPos = new Vector3(-0.003f, 0.001f, 0.0005f),
                            localAngles = new Vector3(5, 90, 355),
                            localScale = new Vector3(0.005f, 0.005f, 0.005f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.FallBoots,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGravBoots"),
                            childName = "FootR",
                            localPos = new Vector3(0, 0, 0.0005f),
                            localAngles = new Vector3(90, 0, 0),
                            localScale = new Vector3(0.002f, 0.002f, 0.002f),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGravBoots"),
                            childName = "FootL",
                            localPos = new Vector3(0, 0, 0.0005f),
                            localAngles = new Vector3(90, 0, 0),
                            localScale = new Vector3(0.002f, 0.002f, 0.002f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.ExecuteLowHealthElite,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGuillotine"),
                            childName = "LegR",
                            localPos = new Vector3(0, 0, 0.0015f),
                            localAngles = new Vector3(90, 0, 0),
                            localScale = new Vector3(0.003f, 0.003f, 0.003f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.EquipmentMagazine,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBattery"),
                            childName = "Chest",
                            localPos = new Vector3(0.0004f, 0.0026f, 0.002f),
                            localAngles = new Vector3(0, -90, 0),
                            localScale = new Vector3(0.002f, 0.002f, 0.002f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.NovaOnHeal,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDevilHorns"),
                            childName = "Head",
                            localPos = new Vector3(-0.0005f, 0, 0),
                            localAngles = new Vector3(0, 180, 20),
                            localScale = new Vector3(0.0075f, 0.0075f, 0.0075f),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDevilHorns"),
                            childName = "Head",
                            localPos = new Vector3(0.0005f, 0, 0),
                            localAngles = new Vector3(0, 180, 340),
                            localScale = new Vector3(-0.0075f, 0.0075f, 0.0075f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.Infusion,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayInfusion"),
                            childName = "Pelvis",
                            localPos = new Vector3(-0.0015f, 0.001f, -0.001f),
                            localAngles = new Vector3(0, 45, 0),
                            localScale = new Vector3(0.006f, 0.006f, 0.006f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.Medkit,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMedkit"),
                            childName = "Chest",
                            localPos = new Vector3(0, -0.0005f, -0.001f),
                            localAngles = new Vector3(290, 180, 0),
                            localScale = new Vector3(0.008f, 0.008f, 0.008f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.Bandolier,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBandolier"),
                            childName = "Chest",
                            localPos = new Vector3(0, 0.0028f, -0.0004f),
                            localAngles = new Vector3(45, 270, 90),
                            localScale = new Vector3(0.008f, 0.008f, 0.008f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.BounceNearby,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayHook"),
                            childName = "Chest",
                            localPos = new Vector3(0, 0.0055f, -0.0015f),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.005f, 0.005f, 0.005f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.IgniteOnKill,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGasoline"),
                            childName = "LegL",
                            localPos = new Vector3(0, 0.002f, 0.0015f),
                            localAngles = new Vector3(90, 90, 0),
                            localScale = new Vector3(0.0075f, 0.0075f, 0.0075f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.StunChanceOnHit,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayStunGrenade"),
                            childName = "LegR",
                            localPos = new Vector3(0.001f, 0.002f, 0),
                            localAngles = new Vector3(90, 0, 0),
                            localScale = new Vector3(0.01f, 0.01f, 0.01f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.Firework,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFirework"),
                            childName = "Pelvis",
                            localPos = new Vector3(-0.0015f, 0.001f, 0.0015f),
                            localAngles = new Vector3(288, 282, 80),
                            localScale = new Vector3(0.0025f, 0.0025f, 0.0025f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.LunarDagger,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayLunarDagger"),
                            childName = "Chest",
                            localPos = new Vector3(-0.0015f, 0.005f, -0.002f),
                            localAngles = new Vector3(45, 120, 90),
                            localScale = new Vector3(0.005f, 0.005f, 0.005f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.Knurl,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayKnurl"),
                            childName = "Chest",
                            localPos = new Vector3(-0.001f, 0.0035f, 0.0015f),
                            localAngles = new Vector3(0, 116, 0),
                            localScale = new Vector3(0.001f, 0.001f, 0.001f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.BeetleGland,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBeetleGland"),
                            childName = "Chest",
                            localPos = new Vector3(0.0035f, 0.005f, 0),
                            localAngles = new Vector3(270, 45, 0),
                            localScale = new Vector3(0.0015f, 0.0015f, 0.0015f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.SprintBonus,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySoda"),
                            childName = "Pelvis",
                            localPos = new Vector3(0.002f, 0.001f, 0),
                            localAngles = new Vector3(270, 0, 0),
                            localScale = new Vector3(0.005f, 0.005f, 0.005f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.SecondarySkillMagazine,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDoubleMag"),
                            childName = "Chest",
                            localPos = new Vector3(0.0015f, 0.004f, -0.003f),
                            localAngles = new Vector3(315, 0, 0),
                            localScale = new Vector3(0.001f, 0.001f, 0.001f),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDoubleMag"),
                            childName = "Chest",
                            localPos = new Vector3(-0.0015f, 0.004f, -0.003f),
                            localAngles = new Vector3(315, 0, 0),
                            localScale = new Vector3(-0.001f, 0.001f, 0.001f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.StickyBomb,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayStickyBomb"),
                            childName = "Pelvis",
                            localPos = new Vector3(-0.002f, 0.002f, 0),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.003f, 0.003f, 0.003f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.TreasureCache,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayKey"),
                            childName = "Pelvis",
                            localPos = new Vector3(0.001f, 0.001f, 0.0015f),
                            localAngles = new Vector3(0, 315, 90),
                            localScale = new Vector3(0.015f, 0.015f, 0.015f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.BossDamageBonus,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAPRound"),
                            childName = "Pelvis",
                            localPos = new Vector3(-0.001f, 0, 0.0015f),
                            localAngles = new Vector3(90, 315, 0),
                            localScale = new Vector3(0.01f, 0.01f, 0.01f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.SlowOnHit,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBauble"),
                            childName = "Pelvis",
                            localPos = new Vector3(0.0045f, -0.003f, -0.002f),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.005f, 0.005f, 0.005f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.ExtraLife,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayHippo"),
                            childName = "Chest",
                            localPos = new Vector3(0, 0.003f, 0.002f),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.004f, 0.004f, 0.004f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = DLC1Content.Items.ExtraLifeVoid,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayHippoVoid"),
                            childName = "Chest",
                            localPos = new Vector3(0, 0.003f, 0.002f),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.004f, 0.004f, 0.004f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.KillEliteFrenzy,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBrainstalk"),
                            childName = "Head",
                            localPos = new Vector3(-0.00002F, 0.00038F, 0.00013F),
                            localAngles = new Vector3(12.43669F, 359.5866F, 0F),
                            localScale = new Vector3(0.00215F, 0.00655F, 0.00203F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.RepeatHeal,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayCorpseFlower"),
                            childName = "ShoulderR",
                            localPos = new Vector3(-0.001f, 0, 0),
                            localAngles = new Vector3(270, 90, 0),
                            localScale = new Vector3(0.005f, 0.005f, 0.005f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.AutoCastEquipment,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFossil"),
                            childName = "Chest",
                            localPos = new Vector3(-0.0023f, 0.0015f, 0),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.005f, 0.005f, 0.005f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.IncreaseHealing,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAntler"),
                            childName = "Head",
                            localPos = new Vector3(0.0005f, 0.002f, 0),
                            localAngles = new Vector3(0, 90, 0),
                            localScale = new Vector3(0.005f, 0.005f, 0.005f),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAntler"),
                            childName = "Head",
                            localPos = new Vector3(-0.0005f, 0.002f, 0),
                            localAngles = new Vector3(0, 90, 0),
                            localScale = new Vector3(0.005f, 0.005f, -0.005f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.TitanGoldDuringTP,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGoldHeart"),
                            childName = "Chest",
                            localPos = new Vector3(-0.0016f, 0.0035f, 0.002f),
                            localAngles = new Vector3(0, 0, 285),
                            localScale = new Vector3(0.002f, 0.002f, 0.002f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.SprintWisp,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBrokenMask"),
                            childName = "ShoulderL",
                            localPos = new Vector3(0, 0.0005f, 0.001f),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.0025f, 0.0025f, 0.0025f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.BarrierOnKill,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBrooch"),
                            childName = "Chest",
                            localPos = new Vector3(-0.002f, 0.0015f, 0.001f),
                            localAngles = new Vector3(45, 45, 90),
                            localScale = new Vector3(0.01f, 0.01f, 0.01f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.TPHealingNova,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGlowFlower"),
                            childName = "ShoulderL",
                            localPos = new Vector3(0.0015f, 0, 0),
                            localAngles = new Vector3(30, 0, 0),
                            localScale = new Vector3(0.005f, 0.005f, 0.005f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.LunarUtilityReplacement,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBirdFoot"),
                            childName = "Head",
                            localPos = new Vector3(0, 0.002f, 0.002f),
                            localAngles = new Vector3(0, 90, 0),
                            localScale = new Vector3(0.005f, 0.005f, 0.005f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.Thorns,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayRazorwireLeft"),
                            childName = "ElbowR",
                            localPos = new Vector3(-0.0006f, 0, 0),
                            localAngles = new Vector3(270, 0, 0),
                            localScale = new Vector3(0.004f, 0.006f, 0.005f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.LunarPrimaryReplacement,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBirdEye"),
                            childName = "Head",
                            localPos = new Vector3(0, 0.0015f, -0.001f),
                            localAngles = new Vector3(0, 90, 90),
                            localScale = new Vector3(0.003f, 0.0035f, 0.005f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.NovaOnLowHealth,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayJellyGuts"),
                            childName = "Head",
                            localPos = new Vector3(0.0008f, -0.001f, 0.001f),
                            localAngles = new Vector3(319, 180, 0),
                            localScale = new Vector3(0.002f, 0.002f, 0.002f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.LunarTrinket,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBeads"),
                            childName = "ElbowL",
                            localPos = new Vector3(-0.00075f, 0.001f, 0),
                            localAngles = new Vector3(0, 0, 90),
                            localScale = new Vector3(0.015f, 0.015f, 0.015f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.Plant,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayInterstellarDeskPlant"),
                            childName = "ShoulderR",
                            localPos = new Vector3(0, -0.0015f, -0.001f),
                            localAngles = new Vector3(25, 0, 0),
                            localScale = new Vector3(0.0015f, 0.0015f, 0.0015f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.Bear,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBear"),
                            childName = "Chest",
                            localPos = new Vector3(0, 0.002f, 0.002f),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.0035f, 0.0035f, 0.0035f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = DLC1Content.Items.BearVoid,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBearVoid"),
                            childName = "Chest",
                            localPos = new Vector3(0, 0.002f, 0.002f),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.0035f, 0.0035f, 0.0035f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.DeathMark,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDeathMark"),
                            childName = "HandL",
                            localPos = new Vector3(0, 0.001f, 0.0005f),
                            localAngles = new Vector3(90, 180, 0),
                            localScale = new Vector3(0.00035f, 0.00035f, 0.00035f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.ExplodeOnDeath,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWilloWisp"),
                            childName = "Pelvis",
                            localPos = new Vector3(-0.002f, 0.001f, 0),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.0006f, 0.0006f, 0.0006f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.Seed,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySeed"),
                            childName = "Chest",
                            localPos = new Vector3(0, 0.005f, -0.002f),
                            localAngles = new Vector3(270, 45, 0),
                            localScale = new Vector3(0.0006f, 0.0006f, 0.0006f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.SprintOutOfCombat,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWhip"),
                            childName = "Pelvis",
                            localPos = new Vector3(0.0018f, 0, -0.001f),
                            localAngles = new Vector3(0, 45, 15),
                            localScale = new Vector3(0.005f, 0.005f, 0.0025f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            /*itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.CooldownOnCrit,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySkull"),
                            childName = "Chest",
                            localPos = new Vector3(0, 0.0035f, 0.0015f),
                            localAngles = new Vector3(270, 0, 0),
                            localScale = new Vector3(0.0035f, 0.0035f, 0.0035f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });*/

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.Phasing,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayStealthkit"),
                            childName = "KneeL",
                            localPos = new Vector3(0, 0.002f, -0.001f),
                            localAngles = new Vector3(90, 0, 0),
                            localScale = new Vector3(0.0025f, 0.005f, 0.005f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.PersonalShield,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayShieldGenerator"),
                            childName = "Chest",
                            localPos = new Vector3(0, 0.0015f, 0.002f),
                            localAngles = new Vector3(45, 90, 270),
                            localScale = new Vector3(0.002f, 0.002f, 0.002f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.ShockNearby,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayTeslaCoil"),
                            childName = "Chest",
                            localPos = new Vector3(0, 0.005f, -0.002f),
                            localAngles = new Vector3(315, 0, 0),
                            localScale = new Vector3(0.005f, 0.005f, 0.005f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.ShieldOnly,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayShieldBug"),
                            childName = "Head",
                            localPos = new Vector3(-0.001f, 0.002f, 0),
                            localAngles = new Vector3(0, 90, 0),
                            localScale = new Vector3(0.005f, 0.005f, 0.005f),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayShieldBug"),
                            childName = "Head",
                            localPos = new Vector3(0.001f, 0.002f, 0),
                            localAngles = new Vector3(0, 90, 0),
                            localScale = new Vector3(0.005f, 0.005f, -0.005f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.AlienHead,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAlienHead"),
                            childName = "PickR",
                            localPos = new Vector3(-0.003f, 0.001f, 0.001f),
                            localAngles = new Vector3(30, 90, 0),
                            localScale = new Vector3(0.015f, 0.015f, 0.015f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.HeadHunter,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySkullCrown"),
                            childName = "Head",
                            localPos = new Vector3(0, 0.0024f, 0),
                            localAngles = new Vector3(0, 180, 0),
                            localScale = new Vector3(0.004f, 0.0015f, 0.001f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.EnergizedOnEquipmentUse,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWarHorn"),
                            childName = "Pelvis",
                            localPos = new Vector3(-0.003f, 0.0015f, 0),
                            localAngles = new Vector3(0, 0, 90),
                            localScale = new Vector3(0.005f, 0.005f, 0.005f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.FlatHealth,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySteakCurved"),
                            childName = "Chest",
                            localPos = new Vector3(0, 0.0022f, 0.002f),
                            localAngles = new Vector3(-45, 0, 0),
                            localScale = new Vector3(0.001f, 0.001f, 0.001f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.Tooth,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayToothMeshLarge"),
                            childName = "Head",
                            localPos = new Vector3(0, 0, -0.0003f),
                            localAngles = new Vector3(45, 0, 0),
                            localScale = new Vector3(0.1f, 0.1f, 0.1f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.Pearl,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayPearl"),
                            childName = "HandR",
                            localPos = new Vector3(0, 0, 0),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.001f, 0.001f, 0.001f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.ShinyPearl,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayShinyPearl"),
                            childName = "HandL",
                            localPos = new Vector3(0, 0, 0),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.001f, 0.001f, 0.001f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.BonusGoldPackOnKill,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayTome"),
                            childName = "LegR",
                            localPos = new Vector3(0, 0.002f, 0.0015f),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.001f, 0.001f, 0.001f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.Squid,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySquidTurret"),
                            childName = "Head",
                            localPos = new Vector3(0.00008F, 0.00143F, -0.00093F),
                            localAngles = new Vector3(278.1043F, 48.54117F, 309.125F),
                            localScale = new Vector3(0.00129F, 0.00149F, 0.00134F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.Icicle,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFrostRelic"),
                            childName = "Base",
                            localPos = new Vector3(0.01f, 0.015f, 0.015f),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(1, 1, 1),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.Talisman,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayTalisman"),
                            childName = "Base",
                            localPos = new Vector3(-0.01f, 0.01f, 0.015f),
                            localAngles = new Vector3(90, 0, 0),
                            localScale = new Vector3(1, 1, 1),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.LaserTurbine,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayLaserTurbine"),
                            childName = "Chest",
                            localPos = new Vector3(0, 0.002f, -0.002f),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.0035f, 0.0035f, 0.0035f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.FocusConvergence,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFocusedConvergence"),
                            childName = "Base",
                            localPos = new Vector3(-0.0075f, 0.015f, 0.01f),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.1f, 0.1f, 0.1f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            /*itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.Incubator,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAncestralIncubator"),
                            childName = "Chest",
                            localPos = new Vector3(0, 0.004f, 0),
                            localAngles = new Vector3(315, 0, 0),
                            localScale = new Vector3(0.001f, 0.001f, 0.001f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });*/

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.FireballsOnHit,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFireballsOnHit"),
                            childName = "PickR",
                            localPos = new Vector3(0.00325f, 0.004f, -0.0008f),
                            localAngles = new Vector3(-78, 270, 0),
                            localScale = new Vector3(0.0015f, 0.0008f, 0.0012f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.SiphonOnLowHealth,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySiphonOnLowHealth"),
                            childName = "Pelvis",
                            localPos = new Vector3(0.0025f, 0, -0.0015f),
                            localAngles = new Vector3(0, 120, 0),
                            localScale = new Vector3(0.0015f, 0.0015f, 0.0015f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.BleedOnHitAndExplode,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBleedOnHitAndExplode"),
                            childName = "LegR",
                            localPos = new Vector3(-0.001f, 0.002f, 0),
                            localAngles = new Vector3(0, 0, 180),
                            localScale = new Vector3(0.001f, 0.001f, 0.001f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.MonstersOnShrineUse,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMonstersOnShrineUse"),
                            childName = "LegR",
                            localPos = new Vector3(0, 0, 0.0015f),
                            localAngles = new Vector3(0, 270, 0),
                            localScale = new Vector3(0.0015f, 0.0015f, 0.0015f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Items.RandomDamageZone,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayRandomDamageZone"),
                            childName = "HandL",
                            localPos = new Vector3(0, 0.001f, 0.001f),
                            localAngles = new Vector3(0, 180, 90),
                            localScale = new Vector3(0.0015f, 0.0005f, 0.001f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Equipment.Fruit,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFruit"),
                            childName = "Chest",
                            localPos = new Vector3(-0.002f, 0.001f, 0),
                            localAngles = new Vector3(-45, -45, 0),
                            localScale = new Vector3(0.003f, 0.003f, 0.003f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Equipment.AffixRed,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteHorn"),
                            childName = "Head",
                            localPos = new Vector3(-0.001f, 0.002f, 0),
                            localAngles = new Vector3(0, 180, 0),
                            localScale = new Vector3(0.001f, 0.001f, 0.001f),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteHorn"),
                            childName = "Head",
                            localPos = new Vector3(0.001f, 0.002f, 0),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.001f, 0.001f, -0.001f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Equipment.AffixBlue,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteRhinoHorn"),
                            childName = "Head",
                            localPos = new Vector3(0, 0.0024f, -0.0015f),
                            localAngles = new Vector3(-45, 180, 0),
                            localScale = new Vector3(0.004f, 0.004f, 0.004f),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteRhinoHorn"),
                            childName = "Head",
                            localPos = new Vector3(0, 0.0024f, -0.0005f),
                            localAngles = new Vector3(-60, 180, 0),
                            localScale = new Vector3(0.003f, 0.003f, 0.003f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Equipment.AffixWhite,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteIceCrown"),
                            childName = "Head",
                            localPos = new Vector3(0, 0.003f, 0),
                            localAngles = new Vector3(-90, 180, 0),
                            localScale = new Vector3(0.0003f, 0.0003f, 0.0003f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Equipment.AffixPoison,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteUrchinCrown"),
                            childName = "Head",
                            localPos = new Vector3(0, 0.002f, 0),
                            localAngles = new Vector3(-90, 180, 0),
                            localScale = new Vector3(0.0006f, 0.0006f, 0.0006f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Equipment.AffixHaunted,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteStealthCrown"),
                            childName = "Head",
                            localPos = new Vector3(0, 0.003f, 0),
                            localAngles = new Vector3(-90, 180, 0),
                            localScale = new Vector3(0.0006f, 0.0006f, 0.0006f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Equipment.CritOnUse,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayNeuralImplant"),
                            childName = "Head",
                            localPos = new Vector3(0, 0.0015f, -0.0025f),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.003f, 0.003f, 0.003f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Equipment.DroneBackup,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayRadio"),
                            childName = "Pelvis",
                            localPos = new Vector3(0.002f, 0.0014f, 0),
                            localAngles = new Vector3(340, 90, 0),
                            localScale = new Vector3(0.005f, 0.005f, 0.005f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Equipment.Lightning,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayLightningArmRight"), //ItemDisplays.capacitorPrefab,
                            childName = "Chest",
                            localPos = new Vector3(0, 0, 0),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.01f, 0.01f, 0.01f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Equipment.BurnNearby,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayPotion"),
                            childName = "Pelvis",
                            localPos = new Vector3(0.0025f, 0, 0),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.0005f, 0.0005f, 0.0005f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Equipment.CrippleWard,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEffigy"),
                            childName = "Pelvis",
                            localPos = new Vector3(0.0025f, -0.00015f, 0),
                            localAngles = new Vector3(0, 270, 0),
                            localScale = new Vector3(0.008f, 0.008f, 0.008f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Equipment.QuestVolatileBattery,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBatteryArray"),
                            childName = "Chest",
                            localPos = new Vector3(0, 0.004f, -0.003f),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.003f, 0.003f, 0.003f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Equipment.GainArmor,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayElephantFigure"),
                            childName = "KneeR",
                            localPos = new Vector3(0, 0.002f, 0.0016f),
                            localAngles = new Vector3(80, 0, 0),
                            localScale = new Vector3(0.005f, 0.005f, 0.005f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Equipment.Recycle,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayRecycler"),
                            childName = "Chest",
                            localPos = new Vector3(0, 0.003f, -0.0028f),
                            localAngles = new Vector3(0, 90, 0),
                            localScale = new Vector3(0.001f, 0.001f, 0.001f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Equipment.FireBallDash,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEgg"),
                            childName = "Pelvis",
                            localPos = new Vector3(0.0025f, 0, 0),
                            localAngles = new Vector3(270, 0, 0),
                            localScale = new Vector3(0.005f, 0.005f, 0.005f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Equipment.Cleanse,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWaterPack"),
                            childName = "Chest",
                            localPos = new Vector3(0, 0.003f, -0.0024f),
                            localAngles = new Vector3(0, 180, 0),
                            localScale = new Vector3(0.001f, 0.001f, 0.001f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Equipment.Tonic,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayTonic"),
                            childName = "Pelvis",
                            localPos = new Vector3(0.0025f, 0, 0),
                            localAngles = new Vector3(335, 90, 0),
                            localScale = new Vector3(0.003f, 0.003f, 0.003f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Equipment.Gateway,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayVase"),
                            childName = "Pelvis",
                            localPos = new Vector3(0.0025f, 0.0015f, 0),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.002f, 0.002f, 0.002f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Equipment.Meteor,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMeteor"),
                            childName = "Base",
                            localPos = new Vector3(0, 0.02f, 0.015f),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(1.5f, 1.5f, 1.5f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Equipment.Saw,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySawmerang"),
                            childName = "Base",
                            localPos = new Vector3(0, 0.025f, 0.02f),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.25f, 0.25f, 0.25f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Equipment.Blackhole,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGravCube"),
                            childName = "Base",
                            localPos = new Vector3(0, 0.02f, 0.015f),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(1, 1, 1),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Equipment.Scanner,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayScanner"),
                            childName = "Pelvis",
                            localPos = new Vector3(0.002f, 0, 0.001f),
                            localAngles = new Vector3(270, 90, 0),
                            localScale = new Vector3(0.002f, 0.002f, 0.002f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Equipment.DeathProjectile,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDeathProjectile"),
                            childName = "Pelvis",
                            localPos = new Vector3(0.002f, -0.0005f, -0.002f),
                            localAngles = new Vector3(0, 145, 0),
                            localScale = new Vector3(0.0015f, 0.0015f, 0.0015f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Equipment.LifestealOnHit,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayLifestealOnHit"),
                            childName = "Head",
                            localPos = new Vector3(-0.002f, 0.004f, 0),
                            localAngles = new Vector3(25, 90, 0),
                            localScale = new Vector3(0.0015f, 0.0015f, 0.0015f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = RoR2Content.Equipment.TeamWarCry,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayTeamWarCry"),
                            childName = "Pelvis",
                            localPos = new Vector3(0, 0, 0.003f),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.001f, 0.001f, 0.001f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

        }

        #region scept
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void fixFuckinScepterDisplay() {
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = AncientScepter.AncientScepterItem.instance.ItemDef,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = AncientScepter.AncientScepterItem.displayPrefab,
                            childName = "Pelvis",
                            localPos = new Vector3(0.00042F, 0.00097F, -0.0014F),
                            localAngles = new Vector3(333.2843F, 198.8161F, 165.1177F),
                            localScale = new Vector3(0.00224F, 0.00224F, 0.00224F),
                            limbMask = LimbFlags.None
                        }
        }
                }
            });
        }
        #endregion

        #region aeth
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void AddAetheriumDisplays() {
                itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                    keyAsset = ItemDisplays.LoadAetheriumKeyAsset("AccursedPotion"),
                    displayRuleGroup = new DisplayRuleGroup {
                        rules = new ItemDisplayRule[]
                        {
                            new ItemDisplayRule
                            {
                                ruleType = ItemDisplayRuleType.ParentedPrefab,
                                followerPrefab = ItemDisplays.LoadAetheriumDisplay("AccursedPotion"),
                                childName = "Pelvis",
                                localPos = new Vector3(-0.0025f, 0.0015f, 0),
                                localAngles = new Vector3(0, 0, 0),
                                localScale = new Vector3(0.01f, 0.01f, 0.01f),
                                limbMask = LimbFlags.None
                            }
                        }
                    }
                });

                itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                    keyAsset = ItemDisplays.LoadAetheriumKeyAsset("AlienMagnet"),
                    displayRuleGroup = new DisplayRuleGroup {
                        rules = new ItemDisplayRule[]
                        {
                            new ItemDisplayRule
                            {
                                ruleType = ItemDisplayRuleType.ParentedPrefab,
                                followerPrefab = ItemDisplays.LoadAetheriumDisplay("AlienMagnet"),
                                childName = "Base",
                                localPos = new Vector3(0.01f, 0.01f, 0.02f),
                                localAngles = new Vector3(0, 0, 0),
                                localScale = new Vector3(0.0015f, 0.0015f, 0.0015f),
                                limbMask = LimbFlags.None
                            }
                        }
                    }
                });

                itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                    keyAsset = ItemDisplays.LoadAetheriumKeyAsset("VoidHeart"),
                    displayRuleGroup = new DisplayRuleGroup {
                        rules = new ItemDisplayRule[]
                        {
                            new ItemDisplayRule
                            {
                                ruleType = ItemDisplayRuleType.ParentedPrefab,
                                followerPrefab = ItemDisplays.LoadAetheriumDisplay("VoidHeart"),
                                childName = "Chest",
                                localPos = new Vector3(0, 0.0025f, 0),
                                localAngles = new Vector3(0, 0, 0),
                                localScale = new Vector3(0.001f, 0.001f, 0.001f),
                                limbMask = LimbFlags.None
                            }
                        }
                    }
                });

                itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                    keyAsset = ItemDisplays.LoadAetheriumKeyAsset("SharkTeeth"),
                    displayRuleGroup = new DisplayRuleGroup {
                        rules = new ItemDisplayRule[]
                        {
                            new ItemDisplayRule
                            {
                                ruleType = ItemDisplayRuleType.ParentedPrefab,
                                followerPrefab = ItemDisplays.LoadAetheriumDisplay("SharkTeeth"),
                                childName = "LegL",
                                localPos = new Vector3(0, 0.005f, 0),
                                localAngles = new Vector3(0, 0, 300),
                                localScale = new Vector3(0.005f, 0.004f, 0.003f),
                                limbMask = LimbFlags.None
                            }
                        }
                    }
                });

                itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                    keyAsset = ItemDisplays.LoadAetheriumKeyAsset("BloodSoakedShield"),
                    displayRuleGroup = new DisplayRuleGroup {
                        rules = new ItemDisplayRule[]
                        {
                            new ItemDisplayRule
                            {
                                ruleType = ItemDisplayRuleType.ParentedPrefab,
                                followerPrefab = ItemDisplays.LoadAetheriumDisplay("BloodSoakedShield"),
                                childName = "ElbowL",
                                localPos = new Vector3(0, 0.0015f, 0.001f),
                                localAngles = new Vector3(0, 0, 0),
                                localScale = new Vector3(0.0025f, 0.0025f, 0.0025f),
                                limbMask = LimbFlags.None
                            }
                        }
                    }
                });

                itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                    keyAsset = ItemDisplays.LoadAetheriumKeyAsset("InspiringDrone"),
                    displayRuleGroup = new DisplayRuleGroup {
                        rules = new ItemDisplayRule[]
                        {
                            new ItemDisplayRule
                            {
                                ruleType = ItemDisplayRuleType.ParentedPrefab,
                                followerPrefab = ItemDisplays.LoadAetheriumDisplay("InspiringDrone"),
                                childName = "Base",
                                localPos = new Vector3(0.015F, -0.01544F, 0.02F),
                                localAngles = new Vector3(285.671F, 270F, 270F),
                                localScale = new Vector3(0.002F, 0.002F, 0.002F),
                                limbMask = LimbFlags.None
                            }
                        }
                    }
                });

                itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                    keyAsset = ItemDisplays.LoadAetheriumKeyAsset("FeatheredPlume"),
                    displayRuleGroup = new DisplayRuleGroup {
                        rules = new ItemDisplayRule[]
                        {
                            new ItemDisplayRule
                            {
                                ruleType = ItemDisplayRuleType.ParentedPrefab,
                                followerPrefab = ItemDisplays.LoadAetheriumDisplay("FeatheredPlume"),
                                childName = "Head",
                                localPos = new Vector3(0, 0.001f, 0),
                                localAngles = new Vector3(45, 0, 0),
                                localScale = new Vector3(0.01f, 0.01f, 0.01f),
                                limbMask = LimbFlags.None
                            }
                        }
                    }
                });

                itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                    keyAsset = ItemDisplays.LoadAetheriumKeyAsset("ShieldingCore"),
                    displayRuleGroup = new DisplayRuleGroup {
                        rules = new ItemDisplayRule[]
                        {
                            new ItemDisplayRule
                            {
                                ruleType = ItemDisplayRuleType.ParentedPrefab,
                                followerPrefab = ItemDisplays.LoadAetheriumDisplay("ShieldingCore"),
                                childName = "Chest",
                                localPos = new Vector3(0, 0.003f, -0.0025f),
                                localAngles = new Vector3(0, 90, 0),
                                localScale = new Vector3(0.0025f, 0.0025f, 0.0025f),
                                limbMask = LimbFlags.None
                            }
                        }
                    }
                });

                itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                    keyAsset = ItemDisplays.LoadAetheriumKeyAsset("UnstableDesign"),
                    displayRuleGroup = new DisplayRuleGroup {
                        rules = new ItemDisplayRule[]
                        {
                            new ItemDisplayRule
                            {
                                ruleType = ItemDisplayRuleType.ParentedPrefab,
                                followerPrefab = ItemDisplays.LoadAetheriumDisplay("UnstableDesign"),
                                childName = "Chest",
                                localPos = new Vector3(0, 0, -0.001f),
                                localAngles = new Vector3(0, 45, 0),
                                localScale = new Vector3(0.01f, 0.01f, 0.01f),
                                limbMask = LimbFlags.None
                            }
                        }
                    }
                });

                itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                    keyAsset = ItemDisplays.LoadAetheriumKeyAsset("WeightedAnklet"),
                    displayRuleGroup = new DisplayRuleGroup {
                        rules = new ItemDisplayRule[]
                        {
                            new ItemDisplayRule
                            {
                                ruleType = ItemDisplayRuleType.ParentedPrefab,
                                followerPrefab = ItemDisplays.LoadAetheriumDisplay("WeightedAnklet"),
                                childName = "KneeL",
                                localPos = new Vector3(0, 0.0015f, 0),
                                localAngles = new Vector3(0, 0, 0),
                                localScale = new Vector3(0.0025f, 0.0025f, 0.0025f),
                                limbMask = LimbFlags.None
                            }
                        }
                    }
                });

                itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                    keyAsset = ItemDisplays.LoadAetheriumKeyAsset("BlasterSword"),
                    displayRuleGroup = new DisplayRuleGroup {
                        rules = new ItemDisplayRule[]
                        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadAetheriumDisplay("BlasterSword"),
                            childName = "PickL",
                            localPos = new Vector3(-0.008f, 0.001f, 0),
                            localAngles = new Vector3(0, 0, 270),
                            localScale = new Vector3(0.0015f, 0.0015f, 0.0015f),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadAetheriumDisplay("BlasterSword"),
                            childName = "PickR",
                            localPos = new Vector3(0.008f, 0.001f, 0),
                            localAngles = new Vector3(0, 0, 90),
                            localScale = new Vector3(0.0015f, 0.0015f, 0.0015f),
                            limbMask = LimbFlags.None
                        }
                        }
                    }
                });

                itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                    keyAsset = ItemDisplays.LoadAetheriumKeyAsset("WitchesRing"),
                    displayRuleGroup = new DisplayRuleGroup {
                        rules = new ItemDisplayRule[]
                        {
                            new ItemDisplayRule
                            {
                                ruleType = ItemDisplayRuleType.ParentedPrefab,
                                followerPrefab = ItemDisplays.LoadAetheriumDisplay("WitchesRing"),
                                childName = "ElbowL",
                                localPos = new Vector3(0.00011F, 0.002F, 0F),
                                localAngles = new Vector3(0F, 270F, 0F),
                                localScale = new Vector3(0.00324F, 0.00324F, 0.00356F),
                                limbMask = LimbFlags.None
                            },
                            new ItemDisplayRule
                            {
                                ruleType = ItemDisplayRuleType.ParentedPrefab,
                                followerPrefab = ItemDisplays.LoadAetheriumDisplay("WitchesRingCircle"),
                                childName = "ElbowL",
                                localPos = new Vector3(0F, 0.002F, 0F),
                                localAngles = new Vector3(90F, 0F, 0F),
                                localScale = new Vector3(0.00244F, 0.00244F, 0.00026F),
                                limbMask = LimbFlags.None
                            }
                        }
                    }
                });

                itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                    keyAsset = ItemDisplays.LoadAetheriumKeyAsset("JarOfReshaping"),
                    displayRuleGroup = new DisplayRuleGroup {
                        rules = new ItemDisplayRule[]
                        {
                            new ItemDisplayRule
                            {
                                ruleType = ItemDisplayRuleType.ParentedPrefab,
                                followerPrefab = ItemDisplays.LoadAetheriumDisplay("JarOfReshaping"),
                                childName = "Pelvis",
                                localPos = new Vector3(-0.00025F, 0F, 0.00191F),
                                localAngles = new Vector3(0F, 4.09304F, 0F),
                                localScale = new Vector3(0.0005F, 0.0005F, 0.00076F),
                                limbMask = LimbFlags.None
                            }
                        }
                    }
                });
        }

        public static GameObject LoadAetheriumDisplay(string name) {
            switch (name) {
                case "AccursedPotion":
                    return Aetherium.Items.AccursedPotion.ItemBodyModelPrefab;
                case "AlienMagnet":
                    return Aetherium.Items.AlienMagnet.ItemFollowerPrefab;
                case "BlasterSword":
                    return Aetherium.Items.BlasterSword.ItemBodyModelPrefab;
                case "BloodSoakedShield":
                    return Aetherium.Items.BloodSoakedShield.ItemBodyModelPrefab;
                case "FeatheredPlume":
                    return Aetherium.Items.FeatheredPlume.ItemBodyModelPrefab;
                case "InspiringDrone":
                    return Aetherium.Items.InspiringDrone.ItemFollowerPrefab;
                case "SharkTeeth":
                    return Aetherium.Items.SharkTeeth.ItemBodyModelPrefab;
                case "ShieldingCore":
                    return Aetherium.Items.ShieldingCore.ItemBodyModelPrefab;
                case "UnstableDesign":
                    return Aetherium.Items.UnstableDesign.ItemBodyModelPrefab;
                case "VoidHeart":
                    return Aetherium.Items.Voidheart.ItemBodyModelPrefab;
                case "WeightedAnklet":
                    return Aetherium.Items.WeightedAnklet.ItemBodyModelPrefab;
                case "WitchesRing":
                    return Aetherium.Items.WitchesRing.ItemBodyModelPrefab;
                case "WitchesRingCircle":
                    return Aetherium.Items.WitchesRing.CircleBodyModelPrefab;
                case "EngiBelt":
                    return Aetherium.Items.EngineersToolbelt.ItemBodyModelPrefab;
                case "JarOfReshaping":
                    return Aetherium.Equipment.JarOfReshaping.ItemBodyModelPrefab;
            }
            return null;
        }
        public static Object LoadAetheriumKeyAsset(string name) {
            switch (name) {
                case "AccursedPotion":
                    return Aetherium.Items.AccursedPotion.instance.ItemDef;
                case "AlienMagnet":
                    return Aetherium.Items.AlienMagnet.instance.ItemDef;
                case "BlasterSword":
                    return Aetherium.Items.BlasterSword.instance.ItemDef;
                case "BloodSoakedShield":
                    return Aetherium.Items.BloodSoakedShield.instance.ItemDef;
                case "FeatheredPlume":
                    return Aetherium.Items.FeatheredPlume.instance.ItemDef;
                case "InspiringDrone":
                    return Aetherium.Items.InspiringDrone.instance.ItemDef;
                case "SharkTeeth":
                    return Aetherium.Items.SharkTeeth.instance.ItemDef;
                case "ShieldingCore":
                    return Aetherium.Items.ShieldingCore.instance.ItemDef;
                case "UnstableDesign":
                    return Aetherium.Items.UnstableDesign.instance.ItemDef;
                case "VoidHeart":
                    return Aetherium.Items.Voidheart.instance.ItemDef;
                case "WeightedAnklet":
                    return Aetherium.Items.WeightedAnklet.instance.ItemDef;
                case "WitchesRing":
                    return Aetherium.Items.WitchesRing.instance.ItemDef;
                case "EngiBelt":
                    return Aetherium.Items.EngineersToolbelt.instance.ItemDef;
                case "JarOfReshaping":
                    return Aetherium.Equipment.JarOfReshaping.instance.EquipmentDef;
            }
            return null;
        }
        #endregion

        #region HEE HEE HEE
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void AddSupplyDropDisplays() {
            itemDisplayRules.Add(CreateSupplyDropRuleGroup("PlagueMask",
                                                           "Head",
                                                           new Vector3(0F, 0.00109F, -0.00241F),
                                                           new Vector3(353.6208F, 0F, 0F),
                                                           new Vector3(0.00246F, 0.0026F, 0.0024F)));
            itemDisplayRules.Add(CreateSupplyDropRuleGroup("PlagueHat",
                                                           "Head",
                                                           new Vector3(0F, 0.00328F, -0.00004F),
                                                           new Vector3(0F, 0F, 0F),
                                                           new Vector3(0.00164F, 0.00442F, 0.00189F)));

            //itemDisplayRules.Add(CreateSupplyDropRuleGroup("Bones",
            //                                               "CalfR",
            //                                               new Vector3(0.17997F, -0.05238F, 0.07133F),
            //                                               new Vector3(13.68323F, 76.44486F, 191.9287F),
            //                                               new Vector3(1.25683F, 1.25683F, 1.25683F)));
            //itemDisplayRules.Add(CreateSupplyDropRuleGroup("Berries",
            //                                               "loinFront2",
            //                                               new Vector3(0.11782F, 0.27382F, -0.13743F),
            //                                               new Vector3(341.1884F, 284.1298F, 180.0032F),
            //                                               new Vector3(0.08647F, 0.08647F, 0.08647F)));
            //itemDisplayRules.Add(CreateSupplyDropRuleGroup("UnassumingTie",
            //                                               "Chest",
            //                                               new Vector3(-0.08132F, 0.30226F, 0.34786F),
            //                                               new Vector3(351.786F, 356.4574F, 0.73319F),
            //                                               new Vector3(0.32213F, 0.35018F, 0.42534F)));
            //itemDisplayRules.Add(CreateSupplyDropRuleGroup("SalvagedWires",
            //                                               "UpperArmL",
            //                                               new Vector3(-0.00419F, 0.10839F, -0.20693F),
            //                                               new Vector3(21.68419F, 165.3445F, 132.0565F),
            //                                               new Vector3(0.63809F, 0.63809F, 0.63809F)));

            //itemDisplayRules.Add(CreateSupplyDropRuleGroup("ShellPlating",
            //                                               "ThighR",
            //                                               new Vector3(0.02115F, 0.52149F, -0.31269F),
            //                                               new Vector3(319.6181F, 168.4007F, 184.779F),
            //                                               new Vector3(0.24302F, 0.26871F, 0.26871F)));
            //itemDisplayRules.Add(CreateSupplyDropRuleGroup("ElectroPlankton",
            //                                               "ThighR",
            //                                               new Vector3(0.21067F, 0.49094F, -0.08702F),
            //                                               new Vector3(8.08377F, 285.087F, 164.4582F),
            //                                               new Vector3(0.11243F, 0.11243F, 0.11243F)));

            //itemDisplayRules.Add(CreateSupplyDropRuleGroup("BloodBook",
            //                                               "Root",
            //                                               new Vector3(2.19845F, -1.51445F, 1.59871F),
            //                                               new Vector3(303.5005F, 271.0879F, 269.2205F),
            //                                               new Vector3(0.12F, 0.12F, 0.12F)));
            //itemDisplayRules.Add(CreateSupplyDropRuleGroup("QSGen",
            //                                               "LowerArmL",
            //                                               new Vector3(0.06003F, 0.1038F, -0.02042F),
            //                                               new Vector3(0F, 18.75576F, 268.4665F),
            //                                               new Vector3(0.12301F, 0.12301F, 0.12301F)));
        }

        public static GameObject LoadSupplyDropDisplay(string name) {
            switch (name) {
                case "Bones":
                    return SupplyDrop.Items.HardenedBoneFragments.ItemBodyModelPrefab;
                case "Berries":
                    return SupplyDrop.Items.NumbingBerries.ItemBodyModelPrefab;
                case "UnassumingTie":
                    return SupplyDrop.Items.UnassumingTie.ItemBodyModelPrefab;
                case "SalvagedWires":
                    return SupplyDrop.Items.SalvagedWires.ItemBodyModelPrefab;

                case "ShellPlating":
                    return SupplyDrop.Items.ShellPlating.ItemBodyModelPrefab;
                case "ElectroPlankton":
                    return SupplyDrop.Items.ElectroPlankton.ItemBodyModelPrefab;
                case "PlagueHat":
                    return SupplyDrop.Items.PlagueHat.ItemBodyModelPrefab;
                case "PlagueMask":
                    GameObject masku = PrefabAPI.InstantiateClone(SupplyDrop.Items.PlagueMask.ItemBodyModelPrefab, "PlagueMask");
                    Material heeheehee = new Material(masku.GetComponent<ItemDisplay>().rendererInfos[0].defaultMaterial);
                    heeheehee.color = Color.green; ;
                    masku.GetComponent<ItemDisplay>().rendererInfos[0].defaultMaterial = heeheehee;
                    return masku;

                case "BloodBook":
                    return SupplyDrop.Items.BloodBook.ItemBodyModelPrefab;
                case "QSGen":
                    return SupplyDrop.Items.QSGen.ItemBodyModelPrefab;
            }
            return null;
        }
        public static Object LoadSupplyDropKeyAsset(string name) {
            switch (name) {
                //would be cool if these are enums maybe
                case "Bones":
                    return SupplyDrop.Items.HardenedBoneFragments.instance.itemDef;
                case "Berries":
                    return SupplyDrop.Items.NumbingBerries.instance.itemDef;
                case "UnassumingTie":
                    return SupplyDrop.Items.UnassumingTie.instance.itemDef;
                case "SalvagedWires":
                    return SupplyDrop.Items.SalvagedWires.instance.itemDef;

                case "ShellPlating":
                    return SupplyDrop.Items.ShellPlating.instance.itemDef;
                case "ElectroPlankton":
                    return SupplyDrop.Items.ElectroPlankton.instance.itemDef;
                case "PlagueHat":
                    return SupplyDrop.Items.PlagueHat.instance.itemDef;
                case "PlagueMask":
                    return SupplyDrop.Items.PlagueMask.instance.itemDef;

                case "BloodBook":
                    return SupplyDrop.Items.BloodBook.instance.itemDef;
                case "QSGen":
                    return SupplyDrop.Items.QSGen.instance.itemDef;

            }
            return null;
        }
        #endregion

        #region goldy
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void fixFuckinGoaldCoastDisplays() {

            itemDisplayRules.Add(CreateGoldCoastRuleGroup("bigsword",
                                                          "Chest",
                                                          new Vector3(-0.000329F, 0.002482F, -0.001994F),
                                                          new Vector3(351.7756F, 182.8893F, 176.6553F),
                                                          new Vector3(0.00075F, 0.00075F, 0.00075F)));
            itemDisplayRules.Add(CreateGoldCoastRuleGroup("eye",
                                                          "Chest",
                                                          new Vector3(-0.000013F, 0.00177F, 0.001923F),
                                                          new Vector3(357.5498F, 279.5151F, 288.6346F),
                                                          new Vector3(0.001F, 0.001F, 0.001F)));
            itemDisplayRules.Add(CreateGoldCoastRuleGroup("knurl",
                                                          "Chest",
                                                          new Vector3(0.00344F, 0.00425F, -0.00053F),
                                                          new Vector3(276.5326F, 108.8338F, 239.9354F),
                                                          new Vector3(0.0016F, 0.0016F, 0.0016F)));

        }
        private static GameObject LoadGoldCoastDisplay(string name) {
            switch (name) {
                case "bigsword":
                    return GoldenCoastPlus.GoldenCoastPlus.bigSwordDef.pickupModelPrefab;
                case "eye":
                    return GoldenCoastPlus.GoldenCoastPlus.laserEyeDef.pickupModelPrefab;
                case "knurl":
                    return GoldenCoastPlus.GoldenCoastPlus.goldenKnurlDef.pickupModelPrefab;
            }
            return null;
        }
        private static Object LoadGoldCoastKeyAsset(string name) {
            switch (name) {
                case "bigsword":
                    return GoldenCoastPlus.GoldenCoastPlus.bigSwordDef;
                case "eye":
                    return GoldenCoastPlus.GoldenCoastPlus.laserEyeDef;
                case "knurl":
                    return GoldenCoastPlus.GoldenCoastPlus.goldenKnurlDef;
            }
            return null;
        }

        #endregion

        #region sivs
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void AddSivsItemsDisplays() {

            if (DiggerPlugin.sivsItemsInstalled)
            {
                itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
                {
                   keyAsset = ItemDisplays.LoadSivKeyAsset("BeetlePlush"),
                    displayRuleGroup = new DisplayRuleGroup
                    {
                        rules = new ItemDisplayRule[]
                        {
                            new ItemDisplayRule
                            {
                                ruleType = ItemDisplayRuleType.ParentedPrefab,
                                followerPrefab = ItemDisplays.LoadSivDisplay("BeetlePlush"),
                                childName = "Chest",
                                localPos = new Vector3(0, 0.005f, -0.0025f),
                                localAngles = new Vector3(0, 0, 0),
                                localScale = new Vector3(0.01f, 0.01f, 0.01f),
                                limbMask = LimbFlags.None
                            }
                        }
                    }
                });

                itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                    keyAsset = ItemDisplays.LoadSivKeyAsset("BisonShield"),
                    displayRuleGroup = new DisplayRuleGroup
                    {
                        rules = new ItemDisplayRule[]
                        {
                            new ItemDisplayRule
                            {
                                ruleType = ItemDisplayRuleType.ParentedPrefab,
                                followerPrefab = ItemDisplays.LoadSivDisplay("BisonShield"),
                                childName = "ElbowR",
                                localPos = new Vector3(0, 0, 0),
                                localAngles = new Vector3(0, 270, 0),
                                localScale = new Vector3(0.007f, 0.007f, 0.007f),
                                limbMask = LimbFlags.None
                            }
                        }
                    }
                });

                itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                    keyAsset = ItemDisplays.LoadSivKeyAsset("FlameGland"),
                    displayRuleGroup = new DisplayRuleGroup
                    {
                        rules = new ItemDisplayRule[]
                        {
                            new ItemDisplayRule
                            {
                                ruleType = ItemDisplayRuleType.ParentedPrefab,
                                followerPrefab = ItemDisplays.LoadSivDisplay("FlameGland"),
                                childName = "Chest",
                                localPos = new Vector3(0, 0, 0),
                                localAngles = new Vector3(0, 270, 0),
                                localScale = new Vector3(0.007f, 0.007f, 0.007f),
                                limbMask = LimbFlags.None
                            }
                        }
                    }
                });

                itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                    keyAsset = ItemDisplays.LoadSivKeyAsset("Geode"),
                    displayRuleGroup = new DisplayRuleGroup
                    {
                        rules = new ItemDisplayRule[]
                        {
                            new ItemDisplayRule
                            {
                                ruleType = ItemDisplayRuleType.ParentedPrefab,
                                followerPrefab = ItemDisplays.LoadSivDisplay("Geode"),
                                childName = "LegL",
                                localPos = new Vector3(-0.001f, 0.0025f, 0),
                                localAngles = new Vector3(0, 0, 270),
                                localScale = new Vector3(0.003f, 0.003f, 0.002f),
                                limbMask = LimbFlags.None
                            }
                        }
                    }
                });

                itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                    keyAsset = ItemDisplays.LoadSivKeyAsset("ImpEye"),
                    displayRuleGroup = new DisplayRuleGroup
                    {
                        rules = new ItemDisplayRule[]
                        {
                            new ItemDisplayRule
                            {
                                ruleType = ItemDisplayRuleType.ParentedPrefab,
                                followerPrefab = ItemDisplays.LoadSivDisplay("ImpEye"),
                                childName = "Head",
                                localPos = new Vector3(0, 0.0015f, -0.001f),
                                localAngles = new Vector3(0, 180, 0),
                                localScale = new Vector3(0.01f, 0.008f, 0.01f),
                                limbMask = LimbFlags.None
                            }
                        }
                    }
                });

                itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                    keyAsset = ItemDisplays.LoadSivKeyAsset("NullSeed"),
                    displayRuleGroup = new DisplayRuleGroup
                    {
                        rules = new ItemDisplayRule[]
                        {
                            new ItemDisplayRule
                            {
                                ruleType = ItemDisplayRuleType.ParentedPrefab,
                                followerPrefab = ItemDisplays.LoadSivDisplay("NullSeed"),
                                childName = "Base",
                                localPos = new Vector3(-0.01f, 0, 0.015f),
                                localAngles = new Vector3(90, 0, 0),
                                localScale = new Vector3(2, 2, 2),
                                limbMask = LimbFlags.None
                            }
                        }
                    }
                });

                itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                    keyAsset = ItemDisplays.LoadSivKeyAsset("Tarbine"),
                    displayRuleGroup = new DisplayRuleGroup
                    {
                        rules = new ItemDisplayRule[]
                        {
                            new ItemDisplayRule
                            {
                                ruleType = ItemDisplayRuleType.ParentedPrefab,
                                followerPrefab = ItemDisplays.LoadSivDisplay("Tarbine"),
                                childName = "PickL",
                                localPos = new Vector3(0, 0.002f, 0.002f),
                                localAngles = new Vector3(0, 180, 270),
                                localScale = new Vector3(0.0075f, 0.0075f, 0.0075f),
                                limbMask = LimbFlags.None
                            }
                        }
                    }
                });

                itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
                    keyAsset = ItemDisplays.LoadSivKeyAsset("Tentacle"),
                    displayRuleGroup = new DisplayRuleGroup
                    {
                        rules = new ItemDisplayRule[]
                        {
                            new ItemDisplayRule
                            {
                                ruleType = ItemDisplayRuleType.ParentedPrefab,
                                followerPrefab = ItemDisplays.LoadSivDisplay("Tentacle"),
                                childName = "Head",
                                localPos = new Vector3(0, 0.002f, -0.0002f),
                                localAngles = new Vector3(0, 0, 0),
                                localScale = new Vector3(0.01f, 0.01f, 0.01f),
                                limbMask = LimbFlags.None
                            }
                        }
                    }
                });
            }
        }

        //sorry siv habibi not 100% passionate about this one but if the community wants it fuck it
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static GameObject LoadSivDisplay(string name)
        {
            switch (name)
            {
                case "BeetlePlush":
                    return SivsItemsRoR2.BeetlePlush.displayPrefab;
                case "BisonShield":
                    return SivsItemsRoR2.BisonShield.displayPrefab;
                case "FlameGland":
                    return SivsItemsRoR2.FlameGland.displayPrefab;
                case "Geode":
                    return SivsItemsRoR2.Geode.displayPrefab;
                case "ImpEye":
                    return SivsItemsRoR2.ImpEye.displayPrefab;
                case "NullSeed":
                    return SivsItemsRoR2.NullSeed.displayPrefab;
                case "Tarbine":
                    return SivsItemsRoR2.Tarbine.displayPrefab;
                case "Tentacle":
                    return SivsItemsRoR2.Tentacle.displayPrefab;
            }
            return null;
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static Object LoadSivKeyAsset(string name) {
            switch (name) {
                case "BeetlePlush":
                    return SivsItemsRoR2.BeetlePlush.itemDef;
                case "BisonShield":
                    return SivsItemsRoR2.BisonShield.itemDef;
                case "FlameGland":
                    return SivsItemsRoR2.FlameGland.itemDef;
                case "Geode":
                    return SivsItemsRoR2.Geode.itemDef;
                case "ImpEye":
                    return SivsItemsRoR2.ImpEye.itemDef;
                case "NullSeed":
                    return SivsItemsRoR2.NullSeed.itemDef;
                case "Tarbine":
                    return SivsItemsRoR2.Tarbine.itemDef;
                case "Tentacle":
                    return SivsItemsRoR2.Tentacle.itemDef;
            }
            return null;
        }
        #endregion

        private static ItemDisplayRuleSet.KeyAssetRuleGroup CreateSupplyDropRuleGroup(string itemName, string childName, Vector3 position, Vector3 rotation, Vector3 scale) {
            try {
                return CreateGenericDisplayRuleGroup(LoadSupplyDropKeyAsset(itemName), LoadSupplyDropDisplay(itemName), childName, position, rotation, scale);
            }
            catch (System.Exception e) {

                //DiggerPlugin.logger.LogWarning($"could not create item display for supply drop's {itemName}. skipping.\n(Error: {e.Message})");
                return new ItemDisplayRuleSet.KeyAssetRuleGroup();
            }
        }

        private static ItemDisplayRuleSet.KeyAssetRuleGroup CreateGoldCoastRuleGroup(string itemName, string childName, Vector3 position, Vector3 rotation, Vector3 scale) {
            try {
                return CreateGenericDisplayRuleGroup(LoadGoldCoastKeyAsset(itemName), LoadGoldCoastDisplay(itemName), childName, position, rotation, scale);
            }
            catch (System.Exception e) {

                //DiggerPlugin.logger.LogWarning($"could not create item display for gold coast's {itemName}. skipping.\n(Error: {e.Message})");
                return new ItemDisplayRuleSet.KeyAssetRuleGroup();
            }
        }

        private static ItemDisplayRuleSet.KeyAssetRuleGroup CreateGenericDisplayRuleGroup(Object keyAsset_, GameObject itemPrefab, string childName, Vector3 position, Vector3 rotation, Vector3 scale) {

            ItemDisplayRule singleRule = CreateDisplayRule(itemPrefab, childName, position, rotation, scale);
            return CreateDisplayRuleGroupWithRules(keyAsset_, singleRule);
        }

        private static ItemDisplayRule CreateDisplayRule(GameObject itemPrefab, string childName, Vector3 position, Vector3 rotation, Vector3 scale) {
            return new ItemDisplayRule {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                childName = childName,
                followerPrefab = itemPrefab,
                limbMask = LimbFlags.None,
                localPos = position,
                localAngles = rotation,
                localScale = scale
            };
        }

        //they use these
        //but use these ones yourself if you are doing multiple
        private static ItemDisplayRuleSet.KeyAssetRuleGroup CreateDisplayRuleGroupWithRules(Object keyAsset_, params ItemDisplayRule[] rules) {
            return new ItemDisplayRuleSet.KeyAssetRuleGroup {
                keyAsset = keyAsset_,
                displayRuleGroup = new DisplayRuleGroup {
                    rules = rules
                }
            };
        }

    }
}
