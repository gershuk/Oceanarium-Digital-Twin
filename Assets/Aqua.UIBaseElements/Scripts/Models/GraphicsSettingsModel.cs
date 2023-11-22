#nullable enable

using System;
using System.IO;

using Aqua.SocketSystem;
using Aqua.UnityEngineSettings;

using UniRx;

using UnityEngine;

using QualityLevel = Aqua.UnityEngineSettings.QualityLevel;

namespace Aqua.UIBaseElements
{
    [Serializable]
    public struct GraphicSettingsData
    {
        [SerializeField]
        public AntiAliasingLevel AntiAliasing;

        [SerializeField]
        public bool IsBloom;

        [SerializeField]
        public bool IsFog;

        [SerializeField]
        public bool IsHdr;

        [SerializeField]
        public bool IsVSync;

        [SerializeField]
        public QualityLevel QualityLevel;

        [SerializeField]
        public Vector2Int Resolution;

        [SerializeField]
        public ScreenMode ScreenMode;

        public GraphicSettingsData (bool isBloom,
                                    bool isFog,
                                    bool isVSync,
                                    bool isHdr,
                                    Vector2Int resolution,
                                    QualityLevel qualityLevel,
                                    AntiAliasingLevel antiAliasing,
                                    ScreenMode screenMode)
        {
            IsBloom = isBloom;
            IsFog = isFog;
            IsVSync = isVSync;
            IsHdr = isHdr;
            Resolution = resolution;
            QualityLevel = qualityLevel;
            AntiAliasing = antiAliasing;
            ScreenMode = screenMode;
        }
    }

    public sealed class GraphicsSettingsModel
    {
        public const string DefaultFileName = @"graphics_settings.json";
        public const string DefaultFilePath = @".\settings\";
        public const string DefaultFullName = DefaultFilePath + DefaultFileName;

        public static GraphicsSettingsModel Default { get; private set; }

        public string? FilePath { get; private set; }

        #region ReactiveProperty
        private readonly ReactiveProperty<bool> _isBloom;
        private readonly ReactiveProperty<bool> _isFog;
        private readonly ReactiveProperty<bool> _isHdr;
        private readonly ReactiveProperty<bool> _isVSync;
        private readonly ReactiveProperty<QualityLevel> _qualityLevel;
        private readonly ReactiveProperty<Vector2Int> _resolution;
        private readonly ReactiveProperty<AntiAliasingLevel> _antiAliasing;
        private readonly ReactiveProperty<ScreenMode> _screenMode;
        #endregion ReactiveProperty

        #region Sockets
        public MulticonnectionSocket<bool, bool> IsBloomSocket { get; }
        public MulticonnectionSocket<bool, bool> IsFogSocket { get; }
        public MulticonnectionSocket<bool, bool> IsHdrSocket { get; }
        public MulticonnectionSocket<bool, bool> IsVSyncSocket { get; }
        public MulticonnectionSocket<QualityLevel,QualityLevel> QualityLevelSocket { get; }
        public MulticonnectionSocket<Vector2Int,Vector2Int> ResolutionSocket { get; }
        public MulticonnectionSocket<AntiAliasingLevel, AntiAliasingLevel> AntiAliasingSocket { get; }
        public MulticonnectionSocket<ScreenMode, ScreenMode> ScreenModeSocket { get; }
        #endregion

        private static void CreateDefaultConfig()
        {
            if (!Directory.Exists(DefaultFilePath))
                Directory.CreateDirectory(DefaultFilePath);

            var defaultData = new GraphicSettingsData()
            {
                Resolution = new(Screen.currentResolution.width, Screen.currentResolution.height),
                ScreenMode = ScreenMode.ExclusiveFullScreen
            };

            using var file = File.Create(DefaultFullName);
            using var writer = new StreamWriter(file);
            writer.WriteLine(JsonUtility.ToJson(defaultData));
        }

        static GraphicsSettingsModel ()
        {
            if (!File.Exists(DefaultFullName))
                CreateDefaultConfig();

            Default = new();
        }

        public GraphicsSettingsModel (GraphicSettingsData initData)
        {
            _isBloom = new(initData.IsBloom);
            _isFog = new(initData.IsFog);
            _isVSync = new(initData.IsVSync);
            _isHdr = new(initData.IsHdr);
            _resolution = new(initData.Resolution);
            _qualityLevel = new(initData.QualityLevel);
            _antiAliasing = new(initData.AntiAliasing);
            _screenMode = new(initData.ScreenMode);

            FilePath = null;

            IsBloomSocket = new(_isBloom);
            IsFogSocket = new(_isFog);
            IsVSyncSocket = new(_isVSync);
            IsHdrSocket= new(_isHdr);
            ResolutionSocket = new(_resolution);
            QualityLevelSocket = new(_qualityLevel);
            AntiAliasingSocket = new(_antiAliasing);
            ScreenModeSocket = new(_screenMode);
        }

        public GraphicsSettingsModel (string path = DefaultFullName) : this(GetDataFromJson(path))
        {
            FilePath = path;
        }

        private static GraphicSettingsData GetDataFromJson (string path) =>
            JsonUtility.FromJson<GraphicSettingsData>(File.ReadAllText(path));

        public void AssignFile (string path) => FilePath = path;

        public GraphicSettingsData GetData () => new(_isBloom.Value,
                                                      _isFog.Value,
                                                      _isVSync.Value,
                                                      _isHdr.Value,
                                                      _resolution.Value,
                                                      _qualityLevel.Value,
                                                      _antiAliasing.Value,
                                                      _screenMode.Value);

        public void SaveDataToAssignedFile () =>
            File.WriteAllText(FilePath ?? throw new FileNotAssignedException(), JsonUtility.ToJson(GetData()));

        public void SetData (GraphicSettingsData data)
        {
            _isBloom.Value = data.IsBloom;
            _isFog.Value = data.IsFog;
            _isVSync.Value = data.IsVSync;
            _isHdr.Value = data.IsHdr;
            _resolution.Value = data.Resolution;
            _qualityLevel.Value = data.QualityLevel;
            _antiAliasing.Value = data.AntiAliasing;
            _screenMode.Value = data.ScreenMode;
        }

        public void ReloadDataFromAssignedFile()
        {
            SetData(GetDataFromJson(FilePath));
        }
    }

    public class FileNotAssignedException : Exception 
    { 

    }
}