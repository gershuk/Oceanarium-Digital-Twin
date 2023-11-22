using UnityEngine;
using UnityEngine.UI;

namespace Aqua.UIBaseElements
{
    public class SettingsSubpanelViewModel : MonoBehaviour
    {
        [SerializeField]
        private Button _applyButton;

        [SerializeField]
        private GraphicsSettingsViewModel _graphicsSettingsViewModel;

        [SerializeField]
        private Button _resetButton;

        [SerializeField]
        private Button _returnButton;

        [SerializeField]
        private MainScreenSwapContainerModel _swapContainer;

        private void OnDestroy () => UnregisterButtons();

        private void RegisterButtons ()
        {
            _returnButton.onClick.AddListener(ReturnToMainMenu);
            _resetButton.onClick.AddListener(ResetSettings);
            _applyButton.onClick.AddListener(ApplySettings);
        }

        private void Start () => RegisterButtons();

        private void UnregisterButtons ()
        {
            _returnButton.onClick.RemoveListener(ReturnToMainMenu);
            _resetButton.onClick.RemoveListener(ResetSettings);
            _applyButton.onClick.RemoveListener(ApplySettings);
        }

        public void ApplySettings () => _graphicsSettingsViewModel.Model.SaveDataToAssignedFile();

        public void ResetSettings () => _graphicsSettingsViewModel.Model.ReloadDataFromAssignedFile();

        public void ReturnToMainMenu ()
        {
            ResetSettings();
            _swapContainer.ActiveMainMenu();
        }
    }
}