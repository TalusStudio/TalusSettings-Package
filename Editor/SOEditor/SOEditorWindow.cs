using UnityEditor;
using UnityEngine;

using Sirenix.OdinInspector.Editor;

using TalusFramework.Base;
using TalusFramework.Managers.Interfaces;
using TalusFramework.Collections.Interfaces;
using TalusFramework.Events.Interfaces;

using TalusSettings.Editor.Definitions;

namespace TalusSettings.Editor.SOEditor
{
    internal class SOEditorWindow : OdinMenuEditorWindow
    {
        [MenuItem("TalusKit/SO Editor %m", priority = 21)]
        private static void OpenWindow()
        {
            var window = GetWindow<SOEditorWindow>();
            window.titleContent = new GUIContent("SO Editor");
            window.minSize = new Vector2(800, 600);
            window.Show();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var settingsHolder = ProjectSettingsHolder.instance;

            var tree = new OdinMenuTree(false);
            tree.Config.DrawSearchToolbar = true;
            tree.AddAllAssetsAtPath("# Managers", settingsHolder.SOPath, typeof(IInitable), true, true)
                .AddThumbnailIcons()
                .SortMenuItemsByName();
            tree.AddAllAssetsAtPath("# Collections", settingsHolder.SOPath, typeof(ICollection), true, true)
                .AddThumbnailIcons()
                .SortMenuItemsByName();
            tree.AddAllAssetsAtPath("# Events", settingsHolder.SOPath, typeof(IBaseEvent), true, true)
                .AddThumbnailIcons()
                .SortMenuItemsByName();
            tree.AddAllAssetsAtPath(" # Variables", settingsHolder.SOPath, typeof(BaseValue), true, true)
                .AddThumbnailIcons()
                .SortMenuItemsByName();

#if ENABLE_BACKEND
            tree.AddAssetAtPath(
                "# Backend/Facebook Settings",
                settingsHolder.GetKeyPath($"{settingsHolder.FacebookAssetName}.asset"),
                typeof(ScriptableObject)
            ).AddThumbnailIcons();

            tree.AddAssetAtPath(
                "# Backend/GA Settings",
                settingsHolder.GetKeyPath($"{settingsHolder.GAAssetName}.asset"),
                typeof(ScriptableObject)
            ).AddThumbnailIcons();
#else
            tree.Add("# Backend (not active)/Facebook Settings", null);
            tree.Add("# Backend (not active)/GA Settings", null);
#endif

            return tree;
        }
    }
}
