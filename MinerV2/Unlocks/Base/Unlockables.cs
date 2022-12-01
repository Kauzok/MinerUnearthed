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