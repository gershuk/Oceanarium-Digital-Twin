using UnityEngine;
using UnityEngine.UI;

namespace Aqua.UIBaseElements
{
    public class ScriptsMenuSubpanelViewModel : MonoBehaviour
    {
        [SerializeField]
        private ElementPickerViewModel _elementPicker;

        [SerializeField]
        private Button _returnButton;

        [SerializeField]
        private Button _startButton;

        [SerializeField]
        private MainScreenSwapContainerModel _swapContainer;

        private void OnDestroy () => UnregisterButtons();

        private void RegisterButtons ()
        {
            _startButton.onClick.AddListener(LoadLevel);
            _returnButton.onClick.AddListener(ReturnToMain);
        }

        private void Start () => RegisterButtons();

        private void UnregisterButtons ()
        {
            _startButton.onClick.RemoveListener(LoadLevel);
            _returnButton.onClick.RemoveListener(ReturnToMain);
        }

        public void LoadLevel () => _elementPicker.ActiveElement();

        public void ReturnToMain () => _swapContainer.ActiveMainMenu();
    }
}