using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.BonesStimulation
{
    public partial class BonesStimulator
    {

        // Muscles ---------

        // Transform Position Space -------------------------------
        [FPD_Suffix(0f, 1f)] public float MovementMuscles = 1f;

        [Space(4)]
        [Range(0f, 1f)] public float MusclesRapidity = .6f;
        [Range(0.05f, 1f)] public float Damping = .4f;

        [Space(4)]
        [Range(0f, 1f)] public float Smoothing = .6f;
        [Range(0f, 1f)] public float MildRotation = .0f;

        [Space(2)] [Tooltip("If moving in world space should have lower influence on the muscles")]
        [Range(0f, 1f)] public float MotionInfluence = 1f;


        // Rotation Space -------------------------------
        [FPD_Suffix(0f, 1f)] public float RotationSpaceMuscles = 0f;
        [Tooltip("(Changes motion) If you experience some jitter animation when crossfading animations then try enabling this")] public bool UseEulerRotation = false;
        [Tooltip("(Not changes motion) If you experience some jitter animation when crossfading animations then try enabling this")] public bool EnsureRotation = false;

        [Range(0f, 1f)] public float RotationsRapidity = .7f;
        [Range(0.05f, 1f)] public float RotationsDamping = 0.25f;
        [Range(0f, 1f)] public float RotationsSwinginess = .6f;

        [FPD_FixedCurveWindow(0f, 0f, 1f, 1f)]
        public AnimationCurve MusclesBlend = AnimationCurve.EaseInOut(0f, 1f, 1f, 1f);
        public float MusclesSimulationSpeed = 1f;

        // Vibrate ---------

        [FPD_Suffix(0f, 1f)] public float VibrateAmount = 0f;
        [Tooltip("Speed of animation")] public float VibrateSpeed = 6f;

        [Tooltip("Scaled offset value for animation")] public float VibrateRange = 2.5f;

        [Tooltip("How animation on different axes should be scaled")] public Vector3 VibrateAxis = Vector3.one;

        public float VibrateRotation = 1f;
        public bool UniRotation = false;

        public float VibratePosition = 0f;
        public float VibrateScale = 0f;

        [Tooltip("Enabling calculating trigonometric offsets for each bone individually instead of doing it once per frame for all bones")]
        public bool DetailedVibrate = true;

        [Tooltip("Spreading muscles effect amount over bones in chain")]
        [FPD_FixedCurveWindow(0f, 0f, 1f, 1f, 0.2f, 0.35f, 1f)]
        public AnimationCurve VibratePosBlend = AnimationCurve.EaseInOut(0f, 1f, 1f, 1f);
        [FPD_FixedCurveWindow(0f, 0f, 1f, 1f, 0.1f, 1f, 0.1f)]
        public AnimationCurve VibrateRotBlend = AnimationCurve.EaseInOut(0f, 1f, 1f, 1f);
        [FPD_FixedCurveWindow(0f, 0f, 1f, 1f, 0.9f, 0.4f, 0.3f)]
        public AnimationCurve VibrateScaleBlend = AnimationCurve.EaseInOut(0f, 1f, 1f, 1f);


        // Squeezing ---------

        [FPD_Suffix(0f, 1f)] public float SqueezingAmount = 0f;
        public float SqueezingSpeed = 15f;
        public float SqueezingMultiply = 0.5f;
        public Vector3 SqueezingAxis = new Vector3(0.2f, 0.15f, 0.2f);


        // Collisions ---------

        #region Physics Experimental Stuff

        [Tooltip("Using some simple calculations to make bones bend on colliders.")]
        public bool UseCollisions = false;
        [Tooltip("Making Movement Muscles react with collision, it will look more realistic with this option enabled, but movement muscles % use amount must be setted high")]
        public bool MovementMusclesCollision = true;

        [Space(4)]
        [FPD_FixedCurveWindow(0f, 0f, 1f, 1f)]
        public AnimationCurve CollidersScaleCurve = AnimationCurve.Linear(0, 1, 1, 1);
        public float CollidersScaleMul = 0.1f;

        [Space(4)]
        public Vector3 OffsetAllColliders = Vector3.zero;

        [Space(4)]
        [Tooltip("If you want to continue checking collision if segment collides with one collider (very useful for example when you using gravity power with ground)")]
        public bool DetailedCollision = true;

        public List<Collider> IncludedColliders;
        protected List<FImp_ColliderData_Base> IncludedCollidersData;
        protected List<FImp_ColliderData_Base> CollidersDataToCheck;


        //[Tooltip("Freeze local X rotation axis")]
        //public bool FreezeXAxis = false;
        //[Tooltip("Freeze local Y rotation axis")]
        //public bool FreezeYAxis = false;
        //[Tooltip("Freeze local Z rotation axis")]
        //public bool FreezeZAxis = false;

        [Tooltip("Set zero to freeze X axis rotation")]
        [Range(0f,1f)]
        public float BlendXAxis = 1f;
        [Tooltip("Set zero to freeze Y axis rotation")]
        [Range(0f,1f)]
        public float BlendYAxis = 1f;
        [Tooltip("Set zero to freeze Z axis rotation")]
        [Range(0f,1f)]
        public float BlendZAxis = 1f;

        [Space(4)]
        [FPD_MinMaxSlider(-180,180)]
        [Tooltip("Limit max rotation angle difference between current animation rotation and animated rotation (+-180 = unlimited)")]
        public Vector2 LimitXAxisRotation = new Vector2(-180, 180);
        [FPD_MinMaxSlider(-180,180)]
        [Tooltip("Limit max rotation angle difference between current animation rotation and animated rotation (+-180 = unlimited)")]
        public Vector2 LimitYAxisRotation = new Vector2(-180, 180);
        [FPD_MinMaxSlider(-180,180)]
        [Tooltip("Limit max rotation angle difference between current animation rotation and animated rotation (+-180 = unlimited)")]
        public Vector2 LimitZAxisRotation = new Vector2(-180,180);

        [Space(4)]
        [Range(0f,1f)]
        public float ElasticLimits = 0f;
        [FPD_FixedCurveWindow]
        public AnimationCurve ElasticLimitCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [Range(0f, 1f)]
        public float DampenOnLimits = 0f;

        #endregion




    }
}
