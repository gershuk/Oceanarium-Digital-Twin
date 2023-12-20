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
        protected MulticonnectionSocket<ScenarioTask?, ScenarioTask?> _firstFailedTaskSocket;

        [SerializeField]
        protected PlayerModel _playerModel;

        protected ScenarioTask[] _tasks;
        public IOutputSocket<ScenarioTask?> FirstFailedTaskSocket => _firstFailedTaskSocket;
        public IReadOnlyList<ScenarioTask> Tasks => _tasks;

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

        protected override void SubInit ()
        {
            _firstFailedTaskSocket = new();
            if (_playerModel == null)
                _playerModel = FindFirstObjectByType<PlayerModel>();
        }

        public override void DestroyScene ()
        {
            _playerModel.State = PlayerControllerState.None;
            Destroy(_playerModel.gameObject);
        }
    }
}