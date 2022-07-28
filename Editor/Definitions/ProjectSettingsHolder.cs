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

        // To copy Elephant Scene from packages.
        [SerializeField]
        private string _ElephantSceneSource = default;
        public string ElephantSceneSource
        {
            get => _ElephantSceneSource;
            set
            {
                _ElephantSceneSource = value;
                SaveSettings();
            }
        }

        [SerializeField]
        private string _ElephantScenePath = $"{BasePath}Scenes/Template_Persistent/Scene_Elephant.unity";
        public string ElephantScenePath
        {
            get => _ElephantScenePath;
            set
            {
                _ElephantScenePath = value;
                SaveSettings();
            }
        }

        [SerializeField]
        private string _ForwarderScenePath = $"{BasePath}Scenes/Template_Persistent/Scene_Forwarder.unity";
        public string ForwarderScenePath
        {
            get => _ForwarderScenePath;
            set
            {
                _ForwarderScenePath = value;
                SaveSettings();
            }
        }

        [SerializeField]
        private string _ElephantAssetName = "ElephantSettings";
        public string ElephantAssetName
        {
            get => _ElephantAssetName;
            set
            {
                _ElephantAssetName = value;
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
            _ElephantSceneSource = $"Packages/com.talus.taluselephant/elephant_scene.unity";
        }
    }
}