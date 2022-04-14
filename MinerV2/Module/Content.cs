using System;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Modules {
    public class Content {

        //must be some DRY way to do this

        //prefabs
        public static void AddBody(GameObject prefab) => AddCharacterBodyPrefab(prefab);
        public static void AddCharacterBodyPrefab(params GameObject[] prefabs) {

            foreach (GameObject prefab in prefabs) {
                ContentPacks.bodyPrefabs.Add(prefab);
            }
        }

        public static void AddMaster(GameObject prefab) => AddMasterPrefab(prefab);
        public static void AddMasterPrefab(params GameObject[] prefabs) {

            foreach (GameObject prefab in prefabs) {
                ContentPacks.masterPrefabs.Add(prefab);
            }
        }

        public static void AddProjectile(GameObject prefab) => AddProjectilePrefab(prefab);
        public static void AddProjectilePrefab(params GameObject[] prefabs) {

            foreach (GameObject prefab in prefabs) {
                ContentPacks.projectilePrefabs.Add(prefab);
            }
        }

        //survivors and skills
        public static void AddSurvivorDef(SurvivorDef survivorDef) {

            ContentPacks.survivorDefs.Add(survivorDef);
        }

        public static void AddUnlockableDef(UnlockableDef unlockableDef) {

            ContentPacks.unlockableDefs.Add(unlockableDef);
        }

        public static void AddSkillDef(SkillDef skillDef) {

            if (ContentPacks.skillDefs.Contains(skillDef))
                return;

            ContentPacks.skillDefs.Add(skillDef);
        }

        public static void AddSkillFamily(SkillFamily skillFamily) {

            ContentPacks.skillFamilies.Add(skillFamily);
        }

        public static void AddEntityState<T>(out bool wasAdded) where T : EntityStates.EntityState {

            wasAdded = true;

            AddEntityState(typeof(T));
        }

        public static void AddEntityState(params Type[] entityStates) {
            foreach (var entityState in entityStates) {
                ContentPacks.entityStates.Add(entityState);
            }
        }

        //other defs
        public static void AddBuffDef(BuffDef buffDef) {

            ContentPacks.buffDefs.Add(buffDef);
        }


        internal static void AddEffect(GameObject effectPrefab) {
            EffectDef newEffectDef = new EffectDef();
            newEffectDef.prefab = effectPrefab;
            newEffectDef.prefabEffectComponent = effectPrefab.GetComponent<EffectComponent>();
            newEffectDef.prefabName = effectPrefab.name;
            newEffectDef.prefabVfxAttributes = effectPrefab.GetComponent<VFXAttributes>();
            newEffectDef.spawnSoundEventName = effectPrefab.name;

            Modules.Content.AddEffectDef(newEffectDef);
        }

        public static void AddEffect(EffectDef def) => AddEffectDef(def);
        public static void AddEffectDef(params EffectDef[] effectDefs) {
            foreach (EffectDef effectDef in effectDefs) {
                ContentPacks.effectDefs.Add(effectDef);
            }
        }

        public static void AddNetworkSoundEventDef(NetworkSoundEventDef networkSoundEventDef) {

            ContentPacks.networkSoundEventDefs.Add(networkSoundEventDef);
        }
    }
}