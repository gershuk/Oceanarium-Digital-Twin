using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Aqua.FPSController;
using Aqua.SceneController;

using UnityEngine;
using UnityEngine.UI;

namespace Aqua.UIBaseElements
{
    public class HUDSubpanelViewModel : MonoBehaviour
    {
        private bool _isInited = false;

        private LoadingScreenViewModel _loadingScreenViewModel;

        public Action CloseHUDSubpanel { get; set; }

        public const string MainMenuName = "MainMenu";

        [SerializeField]
        private Button _mainMenuButton;

        [SerializeField]
        private Button _loadButton;

        [SerializeField]
        private Button _saveButton;

        [SerializeField]
        private Button _continueButton;

        public Button MainMenuButton => _mainMenuButton;
        public Button LoadButton => _loadButton;
        public Button SaveButton => _saveButton;
        public Button ContinueButton => _continueButton;

        private void OnDestroy () => UnregisterButtons();

        private void RegisterButtons ()
        {
            MainMenuButton.onClick.AddListener(GoToMainMenu);
            ContinueButton.onClick.AddListener(Continue);
            LoadButton.onClick.AddListener(Load);
            SaveButton.onClick.AddListener(Save);
        }

        private void Start ()
        {
            ForceInit();
        }

        public void ForceInit ()
        {
            if (_isInited)
                return;

            RegisterButtons();

            _loadingScreenViewModel = FindFirstObjectByType<LoadingScreenViewModel>();

            _isInited = true;
        }

        private void UnregisterButtons ()
        {
            MainMenuButton.onClick.RemoveListener(GoToMainMenu);
            ContinueButton.onClick.RemoveListener(Continue);
            LoadButton.onClick.RemoveListener(Load);
            SaveButton.onClick.RemoveListener(Save);
        }

        public void Continue () => CloseHUDSubpanel();

        public void GoToMainMenu () => _loadingScreenViewModel.StartLoadingCoroutine(MainMenuName);

        public void Save () => Debug.Log($"{nameof(Save)} not implemented");

        public void Load () => Debug.Log($"{nameof(Load)} not implemented");
    }
}
