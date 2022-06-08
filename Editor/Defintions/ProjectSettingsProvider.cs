using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace TalusSettings.Editor.Definitions
{
    internal class ProjectSettingsProvider : SettingsProvider
    {
        private bool _UnlockPanel = true;

        private SerializedObject _SerializedObject;

        public ProjectSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
            : base(path, scopes, keywords)
        { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            ProjectSettingsHolder.instance.SaveSettings();

            _SerializedObject = new SerializedObject(ProjectSettingsHolder.instance);

            _UnlockPanel = true;
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
                        "Talus Prototype - Project Layout (Do not leave any input fields blank!)",
                        "To automate populate Unity project settings"),
                    MessageType.Info,
                    true
                );
                GUI.backgroundColor = defaultColor;

                EditorGUI.BeginChangeCheck();

                GUI.enabled = !_UnlockPanel;
                {

                    SerializedProperty serializedProperty = _SerializedObject.GetIterator();
                    while (serializedProperty.NextVisible(true))
                    {
                        if (serializedProperty.name == "m_Script") { continue; }

                        EditorGUILayout.Separator();

                        EditorGUILayout.LabelField($"{serializedProperty.displayName}:");
                        serializedProperty.stringValue = EditorGUILayout.TextField(serializedProperty.stringValue);
                    }

                    GUILayout.FlexibleSpace();

                    GUI.enabled = true;
                    GUI.backgroundColor = Color.yellow;

                    string lockButtonName = (_UnlockPanel) ? "Unlock Settings" : "Lock Settings";
                    if (GUILayout.Button(lockButtonName, GUILayout.MinHeight(50)))
                    {
                        _UnlockPanel = !_UnlockPanel;
                    }

                    GUI.backgroundColor = Color.green;
                    if (GUILayout.Button("Reset to defaults", GUILayout.MinHeight(50)))
                    {

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
                ProjectSettingsHolder.ProviderPath,
                SettingsScope.Project
            );

        }
    }
}