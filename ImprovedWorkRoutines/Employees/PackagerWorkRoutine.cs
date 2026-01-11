using ImprovedWorkRoutines.Persistence;
using ImprovedWorkRoutines.Persistence.Datas;
using ImprovedWorkRoutines.Utils;
using System.Collections.Generic;
using System.Linq;

#if IL2CPP
using Il2CppFishNet;
using Il2CppScheduleOne.Employees;
using Il2CppScheduleOne.Management;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.ItemFramework;
#elif MONO
using FishNet;
using ScheduleOne.Employees;
using ScheduleOne.Management;
using ScheduleOne.ObjectScripts;
using ScheduleOne.ItemFramework;
#endif

namespace ImprovedWorkRoutines.Employees
{
    public class PackagerWorkRoutine : WorkRoutine
    {
        private static readonly List<PackagerWorkRoutine> cache = [];

        private readonly PackagerData _config;

#if IL2CPP
        private Packager _packager => Employee.Cast<Packager>();
#elif MONO
        private Packager _packager => Employee as Packager;
#endif

        private PackagerWorkRoutine(Packager chemist) : base(chemist)
        {
            _config = SaveConfig.Data.Packagers.Find(x => x.Identifier == chemist.GUID.ToString());
            _config ??= new(chemist.GUID.ToString(), true);
        }

        public static PackagerWorkRoutine RetrieveOrCreate(Packager chemist)
        {
            PackagerWorkRoutine routine = cache.Find(x => x._packager == chemist);

            if (routine == null)
            {
                routine = new(chemist);
                cache.Add(routine);
            }

            return routine;
        }

        public static bool Exists(Packager packager)
        {
            return cache.Any(x => x._packager == packager);
        }

        public static void ClearCache()
        {
            for (int i = cache.Count - 1; i >= 0; i--)
            {
                cache[i].Destroy();
            }

            Logger.Debug("PackagerWorkRoutine", $"Cache cleared");
        }

        public void Destroy()
        {
            SaveConfig.Data.Packagers.Remove(_config);
            cache.Remove(this);

            Logger.Debug("PackagerWorkRoutine", $"Routine for {_packager.fullName} destroyed.");
        }

        protected override void RegisterTasks()
        {
            if (!TasksCreated && _config != null)
            {
                RegisterTask("StartPackaging", "Start packaging", _config.Priorities.StartPackaging, StartPackaging);
                RegisterTask("StartBrickPress", "Start brick press", _config.Priorities.StartBrickPress, StartBrickPress);
                RegisterTask("MovePackagingStationItems", "Move packaging station items", _config.Priorities.MovePackagingStationItems, MovePackagingStationItems);
                RegisterTask("MoveBrickPressItems", "Move brick press items", _config.Priorities.MoveBrickPressItems, MoveBrickPressItems);
                RegisterTask("HandleTransitRoute", "Handle transit route", _config.Priorities.HandleTransitRoute, HandleTransitRoute);

                Logger.Debug("PackagerWorkRoutine", $"{Tasks.Count} tasks for {_packager.fullName} created.");

                base.RegisterTasks();
            }
        }

        public override void UpdateBehaviour()
        {
            CheckWorkConditions();

            if (!InstanceFinder.IsServer)
            {
                return;
            }

#if IL2CPP
            if (_packager.PackagingBehaviour.Active || _packager.MoveItemBehaviour.Active)
            {
                _packager.MarkIsWorking();
            }
            else if (_packager.Fired)
            {
                _packager.LeavePropertyAndDespawn();
            }
            else if (_packager.CanWork())
            {
                if (_packager.configuration.AssignedStationCount + _packager.configuration.Routes.Routes.Count == 0)
                {
                    _packager.SubmitNoWorkReason("I haven't been assigned to any stations or routes.", "You can use your management clipboards to assign stations or routes to me.");
                    _packager.SetIdle(true);
                }
#elif MONO
            if (_packager.PackagingBehaviour.Active || _packager.MoveItemBehaviour.Active)
            {
                Reflection.InvokeMethod(typeof(Employee), "MarkIsWorking", _packager);
            }
            else if (_packager.Fired)
            {
                Reflection.InvokeMethod(typeof(Employee), "LeavePropertyAndDespawn", _packager);
            }
            else if (Reflection.InvokeMethod<bool>(typeof(Employee), "CanWork", _packager))
            {
                if (Reflection.GetPropertyValue<PackagerConfiguration>(typeof(Packager), "configuration", _packager).AssignedStationCount + Reflection.GetPropertyValue<PackagerConfiguration>(typeof(Packager), "configuration", _packager).Routes.Routes.Count == 0)
                {
                    _packager.SubmitNoWorkReason("I haven't been assigned to any stations or routes.", "You can use your management clipboards to assign stations or routes to me.");
                    _packager.SetIdle(true);
                }
#endif
                else
                {
                    base.UpdateBehaviour();
                }
            }
        }

        private bool StartPackaging()
        {
#if IL2CPP
            PackagingStation stationToAttend = _packager.GetStationToAttend();

            if (stationToAttend != null)
            {
                _packager.StartPackaging(stationToAttend);

                return true;
            }
#elif MONO
            PackagingStation stationToAttend = Reflection.InvokeMethod<PackagingStation>(typeof(Packager), "GetStationToAttend", _packager);

            if (stationToAttend != null)
            {
                Reflection.InvokeMethod(typeof(Packager), "StartPackaging", _packager, [stationToAttend]);

                return true;
            }
#endif

            return false;
        }

        private bool StartBrickPress()
        {
#if IL2CPP
            BrickPress brickPress = _packager.GetBrickPress();

            if (brickPress != null)
            {
                _packager.StartPress(brickPress);

                return true;
            }
#elif MONO
            BrickPress brickPress = Reflection.InvokeMethod<BrickPress>(typeof(Packager), "GetBrickPress", _packager);

            if (brickPress != null)
            {
                Reflection.InvokeMethod(typeof(Packager), "StartPress", _packager, [brickPress]);

                return true;
            }
#endif

            return false;
        }

        private bool MovePackagingStationItems()
        {
#if IL2CPP
            PackagingStation stationMoveItems = _packager.GetStationMoveItems();

            if (stationMoveItems != null)
            {
                _packager.StartMoveItem(stationMoveItems);

                return true;
            }
#elif MONO
            PackagingStation stationMoveItems = Reflection.InvokeMethod<PackagingStation>(typeof(Packager), "GetStationMoveItems", _packager);

            if (stationMoveItems != null)
            {
                Reflection.InvokeMethod(typeof(Packager), "StartMoveItem", _packager, [stationMoveItems]);

                return true;
            }
#endif

            return false;
        }

        private bool MoveBrickPressItems()
        {
#if IL2CPP
            BrickPress brickPressMoveItems = _packager.GetBrickPressMoveItems();

            if (brickPressMoveItems != null)
            {
                _packager.StartMoveItem(brickPressMoveItems);

                return true;
            }
#elif MONO
            BrickPress brickPressMoveItems = Reflection.InvokeMethod<BrickPress>(typeof(Packager), "GetBrickPressMoveItems", _packager);

            if (brickPressMoveItems != null)
            {
                Reflection.InvokeMethod(typeof(Packager), "StartMoveItem", _packager, [brickPressMoveItems]);

                return true;
            }
#endif

            return false;
        }

        private bool HandleTransitRoute()
        {
#if IL2CPP
            AdvancedTransitRoute transitRouteReady = _packager.GetTransitRouteReady(out ItemInstance item);

            if (transitRouteReady != null)
            {
                _packager.MoveItemBehaviour.Initialize(transitRouteReady, item, item.Quantity);
                _packager.MoveItemBehaviour.Enable_Networked();

                return true;
            }
#elif MONO
            object[] parameters = [null];
            AdvancedTransitRoute transitRouteReady = Reflection.InvokeMethodWithOut<AdvancedTransitRoute>(typeof(Packager), "GetTransitRouteReady", _packager, parameters, out parameters);

            if (transitRouteReady != null)
            {
                ItemInstance item = (ItemInstance)parameters[0];

                _packager.MoveItemBehaviour.Initialize(transitRouteReady, item, item.Quantity);
                _packager.MoveItemBehaviour.Enable_Networked();

                return true;
            }
#endif

            return false;
        }
    }
}
