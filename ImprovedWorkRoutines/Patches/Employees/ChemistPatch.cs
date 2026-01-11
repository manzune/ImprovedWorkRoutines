using HarmonyLib;
using ImprovedWorkRoutines.Employees;

#if IL2CPP
using Il2CppScheduleOne.Employees;
#elif MONO
using ScheduleOne.Employees;
#endif

namespace ImprovedWorkRoutines.Patches.Employees
{
    [HarmonyPatch(typeof(Chemist))]
    public class ChemistPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("UpdateBehaviour")]
        public static bool UpdateBehaviourPrefix(Chemist __instance)
        {
            if (ModConfig.Chemist.ReorderTasks)
            {
                ChemistWorkRoutine routine = ChemistWorkRoutine.RetrieveOrCreate(__instance);
                routine.UpdateBehaviour();

                return false;
            }

            return true;
        }
    }
}
