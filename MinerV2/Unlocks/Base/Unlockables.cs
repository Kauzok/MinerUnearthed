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

            RegisterUnlockableTokens();

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

        private static void RegisterUnlockableTokens() {

            #region character
            if (DiggerPlugin.direseekerInstalled) {
                LanguageAPI.Add("MINER_UNLOCKABLE_ACHIEVEMENT_NAME", "Forged in Flames");
                LanguageAPI.Add("MINER_UNLOCKABLE_ACHIEVEMENT_DESC", "Defeat the unique guardian of Abyssal Depths.");
                LanguageAPI.Add("MINER_UNLOCKABLE_UNLOCKABLE_NAME", "Forged in Flames");
            } else {
                LanguageAPI.Add("MINER_UNLOCKABLE_ACHIEVEMENT_NAME", "Adrenaline Rush");
                LanguageAPI.Add("MINER_UNLOCKABLE_ACHIEVEMENT_DESC", "Open a Legendary Chest.");
                LanguageAPI.Add("MINER_UNLOCKABLE_UNLOCKABLE_NAME", "Adrenaline Rush");
            }

            LanguageAPI.Add("MINER_MONSOONUNLOCKABLE_ACHIEVEMENT_NAME", "Miner: Mastery");
            LanguageAPI.Add("MINER_MONSOONUNLOCKABLE_ACHIEVEMENT_DESC", "As Miner, beat the game or obliterate on Monsoon.");
            LanguageAPI.Add("MINER_MONSOONUNLOCKABLE_UNLOCKABLE_NAME", "Miner: Mastery");

            string masteryFootnote = DiggerPlugin.starstormInstalled ? "" : "\n<color=#8888>(Typhoon difficulty requires Starstorm 2)</color>";

            LanguageAPI.Add("MINER_TYPHOONUNLOCKABLE_ACHIEVEMENT_NAME", "Miner: Grand Mastery");
            LanguageAPI.Add("MINER_TYPHOONUNLOCKABLE_ACHIEVEMENT_DESC", "As Miner, beat the game or obliterate on Typhoon or higher." + masteryFootnote);
            LanguageAPI.Add("MINER_TYPHOONUNLOCKABLE_UNLOCKABLE_NAME", "Miner: Grand Mastery");
            #endregion

            #region skills
            LanguageAPI.Add("MINER_CRUSHUNLOCKABLE_ACHIEVEMENT_NAME", "Miner: Pillaged Gold");
            LanguageAPI.Add("MINER_CRUSHUNLOCKABLE_ACHIEVEMENT_DESC", "As Miner, apply 12 stacks of cleave to the unique guardian of Gilded Coast.");
            LanguageAPI.Add("MINER_CRUSHUNLOCKABLE_UNLOCKABLE_NAME", "Miner: Pillaged Gold");

            LanguageAPI.Add("MINER_CRACKUNLOCKABLE_ACHIEVEMENT_NAME", "Miner: Junkie");
            LanguageAPI.Add("MINER_CRACKUNLOCKABLE_ACHIEVEMENT_DESC", "As Miner, gain " + DiggerPlugin.maxAdrenaline.Value + " stacks of Adrenaline.");
            LanguageAPI.Add("MINER_CRACKUNLOCKABLE_UNLOCKABLE_NAME", "Miner: Junkie");

            LanguageAPI.Add("MINER_CAVEINUNLOCKABLE_ACHIEVEMENT_NAME", "Miner: Compacted");
            LanguageAPI.Add("MINER_CAVEINUNLOCKABLE_ACHIEVEMENT_DESC", "As Miner, hit 20 enemies with one Backblast.");
            LanguageAPI.Add("MINER_CAVEINUNLOCKABLE_UNLOCKABLE_NAME", "Miner: Compacted");
            #endregion

            #region skans
            LanguageAPI.Add("MINER_PUPLEUNLOCKABLE_ACHIEVEMENT_NAME", "Miner: The Five Keys");
            LanguageAPI.Add("MINER_PUPLEUNLOCKABLE_ACHIEVEMENT_DESC", "As Miner, Discover G-N-O-M-E's debugging secret.");
            LanguageAPI.Add("MINER_PUPLEUNLOCKABLE_UNLOCKABLE_NAME", "Miner: The Five Keys");

            LanguageAPI.Add("MINER_TUNDRAUNLOCKABLE_ACHIEVEMENT_NAME", "Miner: Frozen");
            LanguageAPI.Add("MINER_TUNDRAUNLOCKABLE_ACHIEVEMENT_DESC", "As Miner, reach Rallypoint Delta in under 8 minutes.");
            LanguageAPI.Add("MINER_TUNDRAUNLOCKABLE_UNLOCKABLE_NAME", "Miner: Frozen");

            LanguageAPI.Add("MINER_BLACKSMITHUNLOCKABLE_ACHIEVEMENT_NAME", "Miner: Tempered");
            LanguageAPI.Add("MINER_BLACKSMITHUNLOCKABLE_ACHIEVEMENT_DESC", "As Miner, find a smithing tool on the planet.");
            LanguageAPI.Add("MINER_BLACKSMITHUNLOCKABLE_UNLOCKABLE_NAME", "Miner: Tempered");
            #endregion
        }
    }
}