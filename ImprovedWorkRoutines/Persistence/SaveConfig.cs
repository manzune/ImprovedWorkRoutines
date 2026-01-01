using ImprovedWorkRoutines.Persistence.Datas;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Events;

#if IL2CPP
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Persistence;
#elif MONO
using ScheduleOne.DevUtilities;
using ScheduleOne.Persistence;
#endif

namespace ImprovedWorkRoutines.Persistence
{
    public static class SaveConfig
    {
        public static readonly JsonSerializerSettings JsonSerializerSettings = new()
        {
            NullValueHandling = NullValueHandling.Include,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            Formatting = Formatting.Indented
        };

        public static DataWrapper Data { get; private set; }

        private static string FilePath => Path.Combine(Singleton<LoadManager>.Instance.ActiveSaveInfo.SavePath, $"{ModInfo.Name}.json");

        public static void LoadConfig()
        {
            if (File.Exists(FilePath))
            {
                string text = File.ReadAllText(FilePath);
                Data = JsonConvert.DeserializeObject<DataWrapper>(text, JsonSerializerSettings);
            }
            else
            {
                Data = new()
                {
                    Botanists = []
                };
            }

            Singleton<SaveManager>.Instance.onSaveComplete.AddListener((UnityAction)OnSaveComplete);

            Utils.Logger.Debug($"Config for SaveGame_{Singleton<LoadManager>.Instance.ActiveSaveInfo.SaveSlotNumber} loaded");
        }

        public static void ClearConfig()
        {
            Data = default;

            Singleton<SaveManager>.Instance.onSaveComplete.RemoveListener((UnityAction)OnSaveComplete);
        }

        private static void OnSaveComplete()
        {
            string text = JsonConvert.SerializeObject(Data, JsonSerializerSettings);
            File.WriteAllText(FilePath, text);

            Utils.Logger.Debug($"Config for SaveGame_{Singleton<LoadManager>.Instance.ActiveSaveInfo.SaveSlotNumber} saved");
        }

        public struct DataWrapper
        {
            public List<BotanistData> Botanists;
        }
    }
}
