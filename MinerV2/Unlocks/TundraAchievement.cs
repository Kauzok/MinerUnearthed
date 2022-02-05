using System;
using RoR2;

namespace DiggerPlugin.Achievements {
    public class TundraAchievement : GenericModdedUnlockable {

        public override string AchievementTokenPrefix => "MINER_TUNDRA";
        public override string AchievementSpriteName => "texTundraAchievement";

        public override string PrerequisiteUnlockableIdentifier => "MINER_UNLOCKABLE_ACHIEVEMENT_ID";

        public override BodyIndex LookUpRequiredBodyIndex() {
            return BodyCatalog.FindBodyIndex("MinerBody");
        }

        public void Check(float time) {
            if (time <= 480f && base.meetsBodyRequirement) {
                base.Grant();
            }
        }

        public override void OnInstall() {
            base.OnInstall();

            EntityStates.Digger.DiggerMain.rallypoint += Check;
        }

        public override void OnUninstall() {
            base.OnUninstall();

            EntityStates.Digger.DiggerMain.rallypoint -= Check;
        }
    }
}