using System;
using RoR2;

namespace DiggerPlugin.Achievements
{
    public class CaveInAchievement : GenericModdedUnlockable {

        public override string AchievementTokenPrefix => "MINER_CAVEIN";
        public override string AchievementSpriteName => "texUtilityAchievement";

        public override string PrerequisiteUnlockableIdentifier => "MINER_UNLOCKABLE_ACHIEVEMENT_ID";

        public override BodyIndex LookUpRequiredBodyIndex() {
            return BodyCatalog.FindBodyIndex("MinerBody");
        }

        public void Check(int count) {
            if (count >= 20 && base.meetsBodyRequirement) base.Grant();
        }

        public override void OnInstall() {
            base.OnInstall();

            EntityStates.Digger.BackBlast.Compacted += Check;
        }

        public override void OnUninstall() {
            base.OnUninstall();

            EntityStates.Digger.BackBlast.Compacted -= Check;
        }
    }
}