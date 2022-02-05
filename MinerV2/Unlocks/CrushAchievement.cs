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

        // Token: 0x06003926 RID: 14630 RVA: 0x000DD289 File Offset: 0x000DB489
        public override void OnBodyRequirementMet() {
            base.OnBodyRequirementMet();

            base.SetServerTracked(true);
        }

        // Token: 0x06003927 RID: 14631 RVA: 0x000DD298 File Offset: 0x000DB498
        public override void OnBodyRequirementBroken() {
            base.SetServerTracked(false);

            base.OnBodyRequirementBroken();
        }
        public class CrushAchievementServer : RoR2.Achievements.BaseServerAchievement {

            public void Check(DamageReport report) {
                if (report != null) {
                    if (report.victimBody != null && report.attackerBody != null && report.attackerBody == base.GetCurrentBody()) {
                        if (report.victimBodyIndex == BodyCatalog.FindBodyIndex("TitanGoldBody")) {
                            if (report.victimBody.GetBuffCount(Buffs.cleaveBuff) >= 12f) {
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
}