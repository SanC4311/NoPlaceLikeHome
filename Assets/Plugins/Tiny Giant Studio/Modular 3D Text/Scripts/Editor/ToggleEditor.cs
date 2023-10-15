using TinyGiantStudio.EditorHelpers;
using UnityEditor;
/// Created by Ferdowsur Asif @ Tiny Giant Studio
using UnityEngine;

namespace TinyGiantStudio.Text
{
    [CustomEditor(typeof(Toggle))]
    public class ToggleEditor : Editor
    {
        Toggle myTarget;
        SerializedObject soTarget;

        SerializedProperty isOn;
        SerializedProperty onGraphic;
        SerializedProperty onEvent;
        SerializedProperty offGraphic;
        SerializedProperty offEvent;

        GUIStyle toolbarTitle = null;

        readonly string variableName = "\n\nVariable name: ";



        void OnEnable()
        {
            myTarget = (Toggle)target;
            soTarget = new SerializedObject(target);

            isOn = soTarget.FindProperty("_isOn");
            onGraphic = soTarget.FindProperty("onGraphic");
            onEvent = soTarget.FindProperty("onEvent");
            offGraphic = soTarget.FindProperty("offGraphic");
            offEvent = soTarget.FindProperty("offEvent");
        }

        public override void OnInspectorGUI()
        {
            UpdateStyles();
            soTarget.Update();
            EditorGUI.BeginChangeCheck();

            MainSettings();

            if (EditorGUI.EndChangeCheck())
            {
                myTarget.VisualUpdate();
                soTarget.ApplyModifiedProperties();
                ApplyMyModifiedData();
                EditorUtility.SetDirty(myTarget);
            }
        }

        void ApplyMyModifiedData()
        {
            if (myTarget.IsOn)
                myTarget.ActiveVisualUpdate();
            else
                myTarget.InactiveVisualUpdate();
        }

        void MainSettings()
        {
            EditorGUI.indentLevel = 0;
            GUILayout.Space(2.5f);
            MText_Editor_Methods.ItalicHorizontalField(isOn, "Is On", variableName + "IsOn");
            GUILayout.Space(10f);

            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label(new GUIContent("On", variableName + "onEvent"), toolbarTitle);
            GUILayout.EndHorizontal();
            GUILayout.Space(2.5f);
            MText_Editor_Methods.ItalicHorizontalField(onGraphic, "Graphic", variableName + "onGraphic");
            GUILayout.Space(2.5f);
            EditorGUILayout.PropertyField(onEvent);
            GUILayout.EndVertical();

            GUILayout.Space(25);

            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label(new GUIContent("Off", variableName + "offEvent"), toolbarTitle);
            GUILayout.EndHorizontal();
            GUILayout.Space(2.5f);
            MText_Editor_Methods.ItalicHorizontalField(offGraphic, "Graphic", variableName + "offGraphic");
            GUILayout.Space(2.5f);
            EditorGUILayout.PropertyField(offEvent);
            GUILayout.EndVertical();

        }

        void UpdateStyles()
        {
            if (toolbarTitle == null)
            {
                toolbarTitle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontStyle = FontStyle.BoldAndItalic,
                    fontSize = 14
                };
            }
        }
    }
}