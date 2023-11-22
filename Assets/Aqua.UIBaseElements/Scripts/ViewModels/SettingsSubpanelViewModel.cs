using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Aqua.UIBaseElements
{
    public class SettingsSubpanelViewModel : MonoBehaviour
    {
        [SerializeField]
        private Button _returnButton;

        [SerializeField]
        private Button _applyButton;

        [SerializeField]
        private Button _resetButton;

        [SerializeField]
        private MainScreenSwapContainerModel _swapContainer;

        [SerializeField]
        private GraphicsSettingsViewModel _graphicsSettingsViewModel;

        private void RegisterButtons ()
        {
            _returnButton.onClick.AddListener(ReturnToMainMenu);
            _resetButton.onClick.AddListener(ResetSettings);
            _applyButton.onClick.AddListener(ApplySettings);
        }

        private void UnregisterButtons ()
        {
            _returnButton.onClick.RemoveListener(ReturnToMainMenu);
            _resetButton.onClick.RemoveListener(ResetSettings);
            _applyButton.onClick.RemoveListener(ApplySettings);
        }

        private void OnDestroy ()
        {
            UnregisterButtons();
        }

        private void Start ()
        {
            RegisterButtons();
        }

        public void ReturnToMainMenu ()
        {
            ResetSettings();
            _swapContainer.ActiveMainMenu();
        }

        public void ResetSettings () => _graphicsSettingsViewModel.Model.ReloadDataFromAssignedFile();

        public void ApplySettings () => _graphicsSettingsViewModel.Model.SaveDataToAssignedFile();
    }
}
