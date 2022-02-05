using System;
using RoR2;

namespace DiggerPlugin.Achievements
{
    public class CrushAchievement : GenericModdedUnlockable {

        public override string AchievementTokenPrefix => "MINER_CRUSH";
        public override string AchievementSpriteName => "texPrimaryAchievement";

        public override string PrerequisiteUnlockableIdentifier => "MINER_UNLOCKABLE_ACHIEVEMENT_ID";

        public override BodyIndex LookUpRequiredBodyIndex() {
            return BodyCatalog.FindBodyIndex("MinerBody");
        }

        public void Check(DamageReport report) {
            if (report != null) {
                if (report.victimBody != null && report.attackerBody != null) {
                    if (report.attackerBodyIndex == BodyCatalog.FindBodyIndex("MinerBody") && report.victimBodyIndex == BodyCatalog.FindBodyIndex("TitanGoldBody")) {
                        if (report.victimBody.GetBuffCount(Buffs.cleaveBuff) >= 12f && base.meetsBodyRequirement) {
                            base.Grant();
                        }
                    }
                }
            }
        }

        public override void OnInstall() {
            base.OnInstall();

            GlobalEventManager.onServerDamageDealt += Check;
        }

        public override void OnUninstall() {
            base.OnUninstall();

            GlobalEventManager.onServerDamageDealt -= Check;
        }
    }
}