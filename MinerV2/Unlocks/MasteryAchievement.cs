using System;
using RoR2;

namespace DiggerPlugin.Achievements
{

    public class MasteryAchievementButEpic : BaseMasteryUnlockable
    {
        public override string AchievementTokenPrefix => "MINER_MONSOON";
        public override string PrerequisiteUnlockableIdentifier => "MINER_UNLOCKABLE_ACHIEVEMENT_ID";
        public override string AchievementSpriteName => "texMoltenAchievement";

        public override string RequiredCharacterBody => "MinerBody";

        public override float RequiredDifficultyCoefficient => 3f;
    }

    public class GrandMasteryAchievement : BaseMasteryUnlockable
    {
        public override string AchievementTokenPrefix => "MINER_TYPHOON";
        public override string PrerequisiteUnlockableIdentifier => "MINER_UNLOCKABLE_ACHIEVEMENT_ID";
        public override string AchievementSpriteName => "texGrandMasteryAchievement";

        public override string RequiredCharacterBody => "MinerBody";

        public override float RequiredDifficultyCoefficient => 3.5f;
    }
}