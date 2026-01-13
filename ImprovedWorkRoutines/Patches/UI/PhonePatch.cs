using HarmonyLib;
using ImprovedWorkRoutines.UI;

#if IL2CPP
using Il2CppScheduleOne.UI.Phone;
#elif MONO
using ScheduleOne.UI.Phone;
#endif

namespace ImprovedWorkRoutines.Patches.UI
{
    [HarmonyPatch(typeof(Phone))]
    public class PhonePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("SetIsOpen")]
        public static void SetIsOpenPostfix()
        {
            if (EmployeeManagerApp.Instance == null)
            {
                _ = new EmployeeManagerApp();
            }
        }
    }
}
