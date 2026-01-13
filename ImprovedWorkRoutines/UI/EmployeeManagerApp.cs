using Il2CppScheduleOne;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Employees;
using Il2CppScheduleOne.UI.Phone;
using Il2CppSystem.Runtime.InteropServices.ComTypes;
using MelonLoader.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace ImprovedWorkRoutines.UI
{
    public class EmployeeManagerApp
    {
        public GameObject App;

        public GameObject Container;

        public GameObject Icon;

        public TasksContainer TasksContainer;

        public EmployeeSelector EmployeeSelector;

        private SelectableEmployee _selectedEmployee;

        public static EmployeeManagerApp Instance { get; private set; }

        public bool IsUIBuild { get; private set; }

        public bool IsOpen => App != null && App.activeInHierarchy && Phone.ActiveApp == App;

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
            if (Phone.ActiveApp != null && Phone.ActiveApp != App) return;

            if (PlayerSingleton<AppsCanvas>.InstanceExists)
            {
                PlayerSingleton<AppsCanvas>.Instance.SetIsOpen(true);
            }

            if (PlayerSingleton<HomeScreen>.InstanceExists)
            {
                PlayerSingleton<HomeScreen>.Instance.SetIsOpen(false);
            }

            if (PlayerSingleton<Phone>.InstanceExists)
            {
                PlayerSingleton<Phone>.Instance.SetIsHorizontal(true);
                PlayerSingleton<Phone>.Instance.SetLookOffsetMultiplier(0.6f);
            }

            Phone.ActiveApp = App;

            Container.SetActive(true);
        }

        public void Close()
        {
            if (Phone.ActiveApp != null && Phone.ActiveApp == App)
            {
                Phone.ActiveApp = null;

                Container.SetActive(false);
            }

            if (PlayerSingleton<AppsCanvas>.InstanceExists)
            {
                PlayerSingleton<AppsCanvas>.Instance.SetIsOpen(false);
            }

            if (PlayerSingleton<HomeScreen>.InstanceExists)
            {
                PlayerSingleton<HomeScreen>.Instance.SetIsOpen(true);
            }

            if (PlayerSingleton<Phone>.InstanceExists)
            {
                PlayerSingleton<Phone>.Instance.SetIsHorizontal(false);
                PlayerSingleton<Phone>.Instance.SetLookOffsetMultiplier(1f);
            }
        }

        public void Destroy()
        {
            Instance = null;

            if (PlayerSingleton<Phone>.InstanceExists)
            {
                PlayerSingleton<Phone>.Instance.closeApps -= new Action(Close);
            }

            GameInput.DeregisterExitListener(new Action<ExitAction>(Exit));

            Utils.Logger.Debug("EmployeeManagerApp destroyed.");
        }

        private void BuildUI()
        {
            if (!IsUIBuild)
            {
                GameObject template = PlayerSingleton<HomeScreen>.Instance.transform.parent.Find("AppsCanvas/ProductManagerApp")?.gameObject;

                if (template == null)
                {
                    Utils.Logger.Error("Could not find phone app template.");

                    return;
                }

                App = GameObject.Instantiate(template, template.transform.parent);
                App.name = "EmployeeManagerApp";
                App.SetActive(true);

                Container = App.transform.Find("Container").gameObject;
                Container.SetActive(false);

                Transform topBar = Container.transform.Find("Topbar");
                topBar.GetComponent<Image>().color = new(0.630f, 0.320f, 0.130f, 1f);

                topBar.Find("Title")?.GetComponent<Text>()?.text = "Employee Manager";
                topBar.Find("Subtitle")?.GetComponent<Text>()?.text = String.Empty;

                PlayerSingleton<Phone>.Instance.closeApps += new Action(Close);
                GameInput.RegisterExitListener(new Action<ExitAction>(Exit), 1);

                // Create app icon
                GameObject appIcons = PlayerSingleton<HomeScreen>.Instance.transform.Find("AppIcons").gameObject;
                Icon = appIcons.transform.GetChild(appIcons.transform.childCount - 1).gameObject;

                Text iconLabel = Icon.transform.Find("Label").GetComponent<Text>();
                iconLabel.text = "Employees";

                string iconPath = Path.Combine(MelonEnvironment.UserDataDirectory, ModInfo.NAME, "EmployeeManagerAppIcon.png");
                Image iconImage = Icon.transform.Find("Mask/Image").GetComponent<Image>();

                try
                {
                    if (File.Exists(iconPath))
                    {
                        byte[] array = File.ReadAllBytes(iconPath);
                        Texture2D texture = new(2, 2);
                        ImageConversion.LoadImage(texture, array);

                        if (texture.width == 2 && texture.height == 2)
                        {
                            throw new Exception("Could not load image data into texture.");
                        }
                        else
                        {
                            iconImage.sprite = Sprite.Create(texture, new Rect(0f, 0f, (float)texture.width, (float)texture.height), new Vector2(0.5f, 0.5f), 100f);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utils.Logger.Error("Could not create phone app icon.", ex);
                }

                Button iconButton = Icon.GetComponent<Button>();
                iconButton.onClick.RemoveAllListeners();
                iconButton.onClick.AddListener((UnityAction)Open);

                // Create sub panels
                TasksContainer = new(this);
                EmployeeSelector = new(this);

                IsUIBuild = true;

                Utils.Logger.Debug("EmployeeManagerApp created.");
            }
        }

        private void Exit(ExitAction exit)
        {
            if (!exit.Used && IsOpen && Phone.InstanceExists && Phone.Instance.IsOpen)
            {
                exit.Used = true;
                Close();
            }
        }

        public struct SelectableEmployee
        {
            public Employee Employee;

            public EEmployeeType EmployeeType;

            public List<GameObject> Tasks;
        }
    }
}
