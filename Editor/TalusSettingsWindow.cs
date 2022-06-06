using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;

using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

using TalusBackendData.Editor;
using TalusBackendData.Editor.User;
using TalusBackendData.Editor.Models;

using TalusFramework.Utility;
using TalusFramework.Collections;

#if ENABLE_BACKEND
using Path = System.IO.Path;
using FacebookSettings = Facebook.Unity.Settings.FacebookSettings;
#endif

namespace TalusSettings.Editor
{
    public class TalusSettingsWindow : OdinEditorWindow
    {
        [OnInspectorInit]
        private void UpdateData()
        {

        }

        [OnInspectorDispose]
        private void ClearData()
        {

        }

        [Title("Scene Settings")]
        [LabelWidth(120)]
        [EnableIf("@GetBackendStatus() == true")]
        [ValidateInput(nameof(HasSceneValidReference), nameof(ElephantScene) + " is required!")]
        public SceneReference ElephantScene;

        [LabelWidth(120)]
        [ValidateInput(nameof(HasSceneValidReference), nameof(ForwarderScene) + " is required!")]
        public SceneReference ForwarderScene;

        [LabelWidth(120)]
        [ValidateInput(nameof(HasCollectionValid), nameof(LevelCollection) + " is not valid!", ContinuousValidationCheck = true)]
        public SceneCollection LevelCollection;

        [Title("App Settings")]
        [InfoBox("Get App_ID from Web Dashboard")]
        [LabelWidth(120)]
        [ShowInInspector, Required]
        [InlineButton(nameof(OpenDashboardUrl), Label = "Web Dashboard")]
        public string AppId
        {
            get { return EditorPrefs.GetString(BackendDefinitions.BackendAppIdPref); }
            set { EditorPrefs.SetString(BackendDefinitions.BackendAppIdPref, value); }
        }

        [DisableInPlayMode]
        [PropertySpace(10)]
        [Button(ButtonSizes.Large), GUIColor(0f, 1f, 0f)]
        public void UpdateProjectSettings()
        {
            BackendApi api = new BackendApi(BackendSettings.ApiUrl, BackendSettings.ApiToken);
            api.GetAppInfo(AppId, UpdateBackendData);
        }

        [MenuItem("TalusKit/Backend/Project Settings", false, 10001)]
        private static void OpenWindow()
        {
            if (string.IsNullOrEmpty(BackendSettings.ApiUrl))
            {
                Debug.LogError("ApiUrl can not be empty! (Edit/Project Settings/Talus Studio/Backend Settings)");
            }
            else if (string.IsNullOrEmpty(BackendSettings.ApiToken))
            {
                Debug.LogError("ApiToken can not be empty! (Edit/Project Settings/Talus Studio/Backend Settings)");
            }
            else
            {
                var window = GetWindow<TalusSettingsWindow>();
                window.Show();
            }
        }

        private static void OpenDashboardUrl()
        {
            Application.OpenURL("http://34.252.141.173/dashboard");
        }

        private void UpdateBackendData(AppModel app)
        {
            UpdateBuildSettings();
            UpdateProductSettings(app);

#if ENABLE_BACKEND
            UpdateFacebookAsset(app);
            UpdateElephantAsset(app);
#endif
        }

        private void UpdateProductSettings(AppModel app)
        {
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, app.app_bundle);
            PlayerSettings.productName = app.app_name;
            SaveAssets();

            Debug.Log("Product Name and Bundle ID updated!");
        }

        private void UpdateBuildSettings()
        {
            var scenes = new List<EditorBuildSettingsScene>();

#if ENABLE_BACKEND
            scenes.Add(new EditorBuildSettingsScene(ElephantScene.ScenePath, true));
#endif
            scenes.Add(new EditorBuildSettingsScene(ForwarderScene.ScenePath, true));

            LevelCollection.ForEach(level => scenes.Add(new EditorBuildSettingsScene(level.ScenePath, true)));

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

        private static bool GetBackendStatus() =>
            PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS)
            .Split(';')
            .ToList()
            .Contains("ENABLE_BACKEND");

#region VALIDATIONS
        private bool HasSceneValidReference(SceneReference scene) => scene != null && !scene.IsEmpty;
        private bool HasCollectionValid(SceneCollection collection) => collection != null && collection.Count > 0 && !collection[0].IsEmpty;
#endregion

    }
}
