using System;
using System.IO;

using UnityEditor;
using UnityEngine;

using Facebook.Unity.Settings;

using TalusBackendData.Editor.Models;

namespace TalusSettings.Editor.Definitons
{
    /// <summary>
    ///     Creates backend assets and paths after domain reloading.
    ///     'TalusSettingsWindow.cs' class uses this class to update project settings.
    /// </summary>
    public static class AppSettings
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

            if (string.IsNullOrEmpty(app.fb_app_id)) { Debug.LogWarning("[TalusSettings-Package] Fb_App_Id is empty!"); }
        }

        public static void UpdateElephantAsset(AppModel app)
        {
            ElephantSettings elephantSettings = Resources.Load<ElephantSettings>(SettingsDefinitions.ElephantAssetName);
            if (elephantSettings == null)
            {
                Debug.LogError("[TalusSettings-Package] Elephant Settings can not found!");
                return;
            }

            elephantSettings.GameID = app.elephant_id;
            elephantSettings.GameSecret = app.elephant_secret;
            EditorUtility.SetDirty(elephantSettings);
            SaveAssets();

            if (string.IsNullOrEmpty(app.elephant_id)) { Debug.LogWarning("[TalusSettings-Package] Elephant Game_ID is empty!"); }
            if (string.IsNullOrEmpty(app.elephant_secret)) { Debug.LogWarning("[TalusSettings-Package] Elephant Game_Secret is empty!"); }
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void CreateBackendAssets()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) { return; }

            EditorApplication.delayCall += () =>
            {
                CreateKeysFolder(SettingsDefinitions.KeysFolder, SettingsDefinitions.BasePath);
                CopyElephantScene();
                CreateFacebookAsset();
                CreateElephantAsset();
            };
        }

        private static void CreateKeysFolder(string folderName, string parent)
        {
            if (AssetDatabase.IsValidFolder(Path.Combine(parent, folderName))) { return; }

            AssetDatabase.CreateFolder(parent, folderName);
        }

        private static void CopyElephantScene()
        {
            try
            {
                FileUtil.CopyFileOrDirectory(SettingsDefinitions.ElephantPackageScenePath, SettingsDefinitions.ElephantScenePath);

                SaveAssets();
                Debug.Log($"[TalusSettings-Package] elephant_scene copied to: {SettingsDefinitions.ElephantScenePath}");
            }
            catch (Exception)
            {
                // ignore
            }
        }

        private static void CreateFacebookAsset()
        {
            if (FacebookSettings.NullableInstance != null) { return; }

            string fullPath = SettingsDefinitions.GetKeyPath($"{ SettingsDefinitions.FacebookAssetName}.asset");
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<FacebookSettings>(), fullPath);
            SaveAssets();

            Debug.Log($"[TalusSettings-Package] {fullPath} created!");
        }

        private static void CreateElephantAsset()
        {
            ElephantSettings settings = Resources.Load<ElephantSettings>(SettingsDefinitions.ElephantAssetName);
            if (settings != null) { return; }

            string fullPath = SettingsDefinitions.GetKeyPath($"{SettingsDefinitions.ElephantAssetName}.asset");
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<ElephantSettings>(), AssetDatabase.GenerateUniqueAssetPath(fullPath));
            SaveAssets();

            Debug.Log($"[TalusSettings-Package] {fullPath} created!");
        }

        private static void SaveAssets()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
