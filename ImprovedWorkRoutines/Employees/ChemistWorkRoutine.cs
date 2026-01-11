using ImprovedWorkRoutines.Persistence;
using ImprovedWorkRoutines.Persistence.Datas;
using ImprovedWorkRoutines.Utils;
using System.Collections.Generic;

#if IL2CPP
using Il2CppFishNet;
using Il2CppScheduleOne.Employees;
using Il2CppScheduleOne.Management;
using Il2CppScheduleOne.ObjectScripts;
#elif MONO
using FishNet;
using ScheduleOne.Employees;
using ScheduleOne.Management;
using ScheduleOne.ObjectScripts;
#endif

namespace ImprovedWorkRoutines.Employees
{
    public class ChemistWorkRoutine : WorkRoutine
    {
        private static readonly List<ChemistWorkRoutine> cache = [];

        private readonly ChemistData _config;

#if IL2CPP
        private Chemist _chemist => Employee.Cast<Chemist>();
#elif MONO
        private Chemist _chemist => Employee as Chemist;
#endif

        private ChemistWorkRoutine(Chemist chemist) : base(chemist)
        {
            _config = SaveConfig.Data.Chemists.Find(x => x.Identifier == chemist.GUID.ToString());
            _config ??= new(chemist.GUID.ToString(), true);
        }

        public static ChemistWorkRoutine RetrieveOrCreate(Chemist chemist)
        {
            ChemistWorkRoutine routine = cache.Find(x => x._chemist == chemist);

            if (routine == null)
            {
                routine = new(chemist);
                cache.Add(routine);
            }

            return routine;
        }

        public static void ClearCache()
        {
            for (int i = cache.Count - 1; i >= 0; i--)
            {
                cache[i].Destroy();
            }

            Logger.Debug("ChemistWorkRoutine", $"Cache cleared");
        }

        public void Destroy()
        {
            SaveConfig.Data.Chemists.Remove(_config);
            cache.Remove(this);

            Logger.Debug("ChemistWorkRoutine", $"Routine for {_chemist.fullName} destroyed.");
        }

        protected override void RegisterTasks()
        {
            if (!TasksCreated && _config != null)
            {
                RegisterTask("FinishLabOven", "Finish lab oven", _config.Priorities.FinishLabOven, FinishLabOven);
                RegisterTask("StartLabOven", "Start lab oven", _config.Priorities.StartLabOven, StartLabOven);
                RegisterTask("StartChemistryStation", "Start chemistry station", _config.Priorities.StartChemistryStation, StartChemistryStation);
                RegisterTask("StartCauldron", "Start couldron", _config.Priorities.StartCauldron, StartCauldron);
                RegisterTask("StartMixingStation", "Start mixing station", _config.Priorities.StartMixingStation, StartMixingStation);
                RegisterTask("MoveLabOvenOutput", "Move lab oven output", _config.Priorities.MoveLabOvenOutput, MoveLabOvenOutput);
                RegisterTask("MoveChemistryStationOutput", "Move chemistry station output", _config.Priorities.MoveChemistryStationOutput, MoveChemistryStationOutput);
                RegisterTask("MoveCauldronOutput", "Move couldron output", _config.Priorities.MoveCauldronOutput, MoveCauldronOutput);
                RegisterTask("MoveMixingStationOutput", "Move mix station output", _config.Priorities.MoveMixingStationOutput, MoveMixingStationOutput);

                Logger.Debug("ChemistWorkRoutine", $"{Tasks.Count} tasks for {_chemist.fullName} created.");

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
            if (_chemist.AnyWorkInProgress())
            {
                _chemist.MarkIsWorking();
            }
            else if (_chemist.Fired)
            {
                _chemist.LeavePropertyAndDespawn();
            }
            else if (_chemist.CanWork())
            {
                if (_chemist.configuration.TotalStations == 0)
                {
                    _chemist.SubmitNoWorkReason("I haven't been assigned any stations", "You can use your management clipboards to assign stations to me.");
                    _chemist.SetIdle(true);
                }
#elif MONO
            if (Reflection.InvokeMethod<bool>(typeof(Chemist), "AnyWorkInProgress", _chemist))
            {
                Reflection.InvokeMethod(typeof(Employee), "MarkIsWorking", _chemist);
            }
            else if (_chemist.Fired)
            {
                Reflection.InvokeMethod(typeof(Employee), "LeavePropertyAndDespawn", _chemist);
            }
            else if(Reflection.InvokeMethod<bool>(typeof(Employee), "CanWork", _chemist))
            {
                if (Reflection.GetPropertyValue<ChemistConfiguration>(typeof(Chemist), "configuration", _chemist).TotalStations == 0)
                {
                    _chemist.SubmitNoWorkReason("I haven't been assigned any stations", "You can use your management clipboards to assign stations to me.");
                    _chemist.SetIdle(true);
                }
#endif
                else
                {
                    base.UpdateBehaviour();
                }
            }
        }

        private bool FinishLabOven()
        {
#if IL2CPP
            Il2CppSystem.Collections.Generic.List<LabOven> labOvensReadyToFinish = _chemist.GetLabOvensReadyToFinish();

            if (labOvensReadyToFinish.Count > 0)
            {
                _chemist.FinishLabOven(labOvensReadyToFinish[0]);

                return true;
            }
#elif MONO
            List<LabOven> labOvensReadyToFinish = _chemist.GetLabOvensReadyToFinish();

            if (labOvensReadyToFinish.Count > 0)
            {
                Reflection.InvokeMethod(typeof(Chemist), "FinishLabOven", _chemist, [labOvensReadyToFinish[0]]);

                return true;
            }
#endif

            return false;
        }

        private bool StartLabOven()
        {
#if IL2CPP
            Il2CppSystem.Collections.Generic.List<LabOven> labOvensReadyToStart = _chemist.GetLabOvensReadyToStart();

            if (labOvensReadyToStart.Count > 0)
            {
                _chemist.StartLabOven(labOvensReadyToStart[0]);

                return true;
            }
#elif MONO
            List<LabOven> labOvensReadyToStart = _chemist.GetLabOvensReadyToStart();

            if (labOvensReadyToStart.Count > 0)
            {
                Reflection.InvokeMethod(typeof(Chemist), "StartLabOven", _chemist, [labOvensReadyToStart[0]]);

                return true;
            }
#endif

            return false;
        }

        private bool StartChemistryStation()
        {
#if IL2CPP
            Il2CppSystem.Collections.Generic.List<ChemistryStation> chemistryStationsReadyToStart = _chemist.GetChemistryStationsReadyToStart();

            if (chemistryStationsReadyToStart.Count > 0)
            {
                _chemist.StartChemistryStation(chemistryStationsReadyToStart[0]);

                return true;
            }
#elif MONO
            List<ChemistryStation> chemistryStationsReadyToStart = _chemist.GetChemistryStationsReadyToStart();

            if (chemistryStationsReadyToStart.Count > 0)
            {
                Reflection.InvokeMethod(typeof(Chemist), "StartChemistryStation", _chemist, [chemistryStationsReadyToStart[0]]);

                return true;
            }
#endif

            return false;
        }

        private bool StartCauldron()
        {
#if IL2CPP
            Il2CppSystem.Collections.Generic.List<Cauldron> cauldronsReadyToStart = _chemist.GetCauldronsReadyToStart();

            if (cauldronsReadyToStart.Count > 0)
            {
                _chemist.StartCauldron(cauldronsReadyToStart[0]);

                return true;
            }
#elif MONO
            List<Cauldron> cauldronsReadyToStart = _chemist.GetCauldronsReadyToStart();

            if (cauldronsReadyToStart.Count > 0)
            {
                Reflection.InvokeMethod(typeof(Chemist), "StartCauldron", _chemist, [cauldronsReadyToStart[0]]);

                return true;
            }
#endif

            return false;
        }

        private bool StartMixingStation()
        {
#if IL2CPP
            Il2CppSystem.Collections.Generic.List<MixingStation> mixingStationsReadyToStart = _chemist.GetMixingStationsReadyToStart();

            if (mixingStationsReadyToStart.Count > 0)
            {
                _chemist.StartMixingStation(mixingStationsReadyToStart[0]);

                return true;
            }
#elif MONO
            List<MixingStation> mixingStationsReadyToStart = _chemist.GetMixingStationsReadyToStart();

            if (mixingStationsReadyToStart.Count > 0)
            {
                Reflection.InvokeMethod(typeof(Chemist), "StartMixingStation", _chemist, [mixingStationsReadyToStart[0]]);

                return true;
            }
#endif

            return false;
        }

        private bool MoveLabOvenOutput()
        {
#if IL2CPP
            Il2CppSystem.Collections.Generic.List<LabOven> labOvensReadyToMove = _chemist.GetLabOvensReadyToMove();

            if (labOvensReadyToMove.Count > 0)
            {
                _chemist.MoveItemBehaviour.Initialize((labOvensReadyToMove[0].Configuration.Cast<LabOvenConfiguration>()).DestinationRoute, labOvensReadyToMove[0].OutputSlot.ItemInstance);
                _chemist.MoveItemBehaviour.Enable_Networked();

                return true;
            }
#elif MONO
            List<LabOven> labOvensReadyToMove = Reflection.InvokeMethod<List<LabOven>>(typeof(Chemist), "GetLabOvensReadyToMove", _chemist);

            if (labOvensReadyToMove.Count > 0)
            {
                _chemist.MoveItemBehaviour.Initialize((labOvensReadyToMove[0].Configuration as LabOvenConfiguration).DestinationRoute, labOvensReadyToMove[0].OutputSlot.ItemInstance);
                _chemist.MoveItemBehaviour.Enable_Networked();

                return true;
            }
#endif

            return false;
        }

        private bool MoveChemistryStationOutput()
        {
#if IL2CPP
            Il2CppSystem.Collections.Generic.List<ChemistryStation> chemStationsReadyToMove = _chemist.GetChemStationsReadyToMove();

            if (chemStationsReadyToMove.Count > 0)
            {
                _chemist.MoveItemBehaviour.Initialize((chemStationsReadyToMove[0].Configuration.Cast<ChemistryStationConfiguration>()).DestinationRoute, chemStationsReadyToMove[0].OutputSlot.ItemInstance);
                _chemist.MoveItemBehaviour.Enable_Networked();

                return true;
            }
#elif MONO
            List<ChemistryStation> chemStationsReadyToMove = Reflection.InvokeMethod<List<ChemistryStation>>(typeof(Chemist), "GetChemStationsReadyToMove", _chemist);

            if (chemStationsReadyToMove.Count > 0)
            {
                _chemist.MoveItemBehaviour.Initialize((chemStationsReadyToMove[0].Configuration as ChemistryStationConfiguration).DestinationRoute, chemStationsReadyToMove[0].OutputSlot.ItemInstance);
                _chemist.MoveItemBehaviour.Enable_Networked();

                return true;
            }
#endif

            return false;
        }

        private bool MoveCauldronOutput()
        {
#if IL2CPP
            Il2CppSystem.Collections.Generic.List<Cauldron> cauldronsReadyToMove = _chemist.GetCauldronsReadyToMove();

            if (cauldronsReadyToMove.Count > 0)
            {
                _chemist.MoveItemBehaviour.Initialize((cauldronsReadyToMove[0].Configuration.Cast<CauldronConfiguration>()).DestinationRoute, cauldronsReadyToMove[0].OutputSlot.ItemInstance);
                _chemist.MoveItemBehaviour.Enable_Networked();

                return true;
            }
#elif MONO
            List<Cauldron> cauldronsReadyToMove = Reflection.InvokeMethod<List<Cauldron>>(typeof(Chemist), "GetCauldronsReadyToMove", _chemist);

            if (cauldronsReadyToMove.Count > 0)
            {
                _chemist.MoveItemBehaviour.Initialize((cauldronsReadyToMove[0].Configuration as CauldronConfiguration).DestinationRoute, cauldronsReadyToMove[0].OutputSlot.ItemInstance);
                _chemist.MoveItemBehaviour.Enable_Networked();

                return true;
            }
#endif

            return false;
        }

        private bool MoveMixingStationOutput()
        {
#if IL2CPP
            Il2CppSystem.Collections.Generic.List<MixingStation> mixStationsReadyToMove = _chemist.GetMixStationsReadyToMove();

            if (mixStationsReadyToMove.Count > 0)
            {
                _chemist.MoveItemBehaviour.Initialize((mixStationsReadyToMove[0].Configuration.Cast<MixingStationConfiguration>()).DestinationRoute, mixStationsReadyToMove[0].OutputSlot.ItemInstance);
                _chemist.MoveItemBehaviour.Enable_Networked();

                return true;
            }
#elif MONO
            List<MixingStation> mixStationsReadyToMove = Reflection.InvokeMethod<List<MixingStation>>(typeof(Chemist), "GetMixStationsReadyToMove", _chemist);

            if (mixStationsReadyToMove.Count > 0)
            {
                _chemist.MoveItemBehaviour.Initialize((mixStationsReadyToMove[0].Configuration as MixingStationConfiguration).DestinationRoute, mixStationsReadyToMove[0].OutputSlot.ItemInstance);
                _chemist.MoveItemBehaviour.Enable_Networked();

                return true;
            }
#endif

            return false;
        }
    }
}
