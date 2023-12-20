#nullable enable

using Aqua.SceneController;

using TMPro;

using UnityEngine;

namespace Aqua.UIBaseElements
{
    public enum EndGamePanelState
    {
        None = 0,
        Lose = 1,
        Win = 2,
    }

    // ToDo : Add sockets
    public class EndGamePanelViewModel : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _infoLabel;

        private bool _isInited = false;

        [SerializeField]
        private TMP_Text _mainLabel;

        private ScenarioSceneBuilder? _scenarioSceneBuilder;
        private EndGamePanelState _state;

        public EndGamePanelState State
        {
            get => _state;
            set
            {
                _state = value;

                switch (value)
                {
                    case EndGamePanelState.None:
                        _mainLabel.text = "None";
                        _infoLabel.text = "None";
                        break;

                    case EndGamePanelState.Lose:
                        _mainLabel.text = "Задание провалено.";
                        _infoLabel.text = _scenarioSceneBuilder.FirstFailedTaskSocket.GetValue()?.FailMessage ?? "Задача провалена.";
                        break;

                    case EndGamePanelState.Win:
                        _mainLabel.text = "Задание пройдено.";
                        _infoLabel.text = "Все задачи выполнены.";
                        break;
                }
            }
        }

        public void ForceInit ()
        {
            if (_isInited == true)
                return;

            _scenarioSceneBuilder = FindFirstObjectByType<ScenarioSceneBuilder>();

            _isInited = true;
        }
    }
}