using Aqua.FPSController;
using Aqua.SocketSystem;

using UniRx;
using UniRx.Triggers;

using UnityEngine;

namespace Aqua.UIBaseElements
{
    public enum HUDState
    {
        None = 0,
        Info = 1,
        MenuPanel = 2,
        Win = 3,
        Lose = 4,
    }

    public sealed class PlayerHUDViewModel : MonoBehaviour
    {
        [SerializeField]
        private HUDAimViewModel _aimViewModel;

        [SerializeField]
        private EndGamePanelViewModel _endGamePanelViewModel;

        [SerializeField]
        private HUDSubpanelViewModel _hudSubpanelViewModel;

        private bool _isInited = false;

        [SerializeField]
        private ItemsPanelViewModel _itemsPanelViewModel;

        [SerializeField]
        private PlayerModel _model;

        private ConverterSocket<PlayerControllerState, HUDState> _stateSocket;

        [SerializeField]
        private TaskListViewModel _taskListViewModel;

        private void Awake () => ForceInit();

        private void OnDestroy () => UnsubscriveSockets();

        private void SubscriveSockets ()
        {
            _stateSocket.SubscribeTo(_model.StateSocket, static s => s switch
            {
                PlayerControllerState.None => HUDState.None,
                PlayerControllerState.MovementInput or PlayerControllerState.Cursor => HUDState.Info,
                PlayerControllerState.Menu => HUDState.MenuPanel,
                PlayerControllerState.Win => HUDState.Win,
                PlayerControllerState.Lose => HUDState.Lose,
                _ => throw new System.NotImplementedException(),
            });
            _stateSocket.ReadOnlyProperty.Subscribe(UpdateState);
        }

        private void UnsubscriveSockets () => _stateSocket.UnsubscribeFrom(_model.StateSocket);

        private void UpdateState (HUDState state)
        {
            switch (state)
            {
                case HUDState.None:
                    _taskListViewModel.gameObject.SetActive(false);
                    _itemsPanelViewModel.gameObject.SetActive(false);
                    _hudSubpanelViewModel.gameObject.SetActive(false);
                    _aimViewModel.gameObject.SetActive(false);
                    _endGamePanelViewModel.gameObject.SetActive(false);
                    break;

                case HUDState.Info:
                    _taskListViewModel.gameObject.SetActive(true);
                    _itemsPanelViewModel.gameObject.SetActive(true);
                    _hudSubpanelViewModel.gameObject.SetActive(false);
                    _aimViewModel.gameObject.SetActive(true);
                    _endGamePanelViewModel.gameObject.SetActive(false);
                    break;

                case HUDState.MenuPanel:
                    _taskListViewModel.gameObject.SetActive(false);
                    _itemsPanelViewModel.gameObject.SetActive(false);
                    _hudSubpanelViewModel.gameObject.SetActive(true);
                    _aimViewModel.gameObject.SetActive(false);
                    _endGamePanelViewModel.gameObject.SetActive(false);
                    break;

                case HUDState.Win:
                    _taskListViewModel.gameObject.SetActive(false);
                    _itemsPanelViewModel.gameObject.SetActive(false);
                    _hudSubpanelViewModel.gameObject.SetActive(false);
                    _aimViewModel.gameObject.SetActive(false);
                    _endGamePanelViewModel.gameObject.SetActive(true);
                    _endGamePanelViewModel.State = EndGamePanelState.Win;
                    break;

                case HUDState.Lose:
                    _taskListViewModel.gameObject.SetActive(false);
                    _itemsPanelViewModel.gameObject.SetActive(false);
                    _hudSubpanelViewModel.gameObject.SetActive(false);
                    _aimViewModel.gameObject.SetActive(false);
                    _endGamePanelViewModel.gameObject.SetActive(true);
                    _endGamePanelViewModel.State = EndGamePanelState.Lose;
                    break;
            }
        }

        public void ClosePanel () => _model.State = PlayerControllerState.MovementInput;

        public void ForceInit ()
        {
            if (_isInited)
                return;

            _model = FindObjectOfType<PlayerModel>();
            _model.ForceInit();
            _model.OnDestroyAsObservable().Subscribe(u => Destroy(gameObject)).AddTo(this);

            _itemsPanelViewModel.ForceInit();
            _itemsPanelViewModel.Model = _model.Inventory;

            _aimViewModel.ForceInit();
            _aimViewModel.InfoSocket.SubscribeTo(_model.ObjectScaner.ObservedObjectSocket);

            _hudSubpanelViewModel.ForceInit();
            _hudSubpanelViewModel.CloseHUDSubpanel = ClosePanel;

            _endGamePanelViewModel.ForceInit();

            _stateSocket = new(HUDState.Info);

            SubscriveSockets();

            _isInited = true;
        }

        public void SetTaskList (TaskListModel model) => _taskListViewModel.Model = model;
    }
}