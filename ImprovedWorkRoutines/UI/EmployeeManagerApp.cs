using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Employees;
using Il2CppScheduleOne.UI.Phone;
using System.Collections.Generic;
using UnityEngine;

namespace ImprovedWorkRoutines.UI
{
    public class EmployeeManagerApp
    {
        public GameObject Container;

        public TasksContainer TasksContainer;

        public EmployeeSelector EmployeeSelector;

        public PropertieSelector PropertieSelector;

        private SelectableEmployee _selectedEmployee;

        public static EmployeeManagerApp Instance { get; private set; }

        public bool IsUIBuild { get; private set; }

        public bool IsOpen => Container != null && Container.activeInHierarchy && Phone.ActiveApp == Container;

        public EmployeeManagerApp()
        {
            if (Instance == null)
            {
                BuildUI();
                Instance = this;
            }
        }

        public void Open()
        {
            if (!IsOpen && Phone.ActiveApp != null && Phone.ActiveApp != Container)
            {
                if (PlayerSingleton<Phone>.InstanceExists)
                {
                    PlayerSingleton<Phone>.Instance.SetIsHorizontal(true);
                    PlayerSingleton<Phone>.Instance.SetLookOffsetMultiplier(0.6f);
                }

                Phone.ActiveApp = Container;

                Container.SetActive(true);
            }
        }

        public void Close()
        {
            if (IsOpen && Phone.ActiveApp != null && Phone.ActiveApp == Container)
            {
                if (PlayerSingleton<Phone>.InstanceExists)
                {
                    Phone.Instance.SetIsHorizontal(false);
                    Phone.Instance.SetLookOffsetMultiplier(1f);
                }

                Phone.ActiveApp = null;

                Container.SetActive(false);
            }
        }

        private void BuildUI()
        {
            if (!IsUIBuild)
            {
                IsUIBuild = true;
            }
        }

        public struct SelectableEmployee
        {
            public Employee Employee;

            public EEmployeeType EmployeeType;

            public RectTransform Container;

            public List<GameObject> Tasks;
        }
    }
}
