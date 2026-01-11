using HarmonyLib;
using ImprovedWorkRoutines.Employees;

#if IL2CPP
using Il2CppScheduleOne.Employees;
#elif MONO
using ScheduleOne.Employees;
#endif

namespace ImprovedWorkRoutines.Patches.Employees
{
    [HarmonyPatch(typeof(Packager))]
    public class PackagerPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("UpdateBehaviour")]
        public static bool UpdateBehaviourPrefix(Packager __instance)
        {
            if (ModConfig.Packager.ReorderTasks)
            {
                PackagerWorkRoutine routine = PackagerWorkRoutine.RetrieveOrCreate(__instance);
                routine.UpdateBehaviour();

                return false;
            }

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch("Fire")]
        public static void FirePostfix(Packager __instance)
        {
            if (PackagerWorkRoutine.Exists(__instance))
            {
                PackagerWorkRoutine routine = PackagerWorkRoutine.RetrieveOrCreate(__instance);
                routine.Destroy();
            }
        }
    }
}
