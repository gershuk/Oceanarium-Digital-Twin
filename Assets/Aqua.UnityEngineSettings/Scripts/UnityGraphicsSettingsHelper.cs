#nullable enable

using System;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Aqua.UnityEngineSettings
{
    [Serializable]
    public enum AntiAliasingLevel
    {
        Zero = 0,
        Two = 1,
        Four = 2,
        Eight = 3,
    }

    [Serializable]
    public enum QualityLevel
    {
        Low = 0,
        Middle = 1,
        High = 2,
    }

    [Serializable]
    public enum ScreenMode
    {
        ExclusiveFullScreen = 0,
        FullScreenWindow = 1,
        MaximizedWindow = 2,
        Windowed = 3,
    }

    [RequireComponent(typeof(Volume))]
    public sealed class UnityGraphicsSettingsHelper : MonoBehaviour
    {
        private Bloom? _bloom;
        private bool _isInit = false;
        private VolumeProfile _profile;

        private UniversalRenderPipelineAsset _urpAsset;

        [SerializeField]
        private Volume _volume;

        public AntiAliasingLevel AntiAliasing
        {
            get => (AntiAliasingLevel) _urpAsset.msaaSampleCount;
            set => _urpAsset.msaaSampleCount = (int) value;
        }

        public bool IsBloom
        {
            get => _bloom.active;
            set => _bloom.active = value;
        }

        public bool IsFog
        {
            get => RenderSettings.fog;
            set => RenderSettings.fog = value;
        }

        public bool IsHdr
        {
            get => _urpAsset.supportsHDR;
            set => _urpAsset.supportsHDR = value;
        }

        public bool IsVSync
        {
            get => QualitySettings.vSyncCount switch
            {
                0 => false,
                1 => true,
                _ => throw new NotImplementedException(),
            };
            set => QualitySettings.vSyncCount = value ? 1 : 0;
        }

        public QualityLevel QualityLevel
        {
            get => (QualityLevel) QualitySettings.GetQualityLevel();
            set => QualitySettings.SetQualityLevel((int) value);
        }

        public Vector2Int Resolution
        {
            get => new(Screen.currentResolution.width, Screen.currentResolution.height);
            set => Screen.SetResolution(value.x, value.y, Screen.fullScreenMode);
        }

        public ScreenMode ScreenMode
        {
            get => (ScreenMode) Screen.fullScreenMode;
            set => Screen.fullScreenMode = (FullScreenMode) value;
        }

        private void Awake ()
        {
            if (FindObjectsOfType<UnityGraphicsSettingsHelper>().Length > 1)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            ForceInit();
        }

        public Resolution[] GetResolutions () => Screen.resolutions;

        public void ForceInit ()
        {
            if (_isInit)
                return;

            if (_volume == null)
                _volume = GetComponent<Volume>();
            _profile = _volume.sharedProfile;
            if (!_profile.TryGet(out _bloom))
                throw new NullReferenceException();

            _urpAsset = (UniversalRenderPipelineAsset) GraphicsSettings.defaultRenderPipeline;

            _isInit = true;
        }
    }
}