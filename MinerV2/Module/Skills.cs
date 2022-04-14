using EntityStates;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Modules {

    internal static class Skills
    {
        #region genericskills
        public static void CreateSkillFamilies(GameObject targetPrefab, int families = 15, bool destroyExisting = true) {

            if (destroyExisting) {
                foreach (GenericSkill obj in targetPrefab.GetComponentsInChildren<GenericSkill>()) {
                    UnityEngine.Object.DestroyImmediate(obj);
                }
            }

            SkillLocator skillLocator = targetPrefab.GetComponent<SkillLocator>();

            if ((families & (1 << 0)) != 0) {
                skillLocator.primary = CreateGenericSkillWithSkillFamily(targetPrefab, "Primary");
            }
            if ((families & (1 << 1)) != 0) {
                skillLocator.secondary = CreateGenericSkillWithSkillFamily(targetPrefab, "Secondary");
            }
            if ((families & (1 << 2)) != 0) {
                skillLocator.utility = CreateGenericSkillWithSkillFamily(targetPrefab, "Utility");
            }
            if ((families & (1 << 3)) != 0) {
                skillLocator.special = CreateGenericSkillWithSkillFamily(targetPrefab, "Special");
            }
        }

        public static GenericSkill CreateGenericSkillWithSkillFamily(GameObject targetPrefab, string familyName, bool hidden = false) {

            GenericSkill skill = targetPrefab.AddComponent<GenericSkill>();
            skill.skillName = familyName;
            skill.hideInCharacterSelect = hidden;

            SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
            (newFamily as ScriptableObject).name = targetPrefab.name + familyName + "SkillFamily";
            newFamily.variants = new SkillFamily.Variant[0];

            skill._skillFamily = newFamily;

            Modules.Content.AddSkillFamily(newFamily);
            return skill;
        }
        #endregion

        #region skillfamilies

        //everything calls this
        public static void AddSkillToFamily(SkillFamily skillFamily, SkillDef skillDef, UnlockableDef unlockableDef = null) {

            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            
            (skillDef as ScriptableObject).name = skillDef.skillName;

            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant {
                skillDef = skillDef,
                unlockableDef = unlockableDef,
                viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, false, null)
            };

            Modules.Content.AddSkillDef(skillDef);
        }

        public static void AddSkillsToFamily(SkillFamily skillFamily, params SkillDef[] skillDefs) {

            foreach (SkillDef skillDef in skillDefs) {
                AddSkillToFamily(skillFamily, skillDef);
            }
        }
        public static void AddPrimarySkills(GameObject targetPrefab, params SkillDef[] skillDefs) {
            AddSkillsToFamily(targetPrefab.GetComponent<SkillLocator>().primary.skillFamily, skillDefs);
        }
        public static void AddSecondarySkills(GameObject targetPrefab, params SkillDef[] skillDefs) {
            AddSkillsToFamily(targetPrefab.GetComponent<SkillLocator>().secondary.skillFamily, skillDefs);
        }
        public static void AddUtilitySkills(GameObject targetPrefab, params SkillDef[] skillDefs) {
            AddSkillsToFamily(targetPrefab.GetComponent<SkillLocator>().utility.skillFamily, skillDefs);
        }
        public static void AddSpecialSkills(GameObject targetPrefab, params SkillDef[] skillDefs) {
            AddSkillsToFamily(targetPrefab.GetComponent<SkillLocator>().special.skillFamily, skillDefs);
        }

        /// <summary>
        /// pass in an amount of unlockables equal to or less than skill variants
        /// <code>
        /// AddUnlockablesToFamily(skillLocator.primary, null, skill2UnlockableDef, null, skill4UnlockableDef);
        /// </code>
        /// </summary>
        public static void AddUnlockablesToFamily(SkillFamily skillFamily, params UnlockableDef[] unlockableDefs) {

            for (int i = 0; i < unlockableDefs.Length; i++) {
                SkillFamily.Variant variant = skillFamily.variants[i];
                variant.unlockableDef = unlockableDefs[i];
                skillFamily.variants[i] = variant;
            }
        }
        #endregion
    }
}
