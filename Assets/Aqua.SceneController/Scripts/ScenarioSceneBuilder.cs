#nullable enable

using System.Collections;
using System.Collections.Generic;

using Aqua.FPSController;
using Aqua.SocketSystem;

using UnityEngine;

namespace Aqua.SceneController
{
    public class ScenarioSceneBuilder : SceneBuilder
    {

        protected ScenarioTask[] _tasks;

        private MulticonnectionSocket<ScenarioTask?, ScenarioTask?> _firstFailedTaskSocket;

        public IOutputSocket<ScenarioTask?> FirstFailedTaskSocket => _firstFailedTaskSocket;

        [SerializeField]
        protected PlayerModel _playerModel;

        public IReadOnlyList<ScenarioTask> Tasks => _tasks;

        protected override void SubInit ()
        {
            _firstFailedTaskSocket = new();
            if (_playerModel == null)
                _playerModel = FindFirstObjectByType<PlayerModel>();
        }

        protected override IEnumerator BuildScene ()
        {
            _stateScoket.TrySetValue(BuilderState.Building);
            _buildingPercentSocket.TrySetValue(0);

            _playerModel.ForceInit();
            _playerModel.State = PlayerControllerState.None;

            yield return null;

            _stateScoket.TrySetValue(BuilderState.BuildingEnded);
            _buildingPercentSocket.TrySetValue(1);
        }

        protected override IEnumerator StartScene ()
        {
            _stateScoket.TrySetValue(BuilderState.Starting);

            _playerModel.State = PlayerControllerState.MovementInput;

            _stateScoket.TrySetValue(BuilderState.StartingEnded);

            _startingCoroutine = null;

            yield break;
        }

        public override void DestroyScene ()
        {
            _playerModel.State = PlayerControllerState.None;
        }
    }
}
