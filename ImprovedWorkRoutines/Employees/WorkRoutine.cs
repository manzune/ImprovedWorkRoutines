using System.Collections.Generic;
using System.Linq;
using ImprovedWorkRoutines.Utils;

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

namespace ImprovedWorkRoutines.Employees
{
    public abstract class WorkRoutine
    {
        public delegate bool TaskCallback();

        protected readonly Employee Employee;

        protected readonly SortedDictionary<string, (string description, int priority, TaskCallback callback)> Tasks = [];

        protected bool TasksCreated;

        public WorkRoutine(Employee employee)
        {
            Employee = employee;

            RegisterTasks();

            Logger.Debug("WorkRoutine", $"Routine for {Employee.fullName} created.");
        }

        public void RegisterTask(string identifier, string description, int priority, TaskCallback callback)
        {
            if (!Tasks.Any(t => t.Key == identifier))
            {
                Tasks.Add(identifier, (description, priority, callback));
            }
            else
            {
               Utils.Logger.Error($"Could not register already registered task: {identifier}");
            }
        }

        public void EditTask(string identifier, string description) => EditTask(identifier, description, -1);

        public void EditTask(string identifier, int priority) => EditTask(identifier, string.Empty, priority);

        public void EditTask(string identifier, string description, int priority)
        {
            if (Tasks.Any(t => t.Key == identifier))
            {
                if (description == null || description == string.Empty)
                {
                    description = Tasks[identifier].description;
                }

                if (priority == -1)
                {
                    priority = Tasks[identifier].priority;
                }

                Tasks[identifier] = (description, priority, Tasks[identifier].callback);
            }
            else
            {
               Utils.Logger.Error($"Could not edit non existent task: {identifier}");
            }
        }

        public void RemoveTask(string identifier)
        {
            if (!Tasks.Remove(identifier))
            {
                Utils.Logger.Error($"Could not remove non existent task: {identifier}");
            }
        }

        protected Dictionary<string, (string description, int priority, TaskCallback callback)> GetTasksByPriority()
        {
            return Tasks.OrderBy(t => t.Value.priority).ToDictionary(t => t.Key, t => t.Value);
        }

        protected virtual void RegisterTasks()
        {
            TasksCreated = true;
        }

        public virtual void UpdateBehaviour()
        {
            bool started = false;

            for (int i = 0; i < Tasks.Count; i++)
            {
                started = Tasks.ElementAt(i).Value.callback.Invoke();

                if (started)
                {
                    Logger.Debug("WorkRoutine", $"Task for {Employee.fullName} started: {Tasks.ElementAt(i).Key}");
                    break;
                }
            }

            if (!started)
            {
                Employee.SubmitNoWorkReason("There's nothing for me to do right now.", string.Empty);
                Employee.SetIdle(true);
            }
        }

        protected void CheckWorkConditions()
        {
            if (Employee.Fired || (!(Employee.Behaviour.activeBehaviour == null) && !(Employee.Behaviour.activeBehaviour == Employee.WaitOutside)))
            {
                return;
            }

            bool canWork = true;
            bool shouldGetPaided = false;
            if (Employee.GetHome() == null)
            {
                canWork = false;
                Employee.SubmitNoWorkReason("I haven't been assigned a locker", "You can use your management clipboard to assign me a locker.");
            }
            else if (NetworkSingleton<TimeManager>.Instance.IsEndOfDay)
            {
                canWork = false;
                Employee.SubmitNoWorkReason("Sorry boss, my shift ends at 4AM.", string.Empty);
            }
            else if (!Employee.PaidForToday)
            {
                if (Employee.IsPayAvailable())
                {
                    shouldGetPaided = true;
                }
                else
                {
                    canWork = false;
                    Employee.SubmitNoWorkReason("I haven't been paid yet", "You can place cash in my locker.");
                }
            }

            if (!canWork)
            {
                SetWaitOutside(true);
            }
            else if (InstanceFinder.IsServer && shouldGetPaided && Employee.IsPayAvailable())
            {
                Employee.RemoveDailyWage();
                Employee.SetIsPaid();
            }
        }

        protected void SetWaitOutside(bool wait)
        {
            if (wait)
            {
                if (!Employee.WaitOutside.Enabled)
                {
                    Employee.WaitOutside.Enable_Networked();
                }
            }
            else if (Employee.WaitOutside.Enabled || Employee.WaitOutside.Active)
            {
                Employee.WaitOutside.Disable_Networked(null);
                Employee.WaitOutside.Deactivate_Networked(null);
            }
        }
    }
}
