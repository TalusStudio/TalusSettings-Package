using UnityEditor;
using UnityEngine;

using Sirenix.OdinInspector.Editor;

using TalusFramework.Managers.Interfaces;
using TalusFramework.Collections.Interfaces;
using TalusFramework.Events;

using TalusSettings.Editor.Definitons;

namespace TalusSettings.Editor.ScriptableObjectEditor
{
    internal class ScriptableObjectEditorWindow : OdinMenuEditorWindow
    {
        [MenuItem("TalusKit/SO Editor %m", false, -9000)]
        private static void OpenWindow()
        {
            var window = GetWindow<ScriptableObjectEditorWindow>();
            window.titleContent = new GUIContent("SO Editor");
            window.minSize = new Vector2(800, 600);
            window.Show();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(false);
            tree.Config.DrawSearchToolbar = true;

            tree.AddAllAssetsAtPath("# Managers", SettingsDefinitions.SOPath, typeof(IInitializable), true, true)
                .AddThumbnailIcons()
                .SortMenuItemsByName();

            tree.AddAllAssetsAtPath("# Collections", SettingsDefinitions.SOPath, typeof(ICollection), true, true)
                .AddThumbnailIcons()
                .SortMenuItemsByName();

            tree.AddAllAssetsAtPath("# Events", SettingsDefinitions.SOPath, typeof(GameEvent), true, true)
                .AddThumbnailIcons()
                .SortMenuItemsByName();

#if ENABLE_BACKEND
            tree.AddAssetAtPath(
                "# Backend/Facebook Settings",
                SettingsDefinitions.GetKeyPath($"{SettingsDefinitions.FacebookAssetName}.asset"),
                typeof(UnityEngine.ScriptableObject)
            ).AddThumbnailIcons();

            tree.AddAssetAtPath(
                "# Backend/Elephant Settings",
                SettingsDefinitions.GetKeyPath($"{SettingsDefinitions.ElephantAssetName}.asset"),
                typeof(UnityEngine.ScriptableObject)
            ).AddThumbnailIcons();
#else
            tree.Add("# Backend (not active)/Facebook Settings", null);
            tree.Add("# Backend (not active)/Elephant Settings", null);
#endif

            return tree;
        }
    }
}
