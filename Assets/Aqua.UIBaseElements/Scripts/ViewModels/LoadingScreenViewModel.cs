using System.Collections;
using System.Collections.Generic;

using Aqua.SceneController;
using Aqua.SocketSystem;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Aqua.UIBaseElements
{
    public class LoadingScreenViewModel : MonoBehaviour
    {
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
            _canvas.enabled = false;
        }

        public IEnumerator StartLoadingWithScreen (string name, LoadSceneParameters parameters = new())
        {
            _canvas.enabled = true;
            var loadingOperation = SceneLoader.Instance.LoadSceneAsync(name, parameters);
            while (!loadingOperation.isDone)
            {
                _progressSocket.TrySetValue(loadingOperation.progress);
                yield return null;
            }
            yield return new WaitForSeconds(EndLoadingWaitTime);
            _canvas.enabled = false;
        }
    }
}
