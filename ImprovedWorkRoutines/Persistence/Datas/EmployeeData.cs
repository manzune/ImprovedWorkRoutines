using System.Collections.Generic;

namespace ImprovedWorkRoutines.Persistence.Datas
{
    public class EmployeeData : DataBase
    {
        public Dictionary<string, int> Tasks;

        public EmployeeData(string identifier, bool loadDefaults = false) : base(identifier)
        {
            if (loadDefaults)
            {
                LoadDefaults();
            }
        }

        private void LoadDefaults()
        {
            Tasks = [];
        }
    }
}
