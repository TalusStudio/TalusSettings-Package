using UnityEditor;
using UnityEngine;

namespace TalusSettings.Editor.Definitions
{
    /// <summary>
    ///     ProjectSettingsHolder provides information about project folder structure.
    /// </summary>
    [FilePath("ProjectSettings/TalusProject.asset", FilePathAttribute.Location.ProjectFolder)]
    public class ProjectSettingsHolder : ScriptableSingleton<ProjectSettingsHolder>
    {
        // TalusProject.asset path
        public string Path => GetFilePath();

        // Unity3D - Project Layout Panel Path
        private const string _ProviderPath = "Talus Studio/Project Layout";
        public static string ProviderPath => _ProviderPath;

        private const string s_BasePath = "Assets/";
        public static string BasePath => s_BasePath;

        [SerializeField]
        private string _SOPath = $"{BasePath}Scriptables/";
        public string SOPath
        {
            get => _SOPath;
            set
            {
                _SOPath = value;
                SaveSettings();
            }
        }

        // Keys root path. (facebook settings and elephant settings)
        // Facebook Settings is not affected by the path change (Singleton Scriptable).
        [SerializeField]
        private string _KeysPath = $"{BasePath}Resources/";
        public string KeysPath
        {
            get => _KeysPath;
            set
            {
                _KeysPath = value;
                SaveSettings();
            }
        }

        // To copy SDK Scene from packages.
        [SerializeField]
        private string _SDKSceneSource = default;
        public string SDKSceneSource
        {
            get => _SDKSceneSource;
            set
            {
                _SDKSceneSource = value;
                SaveSettings();
            }
        }

        [SerializeField]
        private string _SDKScenePath = $"{BasePath}Scenes/Scene_SDK.unity";
        public string SDKScenePath
        {
            get => _SDKScenePath;
            set
            {
                _SDKScenePath = value;
                SaveSettings();
            }
        }

        [SerializeField]
        private string _GAAssetName = "GASettings";
        public string GAAssetName
        {
            get => _GAAssetName;
            set
            {
                _GAAssetName = value;
                SaveSettings();
            }
        }

        [SerializeField]
        private string _FacebookAssetName = "FacebookSettings";
        public string FacebookAssetName
        {
            get => _FacebookAssetName;
            set
            {
                _FacebookAssetName = value;
                SaveSettings();
            }
        }

        public string GetKeyPath(string assetName) => System.IO.Path.Combine(KeysPath, assetName);

        public void SaveSettings() => Save(true);

        private void OnEnable()
        {
            _SDKSceneSource = $"Packages/com.talus.taluspublishing/Scene_SDK.unity";
        }
    }
}