using System.Collections;
using System.Collections.Generic;

using Aqua.UnityEngineSettings;

using UniRx;

using UnityEngine;

namespace Aqua.UIBaseElements
{
    public sealed class GraphicsSettingsViewModel : MonoBehaviour
    {
        [SerializeField]
        private UnityGraphicsSettingsHelper _helper;

        private GraphicsSettingsModel _model;

        [SerializeField]
        private GraphicsSettingsView _view;

        public GraphicsSettingsModel Model => _model;

        private void SubscribeHelperToModel ()
        {
            _model.IsBloomSocket.ReadOnlyProperty.Subscribe(value => _helper.IsBloom = value).AddTo(this);
            _model.IsFogSocket.ReadOnlyProperty.Subscribe(value => _helper.IsFog = value).AddTo(this);
            _model.IsHdrSocket.ReadOnlyProperty.Subscribe(value => _helper.IsHdr = value).AddTo(this);
            _model.IsVSyncSocket.ReadOnlyProperty.Subscribe(value => _helper.IsVSync = value).AddTo(this);
            _model.QualityLevelSocket.ReadOnlyProperty.Subscribe(value => _helper.QualityLevel = value).AddTo(this);
            _model.ResolutionSocket.ReadOnlyProperty.Subscribe(value => _helper.Resolution = value).AddTo(this);
            _model.AntiAliasingSocket.ReadOnlyProperty.Subscribe(value => _helper.AntiAliasing = value).AddTo(this);
            _model.ScreenModeSocket.ReadOnlyProperty.Subscribe(value => _helper.ScreenMode = value).AddTo(this);
        }

        private void SubscribeModelToView ()
        {
            _model.IsBloomSocket.SubscribeTo(_view.IsBloomSocket);
            _model.IsFogSocket.SubscribeTo(_view.IsFogSocket);
            _model.IsHdrSocket.SubscribeTo(_view.IsHdrSocket);
            _model.IsVSyncSocket.SubscribeTo(_view.IsVSyncSocket);
            _model.QualityLevelSocket.SubscribeTo(_view.QualityLevelSocket);
            _model.ResolutionSocket.SubscribeTo(_view.ResolutionSocket);
            _model.AntiAliasingSocket.SubscribeTo(_view.AntiAliasingSocket);
            _model.ScreenModeSocket.SubscribeTo(_view.ScreenModeSocket);
        }
        
        private void SubscribeViewToModel ()
        {
            _view.IsBloomSocket.SubscribeTo(_model.IsBloomSocket);
            _view.IsFogSocket.SubscribeTo(_model.IsFogSocket);
            _view.IsHdrSocket.SubscribeTo(_model.IsHdrSocket);
            _view.IsVSyncSocket.SubscribeTo(_model.IsVSyncSocket);
            _view.QualityLevelSocket.SubscribeTo(_model.QualityLevelSocket);
            _view.ResolutionSocket.SubscribeTo(_model.ResolutionSocket);
            _view.AntiAliasingSocket.SubscribeTo(_model.AntiAliasingSocket);
            _view.ScreenModeSocket.SubscribeTo(_model.ScreenModeSocket);
        }

        private void Start ()
        {
            _model = new();

            if (_helper == null)
                _helper = FindAnyObjectByType<UnityGraphicsSettingsHelper>();
            _helper.InitObject();

            if (_view == null)
                _view = GetComponent<GraphicsSettingsView>();
            _view.Init();

            // ToDo : Find how calc max count
            _view.InitAntiAliasingDropdown(3);

            SubscribeHelperToModel();
            SubscribeViewToModel();
            SubscribeModelToView();
        }
    }
}
