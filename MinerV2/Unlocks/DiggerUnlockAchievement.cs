using System;
using RoR2;

namespace DiggerPlugin.Achievements
{

    public class DiggerUnlockAchievement : GenericModdedUnlockable
    {
        public override string AchievementTokenPrefix => "MINER_";
        public override string PrerequisiteUnlockableIdentifier => "";

        public override string AchievementSpriteName => "texMinerAchievement";

        public void CheckChest(On.RoR2.ChestBehavior.orig_Open orig, ChestBehavior self)
        {
            if (self && self.tier3Chance >= 1f) base.Grant();

            orig(self);
        }

        public override void OnInstall()
        {
            base.OnInstall();

            On.RoR2.ChestBehavior.Open += this.CheckChest;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            On.RoR2.ChestBehavior.Open -= this.CheckChest;
        }

    }
}