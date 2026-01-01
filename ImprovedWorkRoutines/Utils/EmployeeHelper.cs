#if IL2CPP
using Il2CppFishNet;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Employees;
using Il2CppScheduleOne.GameTime;
#elif MONO
using FishNet;
using ScheduleOne.DevUtilities;
using ScheduleOne.Employees;
using ScheduleOne.GameTime;
#endif

namespace ImprovedWorkRoutines.Utils
{
    public static class EmployeeHelper
    {
        public static void CheckWorkConditions(Employee employee)
        {
            if (employee.Fired || (!(employee.Behaviour.activeBehaviour == null) && !(employee.Behaviour.activeBehaviour == employee.WaitOutside)))
            {
                return;
            }

            bool canWork = true;
            bool shouldGetPaided = false;
            if (employee.GetHome() == null)
            {
                canWork = false;
                employee.SubmitNoWorkReason("I haven't been assigned a locker", "You can use your management clipboard to assign me a locker.");
            }
            else if (NetworkSingleton<TimeManager>.Instance.IsEndOfDay)
            {
                canWork = false;
                employee.SubmitNoWorkReason("Sorry boss, my shift ends at 4AM.", string.Empty);
            }
            else if (!employee.PaidForToday)
            {
                if (employee.IsPayAvailable())
                {
                    shouldGetPaided = true;
                }
                else
                {
                    canWork = false;
                    employee.SubmitNoWorkReason("I haven't been paid yet", "You can place cash in my locker.");
                }
            }

            if (!canWork)
            {
                SetWaitOutside(employee, true);
            }
            else if (InstanceFinder.IsServer && shouldGetPaided && employee.IsPayAvailable())
            {
                employee.RemoveDailyWage();
                employee.SetIsPaid();
            }
        }

        public static void SetWaitOutside(Employee employee, bool wait)
        {
            if (wait)
            {
                if (!employee.WaitOutside.Enabled)
                {
                    employee.WaitOutside.Enable_Networked();
                }
            }
            else if (employee.WaitOutside.Enabled || employee.WaitOutside.Active)
            {
                employee.WaitOutside.Disable_Networked(null);
                employee.WaitOutside.Deactivate_Networked(null);
            }
        }
    }
}
