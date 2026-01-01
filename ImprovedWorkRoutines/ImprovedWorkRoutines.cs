using ImprovedWorkRoutines;
using MelonLoader;

[assembly: MelonInfo(typeof(ImprovedWorkRoutines.ImprovedWorkRoutines), $"{ModInfo.Name}", ModInfo.Version, ModInfo.Author, ModInfo.DownloadLink)]
[assembly: MelonGame("TVGS", "Schedule I")]
[assembly: MelonColor(255, 224, 138, 72)]
#if IL2CPP
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]
#elif MONO
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.MONO)]
#endif

namespace ImprovedWorkRoutines
{
    public class ImprovedWorkRoutines : MelonMod
    {
        public bool IsInitialized { get; private set; }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (sceneName == "Menu")
            {
                if (!IsInitialized)
                {
                    ModConfig.Initialize();

                    Utils.Logger.Msg($"{ModInfo.Name} v{ModInfo.Version} initialized");

                    IsInitialized = true;
                }
            }
            else if (sceneName == "Main")
            {
            }
        }
    }
}
