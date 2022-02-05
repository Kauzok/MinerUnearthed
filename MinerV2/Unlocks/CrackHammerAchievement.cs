using System;
using RoR2;

namespace DiggerPlugin.Achievements
{
    public class CrackHammerAchievement : GenericModdedUnlockable {

        public override string AchievementTokenPrefix => "MINER_CRACK";
        public override string AchievementSpriteName => "texSecondaryAchievement";

        public override string PrerequisiteUnlockableIdentifier => "MINER_UNLOCKABLE_ACHIEVEMENT_ID";

        public override BodyIndex LookUpRequiredBodyIndex() {
            return BodyCatalog.FindBodyIndex("MinerBody");
        }

        public void Check(bool check) {
            if (check && base.meetsBodyRequirement) base.Grant();
        }

        public override void OnInstall() {
            base.OnInstall();

            EntityStates.Digger.DiggerMain.JunkieAchieved += Check;
        }

        public override void OnUninstall() {
            base.OnUninstall();

            EntityStates.Digger.DiggerMain.JunkieAchieved -= Check;
        }
    }
}