using System.IO;

namespace TalusSettings.Editor
{
    public static class TalusSettingsDefinitions
    {
        // reference to assets path
        private static string BasePath => "Assets";

        // elephant package path
        public static readonly string ElephantPackagePath = "Packages/com.talus.taluselephant";
        public static string ElephantPackageScenePath => Path.Combine(ElephantPackagePath, "elephant_scene.unity");
        public static string ElephantPackageAssetPath => Path.Combine(ElephantPackagePath, "UI/Textures/Resources");

        // backend keys path
        public static readonly string KeysParentFolder = "Assets";
        public static readonly string KeysFolder = "Resources";

        // scenes path
        public static string PersistentScenesPath => Path.Combine(BasePath, "Scenes/Template_Persistent");
        public static string ForwarderScenePath => Path.Combine(PersistentScenesPath, "Scene_Forwarder.unity");
        public static string ElephantScenePath => Path.Combine(PersistentScenesPath, "elephant_scene.unity");
    }
}
