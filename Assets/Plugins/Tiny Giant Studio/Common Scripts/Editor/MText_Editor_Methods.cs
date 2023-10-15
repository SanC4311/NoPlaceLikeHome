using UnityEditor;
using UnityEngine;

namespace TinyGiantStudio.EditorHelpers
{
    public static class MText_Editor_Methods
    {
        readonly private static float defaultTinyHorizontalFieldSize = 50f;
        readonly private static float defaultSmallHorizontalFieldSize = 72.5f;
        readonly private static float defaultNormalltHorizontalFieldSize = 100;
        readonly private static float defaultLargeHorizontalFieldSize = 120f;
        readonly private static float defaultExtraLargeHorizontalFieldSize = 155f;
        readonly private static float defaultGiganticHorizontalFieldSize = 220;
        readonly private static float defaultMegaHorizontalFieldSize = 300;

        static GUIStyle defaultLabel;
        static GUIStyle defaultMultilineLabel;




        public static void HorizontalField(SerializedProperty property, string label, string tooltip = "", FieldSize fieldSize = FieldSize.normal)
        {
            if (property == null)
                return;

            float myMaxWidth = GetMyMaxWidth(fieldSize);
            float defaultWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = myMaxWidth;

            GUILayout.BeginHorizontal();
            GUIContent gUIContent = new GUIContent(label, tooltip);
            EditorGUILayout.PropertyField(property, gUIContent);
            GUILayout.EndHorizontal();


            EditorGUIUtility.labelWidth = defaultWidth;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <param name="tooltip"></param>
        /// <param name="fieldSize"></param>
        /// <param name="applySizeToPropertyField">Reduces total width taken by the property</param>
        public static void ItalicHorizontalField(SerializedProperty property, string label, string tooltip = "", FieldSize fieldSize = FieldSize.normal, bool applySizeToPropertyField = false)
        {
            if (property == null)
                return;

            GenerateStyle();

            float myMaxWidth = GetMyMaxWidth(fieldSize);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent(label, tooltip), defaultLabel, GUILayout.MaxWidth(myMaxWidth));
            if (applySizeToPropertyField)
                EditorGUILayout.PropertyField(property, GUIContent.none, GUILayout.MaxWidth(myMaxWidth / 2));
            else
                EditorGUILayout.PropertyField(property, GUIContent.none);
            GUILayout.EndHorizontal();
        }

        public static void PriviewField(SerializedProperty property, Object targetObject, string label, string tooltip = "")
        {
            if (property == null) return;

            GenerateStyle();

            if (targetObject)
            {
                //Texture2D texture = AssetPreview.GetAssetPreview(targetObject);
                GUILayout.Box(AssetPreview.GetAssetPreview(targetObject), GUIStyle.none, GUILayout.MaxWidth(40), GUILayout.MaxHeight(40));
            }

            try
            {

                GUILayout.BeginVertical();
                GUIContent content = new GUIContent(label, tooltip);
                float minWidth = defaultMultilineLabel.CalcSize(content).x;
                EditorGUILayout.ObjectField(property, new GUIContent(""), GUILayout.MinWidth(minWidth));
                EditorGUILayout.LabelField(content, defaultMultilineLabel);
                GUILayout.EndVertical();
            }
            catch
            {
                //Debug.Log("Error " + property.ToString());
            }
        }


        private static float GetMyMaxWidth(FieldSize fieldSize)
        {
            return fieldSize == FieldSize.tiny ? defaultTinyHorizontalFieldSize : fieldSize == FieldSize.small ? defaultSmallHorizontalFieldSize : fieldSize == FieldSize.normal ? defaultNormalltHorizontalFieldSize : fieldSize == FieldSize.large ? defaultLargeHorizontalFieldSize : fieldSize == FieldSize.extraLarge ? defaultExtraLargeHorizontalFieldSize : fieldSize == FieldSize.gigantic ? defaultGiganticHorizontalFieldSize : fieldSize == FieldSize.mega ? defaultMegaHorizontalFieldSize : defaultNormalltHorizontalFieldSize;
        }




        static void GenerateStyle()
        {
            if (defaultMultilineLabel == null)
            {
                defaultMultilineLabel = new GUIStyle(EditorStyles.wordWrappedLabel)
                {
                    fontSize = 10,
                    fontStyle = FontStyle.Italic,
                    alignment = TextAnchor.MiddleCenter,
                };
                defaultMultilineLabel.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 0.75f);
            }
            if (defaultLabel == null)
            {
                defaultLabel = new GUIStyle(EditorStyles.whiteMiniLabel)
                {
                    fontStyle = FontStyle.Italic,
                    fontSize = 12
                };
                defaultLabel.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 0.75f);
            }
        }
    }
}