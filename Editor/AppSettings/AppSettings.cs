using System;
using System.IO;

using UnityEditor;
using UnityEngine;

using Facebook.Unity.Settings;

using TalusSettings.Editor.Definitions;

using TalusBackendData.Editor.Utility;
using TalusBackendData.Editor.Models;

namespace TalusSettings.Editor.AppSettings
{
    /// <summary>
    ///     Creates backend assets and paths after domain reloading.
    ///     'TalusSettingsWindow.cs' class uses this class to update project settings.
    /// </summary>
    public static class AppSettings
    {
        public static bool UpdateFacebookAsset(AppModel app)
        {
            if (FacebookSettings.NullableInstance == null)
            {
                Debug.LogError("[TalusSettings-Package] Facebook settings can not found!");
                return false;
            }

            FacebookSettings.SelectedAppIndex = 0;
            FacebookSettings.AppLabels[0] = app.app_name;
            FacebookSettings.AppIds[0] = app.fb_app_id;
            EditorUtility.SetDirty(FacebookSettings.Instance);
            SaveAssets();

            if (string.IsNullOrEmpty(app.fb_app_id)) { Debug.LogWarning("[TalusSettings-Package] Fb_App_Id is empty!"); }

            return true;
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void CreateBackendAssets()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) { return; }

            EditorApplication.delayCall += () =>
            {
                if (!CreateKeysFolder(ProjectSettingsHolder.instance.KeysPath))
                {
                    InfoBox.Show(
                        "Talus Backend",
                        $"{ProjectSettingsHolder.instance.KeysPath} couldn't created! Process is cancelling...",
                        "OK"
                    );
                    return;
                }

                CopySDKScene();
                CreateFacebookAsset();
            };
        }

        private static bool CreateKeysFolder(string path)
        {
            if (Directory.Exists(path)) { return true; }

            Directory.CreateDirectory(path);

            return Directory.Exists(path);
        }

        private static void CopySDKScene()
        {
            try
            {
                FileUtil.CopyFileOrDirectory(
                    ProjectSettingsHolder.instance.SDKSceneSource,
                    ProjectSettingsHolder.instance.SDKScenePath
                );

                SaveAssets();
                Debug.Log($"[TalusSettings-Package] elephant_scene copied to: {ProjectSettingsHolder.instance.SDKScenePath}");
            }
            catch (Exception)
            {
                // ignore
            }
        }

        private static void CreateFacebookAsset()
        {
            if (FacebookSettings.NullableInstance != null) { return; }

            string fullPath = ProjectSettingsHolder.instance.GetKeyPath($"{ProjectSettingsHolder.instance.FacebookAssetName}.asset");
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<FacebookSettings>(), fullPath);
            SaveAssets();

            Debug.Log($"[TalusSettings-Package] {fullPath} created!");
        }

        private static void SaveAssets()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /*
        private static void CreateSDKAsset()
        {
            ElephantSettings settings = Resources.Load<ElephantSettings>(ProjectSettingsHolder.instance.SDKAssetName);
            if (settings != null) { return; }

            string fullPath = ProjectSettingsHolder.instance.GetKeyPath($"{ProjectSettingsHolder.instance.SDKAssetName}.asset");
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<ElephantSettings>(), AssetDatabase.GenerateUniqueAssetPath(fullPath));
            SaveAssets();

            Debug.Log($"[TalusSettings-Package] {fullPath} created!");
        }
        */

        /*
        public static bool UpdateSDKAsset(AppModel app)
        {
            var elephantSettings = Resources.Load<ElephantSettings>(ProjectSettingsHolder.instance.SDKAssetName);
            if (elephantSettings == null)
            {
                Debug.LogError("[TalusSettings-Package] Elephant Settings can not found!");
                return false;
            }

            elephantSettings.GameID = app.elephant_id;
            elephantSettings.GameSecret = app.elephant_secret;
            EditorUtility.SetDirty(elephantSettings);
            SaveAssets();

            if (string.IsNullOrEmpty(app.elephant_id)) { Debug.LogWarning("[TalusSettings-Package] Elephant Game_ID is empty!"); }
            if (string.IsNullOrEmpty(app.elephant_secret)) { Debug.LogWarning("[TalusSettings-Package] Elephant Game_Secret is empty!"); }

            return true;
        }
        */
    }
}
