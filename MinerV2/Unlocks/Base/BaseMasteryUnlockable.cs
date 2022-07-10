using System;
using System.Runtime.CompilerServices;
using R2API;
using RoR2;
using UnityEngine;

namespace DiggerPlugin.Achievements
{
    public abstract class BaseMasteryUnlockable : GenericModdedUnlockable
    {
        public abstract string RequiredCharacterBody { get; }
        public abstract float RequiredDifficultyCoefficient { get; }

        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();
            Run.onClientGameOverGlobal += OnClientGameOverGlobal;
        }
        public override void OnBodyRequirementBroken()
        {
            Run.onClientGameOverGlobal -= OnClientGameOverGlobal;
            base.OnBodyRequirementBroken();
        }
        private void OnClientGameOverGlobal(Run run, RunReport runReport)
        {
            if (runReport.gameEnding && runReport.gameEnding.isWin) {

                DifficultyIndex difficultyIndex = runReport.ruleBook.FindDifficulty();
                DifficultyDef runDifficulty = DifficultyCatalog.GetDifficultyDef(difficultyIndex);

                DifficultyDef infernoDef = null;
                if (DiggerPlugin.infernoPluginLoaded)
                {
                    infernoDef = GetInfernoDef();
                }

                if ((infernoDef != null && runDifficulty == infernoDef)
                    || (runDifficulty.countsAsHardMode && runDifficulty.scalingValue >= RequiredDifficultyCoefficient)
                    || (difficultyIndex >= DifficultyIndex.Eclipse1 && difficultyIndex <= DifficultyIndex.Eclipse8))
                {
                    Grant();
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private DifficultyDef GetInfernoDef()
        {
            return Inferno.Main.InfernoDiffDef;
        }

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex(RequiredCharacterBody);
        }
    }
}