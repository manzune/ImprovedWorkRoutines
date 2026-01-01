using MelonLoader;
using MelonLoader.Utils;
using System.IO;

namespace ImprovedWorkRoutines
{
    public static class ModConfig
    {
        private static MelonPreferences_Category generalCategory;

        private static MelonPreferences_Category chemistCategory;

        private static bool isInitialized;

        public static bool Debug
        {
            get => generalCategory.GetEntry<bool>("Debug").Value;
            set => generalCategory.GetEntry<bool>("Debug").Value = value;
        }

        public struct Chemist
        {
            public static float InsertIngredientTime
            {
                get => chemistCategory.GetEntry<float>("InsertIngredientTime").Value;
                set => chemistCategory.GetEntry<float>("InsertIngredientTime").Value = value;
            }
        }

        public static void Initialize()
        {
            if (isInitialized) return;

            generalCategory = MelonPreferences.CreateCategory($"{ModInfo.Name}_01_General", $"{ModInfo.Name} - General Settings", false, true);
            chemistCategory = MelonPreferences.CreateCategory($"{ModInfo.Name}_02_Chemist", $"{ModInfo.Name} - Chemist Settings", false, true);
            string path = Path.Combine(MelonEnvironment.UserDataDirectory, $"{ModInfo.Name}.cfg");

            generalCategory.SetFilePath(path, true, false);
            chemistCategory.SetFilePath(path, true, false);

            CreateEntries();

            if (!File.Exists(path))
            {
                foreach (var entry in generalCategory.Entries)
                {
                    entry.ResetToDefault();
                }

                generalCategory.SaveToFile(false);
                chemistCategory.SaveToFile(false);
            }

            isInitialized = true;
        }

        private static void CreateEntries()
        {
            // General
            generalCategory.CreateEntry<bool>
            (
                identifier: "Debug",
                default_value: false,
                display_name: "Enable Debug Mode",
                description: "Enables debugging for this mod",
                is_hidden: false
            );

            // Chemist
            chemistCategory.CreateEntry<float>
            (
                identifier: "InsertIngredientTime",
                default_value: 1f,
                display_name: "Insert Ingredient Time (Seconds)",
                description: "Time a chemist needs per ingredient to insert it.",
                is_hidden: false
            );
        }
    }
}
