#nullable enable

using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine.SceneManagement;

namespace Aqua.UIBaseElements
{
    public class EndGameSubpanelViewModel : MonoBehaviour
    {
        private LoadingScreenViewModel _loadingScreenViewModel;

        private bool _isInited = false;

        public const string MainMenuName = "MainMenu";

        [SerializeField]
        private Button _returnMenuButton;

        [SerializeField]
        private Button _restartButton;

        public Button ReturnMenuButton => _returnMenuButton;

        public Button RestartButton => _restartButton;

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

        private void RegisterButtons ()
        {
            ReturnMenuButton.onClick.AddListener(GoToMainMenu);
            RestartButton.onClick.AddListener(Restart);
        }

        private void OnDestroy () => UnregisterButtons();

        private void Start ()
        {
            ForceInit();
        }

        private void UnregisterButtons ()
        {
            ReturnMenuButton.onClick.RemoveListener(GoToMainMenu);
            RestartButton.onClick.RemoveListener(Restart);
        }

        public void ForceInit ()
        {
            if (_isInited)
                return;

            RegisterButtons();

            LoadingScreenViewModel = FindFirstObjectByType<LoadingScreenViewModel>();

            _isInited = true;
        }

        public void Restart () => LoadingScreenViewModel.StartLoadingCoroutine(SceneManager.GetActiveScene().name);

        public void GoToMainMenu () => LoadingScreenViewModel.StartLoadingCoroutine(MainMenuName);
    }
}
