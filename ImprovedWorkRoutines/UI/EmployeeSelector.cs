using UnityEngine;

namespace ImprovedWorkRoutines.UI
{
    public class EmployeeSelector
    {
        public GameObject Container;

        private Transform _content;

        private EmployeeManagerApp _app;

        public EmployeeSelector(EmployeeManagerApp app)
        {
            _app = app;

            BuildUI();
        }

        private void BuildUI()
        {
            Container = _app.Container.transform.Find("Details").gameObject;
            _content = Container.transform.Find("Scroll View/Viewport/Content");

            for (int i = _content.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(_content.GetChild(i).gameObject);
            }
        }
    }
}
