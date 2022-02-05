using System;
using RoR2;

namespace DiggerPlugin.Achievements
{

    public class DiggerAltUnlockAchievement : GenericModdedUnlockable

    {
        public override string AchievementTokenPrefix => "MINER_";
        public override string PrerequisiteUnlockableIdentifier => "";

        public override string AchievementSpriteName => "texMinerAchievement";

        private void CheckDireseekerDed(Run run)
        {
            base.Grant();
        }

        public override void OnInstall()
        {
            base.OnInstall();

            DiggerUnlockComponent.OnDeath += CheckDireseekerDed;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            DiggerUnlockComponent.OnDeath -= CheckDireseekerDed;
        }
    }
    //public class DiggerAltUnlockAchievement : ModdedUnlockableAndAchievement<CustomSpriteProvider>
    //{
    //    public override String AchievementIdentifier { get; } = "MINER_UNLOCKABLE_ACHIEVEMENT_ID";
    //    public override String UnlockableIdentifier { get; } = "MINER_UNLOCKABLE_REWARD_ID";
    //    public override String PrerequisiteUnlockableIdentifier { get; } = "MINER_UNLOCKABLE_PREREQ_ID";
    //    public override String AchievementNameToken { get; } = "MINER_UNLOCKABLE_ACHIEVEMENT_NAME";
    //    public override String AchievementDescToken { get; } = "MINER_UNLOCKABLE_ACHIEVEMENT_DESC";
    //    public override String UnlockableNameToken { get; } = "MINER_UNLOCKABLE_UNLOCKABLE_NAME";
    //    protected override CustomSpriteProvider SpriteProvider { get; } = new CustomSpriteProvider("@Miner:Assets/Miner/Icons/texMinerAchievement.png");

    //    private void CheckDeath(Run run)
    //    {
    //        base.Grant();
    //    }

    //    public override void OnInstall()
    //    {
    //        base.OnInstall();

    //        DiggerUnlockComponent.OnDeath += CheckDeath;
    //    }

    //    public override void OnUninstall()
    //    {
    //        base.OnUninstall();

    //        DiggerUnlockComponent.OnDeath -= CheckDeath;
    //    }
    //}
}