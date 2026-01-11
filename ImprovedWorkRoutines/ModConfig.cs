using MelonLoader;
using MelonLoader.Utils;
using System.IO;

namespace ImprovedWorkRoutines
{
    public static class ModConfig
    {
        private static MelonPreferences_Category generalCategory;

        private static MelonPreferences_Category botanistCategory;

        private static MelonPreferences_Category chemistCategory;

        private static MelonPreferences_Category packagerCategory;

        private static bool isInitialized;

        public static bool Debug
        {
            get => generalCategory.GetEntry<bool>("Debug").Value;
            set => generalCategory.GetEntry<bool>("Debug").Value = value;
        }

        public struct Botanist
        {
            public static bool ReorderTasks
            {
                get => botanistCategory.GetEntry<bool>("ReorderTasks").Value;
                set => botanistCategory.GetEntry<bool>("ReorderTasks").Value = value;
            }
        }

        public struct Chemist
        {
            public static bool ReorderTasks
            {
                get => chemistCategory.GetEntry<bool>("ReorderTasks").Value;
                set => chemistCategory.GetEntry<bool>("ReorderTasks").Value = value;
            }

            public static float InsertIngredientTime
            {
                get => chemistCategory.GetEntry<float>("InsertIngredientTime").Value;
                set => chemistCategory.GetEntry<float>("InsertIngredientTime").Value = value;
            }

            public static bool MixingStation
            {
                get => chemistCategory.GetEntry<bool>("MixingStation").Value;
                set => chemistCategory.GetEntry<bool>("MixingStation").Value = value;
            }
        }

        public struct Packager
        {
            public static bool ReorderTasks
            {
                get => packagerCategory.GetEntry<bool>("ReorderTasks").Value;
                set => packagerCategory.GetEntry<bool>("ReorderTasks").Value = value;
            }
        }

        public static void Initialize()
        {
            if (isInitialized) return;

            generalCategory = MelonPreferences.CreateCategory($"{ModInfo.Name}_01_General", $"{ModInfo.Name} - General Settings", false, true);
            botanistCategory = MelonPreferences.CreateCategory($"{ModInfo.Name}_02_Botanist", $"{ModInfo.Name} - Botanist Settings", false, true);
            chemistCategory = MelonPreferences.CreateCategory($"{ModInfo.Name}_03_Chemist", $"{ModInfo.Name} - Chemist Settings", false, true);
            packagerCategory = MelonPreferences.CreateCategory($"{ModInfo.Name}_04_Packager", $"{ModInfo.Name} - Packager Settings", false, true);

            string path = Path.Combine(MelonEnvironment.UserDataDirectory, $"{ModInfo.Name}.cfg");

            generalCategory.SetFilePath(path, true, false);
            botanistCategory.SetFilePath(path, true, false);
            chemistCategory.SetFilePath(path, true, false);
            packagerCategory.SetFilePath(path, true, false);

            CreateEntries();

            if (!File.Exists(path))
            {
                foreach (var entry in generalCategory.Entries)
                {
                    entry.ResetToDefault();
                }

                foreach (var entry in botanistCategory.Entries)
                {
                    entry.ResetToDefault();
                }

                foreach (var entry in chemistCategory.Entries)
                {
                    entry.ResetToDefault();
                }

                foreach (var entry in packagerCategory.Entries)
                {
                    entry.ResetToDefault();
                }

                generalCategory.SaveToFile(false);
                botanistCategory.SaveToFile(false);
                chemistCategory.SaveToFile(false);
                packagerCategory.SaveToFile(false);
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

            // Botanist
            botanistCategory.CreateEntry<bool>
            (
                identifier: "ReorderTasks",
                default_value: true,
                display_name: "Reorder Tasks",
                description: "Enables custom task ordering for botanists",
                is_hidden: false
            );

            // Chemist
            chemistCategory.CreateEntry<bool>
            (
                identifier: "ReorderTasks",
                default_value: true,
                display_name: "Reorder Tasks",
                description: "Enables custom task ordering for chemists",
                is_hidden: false
            );
            chemistCategory.CreateEntry<float>
            (
                identifier: "InsertIngredientTime",
                default_value: 1f,
                display_name: "Insert Ingredient Time (Seconds)",
                description: "Time a chemist needs per ingredient to insert it.",
                is_hidden: false
            );
            chemistCategory.CreateEntry<bool>
            (
                identifier: "MixingStation",
                default_value: true,
                display_name: "Override Mixing Station Routine",
                description: "Overrides the mixing station routine",
                is_hidden: false
            );

            // Packager
            packagerCategory.CreateEntry<bool>
            (
                identifier: "ReorderTasks",
                default_value: true,
                display_name: "Reorder Tasks",
                description: "Enables custom task ordering for packagers",
                is_hidden: false
            );
        }
    }
}
