using UnityEngine;

namespace Aqua.UIBaseElements
{
    public class MainScreenSwapContainerModel : SwapContainerModel
    {
        [SerializeField]
        private GameObject _loadMenu;

        [SerializeField]
        private GameObject _mainMenu;

        [SerializeField]
        private GameObject _scriptsMenu;

        [SerializeField]
        private GameObject _settingsMenu;

        protected override void Start ()
        {
            base.Start();
            _uiElements.Clear();
            _uiElements.Add(_mainMenu);
            _uiElements.Add(_scriptsMenu);
            _uiElements.Add(_loadMenu);
            _uiElements.Add(_settingsMenu);

            DisableAll();

            ActiveMainMenu();
        }

        public void ActiveLoadMenu () => Swap(2);

        public void ActiveMainMenu () => Swap(0);

        public void ActiveScriptsMenu () => Swap(1);

        public void ActiveSettingsMenu () => Swap(3);
    }
}