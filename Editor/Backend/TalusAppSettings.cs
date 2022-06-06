using System;
using System.IO;

using UnityEditor;
using UnityEngine;

using Facebook.Unity.Settings;

using TalusBackendData.Editor.Models;

namespace TalusSettings.Editor.Backend
{
    public static class TalusAppSettings
    {
        public static void UpdateFacebookAsset(AppModel app)
        {
            if (FacebookSettings.NullableInstance == null)
            {
                Debug.LogError("[TalusSettings-Package] Facebook settings can not found!");
                return;
            }

            FacebookSettings.SelectedAppIndex = 0;
            FacebookSettings.AppLabels[0] = app.app_name;
            FacebookSettings.AppIds[0] = app.fb_app_id;
            EditorUtility.SetDirty(FacebookSettings.Instance);
            SaveAssets();

            Debug.Log($"[TalusSettings-Package] Facebook SDK settings updated! fb_app_id: {app.fb_app_id}");
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
                    Debug.LogWarning("[TalusSettings-Package] Elephant Game_ID is empty!");
                }

                if (string.IsNullOrEmpty(app.elephant_secret))
                {
                    Debug.LogWarning("[TalusSettings-Package] Elephant Game_Secret is empty!");
                }
            }
            else
            {
                Debug.LogError("[TalusSettings-Package] Elephant Settings can not found!");
            }
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void CreateBackendAssets()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) { return; }

            EditorApplication.delayCall += () =>
            {
                CopyElephantScene();
                CreateFolderIfNotExists(TalusSettingsDefinitions.KeysFolder, TalusSettingsDefinitions.KeysParentFolder);
                CreateFacebookAsset();
                CreateElephantAsset();
            };
        }

        private static void CopyElephantScene()
        {
            try
            {
                FileUtil.CopyFileOrDirectory(TalusSettingsDefinitions.ElephantPackageScenePath, TalusSettingsDefinitions.ElephantScenePath);

                SaveAssets();
                Debug.Log($"[TalusSettings-Package] elephant_scene copied to: {TalusSettingsDefinitions.ElephantScenePath}");
            }
            catch (Exception)
            {
                // ignore
            }
        }

        private static void CreateFolderIfNotExists(string folderName, string parent)
        {
            if (AssetDatabase.IsValidFolder(Path.Combine(parent, folderName))) { return; }

            AssetDatabase.CreateFolder(parent, folderName);
        }

        private static void CreateFacebookAsset()
        {
            if (FacebookSettings.NullableInstance != null) { return; }

            string fullPath = GetAssetPath("FacebookSettings.asset");
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<FacebookSettings>(), fullPath);
            SaveAssets();

            Debug.Log("[TalusSettings-Package] Facebook Settings created!");
        }

        private static void CreateElephantAsset()
        {
            ElephantSettings settings = Resources.Load<ElephantSettings>("ElephantSettings");
            if (settings == null)
            {
                string fullPath = GetAssetPath("ElephantSettings.asset");
                AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<ElephantSettings>(), AssetDatabase.GenerateUniqueAssetPath(fullPath));
                SaveAssets();

                Debug.Log("[TalusSettings-Package] Elephant Settings created!");
            }
        }

        private static string GetAssetPath(string asset)
        {
            return Path.Combine(Path.Combine(TalusSettingsDefinitions.KeysParentFolder, TalusSettingsDefinitions.KeysFolder), asset);
        }

        private static void SaveAssets()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
