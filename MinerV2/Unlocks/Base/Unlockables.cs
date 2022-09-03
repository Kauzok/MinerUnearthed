using R2API;
using R2API.Utils;
using RoR2;
using DiggerPlugin.Achievements;

namespace DiggerPlugin
{
    public static class Unlockables
    {

        public static UnlockableDef diggerUnlockableDef;
        public static UnlockableDef diggerMasteryUnlockableDef;
        public static UnlockableDef diggerGrandMasteryUnlockableDef;

        public static UnlockableDef crushUnlockableDef;
        public static UnlockableDef crackHammerUnlockableDef;
        public static UnlockableDef caveInUnlockableDef;

        public static UnlockableDef pupleUnlockableDef;
        public static UnlockableDef tundraUnlockableDef;
        public static UnlockableDef blacksmithUnlockableDef;

        public static void RegisterUnlockables() {
            if (DiggerPlugin.direseekerInstalled)
            {
                LanguageAPI.Add("MINER_UNLOCKABLE_ACHIEVEMENT_NAME", "Forged in Flames");
                LanguageAPI.Add("MINER_UNLOCKABLE_ACHIEVEMENT_DESC", "Defeat the unique guardian of Abyssal Depths.");
                LanguageAPI.Add("MINER_UNLOCKABLE_UNLOCKABLE_NAME", "Forged in Flames");
            }
            else
            {
                LanguageAPI.Add("MINER_UNLOCKABLE_ACHIEVEMENT_NAME", "Adrenaline Rush");
                LanguageAPI.Add("MINER_UNLOCKABLE_ACHIEVEMENT_DESC", "Open a Legendary Chest.");
                LanguageAPI.Add("MINER_UNLOCKABLE_UNLOCKABLE_NAME", "Adrenaline Rush");
            }

            if (!DiggerPlugin.forceUnlock.Value) {
                if (DiggerPlugin.direseekerInstalled) {
                    diggerUnlockableDef = UnlockableAPI.AddUnlockable<DiggerAltUnlockAchievement>();
                } else {
                    diggerUnlockableDef = UnlockableAPI.AddUnlockable<DiggerUnlockAchievement>();
                }
            } else {
                diggerUnlockableDef = null;
            }

            diggerMasteryUnlockableDef = UnlockableAPI.AddUnlockable<MasteryAchievementButEpic>();
            diggerGrandMasteryUnlockableDef = UnlockableAPI.AddUnlockable<GrandMasteryAchievement>();

            crushUnlockableDef = UnlockableAPI.AddUnlockable<CrushAchievement>(typeof(CrushAchievement.CrushAchievementServer));
            crackHammerUnlockableDef = UnlockableAPI.AddUnlockable<CrackHammerAchievement>();
            caveInUnlockableDef = UnlockableAPI.AddUnlockable<CaveInAchievement>();

            pupleUnlockableDef = UnlockableAPI.AddUnlockable<PupleAchievement>();
            tundraUnlockableDef = UnlockableAPI.AddUnlockable<TundraAchievement>();
            blacksmithUnlockableDef = UnlockableAPI.AddUnlockable<BlacksmithAchievement>();
        }
    }
}