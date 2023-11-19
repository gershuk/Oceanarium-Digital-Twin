using System;
using UnityEngine;
using UnityEngine.UI;

namespace Aqua.UIBaseElements
{
    public class ScriptsMenuSubpanelViewModel : MonoBehaviour
    {
        [SerializeField]
        private MainScreenSwapContainerModel _swapContainer;

        [SerializeField]
        private ElementPickerViewModel _elementPicker;

        [SerializeField]
        private Button _returnButton;

        [SerializeField]
        private Button _startButton;

        private void RegisterButtons ()
        {
            _startButton.onClick.AddListener(LoadLevel);
            _returnButton.onClick.AddListener(ReturnToMain);
        }

        private void UnregisterButtons ()
        {
            _startButton.onClick.RemoveListener(LoadLevel);
            _returnButton.onClick.RemoveListener(ReturnToMain);
        }

        private void OnDestroy ()
        {
            UnregisterButtons();
        }

        private void Start ()
        {
            RegisterButtons();
        }

        public void ReturnToMain() => _swapContainer.ActiveMainMenu();
        
        public void LoadLevel() => _elementPicker.ActiveElement();
    }
}
