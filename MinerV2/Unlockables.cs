using System;
using R2API;
using RoR2;
using R2API.Utils;

namespace DiggerPlugin
{
    public static class Unlockables
    {
        public static void RegisterUnlockables()
        {
            LanguageAPI.Add("MINER_MONSOONUNLOCKABLE_ACHIEVEMENT_NAME", "Miner: Mastery");
            LanguageAPI.Add("MINER_MONSOONUNLOCKABLE_ACHIEVEMENT_DESC", "As Miner, beat the game or obliterate on Monsoon.");
            LanguageAPI.Add("MINER_MONSOONUNLOCKABLE_UNLOCKABLE_NAME", "Miner: Mastery");

            LanguageAPI.Add("MINER_TUNDRAUNLOCKABLE_ACHIEVEMENT_NAME", "Miner: Frozen");
            LanguageAPI.Add("MINER_TUNDRAUNLOCKABLE_ACHIEVEMENT_DESC", "As Miner, reach Rallypoint Delta in under 8 minutes.");
            LanguageAPI.Add("MINER_TUNDRAUNLOCKABLE_UNLOCKABLE_NAME", "Miner: Frozen");

            LanguageAPI.Add("MINER_PUPLEUNLOCKABLE_ACHIEVEMENT_NAME", "Miner: The Five Keys");
            LanguageAPI.Add("MINER_PUPLEUNLOCKABLE_ACHIEVEMENT_DESC", "As Miner, Discover G-N-O-M-E's debugging secret.");
            LanguageAPI.Add("MINER_PUPLEUNLOCKABLE_UNLOCKABLE_NAME", "Miner: The Five Keys");

            LanguageAPI.Add("MINER_CRUSHUNLOCKABLE_ACHIEVEMENT_NAME", "Miner: Pillaged Gold");
            LanguageAPI.Add("MINER_CRUSHUNLOCKABLE_ACHIEVEMENT_DESC", "As Miner, apply 12 stacks of cleave to the unique guardian of Gilded Coast.");
            LanguageAPI.Add("MINER_CRUSHUNLOCKABLE_UNLOCKABLE_NAME", "Miner: Pillaged Gold");

            LanguageAPI.Add("MINER_CRACKUNLOCKABLE_ACHIEVEMENT_NAME", "Miner: Junkie");
            LanguageAPI.Add("MINER_CRACKUNLOCKABLE_ACHIEVEMENT_DESC", "As Miner, gain " + DiggerPlugin.maxAdrenaline.Value + " stacks of Adrenaline.");
            LanguageAPI.Add("MINER_CRACKUNLOCKABLE_UNLOCKABLE_NAME", "Miner: Junkie");

            LanguageAPI.Add("MINER_CAVEINUNLOCKABLE_ACHIEVEMENT_NAME", "Miner: Compacted");
            LanguageAPI.Add("MINER_CAVEINUNLOCKABLE_ACHIEVEMENT_DESC", "As Miner, hit 20 enemies with one Backblast.");
            LanguageAPI.Add("MINER_CAVEINUNLOCKABLE_UNLOCKABLE_NAME", "Miner: Compacted");

            LanguageAPI.Add("MINER_BLACKSMITHUNLOCKABLE_ACHIEVEMENT_NAME", "Miner: Tempered");
            LanguageAPI.Add("MINER_BLACKSMITHUNLOCKABLE_ACHIEVEMENT_DESC", "As Miner, find a smithing tool on the planet.");
            LanguageAPI.Add("MINER_BLACKSMITHUNLOCKABLE_UNLOCKABLE_NAME", "Miner: Tempered");


            if (DiggerPlugin.direseekerInstalled)
            {
                LanguageAPI.Add("MINER_UNLOCKABLE_ACHIEVEMENT_NAME", "Forged in Flames");
                LanguageAPI.Add("MINER_UNLOCKABLE_ACHIEVEMENT_DESC", "Defeat the unique guardian of Abyssal Depths.");
                LanguageAPI.Add("MINER_UNLOCKABLE_UNLOCKABLE_NAME", "Forged in Flames");

                UnlockablesAPI.AddUnlockable<Achievements.DiggerAltUnlockAchievement>(true);
            }
            else
            {
                LanguageAPI.Add("MINER_UNLOCKABLE_ACHIEVEMENT_NAME", "Adrenaline Rush");
                LanguageAPI.Add("MINER_UNLOCKABLE_ACHIEVEMENT_DESC", "Open a Legendary Chest.");
                LanguageAPI.Add("MINER_UNLOCKABLE_UNLOCKABLE_NAME", "Adrenaline Rush");

                UnlockablesAPI.AddUnlockable<Achievements.DiggerUnlockAchievement>(true);
            }

            UnlockablesAPI.AddUnlockable<Achievements.MasteryAchievement>(true);
            UnlockablesAPI.AddUnlockable<Achievements.TundraAchievement>(true);
            UnlockablesAPI.AddUnlockable<Achievements.PupleAchievement>(true);
            UnlockablesAPI.AddUnlockable<Achievements.CrushAchievement>(true);
            UnlockablesAPI.AddUnlockable<Achievements.CrackHammerAchievement>(true);
            UnlockablesAPI.AddUnlockable<Achievements.CaveInAchievement>(true);
            UnlockablesAPI.AddUnlockable<Achievements.BlacksmithAchievement>(true);
        }
    }
}

namespace DiggerPlugin.Achievements
{
    [R2APISubmoduleDependency(nameof(UnlockablesAPI))]

    public class DiggerUnlockAchievement : ModdedUnlockableAndAchievement<CustomSpriteProvider>
    {
        public override String AchievementIdentifier { get; } = "MINER_UNLOCKABLE_ACHIEVEMENT_ID";
        public override String UnlockableIdentifier { get; } = "MINER_UNLOCKABLE_REWARD_ID";
        public override String PrerequisiteUnlockableIdentifier { get; } = "MINER_UNLOCKABLE_PREREQ_ID";
        public override String AchievementNameToken { get; } = "MINER_UNLOCKABLE_ACHIEVEMENT_NAME";
        public override String AchievementDescToken { get; } = "MINER_UNLOCKABLE_ACHIEVEMENT_DESC";
        public override String UnlockableNameToken { get; } = "MINER_UNLOCKABLE_UNLOCKABLE_NAME";
        protected override CustomSpriteProvider SpriteProvider { get; } = new CustomSpriteProvider("@Miner:Assets/Miner/Icons/texMinerAchievement.png");

        public void Check(On.RoR2.ChestBehavior.orig_Open orig, ChestBehavior self)
        {
            if (self && self.tier3Chance >= 1f) base.Grant();

            orig(self);
        }

        public override void OnInstall()
        {
            base.OnInstall();

            On.RoR2.ChestBehavior.Open += this.Check;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            On.RoR2.ChestBehavior.Open -= this.Check;
        }
    }

    public class DiggerAltUnlockAchievement : ModdedUnlockableAndAchievement<CustomSpriteProvider>
    {
        public override String AchievementIdentifier { get; } = "MINER_UNLOCKABLE_ACHIEVEMENT_ID";
        public override String UnlockableIdentifier { get; } = "MINER_UNLOCKABLE_REWARD_ID";
        public override String PrerequisiteUnlockableIdentifier { get; } = "MINER_UNLOCKABLE_PREREQ_ID";
        public override String AchievementNameToken { get; } = "MINER_UNLOCKABLE_ACHIEVEMENT_NAME";
        public override String AchievementDescToken { get; } = "MINER_UNLOCKABLE_ACHIEVEMENT_DESC";
        public override String UnlockableNameToken { get; } = "MINER_UNLOCKABLE_UNLOCKABLE_NAME";
        protected override CustomSpriteProvider SpriteProvider { get; } = new CustomSpriteProvider("@Miner:Assets/Miner/Icons/texMinerAchievement.png");

        private void CheckDeath(Run run)
        {
            base.Grant();
        }

        public override void OnInstall()
        {
            base.OnInstall();

            DiggerUnlockComponent.OnDeath += CheckDeath;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            DiggerUnlockComponent.OnDeath -= CheckDeath;
        }
    }


    public class MasteryAchievement : ModdedUnlockableAndAchievement<CustomSpriteProvider>
    {
        public override String AchievementIdentifier { get; } = "MINER_MONSOONUNLOCKABLE_ACHIEVEMENT_ID";
        public override String UnlockableIdentifier { get; } = "MINER_MONSOONUNLOCKABLE_REWARD_ID";
        public override String PrerequisiteUnlockableIdentifier { get; } = "MINER_MONSOONUNLOCKABLE_PREREQ_ID";
        public override String AchievementNameToken { get; } = "MINER_MONSOONUNLOCKABLE_ACHIEVEMENT_NAME";
        public override String AchievementDescToken { get; } = "MINER_MONSOONUNLOCKABLE_ACHIEVEMENT_DESC";
        public override String UnlockableNameToken { get; } = "MINER_MONSOONUNLOCKABLE_UNLOCKABLE_NAME";
        protected override CustomSpriteProvider SpriteProvider { get; } = new CustomSpriteProvider("@Miner:Assets/Miner/Icons/texMoltenAchievement.png");

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

    public class TundraAchievement : ModdedUnlockableAndAchievement<CustomSpriteProvider>
    {
        public override String AchievementIdentifier { get; } = "MINER_TUNDRAUNLOCKABLE_ACHIEVEMENT_ID";
        public override String UnlockableIdentifier { get; } = "MINER_TUNDRAUNLOCKABLE_REWARD_ID";
        public override String PrerequisiteUnlockableIdentifier { get; } = "MINER_TUNDRAUNLOCKABLE_PREREQ_ID";
        public override String AchievementNameToken { get; } = "MINER_TUNDRAUNLOCKABLE_ACHIEVEMENT_NAME";
        public override String AchievementDescToken { get; } = "MINER_TUNDRAUNLOCKABLE_ACHIEVEMENT_DESC";
        public override String UnlockableNameToken { get; } = "MINER_TUNDRAUNLOCKABLE_UNLOCKABLE_NAME";
        protected override CustomSpriteProvider SpriteProvider { get; } = new CustomSpriteProvider("@Miner:Assets/Miner/Icons/texTundraAchievement.png");

        public override int LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("MinerBody");
        }

        public void Check(float time)
        {
            if (time <= 480f && base.meetsBodyRequirement)
            {
                base.Grant();
            }
        }

        public override void OnInstall()
        {
            base.OnInstall();

            EntityStates.Digger.DiggerMain.rallypoint += Check;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            EntityStates.Digger.DiggerMain.rallypoint -= Check;
        }
    }

    public class PupleAchievement : ModdedUnlockableAndAchievement<CustomSpriteProvider>
    {
        public override String AchievementIdentifier { get; } = "MINER_PUPLEUNLOCKABLE_ACHIEVEMENT_ID";
        public override String UnlockableIdentifier { get; } = "MINER_PUPLEUNLOCKABLE_REWARD_ID";
        public override String PrerequisiteUnlockableIdentifier { get; } = "MINER_PUPLEUNLOCKABLE_PREREQ_ID";
        public override String AchievementNameToken { get; } = "MINER_PUPLEUNLOCKABLE_ACHIEVEMENT_NAME";
        public override String AchievementDescToken { get; } = "MINER_PUPLEUNLOCKABLE_ACHIEVEMENT_DESC";
        public override String UnlockableNameToken { get; } = "MINER_PUPLEUNLOCKABLE_UNLOCKABLE_NAME";
        protected override CustomSpriteProvider SpriteProvider { get; } = new CustomSpriteProvider("@Miner:Assets/Miner/Icons/texPupleAchievement.png");

        public override int LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("MinerBody");
        }

        public void Check(Run run)
        {
            if (base.meetsBodyRequirement)
            {
                base.Grant();
            }
        }

        public override void OnInstall()
        {
            base.OnInstall();

            EntityStates.Digger.DiggerMain.SecretAchieved += Check;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            EntityStates.Digger.DiggerMain.SecretAchieved -= Check;
        }
    }

    public class CrushAchievement : ModdedUnlockableAndAchievement<CustomSpriteProvider>
    {
        public override String AchievementIdentifier { get; } = "MINER_CRUSHUNLOCKABLE_ACHIEVEMENT_ID";
        public override String UnlockableIdentifier { get; } = "MINER_CRUSHUNLOCKABLE_REWARD_ID";
        public override String PrerequisiteUnlockableIdentifier { get; } = "MINER_CRUSHUNLOCKABLE_PREREQ_ID";
        public override String AchievementNameToken { get; } = "MINER_CRUSHUNLOCKABLE_ACHIEVEMENT_NAME";
        public override String AchievementDescToken { get; } = "MINER_CRUSHUNLOCKABLE_ACHIEVEMENT_DESC";
        public override String UnlockableNameToken { get; } = "MINER_CRUSHUNLOCKABLE_UNLOCKABLE_NAME";
        protected override CustomSpriteProvider SpriteProvider { get; } = new CustomSpriteProvider("@Miner:Assets/Miner/Icons/texPrimaryAchievement.png");

        public override int LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("MinerBody");
        }

        public void Check(DamageReport report)
        {
            if (report != null)
            {
                if (report.victimBody != null && report.attackerBody != null)
                {
                    if (report.attackerBodyIndex == BodyCatalog.FindBodyIndex("MinerBody") && report.victimBodyIndex == BodyCatalog.FindBodyIndex("TitanGoldBody"))
                    {
                        if (report.victimBody.GetBuffCount(DiggerPlugin.cleave) >= 12f && base.meetsBodyRequirement)
                        {
                            base.Grant();
                        }
                    }
                }
            }
        }

        public override void OnInstall()
        {
            base.OnInstall();

            GlobalEventManager.onServerDamageDealt += Check;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            GlobalEventManager.onServerDamageDealt -= Check;
        }
    }

    public class CrackHammerAchievement : ModdedUnlockableAndAchievement<CustomSpriteProvider>
    {
        public override String AchievementIdentifier { get; } = "MINER_CRACKUNLOCKABLE_ACHIEVEMENT_ID";
        public override String UnlockableIdentifier { get; } = "MINER_CRACKUNLOCKABLE_REWARD_ID";
        public override String PrerequisiteUnlockableIdentifier { get; } = "MINER_CRACKUNLOCKABLE_PREREQ_ID";
        public override String AchievementNameToken { get; } = "MINER_CRACKUNLOCKABLE_ACHIEVEMENT_NAME";
        public override String AchievementDescToken { get; } = "MINER_CRACKUNLOCKABLE_ACHIEVEMENT_DESC";
        public override String UnlockableNameToken { get; } = "MINER_CRACKUNLOCKABLE_UNLOCKABLE_NAME";
        protected override CustomSpriteProvider SpriteProvider { get; } = new CustomSpriteProvider("@Miner:Assets/Miner/Icons/texSecondaryAchievement.png");

        public override int LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("MinerBody");
        }

        public void Check(bool check)
        {
            if (check && base.meetsBodyRequirement) base.Grant();
        }

        public override void OnInstall()
        {
            base.OnInstall();

            EntityStates.Digger.DiggerMain.JunkieAchieved += Check;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            EntityStates.Digger.DiggerMain.JunkieAchieved -= Check;
        }
    }

    public class CaveInAchievement : ModdedUnlockableAndAchievement<CustomSpriteProvider>
    {
        public override String AchievementIdentifier { get; } = "MINER_CAVEINUNLOCKABLE_ACHIEVEMENT_ID";
        public override String UnlockableIdentifier { get; } = "MINER_CAVEINUNLOCKABLE_REWARD_ID";
        public override String PrerequisiteUnlockableIdentifier { get; } = "MINER_CAVEINUNLOCKABLE_PREREQ_ID";
        public override String AchievementNameToken { get; } = "MINER_CAVEINUNLOCKABLE_ACHIEVEMENT_NAME";
        public override String AchievementDescToken { get; } = "MINER_CAVEINUNLOCKABLE_ACHIEVEMENT_DESC";
        public override String UnlockableNameToken { get; } = "MINER_CAVEINUNLOCKABLE_UNLOCKABLE_NAME";
        protected override CustomSpriteProvider SpriteProvider { get; } = new CustomSpriteProvider("@Miner:Assets/Miner/Icons/texUtilityAchievement.png");

        public override int LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("MinerBody");
        }

        public void Check(int count)
        {
            if (count >= 20 && base.meetsBodyRequirement) base.Grant();
        }

        public override void OnInstall()
        {
            base.OnInstall();

            EntityStates.Digger.BackBlast.Compacted += Check;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            EntityStates.Digger.BackBlast.Compacted -= Check;
        }
    }

    public class BlacksmithAchievement : ModdedUnlockableAndAchievement<CustomSpriteProvider>
    {
        public override String AchievementIdentifier { get; } = "MINER_BLACKSMITHUNLOCKABLE_ACHIEVEMENT_ID";
        public override String UnlockableIdentifier { get; } = "MINER_BLACKSMITHUNLOCKABLE_REWARD_ID";
        public override String PrerequisiteUnlockableIdentifier { get; } = "MINER_BLACKSMITHUNLOCKABLE_PREREQ_ID";
        public override String AchievementNameToken { get; } = "MINER_BLACKSMITHUNLOCKABLE_ACHIEVEMENT_NAME";
        public override String AchievementDescToken { get; } = "MINER_BLACKSMITHUNLOCKABLE_ACHIEVEMENT_DESC";
        public override String UnlockableNameToken { get; } = "MINER_BLACKSMITHUNLOCKABLE_UNLOCKABLE_NAME";
        protected override CustomSpriteProvider SpriteProvider { get; } = new CustomSpriteProvider("@Miner:Assets/Miner/Icons/texBlacksmithAchievement.png");

        public override int LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("MinerBody");
        }

        private void Get(bool cum)
        {
            if (cum && base.meetsBodyRequirement) base.Grant();
        }

        public override void OnInstall()
        {
            base.OnInstall();

            BlacksmithHammerComponent.HammerGet += Get;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            BlacksmithHammerComponent.HammerGet -= Get;
        }
    }
}