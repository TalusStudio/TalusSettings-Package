using System.IO;

using TalusBackendData.Editor;

namespace TalusSettings.Editor.Definitons
{
    /// <summary>
    ///     Path settings to work with template project.
    /// </summary>
    public static class SettingsDefinitions
    {
        public const string BasePath = "Assets/";

        // base paths.
        public static string SOPath => Path.Combine(BasePath, "ScriptableObjects/");
        public static string KeysPath => Path.Combine(BasePath, "Resources/");
        public static string GetKeyPath(string asset) => Path.Combine(KeysPath, asset);

        // sdk assets.
        public static readonly string ElephantAssetName = "ElephantSettings";
        public static readonly string FacebookAssetName = "FacebookSettings";

        // elephant package paths.
        private static readonly string ElephantPackagePath = $"Packages/{BackendSettingsHolder.instance.Packages["talus-elephant"]}/";
        public static string ElephantPackageScenePath => Path.Combine(ElephantPackagePath, "elephant_scene.unity");

        // scene paths.
        public static string PersistentScenesPath => Path.Combine(BasePath, "Scenes/Template_Persistent/");
        public static string ForwarderScenePath => Path.Combine(PersistentScenesPath, "Scene_Forwarder.unity");
        public static string ElephantScenePath => Path.Combine(PersistentScenesPath, "Scene_Elephant.unity");
    }
}
