using R2API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zio.FileSystems;
using DiggerPlugin;

namespace DiggerUnearthed.Module
{
    public class LanguageTokens
    {
        public static SubFileSystem fileSystem;
        internal static string languageRoot => System.IO.Path.Combine(LanguageTokens.assemblyDir, "language");

        internal static string assemblyDir
        {
            get
            {
                return System.IO.Path.GetDirectoryName(DiggerPlugin.DiggerPlugin.pluginInfo.Location);
            }
        }

        public static void RegisterLanguageTokens()
        {
            On.RoR2.Language.SetFolders += fixme;
            return;

            /*
            LanguageAPI.Add("MINER_NAME", DiggerPlugin.DiggerPlugin.characterName);
            LanguageAPI.Add("MINER_DESCRIPTION", DiggerPlugin.DiggerPlugin.characterDesc);
            LanguageAPI.Add("MINER_SUBTITLE", DiggerPlugin.DiggerPlugin.characterSubtitle);
            LanguageAPI.Add("MINER_LORE", DiggerPlugin.DiggerPlugin.characterLore);
            LanguageAPI.Add("MINER_OUTRO_FLAVOR", DiggerPlugin.DiggerPlugin.characterOutro);

            LanguageAPI.Add("MINER_PASSIVE_NAME", "Gold Rush");
            LanguageAPI.Add("MINER_PASSIVE_DESCRIPTION", "Gain <style=cIsHealth>ADRENALINE</style> when receiving gold, increasing <style=cIsDamage>attack speed</style>, <style=cIsUtility>movement speed</style>, and <style=cIsHealing>health regen</style>.");  // <style=cIsUtility>Any increase in gold refreshes all stacks.</style>


            LanguageAPI.Add("KEYWORD_CLEAVING", "<style=cKeywordName>Cleaving</style><style=cSub>Applies a stacking debuff that lowers <style=cIsDamage>armor</style> by <style=cIsHealth>3 per stack</style>.</style>");
            string desc = "<style=cIsUtility>Agile.</style> Wildly swing at nearby enemies for <style=cIsDamage>" + 100f * gougeDamage.Value + "% damage</style>, <style=cIsHealth>cleaving</style> their armor.";

            LanguageAPI.Add("MINER_PRIMARY_GOUGE_NAME", "Gouge");
            LanguageAPI.Add("MINER_PRIMARY_GOUGE_DESCRIPTION", desc);

            desc = "<style=cIsUtility>Agile.</style> Crush nearby enemies for <style=cIsDamage>" + 100f * crushDamage.Value + "% damage</style>. <style=cIsUtility>Range increases with attack speed</style>.";

            LanguageAPI.Add("MINER_PRIMARY_CRUSH_NAME", "Crush");
            LanguageAPI.Add("MINER_PRIMARY_CRUSH_DESCRIPTION", desc);
            

            string desc = "Charge up and dash into enemies for up to <style=cIsDamage>6x" + 100f * drillChargeDamage.Value + "% damage</style>. <style=cIsUtility>You cannot be hit for the duration.</style>";

            LanguageAPI.Add("MINER_SECONDARY_CHARGE_NAME", "Drill Charge");
            LanguageAPI.Add("MINER_SECONDARY_CHARGE_DESCRIPTION", desc);
            

            desc = "Dash forward, exploding for <style=cIsDamage>2x" + 100f * drillBreakDamage.Value + "% damage</style> on contact with an enemy. <style=cIsUtility>You cannot be hit for the duration.</style>";

            LanguageAPI.Add("MINER_SECONDARY_BREAK_NAME", "Crack Hammer");
            LanguageAPI.Add("MINER_SECONDARY_BREAK_DESCRIPTION", desc);

            LanguageAPI.Add("MINER_UTILITY_BACKBLAST_NAME", "Backblast");
            LanguageAPI.Add("MINER_UTILITY_BACKBLAST_DESCRIPTION", "<style=cIsDamage>Stunning.</style> Blast backwards, hitting nearby enemies for <style=cIsDamage>" + 100f * BackBlast.damageCoefficient + "% damage</style>. Hold to travel further. <style=cIsUtility>You cannot be hit for the duration.</style>");

            LanguageAPI.Add("MINER_UTILITY_CAVEIN_NAME", "Cave In");
            LanguageAPI.Add("MINER_UTILITY_CAVEIN_DESCRIPTION", "<style=cIsUtility>Stunning.</style> Blast backwards, <style=cIsUtility>pulling</style> in all enemies in a large radius. <style=cIsUtility>You cannot be hit for the duration.</style>");
            
            LanguageAPI.Add("MINER_SPECIAL_TOTHESTARSCLASSIC_NAME", "To The Stars");
            LanguageAPI.Add("MINER_SPECIAL_TOTHESTARSCLASSIC_DESCRIPTION", "Jump into the air, hitting enemies directly below for <style=cIsDamage>6x" + 100f * ToTheStarsClassic.damageCoefficient + "% damage</style>.");

            LanguageAPI.Add("MINER_SPECIAL_TOTHESTARS_NAME", "Meteor Shower");
            LanguageAPI.Add("MINER_SPECIAL_TOTHESTARS_DESCRIPTION", "Jump into the air, shooting a spray of shrapnel downwards for <style=cIsDamage>15x" + 100f * ToTheStars.damageCoefficient + "% damage</style>.");
            

            LanguageAPI.Add("MINER_SPECIAL_SCEPTERTOTHESTARS_NAME", "Falling Comet");
            LanguageAPI.Add("MINER_SPECIAL_SCEPTERTOTHESTARS_DESCRIPTION", "Jump into the air, shooting a spray of shrapnel downwards for <style=cIsDamage>30x" + 100f * FallingComet.damageCoefficient + "% damage</style>, then fall downwards and create a huge blast on impact that deals <style=cIsDamage>" + 100f * FallingComet.blastDamageCoefficient + "% damage</style> and <style=cIsDamage>ignites</style> enemies hit.");
            
            LanguageAPI.Add("MINER_SPECIAL_SCEPTERTOTHESTARSCLASSIC_NAME", "Starbound");
            LanguageAPI.Add("MINER_SPECIAL_SCEPTERTOTHESTARSCLASSIC_DESCRIPTION", "<style=cIsDamage>Stunning</style>. Jump into the air, hitting enemies directly below for <style=cIsDamage>10x" + 100f * ToTheStarsClassic.damageCoefficient + "% damage</style> total.");
            */
        }

        //Credits to Anreol for this code
        private static void fixme(On.RoR2.Language.orig_SetFolders orig, RoR2.Language self, System.Collections.Generic.IEnumerable<string> newFolders)
        {
            if (System.IO.Directory.Exists(LanguageTokens.languageRoot))
            {
                var dirs = System.IO.Directory.EnumerateDirectories(System.IO.Path.Combine(LanguageTokens.languageRoot), self.name);
                orig(self, newFolders.Union(dirs));
                return;
            }
            orig(self, newFolders);
        }
    }
}
