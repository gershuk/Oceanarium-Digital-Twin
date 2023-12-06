using System.Collections;
using System.Collections.Generic;

using Aqua.UIBaseElements;

using UnityEngine;

namespace Aqua.Scenes.StartScene
{
    public class Starter : MonoBehaviour
    {
        public const string MainMenuSceneName = "MainMenu";

        [SerializeField]
        private LoadingScreenViewModel _loadingScreenViewModel;

        private void Start ()
        {
            if (_loadingScreenViewModel == null)
                _loadingScreenViewModel = FindFirstObjectByType<LoadingScreenViewModel>();

            _loadingScreenViewModel.StartLoadingCoroutine(MainMenuSceneName);
        }
    }
}
