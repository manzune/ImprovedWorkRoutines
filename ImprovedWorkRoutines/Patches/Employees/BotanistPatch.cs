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
                BotanistWorkRoutine routine = BotanistWorkRoutine.RetrieveOrCreate(__instance);
                routine.UpdateBehaviour();

                return false;
            }

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch("Fire")]
        public static void FirePostfix(Botanist __instance)
        {
            if (BotanistWorkRoutine.Exists(__instance))
            {
                BotanistWorkRoutine routine = BotanistWorkRoutine.RetrieveOrCreate(__instance);
                routine.Destroy();
            }
        }
    }
}
