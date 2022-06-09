using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

using TalusBackendData.Editor.Interfaces;

namespace TalusSettings.Editor.Definitions
{
    internal class ProjectSettingsProvider : BaseSettingsProvider<ProjectSettingsProvider>
    {
        private SerializedObject _SerializedObject;

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
            EditorGUILayout.BeginVertical();
            {
                _SerializedObject.Update();

                Color defaultColor = GUI.color;
                GUI.backgroundColor = Color.yellow;
                EditorGUILayout.HelpBox(
                    string.Join(
                        "\n\n",
                        $"{ProjectSettingsHolder.ProviderPath} (Do not leave any input fields blank!)",
                        "To automate populate Unity project settings"),
                    MessageType.Info,
                    true
                );
                GUI.backgroundColor = defaultColor;

                GUILayout.Space(8);
                EditorGUI.BeginChangeCheck();
                GUI.enabled = !UnlockPanel;
                {

                    SerializedProperty serializedProperty = _SerializedObject.GetIterator();
                    while (serializedProperty.NextVisible(true))
                    {
                        if (serializedProperty.name == "m_Script") { continue; }

                        serializedProperty.stringValue = EditorGUILayout.TextField(
                            GetLabel(serializedProperty.displayName),
                            serializedProperty.stringValue
                        );
                    }

                    GUI.enabled = !UnlockPanel;
                    GUI.backgroundColor = Color.green;
                    if (GUILayout.Button("Reset to defaults", GUILayout.MinHeight(50)))
                    {

                    }
                }
                // unlock button
                base.OnGUI(searchContext);

                if (EditorGUI.EndChangeCheck())
                {
                    _SerializedObject.ApplyModifiedProperties();
                    ProjectSettingsHolder.instance.SaveSettings();
                }
            }
            EditorGUILayout.EndVertical();
        }

        [SettingsProvider]
        public static SettingsProvider CreateProjectSettingsProvider()
        {
            return new ProjectSettingsProvider(
                ProjectSettingsHolder.ProviderPath,
                SettingsScope.Project
            );

        }
    }
}