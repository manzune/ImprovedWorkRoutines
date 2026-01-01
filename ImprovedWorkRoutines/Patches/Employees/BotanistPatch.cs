using HarmonyLib;
using ImprovedWorkRoutines.Employees;

#if IL2CPP
using Il2CppScheduleOne.Employees;
#elif MONO
using ScheduleOne.Employees;
#endif

namespace ImprovedWorkRoutines.Patches.Employees
{
    [HarmonyPatch(typeof(Botanist))]
    public class BotanistPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("UpdateBehaviour")]
        public static bool UpdateBehaviourPrefix(Botanist __instance)
        {
            if (ModConfig.Botanist.ReorderTasks)
            {
                BotanistRoutine routine = BotanistRoutine.RetrieveOrCreate(__instance);
                routine.UpdateBehaviour();

                return false;
            }

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch("Awake")]
        public static void AwakePostfix(Botanist __instance)
        {
            if (ModConfig.Botanist.ReorderTasks)
            {
                BotanistRoutine.RetrieveOrCreate(__instance);
            }
        }
    }
}
