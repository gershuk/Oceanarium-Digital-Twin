#nullable enable

using UnityEngine;

namespace Aqua.UIBaseElements
{
    public class MainMenuSubpanelModel : MonoBehaviour
    {
        [SerializeField]
        private MainScreenSwapContainerModel _swapContainer;

        public void Exit () => Application.Quit();

        public void OpenLoadMenu () => _swapContainer.ActiveLoadMenu();

        public void OpenScriptsMenu () => _swapContainer.ActiveScriptsMenu();

        public void OpenSettingsMenu () => _swapContainer.ActiveSettingsMenu();
    }
}