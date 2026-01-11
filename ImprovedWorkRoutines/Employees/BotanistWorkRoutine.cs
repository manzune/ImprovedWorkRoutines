using System;
using System.Linq;
using System.Collections.Generic;
using ImprovedWorkRoutines.Utils;
using ImprovedWorkRoutines.Persistence;
using ImprovedWorkRoutines.Persistence.Datas;

#if IL2CPP
using Il2CppScheduleOne.Employees;
using Il2CppScheduleOne.Growing;
using Il2CppScheduleOne.Management;
using Il2CppScheduleOne.NPCs.Behaviour;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.StationFramework;
#elif MONO
using ScheduleOne.Employees;
using ScheduleOne.Growing;
using ScheduleOne.ItemFramework;
using ScheduleOne.Management;
using ScheduleOne.NPCs.Behaviour;
using ScheduleOne.ObjectScripts;
using ScheduleOne.StationFramework;
#endif

namespace ImprovedWorkRoutines.Employees
{
    public class BotanistWorkRoutine : WorkRoutine
    {
        private static readonly List<BotanistWorkRoutine> cache = [];

        private readonly BotanistData _config;

#if IL2CPP
        private Botanist _botanist => Employee.Cast<Botanist>();
#elif MONO
        private Botanist _botanist => Employee as Botanist;
#endif

        private BotanistWorkRoutine(Botanist botanist) : base(botanist)
        {
            _config = SaveConfig.Data.Botanists.Find(x => x.Identifier == botanist.GUID.ToString());
            _config ??= new(botanist.GUID.ToString(), true);
        }

        public static BotanistWorkRoutine RetrieveOrCreate(Botanist botanist)
        {
            BotanistWorkRoutine routine = cache.Find(x => x._botanist == botanist);

            if (routine == null)
            {
                routine = new(botanist);
                cache.Add(routine);
            }

            return routine;
        }

        public static bool Exists(Botanist botanist)
        {
            return cache.Any(x => x._botanist == botanist);
        }

        public static void ClearCache()
        {
            for (int i = cache.Count - 1; i >=0; i--)
            {
                cache[i].Destroy();
            }

            Logger.Debug("BotanistWorkRoutine", $"Cache cleared");
        }

        public void Destroy()
        {
            SaveConfig.Data.Botanists.Remove(_config);
            cache.Remove(this);

            Logger.Debug("BotanistWorkRoutine", $"Routine for {_botanist.fullName} destroyed.");
        }

        protected override void RegisterTasks()
        {
            if (!TasksCreated && _config != null)
            {
                RegisterTask("WaterPot", "Water pot", _config.Priorities.WaterPot, WaterPot);
                RegisterTask("MistMushroomBed", "Mist mushroom bed", _config.Priorities.MistMushroomBed, MistMushroomBed);
                RegisterTask("AddSoilToGrowContainer", "Add soil to grow container", _config.Priorities.AddSoilToGrowContainer, AddSoilToGrowContainer);
                RegisterTask("SowSeedInPot", "Sow seed in pot", _config.Priorities.SowSeedInPot, SowSeedInPot);
                RegisterTask("ApplySpawnToMushroomBed", "Apply spawn to mushroom bed", _config.Priorities.ApplySpawnToMushroomBed, ApplySpawnToMushroomBed);
                RegisterTask("ApplyAdditiveToGrowContainer", "Apply additive to grow container", _config.Priorities.ApplyAdditiveToGrowContainer, ApplyAdditiveToGrowContainer);
                RegisterTask("HarvestPot", "Harvest pot", _config.Priorities.HarvestPot, HarvestPot);
                RegisterTask("HarvestMushroomBed", "Harvest mushroom bed", _config.Priorities.HarvestMushroomBed, HarvestMushroomBed);
                RegisterTask("StopDryingRack", "Stop drying rack", _config.Priorities.StopDryingRack, StopDryingRack);
                RegisterTask("MoveDryingRackOutput", "Move drying rack output", _config.Priorities.MoveDryingRackOutput, MoveDryingRackOutput);
                RegisterTask("UseSpawnStation", "Use spawn station", _config.Priorities.UseSpawnStation, UseSpawnStation);
                RegisterTask("MoveSpawnStationOutput", "Move spawn station output", _config.Priorities.MoveSpawnStationOutput, MoveSpawnStationOutput);
                RegisterTask("MoveDryableToRack", "Move dryable to rack", _config.Priorities.MoveDryableToRack, MoveDryableToRack);
                RegisterTask("StartDryingRack", "Start drying rack", _config.Priorities.StartDryingRack, StartDryingRack);

                Logger.Debug("BotanistWorkRoutine", $"{Tasks.Count} tasks for {_botanist.fullName} created.");

                base.RegisterTasks();
            }
        }
        
        public override void UpdateBehaviour()
        {
            CheckWorkConditions();

#if IL2CPP
            if (_botanist._workBehaviours.Find(new Func<Behaviour, bool>(b => b.Active)) != null)
            {
                _botanist.MarkIsWorking();
            }
            else if (_botanist.Fired)
            {
                Destroy();
                _botanist.LeavePropertyAndDespawn();
            }
            else
            {
                if (!_botanist.CanWork())
                {
                    return;
                }

                if (_botanist.configuration.Assigns.SelectedObjects.Count == 0)
                {
                    _botanist.SubmitNoWorkReason("I haven't been assigned anything", "You can use your management clipboards to assign me pots, growing racks, etc.");
                    _botanist.SetIdle(true);
                }
#elif MONO
            if (Reflection.GetFieldValue<List<Behaviour>>(typeof(Botanist), "_workBehaviours", _botanist).Any(b => b.Active))
            {
                Reflection.InvokeMethod(typeof(Employee), "MarkIsWorking", _botanist);
            }
            else if (_botanist.Fired)
            {
                Reflection.InvokeMethod(typeof(Employee), "LeavePropertyAndDespawn", _botanist);
            }
            else
            {
                if (!Reflection.InvokeMethod<bool>(typeof(Employee), "CanWork", _botanist))
                {
                    return;
                }

                if (Reflection.GetPropertyValue<BotanistConfiguration>(typeof(Botanist), "configuration", _botanist).Assigns.SelectedObjects.Count == 0)
                {
                    _botanist.SubmitNoWorkReason("I haven't been assigned anything", "You can use your management clipboards to assign me pots, growing racks, etc.");
                    _botanist.SetIdle(true);
                }
#endif
                else
                {
                    base.UpdateBehaviour();
                }
            }
        }

        private bool WaterPot()
        {
#if IL2CPP
            Pot potForWatering = _botanist.GetPotForWatering(0.2f);

            if (potForWatering != null)
            {
                _botanist._waterPotBehaviour.AssignAndEnable(potForWatering);

                return true;
            }
#elif MONO
            Pot potForWatering = Reflection.InvokeMethod<Pot>(typeof(Botanist), "GetPotForWatering", _botanist, [0.2f]);

            if (potForWatering != null)
            {
                Reflection.GetFieldValue<WaterPotBehaviour>(typeof(Botanist), "_waterPotBehaviour", _botanist).AssignAndEnable(potForWatering);

                return true;
            }
#endif

            return false;
        }

        private bool MistMushroomBed()
        {

#if IL2CPP
            MushroomBed mushroomBedForMisting = _botanist.GetMushroomBedForMisting(0.2f);

            if (mushroomBedForMisting != null)
            {
                _botanist._mistMushroomBedBehaviour.AssignAndEnable(mushroomBedForMisting);

                return true;
            }
#elif MONO
            MushroomBed mushroomBedForMisting = Reflection.InvokeMethod<MushroomBed>(typeof(Botanist), "GetMushroomBedForMisting", _botanist, [0.2f]);

            if (mushroomBedForMisting != null)
            {
                Reflection.GetFieldValue<MistMushroomBedBehaviour>(typeof(Botanist), "_mistMushroomBedBehaviour", _botanist).AssignAndEnable(mushroomBedForMisting);

                return true;
            }
#endif

            return false;
        }

        private bool AddSoilToGrowContainer()
        {
#if IL2CPP
            foreach (GrowContainer item in _botanist.GetGrowContainersForSoilPour())
            {
                if (_botanist._addSoilToGrowContainerBehaviour.DoesBotanistHaveAccessToRequiredSupplies(item))
                {
                    _botanist._addSoilToGrowContainerBehaviour.AssignAndEnable(item);
                    
                    return true;
                }

                string fix = "Make sure there's soil in my supplies stash.";

                if (_botanist.configuration.Supplies.SelectedObject == null)
                {
                    fix = "Use your management clipboard to assign a supplies stash to me, then make sure there's soil in it.";
                }

                _botanist.SubmitNoWorkReason("There are empty pots, but I don't have any soil to pour.", fix);
            }
#elif MONO
            foreach (GrowContainer item in Reflection.InvokeMethod<List<GrowContainer>>(typeof(Botanist), "GetGrowContainersForSoilPour", _botanist))
            {
                if (Reflection.GetFieldValue<AddSoilToGrowContainerBehaviour>(typeof(Botanist), "_addSoilToGrowContainerBehaviour", _botanist).DoesBotanistHaveAccessToRequiredSupplies(item))
                {
                    Reflection.GetFieldValue<AddSoilToGrowContainerBehaviour>(typeof(Botanist), "_addSoilToGrowContainerBehaviour", _botanist).AssignAndEnable(item);

                    return true;
                }

                string fix = "Make sure there's soil in my supplies stash.";

                if (Reflection.GetFieldValue<BotanistConfiguration>(typeof(Botanist), "configuration", _botanist).Supplies.SelectedObject == null)
                {
                    fix = "Use your management clipboard to assign a supplies stash to me, then make sure there's soil in it.";
                }

                _botanist.SubmitNoWorkReason("There are empty pots, but I don't have any soil to pour.", fix);
            }
#endif

            return false;
        }

        private bool SowSeedInPot()
        {
#if IL2CPP
            bool flag = false;
            foreach (Pot item in _botanist.GetPotsReadyForSeed())
            {
                if (!_botanist._sowSeedInPotBehaviour.DoesBotanistHaveAccessToRequiredSupplies(item))
                {
                    if (!flag)
                    {
                        flag = true;
                        string fix = "Make sure I have the right seeds in my supplies stash.";

                        if (_botanist.configuration.Supplies.SelectedObject == null)
                        {
                            fix = "Use your management clipboards to assign a supplies stash to me, and make sure it contains the right seeds.";
                        }

                        _botanist.SubmitNoWorkReason("There is a pot ready for sowing, but I don't have any seeds for it.", fix, 1);
                    }
                }
                else if (_botanist.IsEntityAccessible(item.Cast<ITransitEntity>()))
                {
                    _botanist._sowSeedInPotBehaviour.AssignAndEnable(item);

                    return true;
                }
            }
#elif MONO
            bool flag = false;
            foreach (Pot item in Reflection.InvokeMethod<List<Pot>>(typeof(Botanist), "GetPotsReadyForSeed", _botanist))
            {
                if (!Reflection.GetFieldValue<SowSeedInPotBehaviour>(typeof(Botanist), "_sowSeedInPotBehaviour", _botanist).DoesBotanistHaveAccessToRequiredSupplies(item))
                {
                    if (!flag)
                    {
                        flag = true;
                        string fix = "Make sure I have the right seeds in my supplies stash.";

                        if (Reflection.GetFieldValue<BotanistConfiguration>(typeof(Botanist), "configuration", _botanist).Supplies.SelectedObject == null)
                        {
                            fix = "Use your management clipboards to assign a supplies stash to me, and make sure it contains the right seeds.";
                        }

                        _botanist.SubmitNoWorkReason("There is a pot ready for sowing, but I don't have any seeds for it.", fix, 1);
                    }
                }
                else if (Reflection.InvokeMethod<bool>(typeof(Botanist), "IsEntityAccessible", _botanist, [item]))
                {
                    Reflection.GetFieldValue<SowSeedInPotBehaviour>(typeof(Botanist), "_sowSeedInPotBehaviour", _botanist).AssignAndEnable(item);

                    return true;
                }
            }
#endif

            return false;
        }

        private bool ApplySpawnToMushroomBed()
        {
#if IL2CPP
            bool flag = false;
            foreach (MushroomBed item in _botanist.GetBedsReadyForSpawn())
            {
                if (!_botanist._applySpawnToMushroomBedBehaviour.DoesBotanistHaveAccessToRequiredSupplies(item))
                {
                    if (!flag)
                    {
                        flag = true;
                        string fix3 = "Make sure I have shroom spawn my supplies stash.";

                        if (_botanist.configuration.Supplies.SelectedObject == null)
                        {
                            fix3 = "Use your management clipboards to assign a supplies stash to me, and make sure it contains shroom spawn.";
                        }

                        _botanist.SubmitNoWorkReason("I don't have any shroom spawn to mix into my assigned mushroom beds.", fix3, 1);
                    }
                }
                else if (_botanist.IsEntityAccessible(item.Cast<ITransitEntity>()))
                {
                    _botanist._applySpawnToMushroomBedBehaviour.AssignAndEnable(item);

                    return true;
                }
            }
#elif MONO
            bool flag = false;
            foreach (MushroomBed item in Reflection.InvokeMethod<List<MushroomBed>>(typeof(Botanist), "GetBedsReadyForSpawn", _botanist))
            {
                if (!Reflection.GetFieldValue<ApplySpawnToMushroomBedBehaviour>(typeof(Botanist), "_applySpawnToMushroomBedBehaviour", _botanist).DoesBotanistHaveAccessToRequiredSupplies(item))
                {
                    if (!flag)
                    {
                        flag = true;
                        string fix = "Make sure I have shroom spawn my supplies stash.";

                        if (Reflection.GetFieldValue<BotanistConfiguration>(typeof(Botanist), "configuration", _botanist).Supplies.SelectedObject == null)
                        {
                            fix = "Use your management clipboards to assign a supplies stash to me, and make sure it contains shroom spawn.";
                        }

                        _botanist.SubmitNoWorkReason("I don't have any shroom spawn to mix into my assigned mushroom beds.", fix, 1);
                    }
                }
                else if (Reflection.InvokeMethod<bool>(typeof(Botanist), "IsEntityAccessible", _botanist, [item]))
                {
                    Reflection.GetFieldValue<ApplySpawnToMushroomBedBehaviour>(typeof(Botanist), "_applySpawnToMushroomBedBehaviour", _botanist).AssignAndEnable(item);

                    return true;
                }
            }
#endif

            return false;
        }

        private bool ApplyAdditiveToGrowContainer()
        {
#if IL2CPP
            foreach (GrowContainer growContainersForAdditive in _botanist.GetGrowContainersForAdditives())
            {
                if (growContainersForAdditive != null && _botanist._applyAdditiveToGrowContainerBehaviour.DoesBotanistHaveAccessToRequiredSupplies(growContainersForAdditive))
                {
                    _botanist._applyAdditiveToGrowContainerBehaviour.AssignAndEnable(growContainersForAdditive);

                    return true;
                }
            }
#elif MONO
            foreach (GrowContainer growContainersForAdditive in Reflection.InvokeMethod<List<GrowContainer>>(typeof(Botanist), "GetGrowContainersForAdditives", _botanist))
            {
                if (growContainersForAdditive != null && Reflection.GetFieldValue<ApplyAdditiveToGrowContainerBehaviour>(typeof(Botanist), "_applyAdditiveToGrowContainerBehaviour", _botanist).DoesBotanistHaveAccessToRequiredSupplies(growContainersForAdditive))
                {
                    Reflection.GetFieldValue<ApplyAdditiveToGrowContainerBehaviour>(typeof(Botanist), "_applyAdditiveToGrowContainerBehaviour", _botanist).AssignAndEnable(growContainersForAdditive);

                    return true;
                }
            }
#endif

            return false;
        }

        private bool HarvestPot()
        {
#if IL2CPP
            Il2CppSystem.Collections.Generic.List<Pot> potsForHarvest = _botanist.GetPotsForHarvest();
            if (potsForHarvest != null && potsForHarvest.Count > 0)
            {
                _botanist._harvestPotBehaviour.AssignAndEnable(potsForHarvest[0]);

                return true;
            }
#elif MONO
            List<Pot> potsForHarvest = Reflection.InvokeMethod<List<Pot>>(typeof(Botanist), "GetPotsForHarvest", _botanist);
            if (potsForHarvest != null && potsForHarvest.Count > 0)
            {
                Reflection.GetFieldValue<HarvestPotBehaviour>(typeof(Botanist), "_harvestPotBehaviour", _botanist).AssignAndEnable(potsForHarvest[0]);

                return true;
            }
#endif

            return false;
        }

        private bool HarvestMushroomBed()
        {
#if IL2CPP
            Il2CppSystem.Collections.Generic.List<MushroomBed> mushroomBedsForHarvest = _botanist.GetMushroomBedsForHarvest();
            if (mushroomBedsForHarvest != null && mushroomBedsForHarvest.Count > 0)
            {
                _botanist._harvestMushroomBedBehaviour.AssignAndEnable(mushroomBedsForHarvest[0]);

                return true;
            }
#elif MONO
            List<MushroomBed> mushroomBedsForHarvest = Reflection.InvokeMethod<List<MushroomBed>>(typeof(Botanist), "GetMushroomBedsForHarvest", _botanist);
            if (mushroomBedsForHarvest != null && mushroomBedsForHarvest.Count > 0)
            {
                Reflection.GetFieldValue<HarvestMushroomBedBehaviour>(typeof(Botanist), "_harvestMushroomBedBehaviour", _botanist).AssignAndEnable(mushroomBedsForHarvest[0]);

                return true;
            }
#endif

            return false;
        }

        private bool StopDryingRack()
        {
#if IL2CPP
            foreach (DryingRack item in _botanist.GetRacksToStop())
            {
                if (_botanist.IsEntityAccessible(item.Cast<ITransitEntity>()))
                {
                    _botanist.StopDryingRack(item);

                    return true;
                }
            }
#elif MONO
            foreach (DryingRack item in Reflection.InvokeMethod<List<DryingRack>>(typeof(Botanist), "GetRacksToStop", _botanist))
            {
                if (Reflection.InvokeMethod<bool>(typeof(Botanist), "IsEntityAccessible", _botanist, [item]))
                {
                    Reflection.InvokeMethod(typeof(Botanist), "StopDryingRack", _botanist, [item]);

                    return true;
                }
            }
#endif

            return false;
        }

        private bool MoveDryingRackOutput()
        {
#if IL2CPP
            foreach (DryingRack item in _botanist.GetRacksReadyToMove())
            {
                if (_botanist.IsEntityAccessible(item.Cast<ITransitEntity>()))
                {
                    _botanist.MoveItemBehaviour.Initialize((item.Configuration as DryingRackConfiguration).DestinationRoute, item.OutputSlot.ItemInstance);
                    _botanist.MoveItemBehaviour.Enable_Networked();
                    return true;
                }
            }
#elif MONO
            foreach (DryingRack item in Reflection.InvokeMethod<List<DryingRack>>(typeof(Botanist), "GetRacksReadyToMove", _botanist))
            {
                if (Reflection.InvokeMethod<bool>(typeof(Botanist), "IsEntityAccessible", _botanist, [item]))
                {
                    _botanist.MoveItemBehaviour.Initialize((item.Configuration as DryingRackConfiguration).DestinationRoute, item.OutputSlot.ItemInstance);
                    _botanist.MoveItemBehaviour.Enable_Networked();

                    return true;
                }
            }
#endif

            return false;
        }

        private bool UseSpawnStation()
        {
#if IL2CPP
            foreach (MushroomSpawnStation item in _botanist.GetSpawnStationsReadyToUse())
            {
                if (_botanist.IsEntityAccessible(item.Cast<ITransitEntity>()))
                {
                    _botanist._useSpawnStationBehaviour.AssignStation(item);
                    _botanist._useSpawnStationBehaviour.Enable_Networked();

                    return true;
                }
            }
#elif MONO
            foreach (MushroomSpawnStation item in Reflection.InvokeMethod<List<MushroomSpawnStation>>(typeof(Botanist), "GetSpawnStationsReadyToUse", _botanist))
            {
                if (Reflection.InvokeMethod<bool>(typeof(Botanist), "IsEntityAccessible", _botanist, [item]))
                {
                    Reflection.GetFieldValue<UseSpawnStationBehaviour>(typeof(Botanist), "_useSpawnStationBehaviour", _botanist).AssignStation(item);
                    Reflection.GetFieldValue<UseSpawnStationBehaviour>(typeof(Botanist), "_useSpawnStationBehaviour", _botanist).Enable_Networked();

                    return true;
                }
            }
#endif

            return false;
        }

        private bool MoveSpawnStationOutput()
        {
#if IL2CPP
            foreach (MushroomSpawnStation item in _botanist.GetSpawnStationsReadyToMove())
            {
                if (_botanist.IsEntityAccessible(item.Cast<ITransitEntity>()))
                {
                    _botanist.MoveItemBehaviour.Initialize((item.Configuration as SpawnStationConfiguration).DestinationRoute, item.OutputSlot.ItemInstance);
                    _botanist.MoveItemBehaviour.Enable_Networked();

                    return true;
                }
            }
#elif MONO
            foreach (MushroomSpawnStation item in Reflection.InvokeMethod<List<MushroomSpawnStation>>(typeof(Botanist), "GetSpawnStationsReadyToMove", _botanist))
            {
                if (Reflection.InvokeMethod<bool>(typeof(Botanist), "IsEntityAccessible", _botanist, [item]))
                {
                    _botanist.MoveItemBehaviour.Initialize((item.Configuration as SpawnStationConfiguration).DestinationRoute, item.OutputSlot.ItemInstance);
                    _botanist.MoveItemBehaviour.Enable_Networked();

                    return true;
                }
            }
#endif

            return false;
        }

        private bool MoveDryableToRack()
        {
#if IL2CPP
            if (_botanist.CanMoveDryableToRack(out var dryable, out var destinationRack, out var moveQuantity))
            {
                TransitRoute route = new(_botanist.configuration.Supplies.SelectedObject.Cast<ITransitEntity>(), destinationRack.Cast<ITransitEntity>());

                if (_botanist.MoveItemBehaviour.IsTransitRouteValid(route, dryable.ID))
                {
                    _botanist.MoveItemBehaviour.Initialize(route, dryable, moveQuantity);
                    _botanist.MoveItemBehaviour.Enable_Networked();

                    return true;
                }
            }
#elif MONO
            object[] parameters = [null, null, null];
            bool canMove = Reflection.InvokeMethod<bool>(typeof(Botanist), "CanMoveDryableToRack", _botanist, parameters);
            QualityItemInstance dryable = (QualityItemInstance)parameters[0];
            DryingRack destinationRack = (DryingRack)parameters[1];
            int moveQuantity = (int)parameters[2];

            if (canMove)
            {
                TransitRoute route = new(Reflection.GetFieldValue<BotanistConfiguration>(typeof(Botanist), "configuration", _botanist).Supplies.SelectedObject as ITransitEntity, destinationRack);

                if (_botanist.MoveItemBehaviour.IsTransitRouteValid(route, dryable.ID))
                {
                    _botanist.MoveItemBehaviour.Initialize(route, dryable, moveQuantity);
                    _botanist.MoveItemBehaviour.Enable_Networked();

                    return true;
                }
            }
#endif

            return false;
        }

        private bool StartDryingRack()
        {
#if IL2CPP
            foreach (DryingRack item in _botanist.GetRacksToStart())
            {
                if (_botanist.IsEntityAccessible(item.Cast<ITransitEntity>()))
                {
                    _botanist.StartDryingRack(item);

                    return true;
                }
            }
#elif MONO
            foreach (DryingRack item in Reflection.InvokeMethod<List<DryingRack>>(typeof(Botanist), "GetRacksToStart", _botanist))
            {
                if (Reflection.InvokeMethod<bool>(typeof(Botanist), "IsEntityAccessible", _botanist, [item]))
                {
                    Reflection.InvokeMethod(typeof(Botanist), "StartDryingRack", _botanist, [item]);

                    return true;
                }
            }
#endif

            return false;
        }
    }
}
