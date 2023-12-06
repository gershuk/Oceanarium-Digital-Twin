using System.Collections;

using UnityEngine;

namespace Aqua.SceneController
{
    public class UISceneBuilder : SceneBuilder
    {
        [SerializeField]
        private GameObject _menuRoot;

        protected override void SubInit ()
        {
            base.SubInit();

            _menuRoot.SetActive(false);
        }

        protected override IEnumerator BuildScene ()
        {
            _stateScoket.TrySetValue(BuilderState.Building);
            _buildingPercentSocket.TrySetValue(0);

            _menuRoot.SetActive(false);

            yield return null;

            _stateScoket.TrySetValue(BuilderState.BuildingEnded);
            _buildingPercentSocket.TrySetValue(1);
        }

        protected override IEnumerator StartScene ()
        {
            _stateScoket.TrySetValue(BuilderState.Starting);

            _menuRoot.SetActive(true);

            _stateScoket.TrySetValue(BuilderState.StartingEnded);

            _startingCoroutine = null;

            yield break;
        }

        public override void DestroyScene ()
        {
            base.DestroyScene();
            Destroy(_menuRoot);
        }
    }
}
