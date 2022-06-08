using TalusBackendData.Editor;

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
        /// <summary>
        ///     Unity3D - Project Layout Panel Path
        /// </summary>
        private const string _ProviderPath = "Talus Studio/Project Layout";
        public static string SettingsProviderPath => _ProviderPath;

        /// <summary>
        ///     Root path.
        /// </summary>
        private const string s_BasePath = "Assets/";
        public static string BasePath
        {
            get { return s_BasePath; }
        }

        [SerializeField]
        private string _SOPath = $"{BasePath}ScriptableObjects/";
        public string SOPath
        {
            get { return _SOPath; }
        }

        /// <summary>
        ///     Keys root path. (facebook settings and elephant settings)
        /// </summary>
        [SerializeField]
        private string _KeysPath = $"{BasePath}Resources/";
        public string KeysPath
        {
            get { return _KeysPath; }
        }

        /// <summary>
        ///     To copy elephant scene from packages.
        /// </summary>
        [SerializeField]
        private string _ElephantSceneSource = default;
        public string ElephantSceneSource
        {
            get { return _ElephantSceneSource; }
        }

        [SerializeField]
        private string _ElephantScenePath = $"{BasePath}Scenes/Template_Persistent/Scene_Elephant.unity";
        public string ElephantScenePath
        {
            get { return _ElephantScenePath; }
        }

        [SerializeField]
        private string _ForwarderScenePath = $"{BasePath}Scenes/Template_Persistent/Scene_Forwarder.unity";
        public string ForwarderScenePath
        {
            get { return _ForwarderScenePath; }
        }

        [SerializeField]
        private string _ElephantAssetName = "ElephantSettings";
        public string ElephantAssetName
        {
            get { return _ElephantAssetName; }
        }

        [SerializeField]
        private string _FacebookAssetName = "FacebookSettings";
        public string FacebookAssetName
        {
            get { return _FacebookAssetName; }
        }

        public string GetKeyPath(string assetName)
        {
            return System.IO.Path.Combine(KeysPath, assetName);
        }

        public string Path => GetFilePath();

        public void SaveSettings()
        {
            Save(true);
        }

        private void OnEnable()
        {
            _ElephantSceneSource = $"Packages/{BackendSettingsHolder.instance.Packages["talus-elephant"]}/elephant_scene.unity";
        }
    }
}