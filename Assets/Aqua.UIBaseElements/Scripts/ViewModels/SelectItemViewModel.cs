#nullable enable

using Aqua.Items;
using Aqua.SocketSystem;

using UnityEngine;

namespace Aqua.UIBaseElements
{
    public class SelectItemViewModel : MonoBehaviour
    {
        private readonly UniversalSocket<bool, bool> _isSelectedSocket = new();
        private bool _isInited = false;

        private IInfo _model;

        [SerializeField]
        private SelectFrameView _view;

        public bool IsSelected
        {
            get => _isSelectedSocket.GetValue();
            set
            {
                if (IsSelected == value)
                    return;
                if (!_isSelectedSocket.TrySetValue(value))
                {
                    Debug.LogError("Can't set value when socket has input connection");
                }
            }
        }

        public IOutputSocket<bool> IsSelectedSocket => _isSelectedSocket;

        public IInfo Model
        {
            get => _model;
            set
            {
                if (_model == value)
                    return;

                if (_model != null)
                    UnlinkVeiwToViewModel();

                _model = value;
                LinkVeiwToViewModel();
            }
        }

        private void Awake () => ForceInit();

        private void LinkVeiwToViewModel ()
        {
            _view.IsSelectedSocket.SubscribeTo(IsSelectedSocket);
            _view.TextSocket.SubscribeTo(_model.NameSocket);
            _view.InnerImageSpriteSocket.SubscribeTo(_model.SpriteSocket);
        }

        private void UnlinkVeiwToViewModel ()
        {
            _view.IsSelectedSocket.UnsubscribeFrom(IsSelectedSocket);
            _view.TextSocket.UnsubscribeFrom(_model.NameSocket);
            _view.InnerImageSpriteSocket.UnsubscribeFrom(_model.SpriteSocket);
        }

        public void ForceInit ()
        {
            if (_isInited)
                return;
            if (_view != null)
                _view = GetComponent<SelectFrameView>();
            Model = EmptyInfo.Instance;
            _isInited = true;
        }
    }
}