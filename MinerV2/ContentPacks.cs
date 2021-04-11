using System.Collections.Generic;
using RoR2;
using UnityEngine;

namespace DiggerPlugin
{
    internal class ContentPacks
    {
        internal static ContentPack contentPack;

        internal void CreateContentPack()
        {
            contentPack = new ContentPack()
            {
                artifactDefs = new ArtifactDef[0],
                bodyPrefabs = DiggerPlugin.bodyPrefabs.ToArray(),
                buffDefs = Buffs.buffDefs.ToArray(),
                effectDefs = new EffectDef[0],
                eliteDefs = new EliteDef[0],
                entityStateConfigurations = new EntityStateConfiguration[0],
                entityStateTypes = new System.Type[0],
                equipmentDefs = new EquipmentDef[0],
                gameEndingDefs = new GameEndingDef[0],
                gameModePrefabs = new Run[0],
                itemDefs = new ItemDef[0],
                masterPrefabs = DiggerPlugin.masterPrefabs.ToArray(),
                musicTrackDefs = new MusicTrackDef[0],
                networkedObjectPrefabs = new GameObject[0],
                networkSoundEventDefs = Assets.networkSoundEventDefs.ToArray(),
                projectilePrefabs = DiggerPlugin.projectilePrefabs.ToArray(),
                sceneDefs = new SceneDef[0],
                skillDefs = new RoR2.Skills.SkillDef[0],
                skillFamilies = new RoR2.Skills.SkillFamily[0],
                surfaceDefs = new SurfaceDef[0],
                survivorDefs = DiggerPlugin.survivorDefs.ToArray(),
                unlockableDefs = new UnlockableDef[0]
            };

            On.RoR2.ContentManager.SetContentPacks += AddContent;
        }

        private void AddContent(On.RoR2.ContentManager.orig_SetContentPacks orig, List<ContentPack> newContentPacks)
        {
            newContentPacks.Add(contentPack);
            orig(newContentPacks);
        }
    }
}
