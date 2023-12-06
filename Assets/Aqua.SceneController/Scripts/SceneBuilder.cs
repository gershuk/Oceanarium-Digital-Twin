using System;
using System.Collections;
using System.Collections.Generic;

using Aqua.FPSController;
using Aqua.SocketSystem;

using UnityEngine;

namespace Aqua.SceneController
{
    [Serializable]
    public enum BuilderState
    {
        Idle = 0,
        Building = 1,
        BuildingEnded = 2,
        Starting = 3,
        StartingEnded = 4,
    }

    public class SceneBuilder : MonoBehaviour
    {
        protected Coroutine _buildingCoroutine;
        protected Coroutine _startingCoroutine;

        protected MulticonnectionSocket<float, float> _buildingPercentSocket;

        protected MulticonnectionSocket<BuilderState, BuilderState> _stateScoket;

        public IOutputSocket<float> BuildingPercentSocket => _buildingPercentSocket;

        public IOutputSocket<BuilderState> StateSocket => _stateScoket;

        protected bool _isInited = false;

        protected void Awake ()
        {
            ForceInit();
        }

        protected virtual void SubInit ()
        {

        }

        public void ForceInit ()
        {
            if (_isInited)
                return;

            _buildingPercentSocket = new(0);
            _stateScoket = new(BuilderState.Idle);

            _buildingCoroutine = null;
            _startingCoroutine = null;

            SubInit();

            _isInited = true;
        }

        protected virtual IEnumerator BuildScene ()
        {
            _stateScoket.TrySetValue(BuilderState.Building);
            _buildingPercentSocket.TrySetValue(0);
            yield return null;
            _stateScoket.TrySetValue(BuilderState.BuildingEnded);
            _buildingPercentSocket.TrySetValue(1);
        }

        [ContextMenu(nameof(StartBuildingCorutine))]
        public void StartBuildingCorutine ()
        {
            if (_stateScoket.GetValue() != BuilderState.Idle)
            {
                Debug.LogError($"Can't build scene. Required {nameof(BuilderState.Idle)} state.");
            }

            if (_buildingCoroutine != null)
            {
                Debug.LogError("Already building");
                return;
            }

            _buildingCoroutine = StartCoroutine(BuildScene());
        }

        protected virtual IEnumerator StartScene ()
        {
            _stateScoket.TrySetValue(BuilderState.Starting);

            _stateScoket.TrySetValue(BuilderState.StartingEnded);

            _startingCoroutine = null;

            yield break;
        }

        [ContextMenu(nameof(StartStartingCorutine))]
        public void StartStartingCorutine ()
        {
            if (_stateScoket.GetValue() != BuilderState.BuildingEnded)
            {
                Debug.LogError("Can't start scene. It is not built.");
                return;
            }

            if (_startingCoroutine != null)
            {
                Debug.LogError("Already starting");
                return;
            }

            _startingCoroutine = StartCoroutine(StartScene());
        }

        [ContextMenu(nameof(DestroyScene))]
        public virtual void DestroyScene ()
        {

        }
    }
}
