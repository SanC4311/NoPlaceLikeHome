using UnityEditor;
using UnityEngine;


namespace FIMSpace.BonesStimulation
{
    public partial class BonesStimulator_Editor
    {
        // RESOURCES ----------------------------------------

        public static Texture2D _TexStimulIcon { get { if (__texStimulIcon != null) return __texStimulIcon; __texStimulIcon = Resources.Load<Texture2D>("Bones Stimulator/BonesStimulator"); return __texStimulIcon; } }
        private static Texture2D __texStimulIcon = null;

        public static Texture2D _TexRotationAuto { get { if (__texRot != null) return __texRot; __texRot = Resources.Load<Texture2D>("Bones Stimulator/GearIconAuto"); return __texRot; } }
        private static Texture2D __texRot = null;
        public static Texture2D _TexSquashing { get { if (__texSquash != null) return __texSquash; __texSquash = Resources.Load<Texture2D>("Bones Stimulator/SquashIcon"); return __texSquash; } }
        private static Texture2D __texSquash = null;
        public static Texture2D _TexMuscleIcon { get { if (__texMuscle != null) return __texMuscle; __texMuscle = Resources.Load<Texture2D>("Bones Stimulator/MusclesIcon"); return __texMuscle; } }
        private static Texture2D __texMuscle = null;


        private static UnityEngine.Object _manualFile;
        private static GUIStyle smallStyle { get { if (_smallStyle == null) _smallStyle = new GUIStyle(EditorStyles.miniLabel) { fontStyle = FontStyle.Italic }; return _smallStyle; } }
        private static GUIStyle _smallStyle;

        // HELPER VARIABLES ----------------------------------------

        private BonesStimulator Get { get { if (_get == null) _get = target as BonesStimulator; return _get; } }
        private BonesStimulator _get;

        static bool drawDefaultInspector = false;
        private Color c;
        private bool e;

        SerializedProperty sp_Amount;
        SerializedProperty sp_Bones;
        SerializedProperty sp_MovementMuscles;
        SerializedProperty sp_RotationSpaceMuscles;
        SerializedProperty sp_MusclesCurve;
        SerializedProperty sp_VibrateAmount;
        SerializedProperty sp_SqueezingAmount;
        SerializedProperty sp_Compens;
        SerializedProperty sp_Rate;
        SerializedProperty sp_Grav;
        SerializedProperty sp_GravH;
        SerializedProperty sp_DistanceFrom;
        SerializedProperty sp_UseCollisions;
        SerializedProperty sp_AutoHelperOffset;
        SerializedProperty sp_BlendXAxis;

        private void OnEnable()
        {
            sp_Amount = serializedObject.FindProperty("StimulatorAmount");
            sp_Bones = serializedObject.FindProperty("Bones");
            sp_MovementMuscles = serializedObject.FindProperty("MovementMuscles");
            sp_RotationSpaceMuscles = serializedObject.FindProperty("RotationSpaceMuscles");
            sp_MusclesCurve = serializedObject.FindProperty("MusclesBlend");
            sp_VibrateAmount = serializedObject.FindProperty("VibrateAmount");
            sp_SqueezingAmount = serializedObject.FindProperty("SqueezingAmount");
            sp_Rate = serializedObject.FindProperty("UpdateRate");
            sp_Grav = serializedObject.FindProperty("GravityEffectForce");
            sp_GravH = serializedObject.FindProperty("GravityHeavyness");
            sp_Compens = serializedObject.FindProperty("CompensationTransform");
            sp_DistanceFrom = serializedObject.FindProperty("DistanceFrom");
            sp_UseCollisions = serializedObject.FindProperty("UseCollisions");
            sp_AutoHelperOffset = serializedObject.FindProperty("AutoHelperOffset");
            sp_BlendXAxis = serializedObject.FindProperty("BlendXAxis");
        }


        protected virtual void OnSceneGUI()
        {
            if (Application.isPlaying) return;
            if (!Get.DrawGizmos) return;

            if (Get.MovementMuscles > 0f || Get.RotationSpaceMuscles > 0f)
                if (Get.AutoHelperOffset == false)
                    if (Get._editor_DrawSetup)
                        if (Get.GetLastTransform() != null)
                            if (Get.GetLastTransform().parent)
                                if (!FEngineering.VIsZero(Get.HelperOffset))
                                {
                                    Undo.RecordObject(Get, "position of bones stimulator offset");
                                    Transform root = Get.GetLastTransform();

                                    Vector3 off = root.TransformVector(Get.HelperOffset);
                                    Vector3 pos = root.position + off;
                                    Vector3 transformed = FEditor_TransformHandles.PositionHandle(pos, root.rotation, .3f, true, false);

                                    if (Vector3.Distance(transformed, pos) > 0.00001f)
                                    {
                                        Vector3 diff = transformed - pos;
                                        Get.HelperOffset = root.InverseTransformVector(off + diff);
                                        SerializedObject obj = new SerializedObject(Get);
                                        if (obj != null) { obj.ApplyModifiedProperties(); obj.Update(); }
                                    }
                                }
        }

    }
}