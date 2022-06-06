using System.IO;

using UnityEditor;
using UnityEngine;

using Facebook.Unity.Settings;

using TalusBackendData.Editor.Models;

namespace TalusSettings.Editor
{
    public static class TalusSettingsProvider
    {
        private const string _KeysParentFolder = "Assets";
        private const string _KeysFolder = "Resources";

        [UnityEditor.Callbacks.DidReloadScripts]
        public static void CreateBackendAssets()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) { return; }

            EditorApplication.delayCall += () =>
            {
                CreateFolderIfNotExists(_KeysFolder, _KeysParentFolder);
                CreateFacebookAsset();
                CreateElephantAsset();
            };
        }

        public static void CreateFacebookAsset()
        {
            if (FacebookSettings.NullableInstance != null) { return; }

            string fullPath = GetAssetPath("FacebookSettings.asset");
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<FacebookSettings>(), fullPath);
            SaveAssets();

            Debug.Log("Facebook Settings created!");
        }

        public static void UpdateFacebookAsset(AppModel app)
        {
            if (FacebookSettings.NullableInstance == null)
            {
                Debug.LogError("Facebook settings can not found!");
                return;
            }

            FacebookSettings.SelectedAppIndex = 0;
            FacebookSettings.AppLabels[0] = app.app_name;
            FacebookSettings.AppIds[0] = app.fb_app_id;
            EditorUtility.SetDirty(FacebookSettings.Instance);
            SaveAssets();

            Debug.Log("Facebook SDK settings updated! fb_app_id: " + app.fb_app_id);
        }

        public static void CreateElephantAsset()
        {
            ElephantSettings settings = Resources.Load<ElephantSettings>("ElephantSettings");
            if (settings == null)
            {
                string fullPath = GetAssetPath("ElephantSettings.asset");
                AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<ElephantSettings>(), AssetDatabase.GenerateUniqueAssetPath(fullPath));
                SaveAssets();

                Debug.Log("Elephant Settings created!");
            }
        }

        public static void UpdateElephantAsset(AppModel app)
        {
            ElephantSettings elephantSettings = Resources.Load<ElephantSettings>("ElephantSettings");
            if (elephantSettings != null)
            {
                elephantSettings.GameID = app.elephant_id;
                elephantSettings.GameSecret = app.elephant_secret;
                EditorUtility.SetDirty(elephantSettings);
                SaveAssets();

                if (string.IsNullOrEmpty(app.elephant_id))
                {
                    Debug.LogWarning("Elephant Game_ID is empty!");
                }

                if (string.IsNullOrEmpty(app.elephant_secret))
                {
                    Debug.LogWarning("Elephant Game_Secret is empty!");
                }
            }
            else
            {
                Debug.LogError("Elephant Settings can not found!");
            }
        }

        public static void CreateFolderIfNotExists(string folderName, string parent)
        {
            if (AssetDatabase.IsValidFolder(Path.Combine(parent, folderName))) { return; }

            AssetDatabase.CreateFolder(parent, folderName);
        }

        public static string GetAssetPath(string asset)
        {
            return Path.Combine(Path.Combine(_KeysParentFolder, _KeysFolder), asset);
        }

        public static void SaveAssets()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
