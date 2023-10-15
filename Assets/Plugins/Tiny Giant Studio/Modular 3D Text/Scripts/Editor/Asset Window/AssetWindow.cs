using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

using TinyGiantStudio.Text.FontCreation;
using TinyGiantStudio.EditorHelpers;
using System;

namespace TinyGiantStudio.Text
{
    public class AssetWindow : EditorWindow
    {
        SerializedObject soTarget;

        private readonly string version = "4.2.1";
        private readonly string documentationURL = "https://ferdowsur.gitbook.io/modular-3d-text/";
        private readonly string fontCreationTroubleShootDocURL = "https://ferdowsur.gitbook.io/modular-3d-text/fonts/creating-fonts/troubleshoot";
        //private readonly string fontCreationInEditor_WrittenTutorialURL = "https://ferdowsur.gitbook.io/modular-3d-text/creating-fonts";
        //private readonly string fontCreationInEditor_VideoTutorialURL = "https://youtu.be/m9JwBc-0DUA";
        private readonly string forumURL = "https://forum.unity.com/threads/modular-3d-text-3d-texts-for-your-3d-game.821931/";
        private readonly string supportEmail = "ferdowsurasif@gmail.com";



        public AssetSettings settings;
        private int tab = 0;
        //private bool neverStartedBefore = true;


        GUIStyle tabStyle;
        GUIStyle foldOutStyle;
        GUIStyle bottomInformationStyle;
        GUIStyle groupHeaderStyle;
        GUIStyle groupHeaderButtonStyle;
        GUIStyle labelHeaderStyle;
        GUIStyle labelStyle;
        GUIStyle paragraphStyle;
        GUIStyle urlStyle;
        GUIStyle buttonStyle;
        GUIStyle createFontButtonStyle;

        static Texture titleIcon;

        static Texture iconsPackAssetIcon;
        static Texture betterTransformAssetIcon;
        static Texture modularToDoListsAssetIcon;

        private static Color openedFoldoutTitleColor = new Color(124 / 255f, 170 / 255f, 239 / 255f, 0.9f);

        AnimBool showInputSettings;
        AnimBool showDefaultTextSettings;
        AnimBool showDefaultButtonSettings;
        AnimBool showDefaultListSettings;
        Vector2 scrollPos;



        void OnEnable()
        {
            titleIcon = EditorGUIUtility.Load("Assets/Plugins/Tiny Giant Studio/Modular 3D Text/Utility/Editor Icons/M3D.png") as Texture;
            iconsPackAssetIcon = EditorGUIUtility.Load("Assets/Plugins/Tiny Giant Studio/Modular 3D Text/Utility/Editor Icons/iconsPack.png") as Texture;
            betterTransformAssetIcon = EditorGUIUtility.Load("Assets/Plugins/Tiny Giant Studio/Modular 3D Text/Utility/Editor Icons/betterTransformIcon.png") as Texture;
            modularToDoListsAssetIcon = EditorGUIUtility.Load("Assets/Plugins/Tiny Giant Studio/Modular 3D Text/Utility/Editor Icons/toDoListIcon.png") as Texture;

            VerifyReferences();

            if (soTarget == null)
                soTarget = new SerializedObject(settings);

            showInputSettings = new AnimBool(false);
            showInputSettings.valueChanged.AddListener(Repaint);

            showDefaultTextSettings = new AnimBool(false);
            showDefaultTextSettings.valueChanged.AddListener(Repaint);
            showDefaultButtonSettings = new AnimBool(false);
            showDefaultButtonSettings.valueChanged.AddListener(Repaint);
            showDefaultListSettings = new AnimBool(false);
            showDefaultListSettings.valueChanged.AddListener(Repaint);

#if ENABLE_INPUT_SYSTEM
            if (settings)
            {
                if (!settings.InputActionAsset)
                {
                    settings.FindModularTextInputActionAsset();
                }
            }
#endif
        }





        [UnityEditor.MenuItem("Tools/Tiny Giant Studio/Modular 3D Text", false, 100)]
        public static void ShowWindow()
        {
            EditorWindow editorWindow = GetWindow(typeof(AssetWindow), false, "Modular 3D Text");
            editorWindow.titleContent = new GUIContent(" Modular 3D Text", titleIcon, "");
            editorWindow.minSize = new Vector2(450, 700);
            editorWindow.Show();
        }
        void OnGUI()
        {
            soTarget ??= new SerializedObject(settings);

            GenerateStyle();
            EditorGUI.BeginChangeCheck();

            GUILayout.Space(5);
            Tabs();

            GUILayout.Space(10);

            ProductInformation();
            GUILayout.Space(5);

            if (EditorGUI.EndChangeCheck())
            {
                soTarget?.ApplyModifiedProperties();
                EditorUtility.SetDirty(settings);
            }
        }







        void Tabs()
        {
            tab = GUILayout.Toolbar(tab, new string[] { "Information", "Create Font", "Preference", "Utility", "More Assets!" }, GUILayout.MinHeight(30));
            switch (tab)
            {
                case 0:
                    GeneralInformation();
                    break;
                case 1:
                    FontCreation();
                    break;
                case 2:
                    Preference();
                    break;
                case 3:
                    UtilityTab();
                    break;
                case 4:
                    MoreAssets();
                    break;
                default:
                    GeneralInformation();
                    break;
            }
        }

        void GeneralInformation()
        {
            GridContent("Documentation", "There is no better place to get started than the documentation.", "Documentation", documentationURL);

            GUILayout.BeginHorizontal();
            Forum("Forum", "Join the conversation in Unity Forum.\nI would love to see what you are using the asset for.", "Unity Forum", forumURL);
            SupportLink();
            GUILayout.EndHorizontal();

            GridContent("Review", "Reviews are one of the primary methods of getting discovered in Unity Asset store. If you have some time, please rate the asset a 5 star. I would really appriciate it. \nEither way, thank you for buying the asset! You keep me motivated to continuously improve the asset.", "Rate the Asset", "https://assetstore.unity.com/packages/3d/gui/modular-3d-text-in-game-3d-ui-system-159508?aid=1011ljxWe&utm_source=aff");
            //SetupAsseblyDefinitionFilesButton();
        }

        void FontCreation()
        {
            GUILayout.Space(5);

            CharacterInput();

            GUILayout.Space(10);

            MeshSettings(); //vertext density, size xy, smoothing angle etc.

            GUILayout.Space(15);

            CreateFontButton();

            BottomButtons();

            GUILayout.Label("Note: You can select the exported font for detailed controls", EditorStyles.centeredGreyMiniLabel);
        }

        void Preference()
        {
            if (settings)
            {
                GUILayout.Space(5);
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUIStyle.none, GUI.skin.verticalScrollbar);

                InputPreference();

                GUILayout.BeginVertical(EditorStyles.helpBox);//Start of default values

                GUILayout.BeginVertical(EditorStyles.toolbar);
                GUILayout.Label("Default values", groupHeaderStyle);
                GUILayout.EndVertical();

                GUILayout.Space(5);

                TextPreference();
                GUILayout.Space(15);

                ButtonPreference();
                GUILayout.Space(15);

                ListPreference();
                GUILayout.EndVertical(); //End of default values

                EditorGUILayout.EndScrollView();
            }
        }


        void UtilityTab()
        {
            SetupAsseblyDefinitionFilesButton();
            GUILayout.Space(25);
            HideFlagsButtons();
        }
        void SetupAsseblyDefinitionFilesButton()
        {
            GUILayout.Space(5);
            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("Assembly Definition File", groupHeaderStyle);
            if (IfAssemblyDefinitionAlreadyExists())
            {
                GUILayout.Label(new GUIContent("(Exists)", "Found assembly definition files in 'Assets/Plugins/Tiny Giant Studio/Modular 3D Text/Scripts/Modular 3D Text.asmdef' & 'Assets/Plugins/Tiny Giant Studio/Modular 3D Text/Scripts/Editor/Modular 3D Text Editor.asmdef'"), paragraphStyle, GUILayout.MaxWidth(46));
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            string description = "Assembly Definitions are assets that you can create to organize your scripts into assemblies. " +
                "If you decide to use assebly definitions, remember to have those reference the Modular 3d Text one.";
            GUILayout.Label(description, paragraphStyle);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Setup assebly definition files", buttonStyle, GUILayout.MinHeight(25)))
            {
                SetupAsseblyDefinition();
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        void SetupAsseblyDefinition()
        {
            string path = "Assets/Tiny Giant Studio/Modular 3D Text/Setup Files/Assembly Definition.unitypackage";
            //#if ENABLE_INPUT_SYSTEM
            //            path = "Assets/Tiny Giant Studio/Modular 3D Text/Setup Files/Assembly Definition with New Input System.unitypackage";
            //#endif

            if (string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(path))) //package do not exist.
            {
                string msg = "Can't find package in the correct folder. If it has been moved, please manually install it from:\n" + path + "\nIf it has been removed, please add redownload it from the asset store.";
                Debug.Log(msg);
                EditorUtility.DisplayDialog("Failed!", msg, "Ok");
            }
            else
            {
                AssetDatabase.ImportPackage(path, false);
                EditorUtility.DisplayDialog("Success !", "Assebly definition files have been installed.", "Ok");
            }
        }

        bool IfAssemblyDefinitionAlreadyExists()
        {
            return
                File.Exists("Assets/Plugins/Tiny Giant Studio/Modular 3D Text/Scripts/Modular 3D Text.asmdef")
                &&
                File.Exists("Assets/Plugins/Tiny Giant Studio/Modular 3D Text/Scripts/Editor/Modular 3D Text Editor.asmdef");
        }

        void HideFlagsButtons()
        {
            if (GUILayout.Button("Mark all object in scene visible in hierarchy "))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var t in Object.FindObjectsByType<Transform>(FindObjectsSortMode.None))
#else
                foreach (var t in FindObjectsOfType<Transform>())
#endif
                {
                    if (t.gameObject.hideFlags != HideFlags.None)
                    {
                        t.gameObject.hideFlags = HideFlags.None;
                    }
                }
            }

            if (GUILayout.Button("Hide selected objects in hierarchy "))
            {
                foreach (var t in Selection.objects)
                {
                    if (t.GetType() == typeof(GameObject))
                    {
                        t.hideFlags = HideFlags.HideInHierarchy;
                    }
                }
                EditorApplication.DirtyHierarchyWindowSorting();
            }
        }

        void MoreAssets()
        {
            AssetContent("3D Icons Mega Pack", "Get a pack of useful icons to go with your 3D UI.", "Show in Asset Store", "https://assetstore.unity.com/packages/3d/gui/3d-icons-mega-pack-173600?aid=1011ljxWe&pubref=Modular3DText", iconsPackAssetIcon);
            AssetContent("Better Transform", "The way the transform inspector should have looked by default.", "Show in Asset Store", "https://assetstore.unity.com/packages/tools/utilities/better-transform-258314?aid=1011ljxWe&pubref=Modular3DText", betterTransformAssetIcon);
            AssetContent("Modular To Do Lists", "The ultimate offline unity project management asset.", "Show in Asset Store", "https://assetstore.unity.com/packages/tools/utilities/modular-to-do-lists-pro-offline-project-management-tool-262659?aid=1011ljxWe&pubref=Modular3DText", modularToDoListsAssetIcon);
        }


        #region URL Methods
        void Forum(string title, string description = "", string urlText = "", string url = "", string urlText2 = "", string url2 = "")
        {
            GUILayout.Space(5);
            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label(title, groupHeaderStyle);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            if (description != string.Empty)
                GUILayout.Label(description, paragraphStyle);
            GUILayout.BeginHorizontal();
            if (urlText != string.Empty && url != string.Empty)
            {
                if (GUILayout.Button(urlText, buttonStyle, GUILayout.MinHeight(25)))
                {
                    Application.OpenURL(url);
                }
            }
            if (urlText2 != string.Empty && url2 != string.Empty)
            {
                if (GUILayout.Button(urlText2, buttonStyle, GUILayout.MinHeight(25)))
                {
                    Application.OpenURL(url2);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
        void SupportLink()
        {
            GUILayout.Space(5);
            string description = "Need assistance with anything? Always happy to help.";

            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("Support", groupHeaderStyle);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            GUILayout.Label(description, paragraphStyle);
            GUILayout.Label(supportEmail, labelHeaderStyle);
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Send Email", buttonStyle, GUILayout.MinHeight(25)))
            {
                Application.OpenURL("mailto:" + supportEmail + "?subject=Modular 3D Text Support");
            }
            if (GUILayout.Button("Copy", buttonStyle, GUILayout.MinHeight(25)))
            {
                GUIUtility.systemCopyBuffer = supportEmail;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
        #endregion URL Methods

        void InputPreference()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 0;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            showInputSettings.target = EditorGUILayout.Foldout(showInputSettings.target, "Input", true, foldOutStyle);
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showInputSettings.faded))
            {

                MText_Editor_Methods.HorizontalField(soTarget.FindProperty("autoCreateSceneInputSystem"), "Auto Create Scene Input System", "If enabled, will automatically create the M3D Input System gameobject in the scene when appropriate 3d ui is created.", FieldSize.gigantic);
#if ENABLE_INPUT_SYSTEM
                GUILayout.BeginHorizontal();
                MText_Editor_Methods.HorizontalField(soTarget.FindProperty("_inputActionAsset"), "Input Action Asset", "Default input action asset assigned", FieldSize.large);
                if (GUILayout.Button("Apply to scene", GUILayout.MaxWidth(100)))
                {
                    ApplyInputActionAssetToScene();
                }
                GUILayout.EndHorizontal();
#endif
                var xRToolkitGlobalInputControllerForModularTextAsset = Type.GetType("TinyGiantStudio.Text.XRToolkitEditorSetup");
                if (xRToolkitGlobalInputControllerForModularTextAsset != null)
                {
                    MText_Editor_Methods.HorizontalField(soTarget.FindProperty(nameof(settings.dontAutoCreateRaycasterOrButtonIfVRtoolkitExists)), "Dont auto create raycaster if XR toolkit exists", "", FieldSize.mega);
                }
            }

            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }

        void TextPreference()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 0;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            showDefaultTextSettings.target = EditorGUILayout.Foldout(showDefaultTextSettings.target, "Text", true, foldOutStyle);
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showDefaultTextSettings.faded))
            {
                GUILayout.Space(5f);

                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Font", labelStyle, GUILayout.MaxWidth(40));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultFont"), GUIContent.none);
                if (GUILayout.Button("Apply", buttonStyle, GUILayout.MaxWidth(50)))
                {
                    ApplyDefaultFontToScene();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Size", labelStyle, GUILayout.MaxWidth(40));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultTextSize"), GUIContent.none);
                if (GUILayout.Button("Apply", buttonStyle, GUILayout.MaxWidth(50)))
                {
                    ApplyDefaultTextSizeToScene();
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.EndVertical();

                EditorGUILayout.BeginHorizontal();
                MText_Editor_Methods.PriviewField(soTarget.FindProperty("defaultTextMaterial"), settings.defaultTextMaterial, "Text Material");

                if (GUILayout.Button("Apply", buttonStyle, GUILayout.MaxWidth(50)))
                {
                    ApplyDefaultTextMaterialToScene();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }
        void ButtonPreference()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginVertical(EditorStyles.toolbar);
            showDefaultButtonSettings.target = EditorGUILayout.Foldout(showDefaultButtonSettings.target, "Button", true, foldOutStyle);
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showDefaultButtonSettings.faded))
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginVertical(EditorStyles.toolbar);
                EditorGUILayout.LabelField("Normal", labelHeaderStyle);
                EditorGUILayout.EndVertical();
                GUILayout.Space(5);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Text size", labelStyle, GUILayout.MaxWidth(80));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultButtonNormalTextSize"), GUIContent.none);
                if (GUILayout.Button("Apply", GUILayout.MaxWidth(50)))
                    ApplyDefaultButtonNormalTextSizeToScene();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginHorizontal();
                MText_Editor_Methods.PriviewField(soTarget.FindProperty("defaultButtonNormalTextMaterial"), settings.defaultButtonNormalTextMaterial, "Text Material");
                if (GUILayout.Button("Apply", GUILayout.MaxWidth(50)))
                    ApplyDefaultButtonNormalTextMaterialToScene();
                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();
                MText_Editor_Methods.PriviewField(soTarget.FindProperty("defaultButtonNormalBackgroundMaterial"), settings.defaultButtonNormalBackgroundMaterial, "Background Material");
                if (GUILayout.Button("Apply", GUILayout.MaxWidth(50)))
                    ApplyDefaultButtonNormalBackgroundMaterialToScene();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndHorizontal();

                GUILayout.EndVertical();


                GUILayout.Space(10);


                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginVertical(EditorStyles.toolbar);
                EditorGUILayout.LabelField("Selected", labelHeaderStyle);
                EditorGUILayout.EndVertical();
                GUILayout.Space(5);


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Text Size", labelStyle, GUILayout.MaxWidth(80));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultButtonSelectedTextSize"), GUIContent.none);
                if (GUILayout.Button("Apply", GUILayout.MaxWidth(50)))
                    ApplyDefaultButtonSelectedTextSizeToScene();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                MText_Editor_Methods.PriviewField(soTarget.FindProperty("defaultButtonSelectedTextMaterial"), settings.defaultButtonSelectedTextMaterial, "Text Material");
                if (GUILayout.Button("Apply", GUILayout.MaxWidth(50)))
                    ApplyDefaultButtonSelectedTextMaterialToScene();

                MText_Editor_Methods.PriviewField(soTarget.FindProperty("defaultButtonSelectedBackgroundMaterial"), settings.defaultButtonSelectedBackgroundMaterial, "Backgroud Material");
                if (GUILayout.Button("Apply" +
                    "", GUILayout.MaxWidth(50)))
                    ApplyDefaultButtonSelectedBackgroundMaterialToScene();
                EditorGUILayout.EndHorizontal();

                GUILayout.EndVertical();

                GUILayout.Space(10);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginVertical(EditorStyles.toolbar);
                EditorGUILayout.LabelField("Pressed", labelHeaderStyle);
                EditorGUILayout.EndVertical();
                GUILayout.Space(5);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Text Size", labelStyle, GUILayout.MaxWidth(80));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultButtonPressedTextSize"), GUIContent.none);
                if (GUILayout.Button("Apply", GUILayout.MaxWidth(50)))
                    ApplyDefaultButtonPressedTextSizeToScene();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                MText_Editor_Methods.PriviewField(soTarget.FindProperty("defaultButtonPressedTextMaterial"), settings.defaultButtonPressedTextMaterial, "Text Material");
                if (GUILayout.Button("Apply", GUILayout.MaxWidth(50)))
                    ApplyDefaultButtonPressedTextMaterialToScene();

                MText_Editor_Methods.PriviewField(soTarget.FindProperty("defaultButtonPressedBackgroundMaterial"), settings.defaultButtonPressedBackgroundMaterial, "Backgroud Material");
                if (GUILayout.Button("Apply", GUILayout.MaxWidth(50)))
                    ApplyDefaultButtonPressedBackgroundMaterialToScene();
                EditorGUILayout.EndHorizontal();
                GUILayout.EndVertical();

                GUILayout.Space(10);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginVertical(EditorStyles.toolbar);
                EditorGUILayout.LabelField("Disabled", labelHeaderStyle);
                EditorGUILayout.EndVertical();
                GUILayout.Space(5);


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Text Size", labelStyle, GUILayout.MaxWidth(80));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultButtonDisabledTextSize"), GUIContent.none);
                if (GUILayout.Button("Apply", GUILayout.MaxWidth(50)))
                    ApplyDefaultButtonDisabledTextSizeToScene();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                MText_Editor_Methods.PriviewField(soTarget.FindProperty("defaultButtonDisabledTextMaterial"), settings.defaultButtonDisabledTextMaterial, "Text Material");
                if (GUILayout.Button("Apply", GUILayout.MaxWidth(50)))
                    ApplyDefaultButtonDisabledTextMaterialToScene();


                MText_Editor_Methods.PriviewField(soTarget.FindProperty("defaultButtonDisabledBackgroundMaterial"), settings.defaultButtonDisabledBackgroundMaterial, "Background Material");
                if (GUILayout.Button("Apply", GUILayout.MaxWidth(50)))
                    ApplyDefaultButtonDisabledBackgroundMaterialToScene();
                EditorGUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }
        void ListPreference()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginVertical(EditorStyles.toolbar);
            showDefaultListSettings.target = EditorGUILayout.Foldout(showDefaultListSettings.target, "List", true, foldOutStyle);
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showDefaultListSettings.faded))
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginVertical(EditorStyles.toolbar);
                EditorGUILayout.LabelField("Normal", labelHeaderStyle);
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Text Size", labelStyle, GUILayout.MaxWidth(80));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultListNormalTextSize"), GUIContent.none);
                if (GUILayout.Button("Apply", GUILayout.MaxWidth(50)))
                    ApplyDefaultListNormalTextSizeToScene();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                MText_Editor_Methods.PriviewField(soTarget.FindProperty("defaultListNormalTextMaterial"), settings.defaultListNormalTextMaterial, "Text Material");
                if (GUILayout.Button("Apply", GUILayout.MaxWidth(50)))
                    ApplyDefaultListNormalTextMaterialToScene();

                MText_Editor_Methods.PriviewField(soTarget.FindProperty("defaultListNormalBackgroundMaterial"), settings.defaultListNormalBackgroundMaterial, "Background Material");
                if (GUILayout.Button("Apply", GUILayout.MaxWidth(50)))
                    ApplyDefaultListNormalBackgroundMaterialToScene();
                EditorGUILayout.EndHorizontal();

                GUILayout.EndVertical();


                GUILayout.Space(10);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginVertical(EditorStyles.toolbar);
                EditorGUILayout.LabelField("Selected", labelHeaderStyle);
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Text Size", GUILayout.MaxWidth(80));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultListSelectedTextSize"), GUIContent.none);
                if (GUILayout.Button("Apply", GUILayout.MaxWidth(50)))
                    ApplyDefaultListSelectedTextSizeToScene();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                MText_Editor_Methods.PriviewField(soTarget.FindProperty("defaultListSelectedTextMaterial"), settings.defaultListSelectedTextMaterial, "Text Material");
                if (GUILayout.Button("Apply", GUILayout.MaxWidth(50)))
                    ApplyDefaultListSelectedTextMaterialToScene();

                MText_Editor_Methods.PriviewField(soTarget.FindProperty("defaultListSelectedBackgroundMaterial"), settings.defaultListSelectedBackgroundMaterial, "Background Material");
                if (GUILayout.Button("Apply", GUILayout.MaxWidth(50)))
                    ApplyDefaultListSelectedBackgroundMaterialToScene();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();

                GUILayout.Space(10);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginVertical(EditorStyles.toolbar);
                EditorGUILayout.LabelField("Pressed", labelHeaderStyle);
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Text Size", labelStyle, GUILayout.MaxWidth(80));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultListPressedTextSize"), GUIContent.none);
                if (GUILayout.Button("Apply", GUILayout.MaxWidth(50)))
                    ApplyDefaultListPressedTextSizeToScene();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                MText_Editor_Methods.PriviewField(soTarget.FindProperty("defaultListPressedTextMaterial"), settings.defaultListPressedTextMaterial, "Text Material");
                if (GUILayout.Button("Apply", GUILayout.MaxWidth(50)))
                    ApplyDefaultListPressedTextMaterialToScene();

                MText_Editor_Methods.PriviewField(soTarget.FindProperty("defaultListPressedBackgroundMaterial"), settings.defaultListPressedBackgroundMaterial, "Background Material");
                if (GUILayout.Button("Apply", GUILayout.MaxWidth(50)))
                    ApplyDefaultListPressedBackgroundMaterialToScene();
                EditorGUILayout.EndHorizontal();

                GUILayout.EndVertical();

                GUILayout.Space(10);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginVertical(EditorStyles.toolbar);
                EditorGUILayout.LabelField("Disabled", labelHeaderStyle);
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Text Size", GUILayout.MaxWidth(80));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultListDisabledTextSize"), GUIContent.none);
                if (GUILayout.Button("Apply", GUILayout.MaxWidth(50)))
                    ApplyDefaultListDisabledTextSizeToScene();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                MText_Editor_Methods.PriviewField(soTarget.FindProperty("defaultListDisabledTextMaterial"), settings.defaultListDisabledTextMaterial, "Text Material");
                if (GUILayout.Button("Apply", GUILayout.MaxWidth(50)))
                    ApplyDefaultListDisabledTextMaterialToScene();

                MText_Editor_Methods.PriviewField(soTarget.FindProperty("defaultListDisabledBackgroundMaterial"), settings.defaultListDisabledBackgroundMaterial, "Background Material");
                if (GUILayout.Button("Apply", GUILayout.MaxWidth(100)))
                    ApplyDefaultListDisabledBackgroundMaterialToScene();
                EditorGUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }




        void ProductInformation()
        {
            GUILayout.FlexibleSpace();

            DrawUILine(new Color(1, 1, 1, 0.25f), 1, 0);
            GUILayout.Space(5);
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Version : " + version, bottomInformationStyle);

            GUILayout.FlexibleSpace();
            //GUILayout.Label("Created by Ferdowsur Asif @ Tiny Giant Studio.", bottomInformationStyle);
            GUILayout.Label("Created by ", bottomInformationStyle);
            if (GUILayout.Button("Ferdowsur Asif", urlStyle))
            {
                Application.OpenURL("https://linktr.ee/ferdowsurasif");
            }
            GUILayout.Label(" of ", bottomInformationStyle);
            if (GUILayout.Button("Tiny Giant Studio", urlStyle))
            {
                Application.OpenURL("https://linktr.ee/tinygiantstudio");
            }
            GUILayout.EndHorizontal();
            //GUILayout.Label("Modular 3D Text \n" + version, EditorStyles.boldLabel);
            //EditorGUILayout.LabelField("Created by Ferdowsur Asif @ Tiny Giant Studio.", EditorStyles.miniBoldLabel);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("(Check Update)", urlStyle))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/3d/gui/modular-3d-text-in-game-3d-ui-system-159508?aid=1011ljxWe&utm_source=aff");
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Join in forum", urlStyle))
            {
                Application.OpenURL(forumURL);
            }
            GUILayout.Label(" | ", bottomInformationStyle);
            if (GUILayout.Button("Need help? Email", urlStyle))
            {
                Application.OpenURL("mailto:" + supportEmail + "?subject=Modular 3D Text Support");
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        void ApplyDefaultFontToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultFont.name + "' font to every text active object in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var text in Object.FindObjectsByType<Modular3DText>(FindObjectsSortMode.None))
#else
                foreach (var text in FindObjectsOfType<Modular3DText>())
#endif
                {
                    text.Font = settings.defaultFont;
                    EditorUtility.SetDirty(text);
                }
            }
        }
        void ApplyDefaultTextSizeToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultTextSize + "' font size to every text active object in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var text in Object.FindObjectsByType<Modular3DText>(FindObjectsSortMode.None))
#else
                foreach (var text in FindObjectsOfType<Modular3DText>())
#endif
                {
                    if (ShouldItApplyTextPreference(text))
                        continue;

                    text.FontSize = settings.defaultTextSize;
                    EditorUtility.SetDirty(text);
                }
            }
        }



        void ApplyDefaultTextMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultTextMaterial.name + "' material to every active text in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {

#if UNITY_2023_1_OR_NEWER
                foreach (var text in Object.FindObjectsByType<Modular3DText>(FindObjectsSortMode.None))
#else
                foreach (var text in FindObjectsOfType<Modular3DText>())
#endif
                {
                    if (ShouldItApplyTextPreference(text))
                        continue;

                    text.Material = settings.defaultTextMaterial;
                    EditorUtility.SetDirty(text);
                }
            }
        }
        bool ShouldItApplyTextPreference(Modular3DText text)
        {
            if (text.transform.parent)
            {
                var button = text.transform.parent.GetComponent<Button>();
                if (button)
                {
                    var applyNormalStyle = button.ApplyNormalStyle();
                    if (applyNormalStyle.Item1 || applyNormalStyle.Item2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
#if ENABLE_INPUT_SYSTEM
        void ApplyInputActionAssetToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.InputActionAsset.name + "' to every player controller active object in the scene?" +
               "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
                PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
                for (int i = 0; i < playerInputs.Length; i++)
                {
                    playerInputs[i].actions = settings.InputActionAsset;
                    EditorUtility.SetDirty(playerInputs[i]);
                }
            }
        }
#endif

        #region Button
        //Button
        void ApplyDefaultButtonNormalTextSizeToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultButtonNormalTextSize + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var button in Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
#else
                foreach (var button in FindObjectsOfType<Button>())
#endif
                {
                    button.NormalTextSize = settings.defaultButtonNormalTextSize;
                    if (!Application.isPlaying)
                        button.UnselectedButtonVisualUpdate();
                    EditorUtility.SetDirty(button);
                }
            }
        }
        void ApplyDefaultButtonNormalTextMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultButtonNormalTextMaterial.name + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var button in Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
#else
                foreach (var button in FindObjectsOfType<Button>())
#endif
                {
                    button.NormalTextMaterial = settings.defaultButtonNormalTextMaterial;
                    if (!Application.isPlaying)
                        button.UnselectedButtonVisualUpdate();
                    EditorUtility.SetDirty(button);
                }
            }
        }
        void ApplyDefaultButtonNormalBackgroundMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultButtonNormalBackgroundMaterial.name + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var button in Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
#else
                foreach (var button in FindObjectsOfType<Button>())
#endif
                {
                    button.NormalBackgroundMaterial = settings.defaultButtonNormalBackgroundMaterial;
                    if (!Application.isPlaying)
                        button.UnselectedButtonVisualUpdate();
                    EditorUtility.SetDirty(button);
                }
            }
        }

        void ApplyDefaultButtonSelectedTextSizeToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultButtonSelectedTextSize + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var button in Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
#else
                foreach (var button in FindObjectsOfType<Button>())
#endif
                {
                    button.SelectedTextSize = settings.defaultButtonSelectedTextSize;
                    if (!Application.isPlaying)
                        button.UnselectedButtonVisualUpdate();
                    EditorUtility.SetDirty(button);
                }
            }
        }
        void ApplyDefaultButtonSelectedTextMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultButtonSelectedTextMaterial.name + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var button in Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
#else
                foreach (var button in FindObjectsOfType<Button>())
#endif               
                {
                    button.SelectedTextMaterial = settings.defaultButtonSelectedTextMaterial;
                    EditorUtility.SetDirty(button);
                }
            }
        }
        void ApplyDefaultButtonSelectedBackgroundMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultButtonSelectedBackgroundMaterial.name + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var button in Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
#else
                foreach (var button in FindObjectsOfType<Button>())
#endif
                {
                    button.SelectedBackgroundMaterial = settings.defaultButtonSelectedBackgroundMaterial;
                    EditorUtility.SetDirty(button);
                }
            }
        }

        void ApplyDefaultButtonPressedTextSizeToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultButtonPressedTextSize + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var button in Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
#else
                foreach (var button in FindObjectsOfType<Button>())
#endif
                {
                    button.PressedTextSize = settings.defaultButtonPressedTextSize;
                    EditorUtility.SetDirty(button);
                }
            }
        }
        void ApplyDefaultButtonPressedTextMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultButtonPressedTextMaterial.name + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var button in Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
#else
                foreach (var button in FindObjectsOfType<Button>())
#endif
                {
                    button.PressedTextMaterial = settings.defaultButtonPressedTextMaterial;
                    EditorUtility.SetDirty(button);
                }
            }
        }
        void ApplyDefaultButtonPressedBackgroundMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultButtonPressedBackgroundMaterial.name + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var button in Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
#else
                foreach (var button in FindObjectsOfType<Button>())
#endif
                {
                    button.PressedBackgroundMaterial = settings.defaultButtonPressedBackgroundMaterial;
                    EditorUtility.SetDirty(button);
                }
            }
        }

        void ApplyDefaultButtonDisabledTextSizeToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultButtonDisabledTextSize + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var button in Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
#else
                foreach (var button in FindObjectsOfType<Button>())
#endif
                {
                    button.DisabledTextSize = settings.defaultButtonDisabledTextSize;
                    EditorUtility.SetDirty(button);
                }
            }
        }
        void ApplyDefaultButtonDisabledTextMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultButtonDisabledTextMaterial.name + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var button in Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
#else
                foreach (var button in FindObjectsOfType<Button>())
#endif
                {
                    button.DisabledTextMaterial = settings.defaultButtonDisabledTextMaterial;
                    EditorUtility.SetDirty(button);
                }
            }
        }
        void ApplyDefaultButtonDisabledBackgroundMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultButtonDisabledBackgroundMaterial.name + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var button in Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
#else
                foreach (var button in FindObjectsOfType<Button>())
#endif
                {
                    button.DisabledBackgroundMaterial = settings.defaultButtonDisabledBackgroundMaterial;
                    EditorUtility.SetDirty(button);
                }
            }
        }
        #endregion Button



        #region List
        void ApplyDefaultListNormalTextSizeToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultListNormalTextSize + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var list in Object.FindObjectsByType<List>(FindObjectsSortMode.None))
#else
                foreach (var list in FindObjectsOfType<List>())
#endif
                {
                    list.NormalTextSize = settings.defaultListNormalTextSize;
                    EditorUtility.SetDirty(list);
                }
            }
        }
        void ApplyDefaultListNormalTextMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultButtonNormalTextMaterial.name + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var list in Object.FindObjectsByType<List>(FindObjectsSortMode.None))
#else
                foreach (var list in FindObjectsOfType<List>())
#endif
                {
                    list.NormalTextMaterial = settings.defaultListNormalTextMaterial;
                    EditorUtility.SetDirty(list);
                }
            }
        }
        void ApplyDefaultListNormalBackgroundMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultListNormalBackgroundMaterial.name + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var list in Object.FindObjectsByType<List>(FindObjectsSortMode.None))
#else
                foreach (var list in FindObjectsOfType<List>())
#endif
                {
                    list.NormalBackgroundMaterial = settings.defaultListNormalBackgroundMaterial;
                    EditorUtility.SetDirty(list);
                }
            }
        }
        void ApplyDefaultListSelectedTextSizeToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultListSelectedTextSize + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var list in Object.FindObjectsByType<List>(FindObjectsSortMode.None))
#else
                foreach (var list in FindObjectsOfType<List>())
#endif
                {
                    list.SelectedTextSize = settings.defaultListSelectedTextSize;
                    EditorUtility.SetDirty(list);
                }
            }
        }
        void ApplyDefaultListSelectedTextMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultListSelectedTextMaterial.name + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var list in Object.FindObjectsByType<List>(FindObjectsSortMode.None))
#else
                foreach (var list in FindObjectsOfType<List>())
#endif
                {
                    list.SelectedTextMaterial = settings.defaultListSelectedTextMaterial;
                    EditorUtility.SetDirty(list);
                }
            }
        }
        void ApplyDefaultListSelectedBackgroundMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultListSelectedBackgroundMaterial.name + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var list in Object.FindObjectsByType<List>(FindObjectsSortMode.None))
#else
                foreach (var list in FindObjectsOfType<List>())
#endif
                {
                    list.SelectedBackgroundMaterial = settings.defaultListSelectedBackgroundMaterial;
                    EditorUtility.SetDirty(list);
                }
            }
        }
        void ApplyDefaultListPressedTextSizeToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultListPressedTextSize + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var list in Object.FindObjectsByType<List>(FindObjectsSortMode.None))
#else
                foreach (var list in FindObjectsOfType<List>())
#endif
                {
                    list.PressedTextSize = settings.defaultListPressedTextSize;
                    EditorUtility.SetDirty(list);
                }
            }
        }
        void ApplyDefaultListPressedTextMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultListPressedTextMaterial.name + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var list in Object.FindObjectsByType<List>(FindObjectsSortMode.None))
#else
                foreach (var list in FindObjectsOfType<List>())
#endif
                {
                    list.PressedTextMaterial = settings.defaultListPressedTextMaterial;
                    EditorUtility.SetDirty(list);
                }
            }
        }
        void ApplyDefaultListPressedBackgroundMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultListPressedBackgroundMaterial.name + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var list in Object.FindObjectsByType<List>(FindObjectsSortMode.None))
#else
                foreach (var list in FindObjectsOfType<List>())
#endif
                {
                    list.PressedBackgroundMaterial = settings.defaultListPressedBackgroundMaterial;
                    EditorUtility.SetDirty(list);
                }
            }
        }
        void ApplyDefaultListDisabledTextSizeToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultListDisabledTextSize + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var list in Object.FindObjectsByType<List>(FindObjectsSortMode.None))
#else
                foreach (var list in FindObjectsOfType<List>())
#endif
                {
                    list.DisabledTextSize = settings.defaultListDisabledTextSize;
                    EditorUtility.SetDirty(list);
                }
            }
        }
        void ApplyDefaultListDisabledTextMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultListDisabledTextMaterial.name + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var list in Object.FindObjectsByType<List>(FindObjectsSortMode.None))
#else
                foreach (var list in FindObjectsOfType<List>())
#endif
                {
                    list.DisabledTextMaterial = settings.defaultListDisabledTextMaterial;
                    EditorUtility.SetDirty(list);
                }
            }
        }
        void ApplyDefaultListDisabledBackgroundMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultButtonDisabledBackgroundMaterial.name + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var list in Object.FindObjectsByType<List>(FindObjectsSortMode.None))
#else
                foreach (var list in FindObjectsOfType<List>())
#endif
                {
                    list.DisabledBackgroundMaterial = settings.defaultButtonDisabledBackgroundMaterial;
                    EditorUtility.SetDirty(list);
                }
            }
        }
        #endregion List
















        void BottomButtons()
        {
            if (GUILayout.Button("Reset to default", buttonStyle, GUILayout.Height(25)))
            {
                bool reset = EditorUtility.DisplayDialog("Reset to default", "Reset all the font creation settings", "Confirm", "Cancel");

                if (reset)
                {
                    settings.ResetFontCreationSettings();
                    soTarget = new SerializedObject(settings);
                    EditorUtility.SetDirty(settings);
                }
            }
            HorizontalButtonURL("Common Issues", fontCreationTroubleShootDocURL);
        }

        void CreateFontButton()
        {
            GUILayout.BeginHorizontal();
            GUI.backgroundColor = new Color(0.8f, 0.9f, 1, 1);
            if (GUILayout.Button("Create font", createFontButtonStyle, GUILayout.MinHeight(50)))
            {
                CreateFont();
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();
        }

        void CharacterInput()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label(new GUIContent("Prebuilt characters", "Prebuilt characters have their mesh created early, which reduces compute time by not having to create them runtime. \nAnd modifying the individual settings of each character is possible if they are pre-built."), groupHeaderStyle);

            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(soTarget.FindProperty("charInputStyle"), GUIContent.none);
            if (GUILayout.Button("Log", groupHeaderButtonStyle, GUILayout.MaxWidth(30)))
            {
                TestCharacterList();
            }
            GUILayout.EndHorizontal();
            //DrawUILine(Color.gray);


            if (settings.charInputStyle == AssetSettings.CharInputStyle.CharacterRange)
            {
                CharacterRangeInput();

                GUILayout.Space(5);

                EditorGUILayout.BeginHorizontal();
                //HorizontalButtonURL("Get character list", "https://unicode-table.com/en/");
                //HorizontalButtonURL("How it works", "https://youtu.be/JN_DSmdiRSI"); //to-do make a video with the unity editor
                EditorGUILayout.EndHorizontal();
            }
            else if (settings.charInputStyle == AssetSettings.CharInputStyle.UnicodeRange)
            {
                UnicodeRangeInput();

                EditorGUILayout.BeginHorizontal();
                HorizontalButtonURL("Get character list", "https://unicode-table.com/en/");
                //HorizontalButtonURL("How it works", "https://youtu.be/JN_DSmdiRSI"); //to-do make a video with the unity editor
                EditorGUILayout.EndHorizontal();
            }
            else if (settings.charInputStyle == AssetSettings.CharInputStyle.CustomCharacters)
            {
                CustomCharacters();
            }
            else if (settings.charInputStyle == AssetSettings.CharInputStyle.UnicodeSequence)
            {
                UnicodeSequence();
            }
            //else if (settings.charInputStyle == MText_Settings.CharInputStyle.CharacterSet)
            //{
            //    CharacterSet();
            //}


            //EditorGUILayout.HelpBox("Just a FYI: Having thousands of characters in a single file can cause issues.", MessageType.Info);
            GUILayout.Space(5);
            GUILayout.EndVertical();
        }
        void MeshSettings()
        {
            string tooltip = "Keep these values to 1 unless you really need to change it. Lower smoothing value will give a flat looking font and higher is smoother." +
                "\n\nYou can select the exported font and tweak settings like character/word spacing and even individual characters, including swapping their models.";

            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label(new GUIContent("Mesh settings", tooltip), groupHeaderStyle);
            GUILayout.EndHorizontal();


            EditorGUILayout.PropertyField(soTarget.FindProperty("meshExportStyle"), GUIContent.none);



            GUILayout.BeginHorizontal();

            GUIContent vertexDensity = new GUIContent("Vertex Density", "How many verticies should be used. Has very little impact other than calculation time since vertext density is increased automatically if it fails to be created within the given amount.");
            EditorGUILayout.LabelField(vertexDensity, labelStyle, GUILayout.MaxWidth(120));
            EditorGUILayout.PropertyField(soTarget.FindProperty("vertexDensity"), GUIContent.none);

            GUILayout.Label("", GUILayout.MaxWidth(100));

            GUIContent sizeXY = new GUIContent("Size XY", "Base font size.");
            EditorGUILayout.LabelField(sizeXY, labelStyle, GUILayout.MaxWidth(120));
            EditorGUILayout.PropertyField(soTarget.FindProperty("sizeXY"), GUIContent.none);

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUIContent sizeZ = new GUIContent("Size Z/Depth", "Base depth");
            EditorGUILayout.LabelField(sizeZ, labelStyle, GUILayout.MaxWidth(120));
            EditorGUILayout.PropertyField(soTarget.FindProperty("sizeZ"), GUIContent.none);

            GUILayout.Label("", GUILayout.MaxWidth(100));

            GUIContent smoothingAngle = new GUIContent("Smoothing Angle", "Any verticies with lower angle will be smooth.");
            EditorGUILayout.LabelField(smoothingAngle, labelStyle, GUILayout.MaxWidth(120));
            EditorGUILayout.PropertyField(soTarget.FindProperty("smoothingAngle"), GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.EndVertical();
        }


        void CreateFont()
        {
            GameObject gameObject = new GameObject();

            bool exportAsObj = ExportAs();
            List<char> listOfChar = GetCharacterList();

            CharacterGenerator fontCreator = new CharacterGenerator();
            fontCreator.CreateFont(gameObject, listOfChar, settings.sizeXY, settings.sizeZ, settings.vertexDensity, settings.smoothingAngle, settings.defaultTextMaterial, exportAsObj, out byte[] fontData);

            //if (!fontCreator.WasEverythingProcessed())
            //{
            //    //EditorUtility.DisplayDialog("")
            //}

            EditorUtility.DisplayProgressBar("Creating font", "Mesh creation started", 75 / 100);
            if (gameObject.transform.childCount > 0)
            {
                if (exportAsObj)
                {
                    MText_ObjExporter objExporter = new MText_ObjExporter();
                    string prefabPath = objExporter.DoExport(gameObject, true);
                    if (string.IsNullOrEmpty(prefabPath))
                    {
                        Debug.Log("Object save failed");
                        EditorUtility.ClearProgressBar();
                        return;
                    }
                    MText_FontExporter fontExporter = new MText_FontExporter();
                    fontExporter.CreateFontFile(prefabPath, gameObject.name, fontCreator, fontData);
                }
                else
                {
                    MText_MeshAssetExporter meshAssetExporter = new MText_MeshAssetExporter();
                    meshAssetExporter.DoExport(gameObject);
                }
            }

            EditorUtility.ClearProgressBar();
            if (Application.isPlaying) Destroy(gameObject);
            else DestroyImmediate(gameObject);
        }

        bool ExportAs()
        {
            bool exportAsObj = true;
            if (settings.meshExportStyle != AssetSettings.MeshExportStyle.exportAsObj) exportAsObj = false;
            return exportAsObj;
        }




        void CharacterRangeInput()
        {
            GUILayout.Label("Leave it to '!' & '~' for English.", EditorStyles.centeredGreyMiniLabel);

            //field
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Start", labelStyle, GUILayout.MaxWidth(40));
            EditorGUILayout.PropertyField(soTarget.FindProperty("startChar"), GUIContent.none);
            GUILayout.Label("", GUILayout.MaxWidth(100));
            EditorGUILayout.LabelField("End", labelStyle, GUILayout.MaxWidth(40));
            EditorGUILayout.PropertyField(soTarget.FindProperty("endChar"), GUIContent.none);
            GUILayout.EndHorizontal();
        }
        void UnicodeRangeInput()
        {
            //field
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Start", GUILayout.MaxWidth(120));
            EditorGUILayout.PropertyField(soTarget.FindProperty("startUnicode"), GUIContent.none);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("End", GUILayout.MaxWidth(120));
            EditorGUILayout.PropertyField(soTarget.FindProperty("endUnicode"), GUIContent.none);
            GUILayout.EndHorizontal();

            //info
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Leave it to 0021 & 007E for English.", GUILayout.Width(206));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        void CustomCharacters()
        {
            //field
            EditorGUILayout.PropertyField(soTarget.FindProperty("customCharacters"), GUIContent.none);

            //info
            GUILayout.Label("Just type the characters you want in the font.");
        }
        void UnicodeSequence()
        {
            //field
            EditorGUILayout.PropertyField(soTarget.FindProperty("unicodeSequence"), GUIContent.none);

            //info
            GUILayout.Label("Separate codes with ',' & create ranges with '-' .Example:\n" +
                "0021-007E, 00C0");
        }

        void TestCharacterList()
        {
            List<char> myCharacters = GetCharacterList();
            Debug.Log("Character count: " + myCharacters.Count);
            for (int i = 0; i < myCharacters.Count; i++)
            {
                Debug.Log(myCharacters[i]);
            }
        }

        List<char> GetCharacterFromRange(char start, char end)
        {
            MText_NewFontCharacterRange characterRange = new MText_NewFontCharacterRange();
            List<char> characterList = characterRange.RetrieveCharactersList(start, end);
            return characterList;
        }

        char ConvertCharFromUnicode(string unicode)
        {
            string s = System.Text.RegularExpressions.Regex.Unescape("\\u" + unicode);
            s.ToCharArray();
            if (s.Length > 0)
                return s[0];
            else
                return ' ';
        }



        List<char> GetCharacterList()
        {
            List<char> myChars = new List<char>();

            if (settings.charInputStyle == AssetSettings.CharInputStyle.CharacterRange)
            {
                myChars = GetCharacterFromRange(settings.startChar, settings.endChar);
            }
            else if (settings.charInputStyle == AssetSettings.CharInputStyle.UnicodeRange)
            {
                char start = ConvertCharFromUnicode(settings.startUnicode);
                char end = ConvertCharFromUnicode(settings.endUnicode);

                myChars = GetCharacterFromRange(start, end);
            }
            else if (settings.charInputStyle == AssetSettings.CharInputStyle.CustomCharacters)
            {
                myChars = settings.customCharacters.ToCharArray().ToList();
            }
            else if (settings.charInputStyle == AssetSettings.CharInputStyle.UnicodeSequence)
            {
                MText_NewFontCharacterRange characterRange = new MText_NewFontCharacterRange();
                myChars = characterRange.RetrieveCharacterListFromUnicodeSequence(settings.unicodeSequence);
            }
            myChars = myChars.Distinct().ToList();

            return myChars;
        }


        #region Layout Item
        void HorizontalButtonURL(string text, string url)
        {
            if (GUILayout.Button(text, buttonStyle, GUILayout.MinHeight(25)))
            {
                Application.OpenURL(url);
            }
        }
        void GridContent(string title, string description = "", string urlText = "", string url = "", string urlText2 = "", string url2 = "")
        {
            GUILayout.Space(5);
            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label(title, groupHeaderStyle);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            if (description != string.Empty)
                GUILayout.Label(description, paragraphStyle);
            GUILayout.BeginHorizontal();
            if (urlText != string.Empty && url != string.Empty)
            {
                if (GUILayout.Button(urlText, buttonStyle, GUILayout.MinHeight(25)))
                {
                    Application.OpenURL(url);
                }
            }
            if (urlText2 != string.Empty && url2 != string.Empty)
            {
                if (GUILayout.Button(urlText2, buttonStyle, GUILayout.MinHeight(25)))
                {
                    Application.OpenURL(url2);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }



        void AssetContent(string title, string description = "", string urlText = "", string url = "", Texture texture2D = null)
        {
            GUILayout.Space(5);
            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginHorizontal();
            float size = 65;
            GUILayout.Label(texture2D, EditorStyles.objectFieldThumb, GUILayout.MaxWidth(size), GUILayout.MaxHeight(size));


            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label(title, groupHeaderStyle);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);


            if (description != string.Empty)
                GUILayout.Label(description, paragraphStyle);
            GUILayout.BeginHorizontal();
            if (urlText != string.Empty && url != string.Empty)
            {
                if (GUILayout.Button(urlText, buttonStyle, GUILayout.MinHeight(25)))
                {
                    Application.OpenURL(url);
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }


        #endregion Layout Item
        void VerifyReferences()
        {
            if (!settings)
                settings = StaticMethods.VerifySettings(settings);
        }

        #region Style stuff
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





            if (tabStyle == null)
            {
                tabStyle = new GUIStyle(EditorStyles.toolbarButton)
                {
                    fontStyle = FontStyle.Normal
                };
                tabStyle.normal.textColor = Color.white;
            }

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

            if (bottomInformationStyle == null)
            {
                bottomInformationStyle = new GUIStyle(EditorStyles.wordWrappedLabel)
                {
                    fontStyle = FontStyle.Italic,
                    fontSize = 12
                };
                bottomInformationStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 0.75f);
            }
            if (groupHeaderStyle == null)
            {
                groupHeaderStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                {
                    fontStyle = FontStyle.BoldAndItalic,
                    fontSize = 12
                };

                groupHeaderStyle.normal.textColor = GetImportantLabelColor();
            }
            if (paragraphStyle == null)
            {
                paragraphStyle = new GUIStyle(EditorStyles.wordWrappedLabel)
                {
                    fontStyle = FontStyle.Italic,
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 12
                };
            }
            if (labelHeaderStyle == null)
            {
                labelHeaderStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                {
                    fontStyle = FontStyle.Italic,
                    fontSize = 12
                };
                labelHeaderStyle.normal.textColor = Color.white;
            }
            if (groupHeaderButtonStyle == null)
            {
                groupHeaderButtonStyle = new GUIStyle(EditorStyles.miniButtonRight)
                {
                    fontStyle = FontStyle.Italic,
                    fontSize = 10
                };
            }
            if (urlStyle == null)
            {
                urlStyle = new GUIStyle(EditorStyles.wordWrappedLabel)
                {
                    fontStyle = FontStyle.Italic,
                    fontSize = 12
                };
                urlStyle.normal.textColor = new Color(91f / 255f, 142f / 255f, 244f / 255f, 0.95f);
            }
            if (buttonStyle == null)
            {
                buttonStyle = new GUIStyle(GUI.skin.button)
                {
                    fontStyle = FontStyle.BoldAndItalic,
                    fontSize = 12
                };
            }
            if (createFontButtonStyle == null)
            {
                createFontButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    fontStyle = FontStyle.BoldAndItalic,
                    fontSize = 14
                };
            }
            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(EditorStyles.whiteMiniLabel)
                {
                    fontStyle = FontStyle.Italic,
                    fontSize = 12
                };
                labelStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 0.75f);
            }
        }

        Color GetImportantLabelColor()
        {
            if (!settings)
                return new Color(1f, 0.8f, 0.5f, 1);

            if (EditorGUIUtility.isProSkin)
                return settings.importantLabelColor_darkSkin;
            else
                return settings.importantLabelColor_lightSkin;
        }

        void DrawUILine(Color color, int thickness = 1, int padding = 1)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }
        #endregion
    }
}