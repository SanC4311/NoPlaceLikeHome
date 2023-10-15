using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using TinyGiantStudio.Modules;

#if MODULAR_3D_TEXT
using TinyGiantStudio.Text;
#endif

using TinyGiantStudio.EditorHelpers;


namespace TinyGiantStudio.Layout
{
    [CustomEditor(typeof(LinearLayoutGroup))]
    public class LinearLayoutGroupEditor : Editor
    {
#if MODULAR_3D_TEXT
        static AssetSettings settings;
#endif

        private static GUIStyle foldOutStyle = null;
        private static Color openedFoldoutTitleColor = new Color(124 / 255f, 170 / 255f, 239 / 255f, 0.9f);

        LinearLayoutGroup myTarget;
        SerializedObject soTarget;


        SerializedProperty autoItemSize;
        SerializedProperty bounds;
        SerializedProperty spacing;
        SerializedProperty alignment;
        SerializedProperty elementUpdater;
        SerializedProperty alwaysUpdateInPlayMode;

        AnimBool showDebug;

        void OnEnable()
        {
            myTarget = (LinearLayoutGroup)target;
            soTarget = new SerializedObject(target);

#if MODULAR_3D_TEXT
            if (!settings)
                settings = StaticMethods.VerifySettings(settings);
#endif
            FindProperties();
        }


        public override void OnInspectorGUI()
        {
            GenerateStlye();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(alignment, GUIContent.none);
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(autoItemSize);
            EditorGUILayout.PropertyField(spacing);
            EditorGUILayout.Space(5);
            if (ModuleDrawer.ElementUpdatersExist())
                ModuleDrawer.ElementUpdaterContainerList("Element Updater", "", myTarget.elementUpdater, elementUpdater, soTarget);
            EditorGUILayout.Space(5);
            MText_Editor_Methods.ItalicHorizontalField(alwaysUpdateInPlayMode, "Always update in playmode", "For performance, it's better to leave it to false and call UpdateLayout() after making changes.\nTurn this on if you are in a hurry or testing stuff.", FieldSize.gigantic);
            EditorGUILayout.Space(5);

            DebugFoldout();
            if (EditorGUI.EndChangeCheck())
            {
                //soTarget.ApplyModifiedProperties();
                LinearLayoutGroup.Alignment anchor = myTarget.alignment;
                if (soTarget.ApplyModifiedProperties())
                {
#if MODULAR_3D_TEXT
                    if (myTarget.GetComponent<Modular3DText>())
                    {
                        if (anchor != myTarget.alignment)
                        {
                            myTarget.GetComponent<Modular3DText>().CleanUpdateText();
                        }
                        else
                        {
                            //if (!myTarget.GetComponent<Modular3DText>().ShouldItCreateChild())
                            {
                                myTarget.GetComponent<Modular3DText>().CleanUpdateText();
                            }
                        }
                    }
#endif
                }
                EditorUtility.SetDirty(myTarget);
            }
        }

        private static void GenerateStlye()
        {
#if MODULAR_3D_TEXT
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
#endif
            if (foldOutStyle == null)
            {
                foldOutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    overflow = new RectOffset(-10, 0, 3, 0),
                    padding = new RectOffset(15, 0, -3, 0),
                    fontStyle = FontStyle.Bold
                };
                foldOutStyle.onNormal.textColor = openedFoldoutTitleColor;
            }
        }

        private void DebugFoldout()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 1;
            GUILayout.BeginVertical(EditorStyles.toolbar);
            showDebug.target = EditorGUILayout.Foldout(showDebug.target, "Debug", true, foldOutStyle);
            GUILayout.EndVertical();
            if (EditorGUILayout.BeginFadeGroup(showDebug.faded))
            {
                EditorGUILayout.PropertyField(bounds);
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }

        private void FindProperties()
        {
            bounds = soTarget.FindProperty("bounds");
            autoItemSize = soTarget.FindProperty("autoItemSize");
            spacing = soTarget.FindProperty("spacing");
            alignment = soTarget.FindProperty("alignment");
            alwaysUpdateInPlayMode = soTarget.FindProperty("alwaysUpdateInPlayMode");
            elementUpdater = soTarget.FindProperty("elementUpdater");


            showDebug = new AnimBool(false);
            showDebug.valueChanged.AddListener(Repaint);
        }
    }
}