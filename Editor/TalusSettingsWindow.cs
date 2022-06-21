using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

using TalusSettings.Editor.Definitions;

using TalusBackendData.Editor;
using TalusBackendData.Editor.Models;
using TalusBackendData.Editor.Utility;

using TalusFramework.Utility;
using TalusFramework.Collections;

namespace TalusSettings.Editor
{
    /// <summary>
    ///     To update project settings with backend data.
    ///     1. Updates product name and ios bundle id.
    ///     2. Create/Update FB and Elephant SDK keys.
    /// </summary>
    ///
    [DetailedInfoBox(
        "Click here for details!",
        "Talus Studio/Prototype - Build Settings\n\n" +
        "This window makes all the necessary changes to properly upload the project to TestFlight.\n\n" +
        "NOTE: If you create new Level Collection, don't forget to reference that new collection in Runtime Data Manager scriptable object." +
        "There is an editor window 'TalusKit/SO Editor' to inspect managers. (shortcut: CTRL + M)"
    )]
    internal class TalusSettingsWindow : OdinEditorWindow
    {
#if ENABLE_BACKEND
        private SceneReference _ElephantScene;

        [LabelWidth(120)]
        [ShowInInspector]
        [HideReferenceObjectPicker]
        public SceneReference ElephantScene
        {
            get { return _ElephantScene; }
            set { }
        }
#endif

        private SceneReference _ForwarderScene;

        [LabelWidth(120)]
        [ShowInInspector]
        [HideReferenceObjectPicker]
        public SceneReference ForwarderScene
        {
            get { return _ForwarderScene; }
            set { }
        }

        [LabelWidth(120)]
        [InlineButton(nameof(CreateSceneCollection), Label = "Create")]
        [PropertyOrder(998)]
        [PropertySpace(SpaceBefore = 16)]
        [Required]
        public SceneCollection LevelCollection;

        [LabelWidth(120)]
        [ShowInInspector, Required]
        [InlineButton(nameof(OpenDashboardUrl), Label = "Get ID ")]
        [PropertyOrder(999)]
        public string AppID
        {
            get { return BackendSettingsHolder.instance.AppId; }
            set { BackendSettingsHolder.instance.AppId = value; }
        }

        public void UpdateProjectSettings()
        {
#if ENABLE_BACKEND
            if (!IsSceneValid(ElephantScene))
            {
                InfoBox.Show("Error :(", $"{nameof(ElephantScene)} cannot be null!", "OK, I understand");
                return;
            }
#endif

            if (!IsSceneValid(ForwarderScene))
            {
                InfoBox.Show("Error :(", $"{nameof(ForwarderScene)} cannot be null!", "OK, I understand");
                return;
            }

            if (!IsSceneCollectionValid(LevelCollection))
            {
                InfoBox.Show("Error :(", $"There is/are invalid scene reference(s) in {nameof(LevelCollection)}.", "OK, I understand");
                return;
            }

            BackendApi api = new BackendApi(BackendSettingsHolder.instance.ApiUrl, BackendSettingsHolder.instance.ApiToken);
            api.GetAppInfo(AppID, UpdateBackendData);
        }

        [OnInspectorInit]
        private void InitWindow()
        {
#if ENABLE_BACKEND
            _ElephantScene = new SceneReference(ProjectSettingsHolder.instance.ElephantScenePath);
#endif
            _ForwarderScene = new SceneReference(ProjectSettingsHolder.instance.ForwarderScenePath);
        }

        [MenuItem("TalusBackend/Build Settings", false, 10001)]
        private static void OpenWindow()
        {
            if (string.IsNullOrEmpty(BackendSettingsHolder.instance.ApiUrl))
            {
                InfoBox.ShowBackendParameterError(nameof(BackendSettingsHolder.instance.ApiUrl));
                return;
            }

            if (string.IsNullOrEmpty(BackendSettingsHolder.instance.ApiToken))
            {
                InfoBox.ShowBackendParameterError(nameof(BackendSettingsHolder.instance.ApiToken));
                return;
            }

            var window = GetWindow<TalusSettingsWindow>();
            window.minSize = new Vector2(430, 250);
            window.Show();
        }

        protected override void Initialize()
        {
            var window = GetWindow<TalusSettingsWindow>();
            window.OnEndGUI += () =>
            {
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Update Project Settings", GUILayout.MinHeight(50)))
                {
                    UpdateProjectSettings();
                }
            };
        }

        private static void OpenDashboardUrl()
        {
            Application.OpenURL("http://34.252.141.173/dashboard");
        }

        private static void CreateSceneCollection()
        {
            Debug.LogError("Not implemented!");
        }

        private void UpdateBackendData(AppModel app)
        {
            UpdateSceneSettings();
            UpdateProductSettings(app);

#if ENABLE_BACKEND
            if (!AppSettings.AppSettings.UpdateFacebookAsset(app))
            {
                InfoBox.Show("Error !", $"Facebook settings couldn't updated!", "OK");
                return;
            }

            if (!AppSettings.AppSettings.UpdateElephantAsset(app))
            {
                InfoBox.Show("Error !", $"Elephant settings couldn't updated!", "OK");
                return;
            }
#endif

            InfoBox.Show("Success !", $"App settings updated!\n\n{app}", "OK");
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
        private bool IsSceneCollectionValid(SceneCollection collection)
        {
            if (collection == null) { return false; }

            int badReferenceCount = 0;
            collection.ForEach(sceneReference =>
            {
                if (!IsSceneValid(sceneReference))
                {
                    ++badReferenceCount;
                }
            });

            return collection.Count > 0 && badReferenceCount == 0;
        }

        private bool IsSceneValid(SceneReference scene)
        {
            return scene != null && !scene.IsEmpty;
        }
#endregion
    }
}
