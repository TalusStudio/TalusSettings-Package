using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace TalusSettings.Editor.Definitions
{
    public class ProjectSettingsProvider : SettingsProvider
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
            EditorGUILayout.BeginVertical();
            {
                _SerializedObject.Update();

                EditorGUILayout.HelpBox(
                    string.Join(
                        "\n\n",
                        "Talus Prototype - Project Layout",
                        "To work with CI/CD automation."),
                    MessageType.Info,
                    true
                );

                {
                    EditorGUI.BeginChangeCheck();

                    SerializedProperty serializedProperty = _SerializedObject.GetIterator();
                    while (serializedProperty.NextVisible(true))
                    {
                        if (serializedProperty.name == "m_Script") { continue; }

                        EditorGUILayout.Separator();

                        EditorGUILayout.LabelField($"{serializedProperty.displayName}:");
                        serializedProperty.stringValue = EditorGUILayout.TextField(serializedProperty.stringValue);
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        _SerializedObject.ApplyModifiedProperties();
                        ProjectSettingsHolder.instance.SaveSettings();
                    }
                }
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