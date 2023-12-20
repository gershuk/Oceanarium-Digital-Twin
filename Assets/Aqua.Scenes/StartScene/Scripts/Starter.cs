using Aqua.UIBaseElements;

using UnityEngine;

namespace Aqua.Scenes.StartScene
{
    public class Starter : MonoBehaviour
    {
        [SerializeField]
        private LoadingScreenViewModel _loadingScreenViewModel;

        public const string MainMenuSceneName = "MainMenu";

        private void Start ()
        {
            if (_loadingScreenViewModel == null)
                _loadingScreenViewModel = FindFirstObjectByType<LoadingScreenViewModel>();

            _loadingScreenViewModel.StartLoadingCoroutine(MainMenuSceneName);
        }
    }
}