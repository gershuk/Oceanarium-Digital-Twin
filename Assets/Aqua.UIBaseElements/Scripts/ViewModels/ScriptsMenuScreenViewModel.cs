#nullable enable

using System;

using UnityEngine;

namespace Aqua.UIBaseElements
{
    [Serializable]
    public struct SceneData
    {
        [SerializeField]
        private string? _description;

        [SerializeField]
        private string _fileName;

        [SerializeField]
        private string _name;

        public string? Description { get => _description; set => _description = value; }

        public string FileName { get => _fileName; set => _fileName = value; }

        public string Name { get => _name; set => _name = value; }

        public SceneData (string name, string? description, string fileName)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _description = description;
            _fileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        }
    }

    public class ScriptsMenuScreenViewModel : MonoBehaviour
    {
        private LoadingScreenViewModel _loadingScreenViewModel;

        [SerializeField]
        private ElementPickerViewModel _scenePicker;

        [SerializeField]
        private SceneData[] _scenesData;

        private void Start ()
        {
            _loadingScreenViewModel = FindFirstObjectByType<LoadingScreenViewModel>();
            if (_scenesData != null)
            {
                foreach (var sceneData in _scenesData)
                {
                    _scenePicker.AddElement(sceneData.Name, () => _loadingScreenViewModel.StartLoadingCoroutine(sceneData.FileName)); //SceneLoader.Instance.LoadScene(sceneData.FileName));
                }
            }
        }
    }
}