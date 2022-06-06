using System;
using System.IO;

using UnityEditor;
using UnityEngine;

using Facebook.Unity.Settings;

using TalusBackendData.Editor.Models;

namespace TalusSettings.Editor.Definitons
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

            if (string.IsNullOrEmpty(app.fb_app_id))
            {
                Debug.LogWarning("[TalusSettings-Package] Facebook App_ID is empty!");
            }
        }

        public static void UpdateElephantAsset(AppModel app)
        {
            ElephantSettings elephantSettings = Resources.Load<ElephantSettings>(TalusSettingsDefinitions.ElephantAssetName);
            if (elephantSettings == null)
            {
                Debug.LogError("[TalusSettings-Package] Elephant Settings can not found!");
                return;
            }

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

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void CreateBackendAssets()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) { return; }

            EditorApplication.delayCall += () =>
            {
                CopyElephantScene();
                CreateFolderIfNotExists(TalusSettingsDefinitions.KeysFolder, TalusSettingsDefinitions.BasePath);
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

            string fullPath = GetKeyPath($"{ TalusSettingsDefinitions.FacebookAssetName}.asset");
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<FacebookSettings>(), fullPath);
            SaveAssets();

            Debug.Log($"[TalusSettings-Package] {fullPath} created!");
        }

        private static void CreateElephantAsset()
        {
            ElephantSettings settings = Resources.Load<ElephantSettings>(TalusSettingsDefinitions.ElephantAssetName);
            if (settings != null) { return; }

            string fullPath = GetKeyPath($"{TalusSettingsDefinitions.ElephantAssetName}.asset");
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<ElephantSettings>(), AssetDatabase.GenerateUniqueAssetPath(fullPath));
            SaveAssets();

            Debug.Log($"[TalusSettings-Package] {fullPath} created!");
        }

        private static string GetKeyPath(string asset)
        {
            return Path.Combine(Path.Combine(TalusSettingsDefinitions.BasePath, TalusSettingsDefinitions.KeysFolder), asset);
        }

        private static void SaveAssets()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
