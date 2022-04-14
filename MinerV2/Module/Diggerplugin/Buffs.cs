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
            goldRushBuff = AddNewBuff("GoldRush",
                                      Assets.mainAssetBundle.LoadAsset<Sprite>("texIconGoldRush"),// LegacyResourcesAPI.Load<Sprite>("Textures/BuffIcons/texBuffOnFireIcon"), 
                                      new Color(0.8584906f, 0.7394358f, 0.2632165f),
                                      true, 
                                      false);

            cleaveBuff = AddNewBuff("Cleave",
                                    LegacyResourcesAPI.Load<BuffDef>("BuffDefs/Pulverized").iconSprite, 
                                    DiggerPlugin.characterColor, 
                                    true,
                                    true);
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

            Modules.Content.AddBuffDef(buffDef);

            return buffDef;
        }
    }
}
