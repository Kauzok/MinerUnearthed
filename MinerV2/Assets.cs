using System.Reflection;
using R2API;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using RoR2;
using System.Collections.Generic;

namespace DiggerPlugin
{
    public static class Assets
    {
        public static AssetBundle mainAssetBundle = null;

        public static GameObject blacksmithHammer;
        public static GameObject blacksmithAnvil;

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
        public static Mesh blacksmithMesh;
        public static Mesh grandMasteryMesh;
        public static Mesh dripMesh;
        public static Mesh steveMesh;

        public static GameObject swingFX;
        public static GameObject crushSwingFX;
        public static GameObject empoweredSwingFX;
        public static GameObject hitFX;
        public static GameObject slashFX;
        public static GameObject heavyHitFX;

        public static NetworkSoundEventDef pickHitEventDef;

        public static List<NetworkSoundEventDef> networkSoundEventDefs = new List<NetworkSoundEventDef>();

        public static void PopulateAssets()
        {
            if (mainAssetBundle == null)
            {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DiggerUnearthed.digger"))
                {
                    mainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                }
            }

            using (Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream("DiggerUnearthed.DiggerBank.bnk"))
            {
                byte[] array = new byte[manifestResourceStream2.Length];
                manifestResourceStream2.Read(array, 0, array.Length);
                SoundAPI.SoundBanks.Add(array);
            }

            using (Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream("DiggerUnearthed.DiggerV2.bnk"))
            {
                byte[] array = new byte[manifestResourceStream2.Length];
                manifestResourceStream2.Read(array, 0, array.Length);
                SoundAPI.SoundBanks.Add(array);
            }

            blacksmithHammer = mainAssetBundle.LoadAsset<GameObject>("BlacksmithHammer");
            blacksmithHammer.AddComponent<BlacksmithHammerComponent>();
            blacksmithHammer.GetComponentInChildren<MeshRenderer>().material.shader = Resources.Load<Shader>("Shaders/Deferred/hgstandard");

            blacksmithAnvil = mainAssetBundle.LoadAsset<GameObject>("BlacksmithAnvil");
            blacksmithAnvil.GetComponentInChildren<MeshRenderer>().material.shader = Resources.Load<Shader>("Shaders/Deferred/hgstandard");
            blacksmithAnvil.gameObject.layer = LayerIndex.world.intVal;

            charPortrait = mainAssetBundle.LoadAsset<Texture>("texMinerIcon");

            iconP = mainAssetBundle.LoadAsset<Sprite>("GoldRushIcon");
            icon1 = mainAssetBundle.LoadAsset<Sprite>("GougeIcon");
            icon1B = mainAssetBundle.LoadAsset<Sprite>("CrushIcon");
            icon2 = mainAssetBundle.LoadAsset<Sprite>("DrillChargeIcon");
            icon2B = mainAssetBundle.LoadAsset<Sprite>("DrillBreakIcon");
            icon3 = mainAssetBundle.LoadAsset<Sprite>("BackblastIcon");
            icon3B = mainAssetBundle.LoadAsset<Sprite>("CaveInIcon");
            icon4 = mainAssetBundle.LoadAsset<Sprite>("ToTheStarsIcon");
            icon4S = mainAssetBundle.LoadAsset<Sprite>("ToTheStarsScepterIcon");
            icon4B = mainAssetBundle.LoadAsset<Sprite>("TimedExplosiveIcon");

            c4Model = mainAssetBundle.LoadAsset<GameObject>("MinerC4");

            tundraMesh = mainAssetBundle.LoadAsset<Mesh>("TundraMesh");
            blacksmithMesh = mainAssetBundle.LoadAsset<Mesh>("BlacksmithMesh");
            grandMasteryMesh = mainAssetBundle.LoadAsset<Mesh>("meshMinerGM");
            dripMesh = mainAssetBundle.LoadAsset<Mesh>("DripMesh");
            steveMesh = mainAssetBundle.LoadAsset<Mesh>("SteveMesh");

            swingFX = LoadEffect("MinerSwing", "", mainAssetBundle);
            crushSwingFX = LoadEffect("MinerSwingAltAlt", "", mainAssetBundle, true);
            empoweredSwingFX = LoadEffect("MinerSwingEmpowered", "", mainAssetBundle);
            hitFX = LoadEffect("ImpactMinerSwing", "", mainAssetBundle);
            slashFX = LoadEffect("ImpactMinerSlash", "", mainAssetBundle);
            heavyHitFX = LoadEffect("ImpactMinerHeavy", "", mainAssetBundle);

            pickHitEventDef = CreateNetworkSoundEventDef(Sounds.Hit);
        }

        internal static NetworkSoundEventDef CreateNetworkSoundEventDef(string eventName)
        {
            NetworkSoundEventDef networkSoundEventDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            networkSoundEventDef.akId = AkSoundEngine.GetIDFromString(eventName);
            networkSoundEventDef.eventName = eventName;

            networkSoundEventDefs.Add(networkSoundEventDef);

            return networkSoundEventDef;
        }

        private static GameObject LoadEffect(string resourceName, string soundName, AssetBundle bundle, bool applyScale = false)
        {
            GameObject newEffect = bundle.LoadAsset<GameObject>(resourceName);

            newEffect.AddComponent<DestroyOnTimer>().duration = 12;
            newEffect.AddComponent<NetworkIdentity>();
            newEffect.AddComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
            var effect = newEffect.AddComponent<EffectComponent>();
            effect.applyScale = applyScale;
            effect.effectIndex = EffectIndex.Invalid;
            effect.parentToReferencedTransform = true;
            effect.positionAtReferencedTransform = true;
            effect.soundName = soundName;

            EffectAPI.AddEffect(newEffect);

            return newEffect;
        }
    }
}