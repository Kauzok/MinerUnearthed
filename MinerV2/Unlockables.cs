using System;
using R2API;
using RoR2;
using R2API.Utils;

namespace MinerPlugin
{
    public static class Unlockables
    {
        public static void RegisterUnlockables()
        {
            LanguageAPI.Add("MINER_MONSOONUNLOCKABLE_ACHIEVEMENT_NAME", "Miner: Mastery");
            LanguageAPI.Add("MINER_MONSOONUNLOCKABLE_ACHIEVEMENT_DESC", "As Miner, beat the game or obliterate on Monsoon.");
            LanguageAPI.Add("MINER_MONSOONUNLOCKABLE_UNLOCKABLE_NAME", "Miner: Mastery");

            LanguageAPI.Add("MINER_TUNDRAUNLOCKABLE_ACHIEVEMENT_NAME", "Miner: Frozen");
            LanguageAPI.Add("MINER_TUNDRAUNLOCKABLE_ACHIEVEMENT_DESC", "As Miner, beat the game.");
            LanguageAPI.Add("MINER_TUNDRAUNLOCKABLE_UNLOCKABLE_NAME", "Miner: Frozen");

            UnlockablesAPI.AddUnlockable<Achievements.MasteryAchievement>(true);
            UnlockablesAPI.AddUnlockable<Achievements.TundraAchievement>(true);
        }
    }
}

namespace MinerPlugin.Achievements
{
    [R2APISubmoduleDependency(nameof(UnlockablesAPI))]

    public class MasteryAchievement : ModdedUnlockableAndAchievement<VanillaSpriteProvider>
    {
        public override String AchievementIdentifier { get; } = "MINER_MONSOONUNLOCKABLE_ACHIEVEMENT_ID";
        public override String UnlockableIdentifier { get; } = "MINER_MONSOONUNLOCKABLE_REWARD_ID";
        public override String PrerequisiteUnlockableIdentifier { get; } = "MINER_MONSOONUNLOCKABLE_PREREQ_ID";
        public override String AchievementNameToken { get; } = "MINER_MONSOONUNLOCKABLE_ACHIEVEMENT_NAME";
        public override String AchievementDescToken { get; } = "MINER_MONSOONUNLOCKABLE_ACHIEVEMENT_DESC";
        public override String UnlockableNameToken { get; } = "MINER_MONSOONUNLOCKABLE_UNLOCKABLE_NAME";
        protected override VanillaSpriteProvider SpriteProvider { get; } = new VanillaSpriteProvider("");

        public override int LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("MinerBody");
        }

        public void ClearCheck(Run run, RunReport runReport)
        {
            if (run is null) return;
            if (runReport is null) return;

            if (!runReport.gameEnding) return;

            if (runReport.gameEnding.isWin)
            {
                DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(runReport.ruleBook.FindDifficulty());

                if (difficultyDef != null && difficultyDef.countsAsHardMode)
                {
                    if (base.meetsBodyRequirement)
                    {
                        base.Grant();
                    }
                }
            }
        }

        public override void OnInstall()
        {
            base.OnInstall();

            Run.onClientGameOverGlobal += this.ClearCheck;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            Run.onClientGameOverGlobal -= this.ClearCheck;
        }
    }

    public class TundraAchievement : ModdedUnlockableAndAchievement<VanillaSpriteProvider>
    {
        public override String AchievementIdentifier { get; } = "MINER_TUNDRAUNLOCKABLE_ACHIEVEMENT_ID";
        public override String UnlockableIdentifier { get; } = "MINER_TUNDRAUNLOCKABLE_REWARD_ID";
        public override String PrerequisiteUnlockableIdentifier { get; } = "MINER_TUNDRAUNLOCKABLE_PREREQ_ID";
        public override String AchievementNameToken { get; } = "MINER_TUNDRAUNLOCKABLE_ACHIEVEMENT_NAME";
        public override String AchievementDescToken { get; } = "MINER_TUNDRAUNLOCKABLE_ACHIEVEMENT_DESC";
        public override String UnlockableNameToken { get; } = "MINER_TUNDRAUNLOCKABLE_UNLOCKABLE_NAME";
        protected override VanillaSpriteProvider SpriteProvider { get; } = new VanillaSpriteProvider("");

        public override int LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("MinerBody");
        }

        public void ClearCheck(Run run, RunReport runReport)
        {
            if (run is null) return;
            if (runReport is null) return;

            if (!runReport.gameEnding) return;

            if (runReport.gameEnding.isWin)
            {
                if (base.meetsBodyRequirement)
                {
                    base.Grant();
                }
            }
        }

        public override void OnInstall()
        {
            base.OnInstall();

            Run.onClientGameOverGlobal += this.ClearCheck;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            Run.onClientGameOverGlobal -= this.ClearCheck;
        }
    }
}