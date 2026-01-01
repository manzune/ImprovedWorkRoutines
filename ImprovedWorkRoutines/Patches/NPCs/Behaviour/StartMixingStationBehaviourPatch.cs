using HarmonyLib;
using ImprovedWorkRoutines.NPCs.Behaviour;

#if IL2CPP
using Il2CppScheduleOne.Employees;
using Il2CppScheduleOne.ObjectScripts;
using S1StartMixingStationBehaviour = Il2CppScheduleOne.NPCs.Behaviour.StartMixingStationBehaviour;
#elif MONO
using ScheduleOne.Employees;
using ScheduleOne.ObjectScripts;
using S1StartMixingStationBehaviour = ScheduleOne.NPCs.Behaviour.StartMixingStationBehaviour;
#endif

namespace ImprovedWorkRoutines.Patches.NPCs.Behaviour
{
    [HarmonyPatch(typeof(S1StartMixingStationBehaviour))]
    public class StartMixingStationBehaviourPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("RpcLogic___StartCook_2166136261")]
        public static bool RpcLogic___StartCook_2166136261Prefix(S1StartMixingStationBehaviour __instance)
        {
            if (ModConfig.Chemist.MixingStation)
            {
                StartMixingStationBehaviour modified = StartMixingStationBehaviour.RetrieveOrCreate(__instance);
                if (modified != null)
                {
                    modified.RpcLogic___StartCook_2166136261();

                    return false;
                }
            }

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch("Awake")]
        public static void AwakePostfix(S1StartMixingStationBehaviour __instance)
        {
            if (ModConfig.Chemist.MixingStation)
            {
                StartMixingStationBehaviour.RetrieveOrCreate(__instance);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("AssignStation")]
        public static void AssignStationPostfix(S1StartMixingStationBehaviour __instance, MixingStation station)
        {
            if (ModConfig.Chemist.MixingStation)
            {
                StartMixingStationBehaviour modified = StartMixingStationBehaviour.RetrieveOrCreate(__instance);
                modified?.AssignStation(station);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("StopCook")]
        public static void StopCookPostfix(S1StartMixingStationBehaviour __instance)
        {
            if (ModConfig.Chemist.MixingStation)
            {
                StartMixingStationBehaviour modified = StartMixingStationBehaviour.RetrieveOrCreate(__instance);
                modified?.StopCook();
            }
        }
    }
}
