using System.IO;

namespace TalusSettings.Editor
{
    public static class TalusSettingsDefinitions
    {
        // reference to assets path
        public static string BasePath => "Assets";

#if ENABLE_BACKEND
        // backend keys path
        public static readonly string KeysFolder = "Resources";
        public static readonly string ElephantAssetName = "ElephantSettings";
        public static readonly string FacebookAssetName = "FacebookSettings";

        // elephant package path
        public static readonly string ElephantPackagePath = "Packages/com.talus.taluselephant";
        public static string ElephantPackageScenePath => Path.Combine(ElephantPackagePath, "elephant_scene.unity");
        public static string ElephantPackageAssetPath => Path.Combine(ElephantPackagePath, "UI/Textures/Resources");
#endif

        // scenes path
        public static string PersistentScenesPath => Path.Combine(BasePath, "Scenes/Template_Persistent");
        public static string ForwarderScenePath => Path.Combine(PersistentScenesPath, "Scene_Forwarder.unity");

#if ENABLE_BACKEND
        public static string ElephantScenePath => Path.Combine(PersistentScenesPath, "elephant_scene.unity");
#endif
    }
}
