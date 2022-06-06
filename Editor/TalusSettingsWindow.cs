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

namespace TalusSettings.Editor
{
    public class TalusSettingsWindow : OdinEditorWindow
    {
        [Title("Scene Settings")]
        [LabelWidth(120)]
        [EnableIf(nameof(IsBackendActive))]
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
                EditorUtility.DisplayDialog(
                    "TalusSettings-Package",
                    "'Api URL' can not be empty!\n(Edit/Project Settings/Talus Studio/Backend Settings)",
                    "OK, I understand"
                );
            }
            else if (string.IsNullOrEmpty(BackendSettings.ApiToken))
            {
                EditorUtility.DisplayDialog(
                    "TalusSettings-Package",
                    "'Api Token' can not be empty!\n(Edit/Project Settings/Talus Studio/Backend Settings)",
                    "OK, I understand"
                );
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
            TalusSettingsProvider.UpdateFacebookAsset(app);
            TalusSettingsProvider.UpdateElephantAsset(app);
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


#region VALIDATIONS
        private static bool IsBackendActive() => PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS)
            .Split(';')
            .ToList()
            .Contains("ENABLE_BACKEND");

        private bool HasSceneValidReference(SceneReference scene) => scene != null && !scene.IsEmpty;
        private bool HasCollectionValid(SceneCollection collection) => collection != null && collection.Count > 0 && !collection[0].IsEmpty;
        #endregion

        private static void SaveAssets()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }
}
