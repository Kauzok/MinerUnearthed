using System;
using RoR2;

namespace DiggerPlugin.Achievements
{

    public class DiggerAltUnlockAchievement : GenericModdedUnlockable

    {
        public override string AchievementTokenPrefix => "MINER_";
        public override string AchievementNameToken => "MINER_ALTUNLOCKABLE_ACHIEVEMENT_NAME";
        public override string AchievementDescToken => "MINER_ALTUNLOCKABLE_ACHIEVEMENT_DESC";
        public override string UnlockableNameToken => "MINER_ALTUNLOCKABLE_UNLOCKABLE_NAME";
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
}