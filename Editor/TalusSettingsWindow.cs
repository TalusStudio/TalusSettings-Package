using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

using TalusBackendData.Editor;
using TalusBackendData.Editor.Models;
using TalusBackendData.Editor.Utility;

#if ENABLE_BACKEND
using TalusSettings.Editor.Definitions;
#endif

namespace TalusSettings.Editor
{
    /// <summary>
    ///     To update project settings with backend data.
    ///     1. Updates product name and bundle id.
    ///     2. Create/Update FB and GA SDK keys.
    /// </summary>
    ///
    [DetailedInfoBox(
        "Click here for details!",
        "Talus Studio/Prototype - Build Settings\n\n" +
        "This window makes all the necessary changes to properly upload the project to TestFlight and Google Play.\n\n"
    )]
    internal class TalusSettingsWindow : OdinEditorWindow
    {
        [LabelWidth(120)]
        [ShowInInspector, Required]
        [InlineButton(nameof(OpenDashboardUrl), Label = "Get ID ")]
        [PropertyOrder(999)]
        public string AppID
        {
            get { return BackendSettingsHolder.instance.AppId; }
            set { BackendSettingsHolder.instance.AppId = value; }
        }

        [OnInspectorInit]
        private void InitWindow()
        { }

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

        public void UpdateProjectSettings()
        {
            BackendApi api = new(BackendSettingsHolder.instance.ApiUrl, BackendSettingsHolder.instance.ApiToken);
            api.GetAppInfo(AppID, (app) =>
            {
                UpdateProductSettings(app);
                UpdateBackendKeys(app);
                InfoBox.Show("Success !", $"App settings updated!\n\n{app}", "OK");
            });
        }

        private void UpdateProductSettings(AppModel app)
        {
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, app.app_bundle);
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, app.app_bundle);
            PlayerSettings.productName = app.app_name;

            SaveAssets();
        }

        private void UpdateBackendKeys(AppModel app)
        {
#if ENABLE_BACKEND
            if (!AppSettings.AppSettings.UpdateFacebookAsset(app))
            {
                InfoBox.Show("Error !", $"Facebook settings couldn't updated!", "OK");
                return;
            }

            /*
            if (!AppSettings.AppSettings.UpdateSDKAsset(app))
            {
                InfoBox.Show("Error !", $"SDK settings couldn't updated!", "OK");
                return;
            }
            */
#endif
        }

        private static void OpenDashboardUrl()
        {
            Application.OpenURL($"{BackendSettingsHolder.instance.ApiUrl}/dashboard");
        }

        private static void SaveAssets()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
