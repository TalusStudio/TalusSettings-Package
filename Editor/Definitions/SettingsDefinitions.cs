using System.IO;

namespace TalusSettings.Editor.Definitons
{
    /// <summary>
    ///     Path settings to work with template project.
    /// </summary>
    public static class SettingsDefinitions
    {
        public const string BasePath = @"Assets/";
        public const string SOPathName = "ScriptableObjects";
        public const string KeysPathName = "Resources";

        // base paths.
        public static string SOPath => Path.Combine(BasePath, $"{SOPathName}/");
        public static string KeysPath => Path.Combine(BasePath, $"{KeysPathName}/");
        public static string GetKeyPath(string asset) => Path.Combine(KeysPath, asset);

        // sdk assets.
        public static readonly string ElephantAssetName = "ElephantSettings";
        public static readonly string FacebookAssetName = "FacebookSettings";

        // elephant package paths.
        public static readonly string ElephantPackagePath = @"Packages/com.talus.taluselephant/";
        public static string ElephantPackageScenePath => Path.Combine(ElephantPackagePath, "elephant_scene.unity");

        // scene paths.
        public static string PersistentScenesPath => Path.Combine(BasePath, @"Scenes/Template_Persistent/");
        public static string ForwarderScenePath => Path.Combine(PersistentScenesPath, "Scene_Forwarder.unity");
        public static string ElephantScenePath => Path.Combine(PersistentScenesPath, "elephant_scene.unity");
    }
}
