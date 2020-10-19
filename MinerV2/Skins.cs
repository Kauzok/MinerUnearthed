using System;
using UnityEngine;
using R2API;
using RoR2;
using R2API.Utils;
using System.Collections.Generic;

namespace MinerPlugin
{
    public static class Skins
    {
        public static void RegisterSkins()
        {
            GameObject bodyPrefab = MinerPlugin.characterPrefab;

            GameObject model = bodyPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            ModelSkinController skinController = model.AddComponent<ModelSkinController>();
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            SkinnedMeshRenderer mainRenderer = Reflection.GetFieldValue<SkinnedMeshRenderer>(characterModel, "mainSkinnedMeshRenderer");

            LanguageAPI.Add("MINERBODY_DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add("MINERBODY_MOLTEN_SKIN_NAME", "Molten");
            LanguageAPI.Add("MINERBODY_PUPLE_SKIN_NAME", "Puple");
            LanguageAPI.Add("MINERBODY_TUNDRA_SKIN_NAME", "Tundra");
            LanguageAPI.Add("MINERBODY_IRON_SKIN_NAME", "Iron");
            LanguageAPI.Add("MINERBODY_GOLD_SKIN_NAME", "Gold");
            LanguageAPI.Add("MINERBODY_DIAMOND_SKIN_NAME", "Diamond");

            LoadoutAPI.SkinDefInfo skinDefInfo = default(LoadoutAPI.SkinDefInfo);
            skinDefInfo.BaseSkins = Array.Empty<SkinDef>();
            skinDefInfo.MinionSkinReplacements = new SkinDef.MinionSkinReplacement[0];
            skinDefInfo.ProjectileGhostReplacements = new SkinDef.ProjectileGhostReplacement[0];

            skinDefInfo.GameObjectActivations = new SkinDef.GameObjectActivation[0];

            skinDefInfo.Icon = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponentInChildren<ModelSkinController>().skins[0].icon;
            skinDefInfo.MeshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    renderer = mainRenderer,
                    mesh = mainRenderer.sharedMesh
                }
            };
            skinDefInfo.Name = "MINERBODY_DEFAULT_SKIN_NAME";
            skinDefInfo.NameToken = "MINERBODY_DEFAULT_SKIN_NAME";
            skinDefInfo.RendererInfos = characterModel.baseRendererInfos;
            skinDefInfo.RootObject = model;
            skinDefInfo.UnlockableName = "";

            CharacterModel.RendererInfo[] rendererInfos = skinDefInfo.RendererInfos;
            CharacterModel.RendererInfo[] array = new CharacterModel.RendererInfo[rendererInfos.Length];
            rendererInfos.CopyTo(array, 0);

            //clone commando material for that spicy hopoo shader
            Material material = array[0].defaultMaterial;

            if (material)
            {
                material = UnityEngine.Object.Instantiate<Material>(Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[0].defaultMaterial);
                material.SetColor("_Color", Color.white);
                material.SetTexture("_MainTex", Assets.MainAssetBundle.LoadAsset<Material>("matMiner").GetTexture("_MainTex"));
                material.SetColor("_EmColor", Color.white);
                material.SetFloat("_EmPower", 0.4f);
                material.SetTexture("_EmTex", Assets.MainAssetBundle.LoadAsset<Material>("matMiner").GetTexture("_EmissionMap"));
                material.SetFloat("_NormalStrength", 0);
                //material.SetTexture("_NormalTex", Assets.MainAssetBundle.LoadAsset<Material>("matMiner").GetTexture("_BumpMap"));

                array[0].defaultMaterial = material;
            }

            skinDefInfo.RendererInfos = array;

            SkinDef defaultSkin = LoadoutAPI.CreateNewSkinDef(skinDefInfo);

            LoadoutAPI.SkinDefInfo moltenSkinDefInfo = default(LoadoutAPI.SkinDefInfo);
            moltenSkinDefInfo.BaseSkins = Array.Empty<SkinDef>();
            moltenSkinDefInfo.MinionSkinReplacements = new SkinDef.MinionSkinReplacement[0];
            moltenSkinDefInfo.ProjectileGhostReplacements = new SkinDef.ProjectileGhostReplacement[0];
            moltenSkinDefInfo.GameObjectActivations = new SkinDef.GameObjectActivation[0];
            moltenSkinDefInfo.Icon = LoadoutAPI.CreateSkinIcon(new Color(0.43f, 0.1f, 0.1f), Color.red, new Color(0.31f, 0.04f, 0.07f), Color.black);
            moltenSkinDefInfo.MeshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    renderer = mainRenderer,
                    mesh = mainRenderer.sharedMesh
                }
            };
            moltenSkinDefInfo.Name = "MINERBODY_MOLTEN_SKIN_NAME";
            moltenSkinDefInfo.NameToken = "MINERBODY_MOLTEN_SKIN_NAME";
            moltenSkinDefInfo.RendererInfos = characterModel.baseRendererInfos;
            moltenSkinDefInfo.RootObject = model;
            moltenSkinDefInfo.UnlockableName = "MINER_MONSOONUNLOCKABLE_REWARD_ID";

            rendererInfos = skinDefInfo.RendererInfos;
            array = new CharacterModel.RendererInfo[rendererInfos.Length];
            rendererInfos.CopyTo(array, 0);

            material = array[0].defaultMaterial;

            if (material)
            {
                material = UnityEngine.Object.Instantiate<Material>(material);
                material.SetTexture("_MainTex", Assets.MainAssetBundle.LoadAsset<Material>("matMinerMolten").GetTexture("_MainTex"));
                material.SetColor("_EmColor", Color.white);
                material.SetFloat("_EmPower", 5);
                material.SetTexture("_EmTex", Assets.MainAssetBundle.LoadAsset<Material>("matMinerMolten").GetTexture("_EmissionMap"));

                array[0].defaultMaterial = material;
            }

            moltenSkinDefInfo.RendererInfos = array;

            SkinDef moltenSkin = LoadoutAPI.CreateNewSkinDef(moltenSkinDefInfo);

            LoadoutAPI.SkinDefInfo pupleSkinDefInfo = default(LoadoutAPI.SkinDefInfo);
            pupleSkinDefInfo.BaseSkins = Array.Empty<SkinDef>();
            pupleSkinDefInfo.MinionSkinReplacements = new SkinDef.MinionSkinReplacement[0];
            pupleSkinDefInfo.ProjectileGhostReplacements = new SkinDef.ProjectileGhostReplacement[0];
            pupleSkinDefInfo.GameObjectActivations = new SkinDef.GameObjectActivation[0];
            pupleSkinDefInfo.Icon = Resources.Load<GameObject>("Prefabs/CharacterBodies/EngiBody").GetComponentInChildren<ModelSkinController>().skins[0].icon;
            pupleSkinDefInfo.MeshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    renderer = mainRenderer,
                    mesh = mainRenderer.sharedMesh
                }
            };
            pupleSkinDefInfo.Name = "MINERBODY_PUPLE_SKIN_NAME";
            pupleSkinDefInfo.NameToken = "MINERBODY_PUPLE_SKIN_NAME";
            pupleSkinDefInfo.RendererInfos = characterModel.baseRendererInfos;
            pupleSkinDefInfo.RootObject = model;
            pupleSkinDefInfo.UnlockableName = "";

            rendererInfos = skinDefInfo.RendererInfos;
            array = new CharacterModel.RendererInfo[rendererInfos.Length];
            rendererInfos.CopyTo(array, 0);

            material = array[0].defaultMaterial;

            if (material)
            {
                material = UnityEngine.Object.Instantiate<Material>(material);
                material.SetTexture("_MainTex", Assets.MainAssetBundle.LoadAsset<Material>("matMinerPuple").GetTexture("_MainTex"));
                material.SetColor("_EmColor", Color.white);
                material.SetFloat("_EmPower", 1);
                material.SetTexture("_EmTex", Assets.MainAssetBundle.LoadAsset<Material>("matMinerPuple").GetTexture("_EmissionMap"));

                array[0].defaultMaterial = material;
            }

            pupleSkinDefInfo.RendererInfos = array;

            SkinDef pupleSkin = LoadoutAPI.CreateNewSkinDef(pupleSkinDefInfo);

            LoadoutAPI.SkinDefInfo tundraSkinDefInfo = default(LoadoutAPI.SkinDefInfo);
            tundraSkinDefInfo.BaseSkins = Array.Empty<SkinDef>();
            tundraSkinDefInfo.MinionSkinReplacements = new SkinDef.MinionSkinReplacement[0];
            tundraSkinDefInfo.ProjectileGhostReplacements = new SkinDef.ProjectileGhostReplacement[0];
            tundraSkinDefInfo.GameObjectActivations = new SkinDef.GameObjectActivation[0];
            tundraSkinDefInfo.Icon = LoadoutAPI.CreateSkinIcon(new Color(0.83f, 0.83f, 0.83f), new Color(0.64f, 0.64f, 0.64f), new Color(0.25f, 0.25f, 0.25f), new Color(0f, 0f, 0f));
            tundraSkinDefInfo.MeshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    renderer = mainRenderer,
                    mesh = Assets.tundraMesh
                }
            };
            tundraSkinDefInfo.Name = "MINERBODY_TUNDRA_SKIN_NAME";
            tundraSkinDefInfo.NameToken = "MINERBODY_TUNDRA_SKIN_NAME";
            tundraSkinDefInfo.RendererInfos = characterModel.baseRendererInfos;
            tundraSkinDefInfo.RootObject = model;
            tundraSkinDefInfo.UnlockableName = "MINER_TUNDRAUNLOCKABLE_REWARD_ID";

            rendererInfos = skinDefInfo.RendererInfos;
            array = new CharacterModel.RendererInfo[rendererInfos.Length];
            rendererInfos.CopyTo(array, 0);

            material = array[0].defaultMaterial;

            if (material)
            {
                material = UnityEngine.Object.Instantiate<Material>(material);
                material.SetTexture("_MainTex", Assets.MainAssetBundle.LoadAsset<Material>("matMinerTundra").GetTexture("_MainTex"));
                material.SetColor("_EmColor", Color.white);
                material.SetFloat("_EmPower", 1);
                material.SetTexture("_EmTex", Assets.MainAssetBundle.LoadAsset<Material>("matMinerTundra").GetTexture("_EmissionMap"));

                array[0].defaultMaterial = material;
            }

            tundraSkinDefInfo.RendererInfos = array;

            SkinDef tundraSkin = LoadoutAPI.CreateNewSkinDef(tundraSkinDefInfo);

            //minecraft-
            LoadoutAPI.SkinDefInfo ironSkinDefInfo = default(LoadoutAPI.SkinDefInfo);
            ironSkinDefInfo.BaseSkins = Array.Empty<SkinDef>();
            ironSkinDefInfo.MinionSkinReplacements = new SkinDef.MinionSkinReplacement[0];
            ironSkinDefInfo.ProjectileGhostReplacements = new SkinDef.ProjectileGhostReplacement[0];
            ironSkinDefInfo.GameObjectActivations = new SkinDef.GameObjectActivation[0];
            ironSkinDefInfo.Icon = LoadoutAPI.CreateSkinIcon(new Color(0.43f, 0.1f, 0.1f), Color.grey, new Color(0.31f, 0.04f, 0.07f), Color.black);
            ironSkinDefInfo.MeshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    renderer = mainRenderer,
                    mesh = mainRenderer.sharedMesh
                }
            };
            ironSkinDefInfo.Name = "MINERBODY_IRON_SKIN_NAME";
            ironSkinDefInfo.NameToken = "MINERBODY_IRON_SKIN_NAME";
            ironSkinDefInfo.RendererInfos = characterModel.baseRendererInfos;
            ironSkinDefInfo.RootObject = model;
            ironSkinDefInfo.UnlockableName = "";

            rendererInfos = skinDefInfo.RendererInfos;
            array = new CharacterModel.RendererInfo[rendererInfos.Length];
            rendererInfos.CopyTo(array, 0);

            material = array[0].defaultMaterial;

            if (material)
            {
                material = UnityEngine.Object.Instantiate<Material>(material);
                material.SetTexture("_MainTex", Assets.MainAssetBundle.LoadAsset<Material>("matMinerIron").GetTexture("_MainTex"));
                material.SetColor("_EmColor", Color.white);
                material.SetFloat("_EmPower", 0);

                array[0].defaultMaterial = material;
            }

            ironSkinDefInfo.RendererInfos = array;

            SkinDef ironSkin = LoadoutAPI.CreateNewSkinDef(ironSkinDefInfo);

            LoadoutAPI.SkinDefInfo goldSkinDefInfo = default(LoadoutAPI.SkinDefInfo);
            goldSkinDefInfo.BaseSkins = Array.Empty<SkinDef>();
            goldSkinDefInfo.MinionSkinReplacements = new SkinDef.MinionSkinReplacement[0];
            goldSkinDefInfo.ProjectileGhostReplacements = new SkinDef.ProjectileGhostReplacement[0];
            goldSkinDefInfo.GameObjectActivations = new SkinDef.GameObjectActivation[0];
            goldSkinDefInfo.Icon = LoadoutAPI.CreateSkinIcon(new Color(0.43f, 0.1f, 0.1f), Color.yellow, new Color(0.31f, 0.04f, 0.07f), Color.black);
            goldSkinDefInfo.MeshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    renderer = mainRenderer,
                    mesh = mainRenderer.sharedMesh
                }
            };
            goldSkinDefInfo.Name = "MINERBODY_GOLD_SKIN_NAME";
            goldSkinDefInfo.NameToken = "MINERBODY_GOLD_SKIN_NAME";
            goldSkinDefInfo.RendererInfos = characterModel.baseRendererInfos;
            goldSkinDefInfo.RootObject = model;
            goldSkinDefInfo.UnlockableName = "";

            rendererInfos = skinDefInfo.RendererInfos;
            array = new CharacterModel.RendererInfo[rendererInfos.Length];
            rendererInfos.CopyTo(array, 0);

            material = array[0].defaultMaterial;

            if (material)
            {
                material = UnityEngine.Object.Instantiate<Material>(material);
                material.SetTexture("_MainTex", Assets.MainAssetBundle.LoadAsset<Material>("matMinerGold").GetTexture("_MainTex"));
                material.SetColor("_EmColor", Color.white);
                material.SetFloat("_EmPower", 0f);
                material.SetTexture("_EmTex", Assets.MainAssetBundle.LoadAsset<Material>("matMinerGold").GetTexture("_EmissionMap"));

                array[0].defaultMaterial = material;
            }

            goldSkinDefInfo.RendererInfos = array;

            SkinDef goldSkin = LoadoutAPI.CreateNewSkinDef(goldSkinDefInfo);

            LoadoutAPI.SkinDefInfo diamondSkinDefInfo = default(LoadoutAPI.SkinDefInfo);
            diamondSkinDefInfo.BaseSkins = Array.Empty<SkinDef>();
            diamondSkinDefInfo.MinionSkinReplacements = new SkinDef.MinionSkinReplacement[0];
            diamondSkinDefInfo.ProjectileGhostReplacements = new SkinDef.ProjectileGhostReplacement[0];
            diamondSkinDefInfo.GameObjectActivations = new SkinDef.GameObjectActivation[0];
            diamondSkinDefInfo.Icon = LoadoutAPI.CreateSkinIcon(new Color(0.43f, 0.1f, 0.1f), Color.blue, new Color(0.31f, 0.04f, 0.07f), Color.black);
            diamondSkinDefInfo.MeshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    renderer = mainRenderer,
                    mesh = mainRenderer.sharedMesh
                }
            };
            diamondSkinDefInfo.Name = "MINERBODY_DIAMOND_SKIN_NAME";
            diamondSkinDefInfo.NameToken = "MINERBODY_DIAMOND_SKIN_NAME";
            diamondSkinDefInfo.RendererInfos = characterModel.baseRendererInfos;
            diamondSkinDefInfo.RootObject = model;
            diamondSkinDefInfo.UnlockableName = "";

            rendererInfos = skinDefInfo.RendererInfos;
            array = new CharacterModel.RendererInfo[rendererInfos.Length];
            rendererInfos.CopyTo(array, 0);

            material = array[0].defaultMaterial;

            if (material)
            {
                material = UnityEngine.Object.Instantiate<Material>(material);
                material.SetTexture("_MainTex", Assets.MainAssetBundle.LoadAsset<Material>("matMinerDiamond").GetTexture("_MainTex"));
                material.SetColor("_EmColor", Color.white);
                material.SetFloat("_EmPower", 0.75f);
                material.SetTexture("_EmTex", Assets.MainAssetBundle.LoadAsset<Material>("matMinerDiamond").GetTexture("_EmissionMap"));

                array[0].defaultMaterial = material;
            }

            diamondSkinDefInfo.RendererInfos = array;

            SkinDef diamondSkin = LoadoutAPI.CreateNewSkinDef(diamondSkinDefInfo);

            var skinDefs = new List<SkinDef>()
            {
                defaultSkin,
                moltenSkin,
                tundraSkin,
            };

            if (MinerPlugin.extraSkins.Value)
            {
                skinDefs.Add(pupleSkin);
                skinDefs.Add(ironSkin);
                skinDefs.Add(goldSkin);
                skinDefs.Add(diamondSkin);
            }

            skinController.skins = skinDefs.ToArray();
        }
    }
}
