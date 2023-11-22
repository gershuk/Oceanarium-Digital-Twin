using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Aqua.UIBaseElements
{
    public sealed class MainMenuPanelController : MonoBehaviour
    {
        [SerializeField]
        private Button _exitButton;

        [SerializeField]
        private Button _loadButton;

        [SerializeField]
        private Button _newButton;

        [SerializeField]
        private Button _settingsButton;

        [SerializeField]
        private MainScreenSwapContainerModel _swapContainer;

        public Button ExitButton => _exitButton;
        public Button LoadButton => _loadButton;
        public Button NewButton => _newButton;
        public Button SettingsButton => _settingsButton;

        private void RegisterButtons() 
        {
            _newButton.onClick.AddListener(OpenScriptsMenu);
            _loadButton.onClick.AddListener(OpenLoadMenu);
            _settingsButton.onClick.AddListener(OpenSettingsMenu);
            _exitButton.onClick.AddListener(Exit);
        }

        private void UnregisterButtons ()
        {
            _newButton.onClick.RemoveListener(OpenScriptsMenu);
            _loadButton.onClick.RemoveListener(OpenLoadMenu);
            _settingsButton.onClick.RemoveListener(OpenSettingsMenu);
            _exitButton.onClick.RemoveListener(Exit);
        }

        private void OnDestroy ()
        {
            UnregisterButtons();
        }

        private void Start ()
        {
            RegisterButtons();
        }

        public void Exit () => Application.Quit();

        public void OpenLoadMenu () => _swapContainer.ActiveLoadMenu();

        public void OpenScriptsMenu () => _swapContainer.ActiveScriptsMenu();

        public void OpenSettingsMenu () => _swapContainer.ActiveSettingsMenu();
    }
}