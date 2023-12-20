#nullable enable

using UniRx;
using UniRx.Triggers;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Aqua.UIBaseElements
{
    public class EndGameSubpanelViewModel : MonoBehaviour
    {
        private bool _isInited = false;
        private LoadingScreenViewModel _loadingScreenViewModel;

        [SerializeField]
        private Button _restartButton;

        [SerializeField]
        private Button _returnMenuButton;

        public const string MainMenuName = "MainMenu";

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

        public Button RestartButton => _restartButton;
        public Button ReturnMenuButton => _returnMenuButton;

        private void OnDestroy () => UnregisterButtons();

        private void RegisterButtons ()
        {
            ReturnMenuButton.onClick.AddListener(GoToMainMenu);
            RestartButton.onClick.AddListener(Restart);
        }

        private void Start () => ForceInit();

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

        public void GoToMainMenu () => LoadingScreenViewModel.StartLoadingCoroutine(MainMenuName);

        public void Restart () => LoadingScreenViewModel.StartLoadingCoroutine(SceneManager.GetActiveScene().name);
    }
}