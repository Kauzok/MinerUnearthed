using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace DiggerPlugin
{
    public static class Buffs
    {
        internal static BuffDef goldRushBuff;
        internal static BuffDef cleaveBuff;

        //internal static List<BuffDef> buffDefs = new List<BuffDef>();

        internal static void RegisterBuffs()
        {
            goldRushBuff = AddNewBuff("GoldRush", LegacyResourcesAPI.Load<Sprite>("Textures/BuffIcons/texBuffOnFireIcon"), DiggerPlugin.characterColor, true, false);
            cleaveBuff = AddNewBuff("Cleave", LegacyResourcesAPI.Load<Sprite>("Textures/BuffIcons/texBuffPulverizeIcon"), DiggerPlugin.characterColor, true, true);
        }

        // simple helper method
        internal static BuffDef AddNewBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff)
        {
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = buffName;
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.eliteDef = null;
            buffDef.iconSprite = buffIcon;

            R2API.ContentAddition.AddBuffDef(buffDef);

            return buffDef;
        }
    }
}
