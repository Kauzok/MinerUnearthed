using System;
using RoR2;

namespace DiggerPlugin.Achievements {
    public class PupleAchievement : GenericModdedUnlockable {

        public override string AchievementTokenPrefix => "MINER_PUPLE";
        public override string AchievementSpriteName => "texPupleAchievement";

        public override string PrerequisiteUnlockableIdentifier => "MINER_UNLOCKABLE_ACHIEVEMENT_ID";

        public override BodyIndex LookUpRequiredBodyIndex() {
            return BodyCatalog.FindBodyIndex("MinerBody");
        }

        public void Check(Run run) {
            if (base.meetsBodyRequirement) {
                base.Grant();
            }
        }

        public override void OnInstall() {
            base.OnInstall();

            EntityStates.Digger.DiggerMain.SecretAchieved += Check;
        }

        public override void OnUninstall() {
            base.OnUninstall();

            EntityStates.Digger.DiggerMain.SecretAchieved -= Check;
        }
    }
}