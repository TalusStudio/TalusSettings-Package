using System.Collections.Generic;

using UnityEditor;
using UnityEngine.UIElements;

using TalusBackendData.Editor.Interfaces;

namespace TalusSettings.Editor.Definitions
{
    internal class ProjectSettingsProvider : BaseSettingsProvider<ProjectSettingsProvider>
    {
        public override string Title => $"{ProjectSettingsHolder.ProviderPath} (Do not leave any input fields blank!)";
        public override string Description => "To automate populate Unity project settings";

        public override SerializedObject SerializedObject => _SerializedObject;
        private SerializedObject _SerializedObject;

        [SettingsProvider]
        public static SettingsProvider CreateProjectSettingsProvider()
        {
            return new ProjectSettingsProvider(ProjectSettingsHolder.ProviderPath, SettingsScope.Project);
        }

        public ProjectSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
            : base(path, scopes, keywords)
        { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);

            ProjectSettingsHolder.instance.SaveSettings();

            _SerializedObject = new SerializedObject(ProjectSettingsHolder.instance);
        }

        public override void OnGUI(string searchContext)
        {
            _SerializedObject.Update();

            base.OnGUI(searchContext);

            if (EditorGUI.EndChangeCheck())
            {
                _SerializedObject.ApplyModifiedProperties();
                ProjectSettingsHolder.instance.SaveSettings();
            }
        }
    }
}