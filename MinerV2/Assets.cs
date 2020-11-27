using System.Reflection;
using R2API;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using RoR2;

namespace MinerPlugin
{
    public static class Assets
    {
        public static AssetBundle mainAssetBundle = null;

        public static Texture charPortrait;

        public static Sprite iconP;
        public static Sprite icon1;
        public static Sprite icon1B;
        public static Sprite icon2;
        public static Sprite icon2B;
        public static Sprite icon3;
        public static Sprite icon3B;
        public static Sprite icon4;
        public static Sprite icon4S;
        public static Sprite icon4B;

        public static GameObject c4Model;

        public static Mesh tundraMesh;
        public static Mesh steveMesh;

        public static GameObject swingFX;
        public static GameObject empoweredSwingFX;
        public static GameObject hitFX;
        public static GameObject heavyHitFX;

        public static void PopulateAssets()
        {
            if (mainAssetBundle == null)
            {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MinerV2.miner"))
                {
                    mainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                    var provider = new AssetBundleResourcesProvider("@Miner", mainAssetBundle);
                    ResourcesAPI.AddProvider(provider);
                }
            }

            using (Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream("MinerV2.MinerBank.bnk"))
            {
                byte[] array = new byte[manifestResourceStream2.Length];
                manifestResourceStream2.Read(array, 0, array.Length);
                SoundAPI.SoundBanks.Add(array);
            }

            using (Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream("MinerV2.MinerV2.bnk"))
            {
                byte[] array = new byte[manifestResourceStream2.Length];
                manifestResourceStream2.Read(array, 0, array.Length);
                SoundAPI.SoundBanks.Add(array);
            }

            charPortrait = mainAssetBundle.LoadAsset<Sprite>("texMinerIcon").texture;

            iconP = mainAssetBundle.LoadAsset<Sprite>("GoldRushIcon");
            icon1 = mainAssetBundle.LoadAsset<Sprite>("CrushIcon");
            icon1B = mainAssetBundle.LoadAsset<Sprite>("GougeIcon");
            icon2 = mainAssetBundle.LoadAsset<Sprite>("DrillChargeIcon");
            icon2B = mainAssetBundle.LoadAsset<Sprite>("DrillBreakIcon");
            icon3 = mainAssetBundle.LoadAsset<Sprite>("BackblastIcon");
            icon3B = mainAssetBundle.LoadAsset<Sprite>("CaveInIcon");
            icon4 = mainAssetBundle.LoadAsset<Sprite>("ToTheStarsIcon");
            icon4S = mainAssetBundle.LoadAsset<Sprite>("ToTheStarsScepterIcon");
            icon4B = mainAssetBundle.LoadAsset<Sprite>("TimedExplosiveIcon");

            c4Model = mainAssetBundle.LoadAsset<GameObject>("MinerC4");

            tundraMesh = mainAssetBundle.LoadAsset<Mesh>("TundraMesh");
            steveMesh = mainAssetBundle.LoadAsset<Mesh>("SteveMesh");

            swingFX = mainAssetBundle.LoadAsset<GameObject>("MinerSwing");
            empoweredSwingFX = mainAssetBundle.LoadAsset<GameObject>("MinerSwingEmpowered");
            hitFX = mainAssetBundle.LoadAsset<GameObject>("ImpactMinerSwing");
            heavyHitFX = mainAssetBundle.LoadAsset<GameObject>("ImpactMinerHeavy");

            swingFX.AddComponent<DestroyOnTimer>().duration = 5;
            swingFX.AddComponent<NetworkIdentity>();
            swingFX.AddComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
            var effect = swingFX.AddComponent<EffectComponent>();
            effect.applyScale = false;
            effect.effectIndex = EffectIndex.Invalid;
            effect.parentToReferencedTransform = true;
            effect.positionAtReferencedTransform = true;
            effect.soundName = "";

            empoweredSwingFX.AddComponent<DestroyOnTimer>().duration = 5;
            empoweredSwingFX.AddComponent<NetworkIdentity>();
            empoweredSwingFX.AddComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
            effect = empoweredSwingFX.AddComponent<EffectComponent>();
            effect.applyScale = false;
            effect.effectIndex = EffectIndex.Invalid;
            effect.parentToReferencedTransform = true;
            effect.positionAtReferencedTransform = true;
            effect.soundName = "";

            hitFX.AddComponent<DestroyOnTimer>().duration = 5;
            hitFX.AddComponent<NetworkIdentity>();
            hitFX.AddComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
            effect = hitFX.AddComponent<EffectComponent>();
            effect.applyScale = false;
            effect.effectIndex = EffectIndex.Invalid;
            effect.parentToReferencedTransform = true;
            effect.positionAtReferencedTransform = true;
            effect.soundName = Sounds.Swing;

            heavyHitFX.AddComponent<DestroyOnTimer>().duration = 5;
            heavyHitFX.AddComponent<NetworkIdentity>();
            heavyHitFX.AddComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
            effect = heavyHitFX.AddComponent<EffectComponent>();
            effect.applyScale = false;
            effect.effectIndex = EffectIndex.Invalid;
            effect.parentToReferencedTransform = true;
            effect.positionAtReferencedTransform = true;
            effect.soundName = Sounds.Swing;

            EffectAPI.AddEffect(swingFX);
            EffectAPI.AddEffect(empoweredSwingFX);
            EffectAPI.AddEffect(hitFX);
            EffectAPI.AddEffect(heavyHitFX);
        }
    }
}