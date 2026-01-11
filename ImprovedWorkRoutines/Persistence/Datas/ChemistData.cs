namespace ImprovedWorkRoutines.Persistence.Datas
{
    public class ChemistData : DataBase
    {
        public PriorityWrapper Priorities;

        public ChemistData(string identifier, bool loadDefaults = false) : base(identifier)
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
                FinishLabOven = 0,
                StartLabOven = 1,
                StartChemistryStation = 2,
                StartCauldron = 3,
                StartMixingStation = 4,
                MoveLabOvenOutput = 5,
                MoveChemistryStationOutput = 6,
                MoveCauldronOutput = 7,
                MoveMixingStationOutput = 8
            };
        }

        public struct PriorityWrapper
        {
            public int FinishLabOven;

            public int StartLabOven;

            public int StartChemistryStation;

            public int StartCauldron;

            public int StartMixingStation;

            public int MoveLabOvenOutput;

            public int MoveChemistryStationOutput;

            public int MoveCauldronOutput;

            public int MoveMixingStationOutput;
        };
    }
}
