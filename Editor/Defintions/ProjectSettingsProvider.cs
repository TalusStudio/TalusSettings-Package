using System.Collections.Generic;

using UnityEngine.UIElements;
using UnityEditor;

namespace TalusSettings.Editor.Definitions
{
    class ProjectSettingsProvider : SettingsProvider
    {
        private SerializedObject _SerializedObject;

        public ProjectSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
            : base(path, scopes, keywords)
        { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            ProjectSettingsHolder.instance.SaveSettings();

            _SerializedObject = new SerializedObject(ProjectSettingsHolder.instance);
        }

        public override void OnGUI(string searchContext)
        {
            _SerializedObject.Update();

            EditorGUILayout.HelpBox(
                string.Join(
                    "\n\n",
                    "Talus Backend - Project Layout",
                    "You can see default project folder layout to work with CI/CD automation."),
                MessageType.Info,
                true
            );

            EditorGUILayout.BeginVertical();

            SerializedProperty prop = _SerializedObject.GetIterator();
            if (prop.NextVisible(true))
            {
                do
                {
                   EditorGUILayout.PropertyField(_SerializedObject.FindProperty(prop.name), true);
                }
                while (prop.NextVisible(false));
            }

            EditorGUILayout.EndVertical();
        }

        [SettingsProvider]
        public static SettingsProvider CreateProjectSettingsProvider()
        {
            return new ProjectSettingsProvider(
                ProjectSettingsHolder.SettingsProviderPath,
                SettingsScope.Project
            );

        }
    }
}