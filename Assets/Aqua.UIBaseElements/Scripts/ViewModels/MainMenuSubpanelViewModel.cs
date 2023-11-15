using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Aqua.UIBaseElements
{
    public sealed class MainMenuPanelController : MonoBehaviour
    {
        [SerializeField]
        private MainMenuSubpanelModel _mainMenuSubpanelModel;

        [SerializeField]
        private Button _newButton;

        [SerializeField]
        private Button _loadButton;

        [SerializeField]
        private Button _settingsButton;

        [SerializeField] 
        private Button _exitButton;

        public Button NewButton => _newButton;
        public Button LoadButton => _loadButton;
        public Button SettingsButton => _settingsButton;
        public Button ExitButton => _exitButton;

        private void Start()
        {
            if (_mainMenuSubpanelModel == null)
                _mainMenuSubpanelModel = GetComponent<MainMenuSubpanelModel>();

            _exitButton.onClick.AddListener(_mainMenuSubpanelModel.Exit);
        }
    }
}
