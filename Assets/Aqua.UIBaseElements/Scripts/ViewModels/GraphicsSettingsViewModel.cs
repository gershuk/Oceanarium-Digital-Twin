using Aqua.UnityEngineSettings;

using UniRx;

using UnityEngine;

namespace Aqua.UIBaseElements
{
    public sealed class GraphicsSettingsViewModel : MonoBehaviour
    {
        [SerializeField]
        private UnityGraphicsSettingsHelper _helper;

        [SerializeField]
        private GraphicsSettingsView _view;

        public GraphicsSettingsModel Model { get; private set; }

        private void Start ()
        {
            Model = new();

            if (_helper == null)
                _helper = FindAnyObjectByType<UnityGraphicsSettingsHelper>();
            _helper.ForceInit();

            if (_view == null)
                _view = GetComponent<GraphicsSettingsView>();
            _view.Init();

            // ToDo : Find how calc max count
            _view.InitAntiAliasingDropdown(3);

            SubscribeHelperToModel();
            SubscribeViewToModel();
            SubscribeModelToView();
        }

        private void SubscribeHelperToModel ()
        {
            Model.IsBloomSocket.ReadOnlyProperty.Subscribe(value => _helper.IsBloom = value).AddTo(this);
            Model.IsFogSocket.ReadOnlyProperty.Subscribe(value => _helper.IsFog = value).AddTo(this);
            Model.IsHdrSocket.ReadOnlyProperty.Subscribe(value => _helper.IsHdr = value).AddTo(this);
            Model.IsVSyncSocket.ReadOnlyProperty.Subscribe(value => _helper.IsVSync = value).AddTo(this);
            Model.QualityLevelSocket.ReadOnlyProperty.Subscribe(value => _helper.QualityLevel = value).AddTo(this);
            Model.ResolutionSocket.ReadOnlyProperty.Subscribe(value => _helper.Resolution = value).AddTo(this);
            Model.AntiAliasingSocket.ReadOnlyProperty.Subscribe(value => _helper.AntiAliasing = value).AddTo(this);
            Model.ScreenModeSocket.ReadOnlyProperty.Subscribe(value => _helper.ScreenMode = value).AddTo(this);
        }

        private void SubscribeModelToView ()
        {
            Model.IsBloomSocket.SubscribeTo(_view.IsBloomSocket);
            Model.IsFogSocket.SubscribeTo(_view.IsFogSocket);
            Model.IsHdrSocket.SubscribeTo(_view.IsHdrSocket);
            Model.IsVSyncSocket.SubscribeTo(_view.IsVSyncSocket);
            Model.QualityLevelSocket.SubscribeTo(_view.QualityLevelSocket);
            Model.ResolutionSocket.SubscribeTo(_view.ResolutionSocket);
            Model.AntiAliasingSocket.SubscribeTo(_view.AntiAliasingSocket);
            Model.ScreenModeSocket.SubscribeTo(_view.ScreenModeSocket);
        }

        private void SubscribeViewToModel ()
        {
            _view.IsBloomSocket.SubscribeTo(Model.IsBloomSocket);
            _view.IsFogSocket.SubscribeTo(Model.IsFogSocket);
            _view.IsHdrSocket.SubscribeTo(Model.IsHdrSocket);
            _view.IsVSyncSocket.SubscribeTo(Model.IsVSyncSocket);
            _view.QualityLevelSocket.SubscribeTo(Model.QualityLevelSocket);
            _view.ResolutionSocket.SubscribeTo(Model.ResolutionSocket);
            _view.AntiAliasingSocket.SubscribeTo(Model.AntiAliasingSocket);
            _view.ScreenModeSocket.SubscribeTo(Model.ScreenModeSocket);
        }
    }
}