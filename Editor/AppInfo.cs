using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using TalusBackendData.Editor;
using TalusBackendData.Editor.User;
using TalusBackendData.Editor.Models;

#if ENABLE_BACKEND
using Path = System.IO.Path;
using FacebookSettings = Facebook.Unity.Settings.FacebookSettings;
#endif

namespace TalusSettings.Editor
{
    internal static class AppInfo
    {
#if ENABLE_BACKEND
        private static readonly string _elephantScenePath = "Assets/Scenes/Template_Persistent/elephant_scene.unity";
#endif
        private static readonly string _forwarderScenePath = "Assets/Scenes/Template_Persistent/ForwarderScene.unity";


        [MenuItem("TalusKit/Backend/Fetch App Info", false, 10001)]
        public static void FetchAppInfo()
        {
            if (string.IsNullOrEmpty(BackendSettings.ApiUrl))
            {
                Debug.LogError("ApiUrl can not be empty! (Edit/Preferences/Talus/Backend Settings)");
            }
            else if (string.IsNullOrEmpty(BackendSettings.ApiToken))
            {
                Debug.LogError("ApiToken can not be empty! (Edit/Preferences/Talus/Backend Settings)");
            }
            else if (string.IsNullOrEmpty(BackendSettings.AppId))
            {
                Debug.LogError("AppId can not be empty! (Edit/Preferences/Talus/Backend Settings)");
            }
            else
            {
                new BackendApi(BackendSettings.ApiUrl, BackendSettings.ApiToken)
                    .GetAppInfo(BackendSettings.AppId, UpdateBackendData);

                Debug.Log("Fetching app info...");
            }
        }

        private static void UpdateBackendData(AppModel app)
        {
            UpdateProductSettings(app);
            UpdateBuildSettings();

#if ENABLE_BACKEND
            UpdateFacebookAsset(app);
            UpdateElephantAsset(app);
#endif
        }

        private static void UpdateProductSettings(AppModel app)
        {
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, app.app_bundle);
            PlayerSettings.productName = app.app_name;
            SaveAssets();

            Debug.Log("Product Name and Bundle ID updated!");
        }

        private static void UpdateBuildSettings()
        {
            var scenes = new List<EditorBuildSettingsScene>();

#if ENABLE_BACKEND
            scenes.Add(new EditorBuildSettingsScene(_elephantScenePath, true));
#endif
            scenes.Add(new EditorBuildSettingsScene(_forwarderScenePath, true));

            EditorBuildSettings.scenes = scenes.ToArray();
            SaveAssets();
        }

#if ENABLE_BACKEND
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void CreateBackendAssets()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) { return; }

            EditorApplication.delayCall += () =>
            {
                CreateFolderIfNotExists("Resources", "Assets");
                CreateFacebookAsset();
                CreateElephantAsset();
            };
        }

        private static void CreateFacebookAsset()
        {
            if (FacebookSettings.NullableInstance != null) { return; }

            string fullPath = GetAssetPath("FacebookSettings.asset");
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<FacebookSettings>(), fullPath);
            SaveAssets();

            Debug.Log("Facebook Settings created!");
        }

        private static void UpdateFacebookAsset(AppModel app)
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

        private static void CreateElephantAsset()
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

        private static void UpdateElephantAsset(AppModel app)
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

        private static void CreateFolderIfNotExists(string folderName, string parent)
        {
            if (AssetDatabase.IsValidFolder(Path.Combine(parent, folderName))) { return; }

            AssetDatabase.CreateFolder(parent, folderName);
        }

        private static string GetAssetPath(string asset)
        {
            return Path.Combine(Path.Combine("Assets", "Resources"), asset);
        }
#endif
        private static void SaveAssets()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
