using System.Collections.Generic;

namespace ImprovedWorkRoutines.Persistence.Datas
{
    public class BotanistData : DataBase
    {
        public PriorityWrapper Priorities;

        public BotanistData(string identifier, bool loadDefaults = false) : base(identifier)
        {
            if (loadDefaults)
            {
                LoadDefaults();
            }
        }

        public void LoadDefaults()
        {
            Priorities = new()
            {
                WaterPot = 0,
                MistMushroomBed = 1,
                AddSoilToGrowContainer = 2,
                SowSeedInPot = 3,
                ApplySpawnToMushroomBed = 4,
                ApplyAdditiveToGrowContainer = 5,
                HarvestPot = 6,
                HarvestMushroomBed = 7,
                StopDryingRack = 8,
                MoveDryingRackOutput = 9,
                UseSpawnStation = 10,
                MoveSpawnStationOutput = 11,
                MoveDryableToRack = 12,
                StartDryingRack = 13
            };
        }

        public struct PriorityWrapper
        {
            public int WaterPot;

            public int MistMushroomBed;

            public int AddSoilToGrowContainer;

            public int SowSeedInPot;

            public int ApplySpawnToMushroomBed;

            public int ApplyAdditiveToGrowContainer;

            public int HarvestPot;

            public int HarvestMushroomBed;

            public int StopDryingRack;

            public int MoveDryingRackOutput;

            public int UseSpawnStation;

            public int MoveSpawnStationOutput;

            public int MoveDryableToRack;

            public int StartDryingRack;
        };
    }
}
