using System.Collections;
using System.Collections.Generic;

using Aqua.FPSController;

using UnityEngine;

namespace Aqua.SceneController
{
    public class ScenarioSceneBuilder : SceneBuilder
    {
        [SerializeField]
        private PlayerModel _playerModel;

        protected override void SubInit ()
        {
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
