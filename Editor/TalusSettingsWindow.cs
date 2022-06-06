using System.Collections.Generic;

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
#if ENABLE_BACKEND
        private const string _ElephantScenePath = "Assets/Scenes/Template_Persistent/elephant_scene.unity";
#endif

        private const string _ForwarderScenePath = "Assets/Scenes/Template_Persistent/Scene_Forwarder.unity";

#if ENABLE_BACKEND
        [BoxGroup("Base Settings", Order = 0, CenterLabel = true)]
        [LabelWidth(100)]
        [ValidateInput(nameof(IsSceneValid), nameof(ElephantScene) + " is required!")]
        [ShowInInspector]
        public SceneReference ElephantScene
        {
            get { return new SceneReference(_ElephantScenePath); }
        }
#endif

        [BoxGroup("Base Settings")]
        [LabelWidth(100)]
        [ValidateInput(nameof(IsSceneValid), nameof(ForwarderScene) + " is required!")]
        [ShowInInspector]
        public SceneReference ForwarderScene
        {
            get { return new SceneReference(_ForwarderScenePath); }
        }

        [BoxGroup("Game Settings", Order = 1, CenterLabel = true)]
        [LabelWidth(100)]
        [ValidateInput(nameof(IsCollectionValid), nameof(LevelCollection) + " is not valid!", ContinuousValidationCheck = true)]
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
                InfoBox.Create(
                    "TalusSettings - Package | Error!",
                    $"{nameof(ElephantScene)} cannot be null!",
                    "OK, I understand"
                );

                return;
            }
#endif

            if (!IsSceneValid(ForwarderScene))
            {
                InfoBox.Create(
                    "TalusSettings - Package | Error!",
                    $"{nameof(ForwarderScene)} cannot be null!",
                    "OK, I understand"
                );

                return;
            }

            if (!IsCollectionValid(LevelCollection))
            {
                InfoBox.Create(
                    "TalusSettings - Package | Error!",
                    $"There is/are invalid scene reference(s) in {nameof(LevelCollection)}.",
                    "OK, I understand"
                );

                return;
            }

            BackendApi api = new BackendApi(BackendSettings.ApiUrl, BackendSettings.ApiToken);
            api.GetAppInfo(AppId, UpdateBackendData);
        }

        [MenuItem("TalusKit/Backend/App Settings", false, 10001)]
        private static void OpenWindow()
        {
            if (string.IsNullOrEmpty(BackendSettings.ApiUrl))
            {
                InfoBox.Create(
                    "TalusSettings-Package | Error!",
                    "'Api URL' can not be empty!\n\n(Edit/Project Settings/Talus Studio/Backend Settings)",
                    "Open Settings",
                    "Close",
                    () => SettingsService.OpenProjectSettings(BackendSettings.Path)
                );
            }
            else if (string.IsNullOrEmpty(BackendSettings.ApiToken))
            {
                InfoBox.Create(
                    "TalusSettings-Package | Error!",
                    "'Api Token' can not be empty!\n\n(Edit/Project Settings/Talus Studio/Backend Settings)",
                    "Open Settings",
                    "Close",
                    () => SettingsService.OpenProjectSettings(BackendSettings.Path)
                );
            }
            else
            {
                var window = GetWindow<TalusSettingsWindow>();
                window.minSize = new Vector2(400, 320);
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
            TalusSettingsProvider.UpdateFacebookAsset(app);
            TalusSettingsProvider.UpdateElephantAsset(app);
#endif

            InfoBox.Create(
                "TalusSettings-Package | Success!",
                $"App settings updated!\n\n{app}",
                "OK"
            );
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


        private static bool IsBackendActive()
        {
#if ENABLE_BACKEND
            return true;
#else
            return false;
#endif
        }

        private bool IsSceneValid(SceneReference scene)
        {
            return scene != null && !scene.IsEmpty;
        }

        private bool IsCollectionValid(SceneCollection collection)
        {
            if (collection == null)
            {
                return false;
            }

            int badSceneReferenceCount = 0;
            collection.ForEach(sceneReference => {
                if (sceneReference == null || sceneReference.IsEmpty)
                {
                    ++badSceneReferenceCount;
                }
            });

            return collection.Count > 0 && badSceneReferenceCount == 0;
        }

        private static void SaveAssets()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
