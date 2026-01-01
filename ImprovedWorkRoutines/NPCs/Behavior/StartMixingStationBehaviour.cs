using System.Collections.Generic;
using UnityEngine;
using MelonLoader;
using System.Collections;
using static UnityEngine.UI.Image;

#if IL2CPP
using Il2CppScheduleOne.Employees;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.ItemFramework;
using Il2CppFishNet;
using Il2CppInterop.Runtime;
using S1StartMixingStationBehaviour = Il2CppScheduleOne.NPCs.Behaviour.StartMixingStationBehaviour;
#elif MONO
using ScheduleOne.Employees;
using ScheduleOne.ObjectScripts;
using ScheduleOne.ItemFramework;
using System.Reflection;
using FishNet;
using S1StartMixingStationBehaviour = ScheduleOne.NPCs.Behaviour.StartMixingStationBehaviour;
#endif

namespace ImprovedWorkRoutines.NPCs.Behavior
{
    public class StartMixingStationBehaviour
    {
        private static readonly List<StartMixingStationBehaviour> actives = [];

        private readonly Chemist _chemist;

        private readonly S1StartMixingStationBehaviour _original;

        private MixingStation _targetStation;

        private object _routine;

        private StartMixingStationBehaviour(S1StartMixingStationBehaviour original)
        {
            _original = original;
#if IL2CPP
            _chemist = original.Npc.Cast<Chemist>();
#elif MONO
            _chemist = original.Npc as Chemist;
#endif
        }

        public static StartMixingStationBehaviour RetrieveOrCreate(S1StartMixingStationBehaviour original)
        {
            StartMixingStationBehaviour behavior = actives.Find(x => x._original == original);

            if (behavior == null)
            {
                behavior = new(original);
                actives.Add(behavior);

                Utils.Logger.Debug("StartMixingStationBehaviour", $"Created for: {original.Npc.fullName}");
            }

            return behavior;
        }

        public void AssignStation(MixingStation station)
        {
            _targetStation = station;
            Utils.Logger.Debug("StartMixingStationBehaviour", $"Station assigned for: {_chemist.fullName}");
        }

        public void RpcLogic___StartCook_2166136261()
        {
            if (_routine == null && _targetStation != null)
            {
                _routine = MelonCoroutines.Start(CookRoutine());
            }

            IEnumerator CookRoutine()
            {
                Utils.Logger.Debug("StartMixingStationBehaviour", $"Routine patched for: {_chemist.fullName}");

                _chemist.Movement.FacePoint(_targetStation.transform.position);
                yield return new WaitForSeconds(0.5f);
#if IL2CPP
                if (!_original.CanCookStart())
                {
                    _original.StopCook();
#elif MONO
                if (!(bool)typeof(S1StartMixingStationBehaviour).GetMethod("CanCookStart", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField).Invoke(_original, []))
                {
                    typeof(S1StartMixingStationBehaviour).GetMethod("StopCook", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(_original, []);
#endif
                    _original.Deactivate_Networked(null);
                }
                else
                {
                    _targetStation.SetNPCUser(_chemist.NetworkObject);
                    _chemist.SetAnimationBool_Networked(null, "UseChemistryStation", value: true);

#if IL2CPP
                    QualityItemInstance product = _targetStation.ProductSlot.ItemInstance.Cast<QualityItemInstance>();
#elif MONO
                    QualityItemInstance product = _targetStation.ProductSlot.ItemInstance as QualityItemInstance;
#endif
                    ItemInstance mixer = _targetStation.MixerSlot.ItemInstance;
                    int mixQuantity = _targetStation.GetMixQuantity();

                    for (int i = 0; i < mixQuantity; i++)
                    {
                        yield return new WaitForSeconds(ModConfig.Chemist.InsertIngredientTime);
                    }

                    if (InstanceFinder.IsServer)
                    {
                        _targetStation.ProductSlot.ChangeQuantity(-mixQuantity);
                        _targetStation.MixerSlot.ChangeQuantity(-mixQuantity);

                        MixOperation operation = new(product.ID, product.Quality, mixer.ID, mixQuantity);
                        _targetStation.SendMixingOperation(operation, 0);
                    }

#if IL2CPP
                    _original.StopCook();
#elif MONO
                    typeof(S1StartMixingStationBehaviour).GetMethod("StopCook", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(_original, []);
#endif
                    _original.Deactivate_Networked(null);
                }
            }
        }

        public void StopCook()
        {
            if (_routine != null)
            {
                MelonCoroutines.Stop(_routine);
                _routine = null;

                Utils.Logger.Debug("StartMixingStationBehaviour", $"Routine stopped for: {_chemist.fullName}");
            }
        }
    }
}
