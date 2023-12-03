using Aqua.FPSController;

using UnityEngine;

namespace Aqua.UIBaseElements
{
    public class PlayerHUDViewModel : MonoBehaviour
    {
        private bool _isInited = false;

        [SerializeField]
        private TaskListViewModel _taskListViewModel;

        [SerializeField]
        private ItemsPanelViewModel _itemsPanelViewModel;

        [SerializeField]
        private HUDAimViewModel _aimViewModel;

        [SerializeField]
        private PlayerModel _model;

        public void ForceInit ()
        {
            if (_isInited)
                return;

            _model = FindObjectOfType<PlayerModel>();

            _itemsPanelViewModel.ForceInit();
            _itemsPanelViewModel.Model = _model.Inventory;

            _aimViewModel.ForceInit();
            _aimViewModel.InfoSocket.SubscribeTo(_model.Inventory.CurrentObservedObjectSocket);

            _isInited = true;
        }

        private void Awake ()
        {
            ForceInit();
        }
    }
}
