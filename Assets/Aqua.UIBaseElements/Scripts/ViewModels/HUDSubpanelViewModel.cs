using System;

using UniRx;
using UniRx.Triggers;

using UnityEngine;
using UnityEngine.UI;

namespace Aqua.UIBaseElements
{
    public class HUDSubpanelViewModel : MonoBehaviour
    {
        [SerializeField]
        private Button _continueButton;

        private bool _isInited = false;

        [SerializeField]
        private Button _loadButton;

        private LoadingScreenViewModel _loadingScreenViewModel;

        [SerializeField]
        private Button _mainMenuButton;

        [SerializeField]
        private Button _saveButton;

        public const string MainMenuName = "MainMenu";
        public Action CloseHUDSubpanel { get; set; }
        public Button ContinueButton => _continueButton;
        public Button LoadButton => _loadButton;

        public LoadingScreenViewModel LoadingScreenViewModel
        {
            get => _loadingScreenViewModel;
            protected set
            {
                _loadingScreenViewModel = value;

                if (_loadingScreenViewModel != null)
                {
                    _loadingScreenViewModel.OnDestroyAsObservable()
                                           .Subscribe(u => _loadingScreenViewModel = FindFirstObjectByType<LoadingScreenViewModel>())
                                           .AddTo(this);
                }
            }
        }

        public Button MainMenuButton => _mainMenuButton;
        public Button SaveButton => _saveButton;

        private void OnDestroy () => UnregisterButtons();

        private void RegisterButtons ()
        {
            MainMenuButton.onClick.AddListener(GoToMainMenu);
            ContinueButton.onClick.AddListener(Continue);
            LoadButton.onClick.AddListener(Load);
            SaveButton.onClick.AddListener(Save);
        }

        private void Start () => ForceInit();

        private void UnregisterButtons ()
        {
            MainMenuButton.onClick.RemoveListener(GoToMainMenu);
            ContinueButton.onClick.RemoveListener(Continue);
            LoadButton.onClick.RemoveListener(Load);
            SaveButton.onClick.RemoveListener(Save);
        }

        public void Continue () => CloseHUDSubpanel();

        public void ForceInit ()
        {
            if (_isInited)
                return;

            RegisterButtons();

            LoadingScreenViewModel = FindFirstObjectByType<LoadingScreenViewModel>();

            _isInited = true;
        }

        public void GoToMainMenu () => LoadingScreenViewModel.StartLoadingCoroutine(MainMenuName);

        public void Load () => Debug.Log($"{nameof(Load)} not implemented");

        public void Save () => Debug.Log($"{nameof(Save)} not implemented");
    }
}