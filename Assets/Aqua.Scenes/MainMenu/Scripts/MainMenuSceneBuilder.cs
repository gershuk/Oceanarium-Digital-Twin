using System.Collections;
using System.Collections.Generic;
using Aqua.SceneController;
using Aqua.UIBaseElements;

using UnityEngine;

namespace Aqua.Scenes.MainMenu
{
    public class MainMenuSceneBuilder : UISceneBuilder
    {
        [SerializeField]
        private GraphicsSettingsViewModel _graphicsSettingsViewModel;

        protected override void SubInit ()
        {
            base.SubInit();

            // ToDo : Remove with notmal model decomposition
            if (_graphicsSettingsViewModel == null)
                _graphicsSettingsViewModel = FindFirstObjectByType<GraphicsSettingsViewModel>();

            _graphicsSettingsViewModel.ForceInit();
        }
    }
}
