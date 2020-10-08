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
        public static AssetBundle MainAssetBundle = null;

        public static Texture charPortrait;

        public static Sprite iconP;
        public static Sprite icon1;
        public static Sprite icon2;
        public static Sprite icon2B;
        public static Sprite icon3;
        public static Sprite icon3B;
        public static Sprite icon4;
        public static Sprite icon4B;

        public static Mesh tundraMesh;

        public static GameObject styleMeter;

        public static Sprite styleD;
        public static Sprite styleC;
        public static Sprite styleB;
        public static Sprite styleA;
        public static Sprite styleS;
        public static Sprite styleSS;
        public static Sprite styleSSS;

        public static Sprite styleTextD;
        public static Sprite styleTextC;
        public static Sprite styleTextB;
        public static Sprite styleTextA;
        public static Sprite styleTextS;
        public static Sprite styleTextSS;
        public static Sprite styleTextSSS;

        public static void PopulateAssets()
        {
            if (MainAssetBundle == null)
            {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MinerV2.miner"))
                {
                    MainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                    var provider = new AssetBundleResourcesProvider("@Miner", MainAssetBundle);
                    ResourcesAPI.AddProvider(provider);
                }
            }

            using (Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream("MinerV2.MinerBank.bnk"))
            {
                byte[] array = new byte[manifestResourceStream2.Length];
                manifestResourceStream2.Read(array, 0, array.Length);
                SoundAPI.SoundBanks.Add(array);
            }

            charPortrait = MainAssetBundle.LoadAsset<Sprite>("texMinerIcon").texture;

            iconP = MainAssetBundle.LoadAsset<Sprite>("GoldRushIcon");
            icon1 = MainAssetBundle.LoadAsset<Sprite>("CrushIcon");
            icon2 = MainAssetBundle.LoadAsset<Sprite>("DrillChargeIcon");
            icon2B = MainAssetBundle.LoadAsset<Sprite>("DrillBreakIcon");
            icon3 = MainAssetBundle.LoadAsset<Sprite>("BackblastIcon");
            icon3B = MainAssetBundle.LoadAsset<Sprite>("CaveInIcon");
            icon4 = MainAssetBundle.LoadAsset<Sprite>("ToTheStarsIcon");
            icon4B = MainAssetBundle.LoadAsset<Sprite>("TimedExplosiveIcon");

            tundraMesh = MainAssetBundle.LoadAsset<Mesh>("TundraMesh");

            styleMeter = MainAssetBundle.LoadAsset<GameObject>("StylePanel");

            styleD = MainAssetBundle.LoadAsset<Sprite>("StyleD");
            styleC = MainAssetBundle.LoadAsset<Sprite>("StyleC");
            styleB = MainAssetBundle.LoadAsset<Sprite>("StyleB");
            styleA = MainAssetBundle.LoadAsset<Sprite>("StyleA");
            styleS = MainAssetBundle.LoadAsset<Sprite>("StyleS");
            styleSS = MainAssetBundle.LoadAsset<Sprite>("StyleSS");
            styleSSS = MainAssetBundle.LoadAsset<Sprite>("StyleSSS");

            styleTextD = MainAssetBundle.LoadAsset<Sprite>("StyleDismal");
            styleTextC = MainAssetBundle.LoadAsset<Sprite>("StyleCrazy");
            styleTextB = MainAssetBundle.LoadAsset<Sprite>("StyleBadass");
            styleTextA = MainAssetBundle.LoadAsset<Sprite>("StyleApocalyptic");
            styleTextS = MainAssetBundle.LoadAsset<Sprite>("StyleSavage");
            styleTextSS = MainAssetBundle.LoadAsset<Sprite>("StyleSickSkills");
            styleTextSSS = MainAssetBundle.LoadAsset<Sprite>("StyleSmokinSexyStyle");
        }
    }
}