using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

using TalusBackendData.Editor;
using TalusBackendData.Editor.Models;
using TalusBackendData.Editor.Utility;

using TalusFramework.Utility;
using TalusFramework.Collections;
using TalusSettings.Editor.Definitons;

#if ENABLE_BACKEND
using AppSettings = TalusSettings.Editor.Backend.AppSettings;
#endif

namespace TalusSettings.Editor
{
    /// <summary>
    ///     Window Path on Unity3D: 'TalusKit/Backend/App Settings'.
    ///     To update project settings with backend data.
    ///     Updates product name and ios bundle id.
    ///     Create/Update FB and Elephant SDK keys.
    /// </summary>
    internal class TalusSettingsWindow : OdinEditorWindow
    {
#if ENABLE_BACKEND
        private SceneReference _ElephantScene;

        [BoxGroup("Base Settings", Order = 0, CenterLabel = true)]
        [LabelWidth(100)]
        [ValidateInput(nameof(IsSceneValid), nameof(ElephantScene) + " is required!")]
        [ShowInInspector]
        [HideReferenceObjectPicker]
        public SceneReference ElephantScene
        {
            get { return _ElephantScene; }
            set { _ElephantScene = value.Clone(); }
        }
#endif

        private SceneReference _ForwarderScene;

        [BoxGroup("Base Settings", Order = 0, CenterLabel = true)]
        [LabelWidth(100)]
        [ValidateInput(nameof(IsSceneValid), nameof(ForwarderScene) + " is required!")]
        [ShowInInspector]
        [HideReferenceObjectPicker]
        public SceneReference ForwarderScene
        {
            get { return _ForwarderScene; }
            set { _ForwarderScene = value.Clone(); }
        }

        [BoxGroup("Game Settings", Order = 1, CenterLabel = true)]
        [LabelWidth(100)]
        [ValidateInput(nameof(IsSceneCollectionValid), nameof(LevelCollection) + " is not valid!", ContinuousValidationCheck = true)]
        public SceneCollection LevelCollection;

        [BoxGroup("App Settings", Order = 2, CenterLabel = true)]
        [InfoBox("Get App_ID from Web Dashboard")]
        [LabelWidth(100)]
        [ShowInInspector, Required]
        [InlineButton(nameof(OpenDashboardUrl), Label = "Web Dashboard")]
        public string AppId
        {
            get { return EditorPrefs.GetString(BackendDefinitions.BackendAppIdPref); }
            set { EditorPrefs.SetString(BackendDefinitions.BackendAppIdPref, value); }
        }

        [DisableInPlayMode]
        [PropertyOrder(999)]
        [GUIColor(0f, 1f, 0f)]
        [Button(ButtonSizes.Gigantic)]
        public void UpdateProjectSettings()
        {
#if ENABLE_BACKEND
            if (!IsSceneValid(ElephantScene))
            {
                InfoBox.Create("Error :(", $"{nameof(ElephantScene)} cannot be null!", "OK, I understand");
                return;
            }
#endif

            if (!IsSceneValid(ForwarderScene))
            {
                InfoBox.Create("Error :(", $"{nameof(ForwarderScene)} cannot be null!", "OK, I understand");
                return;
            }

            if (!IsSceneCollectionValid(LevelCollection))
            {
                InfoBox.Create("Error :(", $"There is/are invalid scene reference(s) in {nameof(LevelCollection)}.", "OK, I understand");
                return;
            }

            BackendApi api = new BackendApi(BackendDefinitions.ApiUrl, BackendDefinitions.ApiToken);
            api.GetAppInfo(AppId, UpdateBackendData);
        }

        [OnInspectorInit]
        private void InitWindow()
        {
#if ENABLE_BACKEND
            _ElephantScene = new SceneReference(SettingsDefinitions.ElephantScenePath);
#endif
            _ForwarderScene = new SceneReference(SettingsDefinitions.ForwarderScenePath);
        }

        [MenuItem("TalusKit/Backend/App Settings", false, 10001)]
        private static void OpenWindow()
        {
            if (string.IsNullOrEmpty(BackendDefinitions.ApiUrl))
            {
                InfoBox.Create(
                    "Error :(",
                    "'Api URL' can not be empty!\n\n(Edit/Project Settings/Talus Studio/Backend Settings)",
                    "Open Settings",
                    "Close",
                    () => SettingsService.OpenProjectSettings(BackendDefinitions.ProviderPath)
                );
            }
            else if (string.IsNullOrEmpty(BackendDefinitions.ApiToken))
            {
                InfoBox.Create(
                    "Error :(",
                    "'Api Token' can not be empty!\n\n(Edit/Project Settings/Talus Studio/Backend Settings)",
                    "Open Settings",
                    "Close",
                    () => SettingsService.OpenProjectSettings(BackendDefinitions.ProviderPath)
                );
            }
            else
            {
                var window = GetWindow<TalusSettingsWindow>();
                window.minSize = new Vector2(400, 400);
                window.Show();
            }
        }

        private static void OpenDashboardUrl()
        {
            Application.OpenURL("http://34.252.141.173/dashboard");
        }

        private void UpdateBackendData(AppModel app)
        {
            UpdateSceneSettings();
            UpdateProductSettings(app);

#if ENABLE_BACKEND
            AppSettings.UpdateFacebookAsset(app);
            AppSettings.UpdateElephantAsset(app);
#endif

            InfoBox.Create("Success !", $"App settings updated!\n\n{app}", "OK");
        }

        private void UpdateProductSettings(AppModel app)
        {
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, app.app_bundle);
            PlayerSettings.productName = app.app_name;

            SaveAssets();
        }

        private void UpdateSceneSettings()
        {
            var scenes = new List<EditorBuildSettingsScene>();

#if ENABLE_BACKEND
            scenes.Add(new EditorBuildSettingsScene(ElephantScene.ScenePath, true));
#endif
            scenes.Add(new EditorBuildSettingsScene(ForwarderScene.ScenePath, true));

            for (int i = 0; i < LevelCollection.Count; ++i)
            {
                scenes.Add(new EditorBuildSettingsScene(LevelCollection[i].ScenePath, true));
            }

            EditorBuildSettings.scenes = scenes.ToArray();
            SaveAssets();
        }

        private static void SaveAssets()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

#region VALIDATIONS
        private bool IsSceneValid(SceneReference scene)
        {
            return scene != null && !scene.IsEmpty;
        }

        private bool IsSceneCollectionValid(SceneCollection collection)
        {
            if (collection == null) { return false; }

            int badSceneReferenceCount = 0;
            collection.ForEach(sceneReference =>
            {
                if (sceneReference == null || sceneReference.IsEmpty)
                {
                    ++badSceneReferenceCount;
                }
            });

            return collection.Count > 0 && badSceneReferenceCount == 0;
        }
#endregion
    }
}
