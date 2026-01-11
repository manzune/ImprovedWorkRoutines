namespace ImprovedWorkRoutines.Persistence.Datas
{
    public class PackagerData : DataBase
    {
        public PriorityWrapper Priorities;

        public PackagerData(string identifier, bool loadDefaults = false) : base(identifier)
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
                StartPackaging = 0,
                StartBrickPress = 1,
                MovePackagingStationItems = 2,
                MoveBrickPressItems = 3,
                HandleTransitRoute = 4
            };
        }

        public struct PriorityWrapper
        {
            public int StartPackaging;

            public int StartBrickPress;

            public int MovePackagingStationItems;

            public int MoveBrickPressItems;

            public int HandleTransitRoute;
        };
    }
}
