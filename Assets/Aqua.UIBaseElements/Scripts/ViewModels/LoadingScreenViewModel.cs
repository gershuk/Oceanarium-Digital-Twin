using System.Collections;
using System;

using Aqua.SceneController;

#nullable enable

using Aqua.SocketSystem;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Aqua.UIBaseElements
{
    public class LoadingScreenViewModel : MonoBehaviour
    {
        private readonly IUniversalSocket<float, float> _progressSocket = new UniversalSocket<float, float>(0);

        [SerializeField]
        private Canvas _canvas;

        [SerializeField]
        private CursorViewModel _cursorViewModel;

        [SerializeField]
        private LoadingBarView _loadingBar;

        [SerializeField]
        private GameObject _loadingCamera;

        private Coroutine? _loadingCoroutine;
        public const int EndLoadingWaitTime = 5;
        public const float LoadingSceneCoefficient = 0.5f;

        private void DisableView ()
        {
            _canvas.enabled = false;
            _loadingCamera.SetActive(false);
        }

        private void EnableView ()
        {
            _canvas.enabled = true;
            _loadingCamera.SetActive(true);
        }

        private void OnDestroy ()
        {
            if (_loadingCoroutine != null)
                StopCoroutine(_loadingCoroutine);
        }

        private IEnumerator StartLoadingWithScreen (string name, LoadSceneParameters parameters = new())
        {
            if (FindFirstObjectByType<SceneBuilder>() is var oldSceneBuilder and not null)
                oldSceneBuilder.DestroyScene();
            yield return null;
            _cursorViewModel.IsCursorAcitve = true;

            var loadingOperation = SceneLoader.Instance.LoadSceneAsync(name, parameters);

            EnableView();

            do
            {
                _progressSocket.TrySetValue(loadingOperation.progress * LoadingSceneCoefficient);
                yield return null;
            } while (!loadingOperation.isDone);

            var sceneBuilder = FindFirstObjectByType<SceneBuilder>();

            sceneBuilder.StartBuildingCorutine();

            do
            {
                _progressSocket.TrySetValue(LoadingSceneCoefficient + sceneBuilder.BuildingPercentSocket.GetValue());
                yield return null;
            } while (sceneBuilder.StateSocket.GetValue() != BuilderState.BuildingEnded);

            _progressSocket.TrySetValue(1);

            yield return new WaitForSeconds(2);

            DisableView();

            sceneBuilder.StartStartingCorutine();

            _loadingCoroutine = null;
        }

        public void Awake ()
        {
            if (FindObjectsOfType<LoadingScreenViewModel>().Length > 1)
            {
                Destroy(gameObject);
                return;
            }

            _loadingBar.ProgressSocket.SubscribeTo(_progressSocket);

            if (_canvas == null)
                _canvas = GetComponent<Canvas>();

            if (_cursorViewModel == null)
                _cursorViewModel = FindFirstObjectByType<CursorViewModel>();

            DontDestroyOnLoad(gameObject);
            DisableView();
        }

        public void StartLoadingCoroutine (string name, LoadSceneParameters parameters = new())
        {
            if (_loadingCoroutine != null)
                throw new Exception("Already loading scene");

            _loadingCoroutine = StartCoroutine(StartLoadingWithScreen(name, parameters));
        }
    }
}