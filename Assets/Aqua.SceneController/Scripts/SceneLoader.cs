using UnityEngine;
using UnityEngine.SceneManagement;

namespace Aqua.SceneController
{
    public class SceneLoader
    {
        public static SceneLoader Instance = new();

        public void LoadScene (string name, LoadSceneParameters parameters = new()) => SceneManager.LoadScene(name, parameters);

        public AsyncOperation LoadSceneAsync (string name, LoadSceneParameters parameters = new()) => SceneManager.LoadSceneAsync(name, parameters);
    }
}