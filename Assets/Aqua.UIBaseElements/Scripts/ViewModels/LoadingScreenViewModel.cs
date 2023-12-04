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
        private Coroutine? _loadingCoroutine;

        [SerializeField]
        private GameObject _loadingCamera;

        public const int EndLoadingWaitTime = 5;

        [SerializeField]
        private Canvas _canvas;

        [SerializeField] 
        private LoadingBarView _loadingBar;

        private readonly IUniversalSocket<float, float> _progressSocket = new UniversalSocket<float, float>(0);

        public void Awake ()
        {
            if (FindObjectsOfType<LoadingScreenViewModel>().Length > 1)
            {
                Destroy(gameObject);
                return;
            }

            _loadingBar.ProgressSocket.SubscribeTo(_progressSocket);
            if (_canvas != null )
                _canvas = GetComponent<Canvas>();
            DontDestroyOnLoad(gameObject);
            DisableView();
        }

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

        public void StartLoadingCoroutine (string name, LoadSceneParameters parameters = new())
        {
            if (_loadingCoroutine != null)
                throw new Exception("Already loading scene");

            _loadingCoroutine = StartCoroutine(StartLoadingWithScreen(name, parameters));
        }

        private IEnumerator StartLoadingWithScreen (string name, LoadSceneParameters parameters = new())
        {
            var loadingOperation = SceneLoader.Instance.LoadSceneAsync(name, parameters);
            
            EnableView();

            while (!loadingOperation.isDone)
            {
                _progressSocket.TrySetValue(loadingOperation.progress);
                yield return null;
            }

            _progressSocket.TrySetValue(loadingOperation.progress);
            yield return new WaitForSeconds(EndLoadingWaitTime);

            DisableView();

            _loadingCoroutine = null;
        }

        private void OnDestroy ()
        {
            if (_loadingCoroutine != null)
                StopCoroutine(_loadingCoroutine);
        }
    }
}
