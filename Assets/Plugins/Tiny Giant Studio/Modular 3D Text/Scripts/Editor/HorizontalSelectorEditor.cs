using TinyGiantStudio.EditorHelpers;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace TinyGiantStudio.Text
{
    [CustomEditor(typeof(HorizontalSelector))]
    public class HorizontalSelectorEditor : Editor
    {
        public AssetSettings settings;

        HorizontalSelector myTarget;
        SerializedObject soTarget;

        SerializedProperty text;
        SerializedProperty focused;
        SerializedProperty interactable;

        SerializedProperty options;
        SerializedProperty value;



        SerializedProperty audioSource;
        SerializedProperty valueChangeSoundEffect;

        SerializedProperty onSelectEvent;
        SerializedProperty onValueChangedEvent;
        SerializedProperty onValueIncreasedEvent;
        SerializedProperty onValueDecreasedEvent;



        AnimBool showAudioControls;
        AnimBool showEventsControls;



        GUIStyle foldOutStyle;
        Color openedFoldoutTitleColor = new Color(124 / 255f, 170 / 255f, 239 / 255f, 0.9f);

        readonly string variableName = "\n\nVariable name: ";

        void OnEnable()
        {
            myTarget = (HorizontalSelector)target;
            soTarget = new SerializedObject(target);

            FindProperties();
        }

        public override void OnInspectorGUI()
        {
            GenerateStyle();

            if (myTarget.text == null)
                EditorGUILayout.HelpBox("Please reference a text", MessageType.Warning);

            EditorGUI.BeginChangeCheck();
            GUILayout.Space(10);

            MText_Editor_Methods.ItalicHorizontalField(text, "Text", "Reference to the 3d text where the selected text will be shown" + variableName + "text");
            GUILayout.Space(10);

            MText_Editor_Methods.ItalicHorizontalField(focused, "Focused", "If keyboard control is enabled, selected = you can control via selected. \nThis value will be controlled by list, if it is in one" + variableName + "focused");
            MText_Editor_Methods.ItalicHorizontalField(interactable, "Interactable", "If keyboard control is enabled, selected = you can control via selected\nOr selected/deselected in a List" + variableName + "interactable");

            GUILayout.Space(15);
            //MText_Editor_Methods.HorizontalField(value, "Current Value");
            EditorGUILayout.PropertyField(value, new GUIContent("Value", variableName + "value"));
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(options, new GUIContent("Options", variableName + "options"));
            GUILayout.Space(15);

            AudioControl();
            GUILayout.Space(5);
            EventControl();

            if (EditorGUI.EndChangeCheck())
            {
                soTarget.ApplyModifiedProperties();
                myTarget.UpdateText();
            }
        }




        void AudioControl()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 1;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            GUIContent content = new GUIContent("Audio", "");
            showAudioControls.target = EditorGUILayout.Foldout(showAudioControls.target, content, true, foldOutStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showAudioControls.faded))
            {
                EditorGUI.indentLevel = 0;
                MText_Editor_Methods.ItalicHorizontalField(audioSource, "Audio Source", variableName + "audioSource", FieldSize.large);
                MText_Editor_Methods.ItalicHorizontalField(valueChangeSoundEffect, "Value Changed Sound", variableName + "valueChangeSoundEffect", FieldSize.gigantic);

            }

            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
            EditorGUI.indentLevel = 0;
        }
        private void EventControl()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 1;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            GUIContent content = new GUIContent("Events", "");
            showEventsControls.target = EditorGUILayout.Foldout(showEventsControls.target, content, true, foldOutStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showEventsControls.faded))
            {
                EditorGUI.indentLevel = 0;
                EditorGUILayout.PropertyField(onSelectEvent);
                EditorGUILayout.PropertyField(onValueChangedEvent);
                EditorGUILayout.PropertyField(onValueIncreasedEvent);
                EditorGUILayout.PropertyField(onValueDecreasedEvent);
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
            EditorGUI.indentLevel = 0;
        }



        void FindProperties()
        {
            text = soTarget.FindProperty("text");
            focused = soTarget.FindProperty("focused");
            interactable = soTarget.FindProperty("interactable");

            options = soTarget.FindProperty("options");

            value = soTarget.FindProperty("_value");

            audioSource = soTarget.FindProperty("audioSource");
            valueChangeSoundEffect = soTarget.FindProperty("valueChangeSoundEffect");

            onSelectEvent = soTarget.FindProperty("onSelectEvent");
            onValueChangedEvent = soTarget.FindProperty("onValueChangedEvent");
            onValueIncreasedEvent = soTarget.FindProperty("onValueIncreasedEvent");
            onValueDecreasedEvent = soTarget.FindProperty("onValueDecreasedEvent");

            showAudioControls = new AnimBool(false);
            showAudioControls.valueChanged.AddListener(Repaint);

            showEventsControls = new AnimBool(false);
            showEventsControls.valueChanged.AddListener(Repaint);

        }

        void GenerateStyle()
        {
            if (EditorGUIUtility.isProSkin)
            {
                if (settings)
                    openedFoldoutTitleColor = settings.openedFoldoutTitleColor_darkSkin;
            }
            else
            {
                if (settings)
                    openedFoldoutTitleColor = settings.openedFoldoutTitleColor_lightSkin;
            }

            if (foldOutStyle == null)
            {
                //foldOutStyle = new GUIStyle(EditorStyles.foldout);
                foldOutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    //foldOutStyle.overflow = new RectOffset(-10, 0, 3, 0);
                    padding = new RectOffset(14, 0, 0, 0),
                    fontStyle = FontStyle.Bold
                };
                foldOutStyle.onNormal.textColor = openedFoldoutTitleColor;
            }
        }
    }
}