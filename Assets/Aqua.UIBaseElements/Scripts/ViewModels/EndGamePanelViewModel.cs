#nullable enable

using UniRx;
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
        private ScenarioSceneBuilder _scenarioSceneBuilder;

        private bool _isInited = false;

        private EndGamePanelState _state;

        [SerializeField]
        private TMP_Text _mainLabel;

        [SerializeField]
        private TMP_Text _infoLabel;

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
                        _mainLabel.text = "������� ���������.";
                        _infoLabel.text = _scenarioSceneBuilder.FirstFailedTaskSocket.GetValue()?.FailMessage ?? "������ ���������.";
                        break;
                    case EndGamePanelState.Win:
                        _mainLabel.text = "������� ��������.";
                        _infoLabel.text = "��� ������ ���������.";
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
