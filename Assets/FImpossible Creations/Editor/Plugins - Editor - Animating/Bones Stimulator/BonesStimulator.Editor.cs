using FIMSpace.FEditor;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.BonesStimulation
{
    [UnityEditor.CustomEditor(typeof(BonesStimulator))]
    [CanEditMultipleObjects]
    public partial class BonesStimulator_Editor : Editor
    {
        [MenuItem("CONTEXT/BonesStimulator/Switch displaying header bar")]
        private static void HideFImpossibleHeader(MenuCommand menuCommand)
        {
            int current = EditorPrefs.GetInt("FBonesStimHeader", 1);
            if (current == 1) current = 0; else current = 1;
            EditorPrefs.SetInt("FBonesStimHeader", current);
        }

        public override void OnInspectorGUI()
        {
            Undo.RecordObject(target, "Bones Stimulator Inspector");

            SetupLangs();

            serializedObject.Update();

            BonesStimulator Get = (BonesStimulator)target;
            string title = drawDefaultInspector ? " Default Inspector" : (" " + Get._editor_Title);

            if (EditorPrefs.GetInt("FBonesStimHeader", 1) == 1)
            {
                HeaderBoxMain(title, ref Get.DrawGizmos, ref drawDefaultInspector, _TexStimulIcon, Get, 27);
            }

            if (drawDefaultInspector)
                DrawDefaultInspector();
            else
                DrawNewGUI();

            serializedObject.ApplyModifiedProperties();

            if (Application.isPlaying)
            {
                Get._perf_main.Editor_DisplayFoldoutButton(-9, -5);
                if (Get._perf_main._foldout)
                {
                    Get._perf_preUpd.Editor_DisplayAlways("Preparation:");
                    Get._perf_main.Editor_DisplayAlways("Main Algorithm:");

                    long totalT = 0;
                    totalT += Get._perf_preUpd.AverageTicks;
                    totalT += Get._perf_main.AverageTicks;
                    dynamic totalMS = 0;
                    totalMS += Get._perf_preUpd.AverageMS;
                    totalMS += Get._perf_main.AverageMS;
                    EditorGUILayout.LabelField("Total = " + totalT + " ticks  " + totalMS + "ms");
                }
            }

        }


        void DrawNewGUI()
        {
            #region Preparations for unity versions and skin

            c = Color.Lerp(GUI.color * new Color(0.8f, 0.8f, 0.8f, 0.7f), GUI.color, Mathf.InverseLerp(0f, 0.15f, Get.StimulatorAmount));
            e = GUI.enabled;

            RectOffset zeroOff = new RectOffset(0, 0, 0, 0);
            float substr = -.1f; float bgAlpha = 0.3f; if (EditorGUIUtility.isProSkin) { bgAlpha = 0.1f; substr = 0f; }

//#if UNITY_2019_3_OR_NEWER
//            int headerHeight = 22;
//#else
//            int headerHeight = 25;
//#endif

            if (Get.Bones == null)
                Get.Bones = new System.Collections.Generic.List<BonesStimulator.Bone>();

            #endregion


            GUILayout.BeginVertical(FGUI_Resources.BGBoxStyle); GUILayout.Space(1f);


            // ------------------------------------------------------------------------

            GUILayout.BeginVertical(FGUI_Inspector.Style(zeroOff, zeroOff, new Color(.7f - substr, .7f - substr, 0.7f - substr, bgAlpha), Vector4.one * 3, 3));

            Tab_DrawSetup();

            GUILayout.EndVertical();


            // ------------------------------------------------------------------------

            GUILayout.BeginVertical(FGUI_Inspector.Style(zeroOff, zeroOff, new Color(.7f - substr, .7f - substr, .7f - substr, bgAlpha), Vector4.one * 3, 3));

            Tab_DrawTweaking();

            GUILayout.EndVertical();

            GUILayout.Space(2f);
            GUILayout.EndVertical();
        }




        private bool drawHeaderFoldout = false;
        private void HeaderBoxMain(string title, ref bool drawGizmos, ref bool defaultInspector, Texture2D scrIcon, MonoBehaviour target, int height = 22)
        {
            EditorGUILayout.BeginVertical(FGUI_Resources.HeaderBoxStyle);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(new GUIContent(scrIcon), EditorStyles.label, new GUILayoutOption[2] { GUILayout.Width(height - 2), GUILayout.Height(height - 2) }))
            {
                MonoScript script = MonoScript.FromMonoBehaviour(target);
                if (script) EditorGUIUtility.PingObject(script);
                drawHeaderFoldout = !drawHeaderFoldout;
            }

            if (GUILayout.Button(title, FGUI_Resources.GetTextStyle(14, true, TextAnchor.MiddleLeft), GUILayout.Height(height)))
            {
                MonoScript script = MonoScript.FromMonoBehaviour(target);
                if (script) EditorGUIUtility.PingObject(script);
                drawHeaderFoldout = !drawHeaderFoldout;
            }

            if (EditorGUIUtility.currentViewWidth > 326)
                // Youtube channel button
                if (GUILayout.Button(new GUIContent(FGUI_Resources.Tex_Tutorials, "Open FImpossible Creations Channel with tutorial videos in your web browser"), FGUI_Resources.ButtonStyle, new GUILayoutOption[2] { GUILayout.Width(height), GUILayout.Height(height) }))
                {
                    Application.OpenURL("https://www.youtube.com/c/FImpossibleCreations");
                }

            if (EditorGUIUtility.currentViewWidth > 292)
                // Store site button
                if (GUILayout.Button(new GUIContent(FGUI_Resources.Tex_Website, "Open FImpossible Creations Asset Store Page inside your web browser"), FGUI_Resources.ButtonStyle, new GUILayoutOption[2] { GUILayout.Width(height), GUILayout.Height(height) }))
                {
                    Application.OpenURL("https://assetstore.unity.com/publishers/37262");
                }

            // Manual file button
            if (_manualFile == null) _manualFile = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(target))) + "/Bones Stimulator - User Manual.pdf");
            if (_manualFile)
                if (GUILayout.Button(new GUIContent(FGUI_Resources.Tex_Manual, "Open .PDF user manual file for Bones Stimulator"), FGUI_Resources.ButtonStyle, new GUILayoutOption[2] { GUILayout.Width(height), GUILayout.Height(height) }))
                {
                    EditorGUIUtility.PingObject(_manualFile);
                    Application.OpenURL(Application.dataPath + "/" + AssetDatabase.GetAssetPath(_manualFile).Replace("Assets/", ""));
                }

            FGUI_Inspector.DrawSwitchButton(ref drawGizmos, FGUI_Resources.Tex_GizmosOff, FGUI_Resources.Tex_Gizmos, "Toggle drawing gizmos on character in scene window", height, height, true);
            FGUI_Inspector.DrawSwitchButton(ref drawHeaderFoldout, FGUI_Resources.Tex_LeftFold, FGUI_Resources.Tex_DownFold, "Toggle to view additional options for foldouts", height, height);

            EditorGUILayout.EndHorizontal();

            if (drawHeaderFoldout)
            {
                FGUI_Inspector.DrawUILine(0.07f, 0.1f, 1, 4, 0.99f);

                EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginChangeCheck();
                choosedLang = (ELangs)EditorGUILayout.EnumPopup(choosedLang, new GUIStyle(EditorStyles.layerMaskField) { fixedHeight = 0 }, new GUILayoutOption[2] { GUILayout.Width(80), GUILayout.Height(22) });
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetInt("FimposLang", (int)choosedLang);
                    SetupLangs();
                }

                GUILayout.FlexibleSpace();


                bool hierSwitchOn = PlayerPrefs.GetInt("AnimsH", 1) == 1;
                FGUI_Inspector.DrawSwitchButton(ref hierSwitchOn, FGUI_Resources.Tex_HierSwitch, null, "Switch drawing small icons in hierarchy", height, height, true);
                PlayerPrefs.SetInt("AnimsH", hierSwitchOn ? 1 : 0);


                if (GUILayout.Button(new GUIContent(FGUI_Resources.Tex_Rename, "Change component title to yours (current: '" + Get._editor_Title + "'"), FGUI_Resources.ButtonStyle, new GUILayoutOption[2] { GUILayout.Width(height), GUILayout.Height(height) }))
                {
                    string filename = EditorUtility.SaveFilePanelInProject("Type your title (no file will be created)", Get._editor_Title, "", "Type your title (no file will be created)");
                    if (!string.IsNullOrEmpty(filename))
                    {
                        filename = System.IO.Path.GetFileNameWithoutExtension(filename);
                        if (!string.IsNullOrEmpty(filename))
                        { Get._editor_Title = filename; serializedObject.ApplyModifiedProperties(); }
                    }
                }

                // Default inspector switch
                FGUI_Inspector.DrawSwitchButton(ref defaultInspector, FGUI_Resources.Tex_Default, null, "Toggle inspector view to default inspector.\n\nIf you ever edit source code of Biped Animator and add custom variables, you can see them by entering this mode, also sometimes there can be additional/experimental variables to play with.", height, height);

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

    }

}