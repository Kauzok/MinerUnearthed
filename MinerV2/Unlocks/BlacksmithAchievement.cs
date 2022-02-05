using System;
using RoR2;

namespace DiggerPlugin.Achievements
{
    public class BlacksmithAchievement : GenericModdedUnlockable {

        public override string AchievementTokenPrefix => "MINER_BLACKSMITH";
        public override string AchievementSpriteName => "texBlacksmithAchievement";

        public override string PrerequisiteUnlockableIdentifier => "MINER_UNLOCKABLE_ACHIEVEMENT_ID";

        public override BodyIndex LookUpRequiredBodyIndex() {
            return BodyCatalog.FindBodyIndex("MinerBody");
        }

        private void onGetHammer(bool cum) {
            if (cum && base.meetsBodyRequirement) base.Grant();
        }

        public override void OnInstall() {
            base.OnInstall();

            BlacksmithHammerComponent.HammerGetEvent += onGetHammer;
        }

        public override void OnUninstall() {
            base.OnUninstall();

            BlacksmithHammerComponent.HammerGetEvent -= onGetHammer;
        }
    }
}