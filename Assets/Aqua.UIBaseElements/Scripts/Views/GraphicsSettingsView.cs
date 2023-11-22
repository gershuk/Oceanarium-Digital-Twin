using System;
using System.Collections.Generic;
using System.Linq;

using Aqua.SocketSystem;
using Aqua.UnityEngineSettings;

using TMPro;

using UniRx;

using UnityEngine;
using UnityEngine.UI;

using QualityLevelType = Aqua.UnityEngineSettings.QualityLevel;

namespace Aqua.UIBaseElements
{
    public class GraphicsSettingsView : MonoBehaviour
    {
        private bool _isInit = false;

        [SerializeField]
        private Toggle _bloomToggle;

        [SerializeField]
        private Toggle _fogToggle;

        [SerializeField]
        private Toggle _vScyncToggle;

        [SerializeField]
        private Toggle _hdrToggle;

        [SerializeField]
        private TMP_Dropdown _resolutionDropdown;

        [SerializeField]
        private TMP_Dropdown _qualityLevelDropdown;

        [SerializeField]
        private TMP_Dropdown _antiAliasingDropdown;

        [SerializeField]
        private TMP_Dropdown _screenModeDropdown;

        [SerializeField]
        private Resolution[] _resolutions;

        #region ReactiveProperty
        private readonly ReactiveProperty<bool> _isBloom = new();
        private readonly ReactiveProperty<bool> _isFog = new();
        private readonly ReactiveProperty<bool> _isHdr = new();
        private readonly ReactiveProperty<bool> _isVSync = new();
        private readonly ReactiveProperty<QualityLevelType> _qualityLevel = new();
        private readonly ReactiveProperty<Vector2Int> _resolution = new();
        private readonly ReactiveProperty<AntiAliasingLevel> _antiAliasing = new();
        private readonly ReactiveProperty<ScreenMode> _screenMode = new();
        #endregion ReactiveProperty

        #region Sockets
        public MulticonnectionSocket<bool, bool> IsBloomSocket { get; private set; }
        public MulticonnectionSocket<bool, bool> IsFogSocket { get; private set; }
        public MulticonnectionSocket<bool, bool> IsHdrSocket { get; private set; }
        public MulticonnectionSocket<bool, bool> IsVSyncSocket { get; private set; }
        public MulticonnectionSocket<QualityLevelType, QualityLevelType> QualityLevelSocket { get; private set; }
        public MulticonnectionSocket<Vector2Int, Vector2Int> ResolutionSocket { get; private set; }
        public MulticonnectionSocket<AntiAliasingLevel, AntiAliasingLevel> AntiAliasingSocket { get; private set; }
        public MulticonnectionSocket<ScreenMode, ScreenMode> ScreenModeSocket { get; private set; }
        #endregion

        private void InitSockets ()
        {
            IsBloomSocket = new(_isBloom);
            IsFogSocket = new(_isFog);
            IsVSyncSocket = new(_isVSync);
            IsHdrSocket = new(_isHdr);
            ResolutionSocket = new(_resolution);
            QualityLevelSocket = new(_qualityLevel);
            AntiAliasingSocket = new(_antiAliasing);
            ScreenModeSocket = new(_screenMode);
        }

        //ToDo : add combined dispose
        private void LinkSocketsToUI ()
        {
            _bloomToggle.OnValueChangedAsObservable().Subscribe(value => _isBloom.Value = value).AddTo(this);
            _fogToggle.OnValueChangedAsObservable().Subscribe(value => _isFog.Value = value).AddTo(this);
            _vScyncToggle.OnValueChangedAsObservable().Subscribe(value => _isVSync.Value = value).AddTo(this);
            _hdrToggle.OnValueChangedAsObservable().Subscribe(value => _isHdr.Value = value).AddTo(this);
            _resolutionDropdown.onValueChanged.AsObservable().Subscribe(value =>
                                                 _resolution.Value = new(_resolutions[value].width, _resolutions[value].height))
                                                 .AddTo(this);
            _qualityLevelDropdown.onValueChanged.AsObservable().Subscribe(value => _qualityLevel.Value = (QualityLevelType) value).AddTo(this);
            _antiAliasingDropdown.onValueChanged.AsObservable().Subscribe(value => _antiAliasing.Value = (AntiAliasingLevel) value).AddTo(this);
            _screenModeDropdown.onValueChanged.AsObservable().Subscribe(value => _screenMode.Value = (ScreenMode) value).AddTo(this);
        }

        //ToDo : add link method with delegate parameter
        private void LinkUIToSockets ()
        {
            _isBloom.Subscribe(value => _bloomToggle.isOn = value).AddTo(this);
            _isFog.Subscribe(value => _fogToggle.isOn = value).AddTo(this);
            _isVSync.Subscribe(value => _vScyncToggle.isOn = value).AddTo(this);
            _isVSync.Subscribe(value => _vScyncToggle.isOn = value).AddTo(this);
            _isHdr.Subscribe(value => _hdrToggle.isOn = value).AddTo(this);
            _resolution.Subscribe(value => _resolutionDropdown.value = Array.IndexOf(_resolutions, new()
            {
                width = value.x,
                height = value.y,
                refreshRateRatio = Screen.currentResolution.refreshRateRatio
            })).AddTo(this);
            _qualityLevel.Subscribe(value => _qualityLevelDropdown.value = (int) value).AddTo(this);
            _antiAliasing.Subscribe(value => _antiAliasingDropdown.value = (int) value).AddTo(this);
            _screenMode.Subscribe(value => _screenModeDropdown.value = (int) value).AddTo(this);
        }

        public void InitQualityLevelDropdown ()
        {
            _qualityLevelDropdown.ClearOptions();
            _qualityLevelDropdown.AddOptions(new List<string>(3)
            {
                QualityLevelType.Low.ToString(),
                QualityLevelType.Middle.ToString(),
                QualityLevelType.High.ToString(),
            });
        }

        public void InitAntiAliasingDropdown (int levelsCount = 4)
        {
            if (levelsCount is < 1 or > 4)
                throw new ArgumentOutOfRangeException(nameof(levelsCount));

            _antiAliasingDropdown.ClearOptions();
            var levels = new List<string>(levelsCount);

            for (var i = 0; i < levelsCount; ++i)
                levels.Add(((AntiAliasingLevel) i).ToString());
            _antiAliasingDropdown.AddOptions(levels);
        }

        public void InitScreenModeDropdown ()
        {
            _screenModeDropdown.ClearOptions();
            _screenModeDropdown.AddOptions(new List<string>(3)
            {
                ScreenMode.ExclusiveFullScreen.ToString(),
                ScreenMode.FullScreenWindow.ToString(),
                ScreenMode.MaximizedWindow.ToString(),
                ScreenMode.Windowed.ToString(),
            });
        }

        public void Init()
        {
            if (_isInit)
                return;

            InitSockets();

            InitQualityLevelDropdown();
            InitAntiAliasingDropdown();
            InitScreenModeDropdown();

            SetResolutions(Screen.resolutions, Array.IndexOf(Screen.resolutions, Screen.currentResolution));

            LinkSocketsToUI();
            LinkUIToSockets();

            _isInit = true;
        }

        public void Start ()
        {
            Init();
        }

        public void SetResolutions (Resolution[] resolutions, int appliedndex = 0)
        {
            _resolutions = resolutions;
            _resolutionDropdown.options = 
                _resolutions.Select(res => new TMP_Dropdown.OptionData($"{res.width}x{res.height} Hz:{res.refreshRateRatio}"))
                .ToList();
            _resolutionDropdown.value = appliedndex;
        }
    }
}
