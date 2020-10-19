using RoR2;
using RoR2.Skills;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Miner
{
    public class MinerMain : GenericCharacterMain
    {
        public static float passiveStyleCoefficient = 0.4f;

        private StyleSystem.StyleComponent styleComponent;
        private int moneyTracker;
        private float residue;
        private int buffCounter;

        public override void OnEnter()
        {
            base.OnEnter();

            this.styleComponent = base.GetComponent<StyleSystem.StyleComponent>();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (NetworkServer.active) this.UpdatePassiveBuff();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void UpdatePassiveBuff()
        {
            int currentCount = base.characterBody.GetBuffCount(MinerPlugin.MinerPlugin.goldRush);
            int newMoney = (int)base.characterBody.master.money;
            if (this.moneyTracker < newMoney)
            {
                this.RefreshExistingStacks(currentCount);
                this.GiveNewStacks(newMoney);
            }
            this.moneyTracker = newMoney;

            this.HandleStackDecay(currentCount);
        }

        private void RefreshExistingStacks(int currentCount)
        {
            base.characterBody.ClearTimedBuffs(MinerPlugin.MinerPlugin.goldRush);
            for (int i = 0; i < currentCount; i++)
            {
                if (base.characterBody.GetBuffCount(MinerPlugin.MinerPlugin.goldRush) <= MinerPlugin.MinerPlugin.adrenalineCap) base.characterBody.AddTimedBuff(MinerPlugin.MinerPlugin.goldRush, 5);
            }
        }

        private void GiveNewStacks(int newMoney)
        {
            float baseReward = (newMoney - this.moneyTracker) / Mathf.Pow(Run.instance.difficultyCoefficient, 1.25f);
            this.residue = (baseReward + this.residue) % 5;
            float numStacks = (baseReward + this.residue) / 5;

            for (int i = 1; i <= numStacks; i++)
            {
                if (base.characterBody.GetBuffCount(MinerPlugin.MinerPlugin.goldRush) <= MinerPlugin.MinerPlugin.adrenalineCap) base.characterBody.AddTimedBuff(MinerPlugin.MinerPlugin.goldRush, 5);
            }

            if (this.styleComponent) this.styleComponent.AddStyle(MinerMain.passiveStyleCoefficient);
        }

        private void HandleStackDecay(int currentCount)
        {
            if (this.buffCounter != 0 && currentCount == 0)
            {
                for (int i = 1; i < buffCounter * .5; i++)
                {
                    if (base.characterBody.GetBuffCount(MinerPlugin.MinerPlugin.goldRush) <= MinerPlugin.MinerPlugin.adrenalineCap) base.characterBody.AddTimedBuff(MinerPlugin.MinerPlugin.goldRush, 1);
                }
            }

            this.buffCounter = base.characterBody.GetBuffCount(MinerPlugin.MinerPlugin.goldRush);
        }
    }
}