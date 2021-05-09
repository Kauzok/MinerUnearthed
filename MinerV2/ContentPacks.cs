using RoR2.ContentManagement;

namespace DiggerPlugin
{
    internal class ContentPacks : IContentPackProvider
    {
        internal ContentPack contentPack = new ContentPack();
        public string identifier => DiggerPlugin.MODUID;

        public void Initialize()
        {
            ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
        }

        private void ContentManager_collectContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(this);
        }

        public System.Collections.IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            this.contentPack.identifier = this.identifier;
            contentPack.bodyPrefabs.Add(DiggerPlugin.bodyPrefabs.ToArray());
            contentPack.buffDefs.Add(Buffs.buffDefs.ToArray());
            //contentPack.effectDefs.Add(Assets.effectDefs.ToArray());
            //contentPack.entityStateTypes.Add(States.entityStates.ToArray());
            contentPack.masterPrefabs.Add(DiggerPlugin.masterPrefabs.ToArray());
            contentPack.networkSoundEventDefs.Add(Assets.networkSoundEventDefs.ToArray());
            //contentPack.projectilePrefabs.Add(Prefabs.projectilePrefabs.ToArray());
            //contentPack.skillDefs.Add(Skills.skillDefs.ToArray());
            //contentPack.skillFamilies.Add(DiggerPlugin.skillFamilies.ToArray());
            contentPack.survivorDefs.Add(DiggerPlugin.survivorDefs.ToArray());
            //contentPack.unlockableDefs.Add(Unlockables.unlockableDefs.ToArray());

            args.ReportProgress(1f);
            yield break;
        }

        public System.Collections.IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(this.contentPack, args.output);
            args.ReportProgress(1f);
            yield break;
        }

        public System.Collections.IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }
    }
}