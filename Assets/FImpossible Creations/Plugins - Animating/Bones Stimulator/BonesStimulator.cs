using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.BonesStimulation
{
    [AddComponentMenu("FImpossible Creations/Bones Stimulator")]
    [DefaultExecutionOrder(-6)]
    public partial class BonesStimulator : MonoBehaviour
    {
        // IT IS PARTIAL CLASS - REST OF THE CODE IS INSIDE OTHER .CS FILES -----------------
        [Tooltip("How much of bones stimulator effect should be applied")]
        [FPD_Suffix(0f, 1f)] public float StimulatorAmount = 1f;
        /// <summary> Just StimulatorAmount value saved at component Start(). Can be helpful for value controlling scripts but consider using CustomBlendAmount variable instead. </summary>
        public float InitialStimulatorAmount { get; private set; }
        public List<Bone> Bones;

        [Tooltip("If you want use 'Motion Influence' feature put here root game object of your character")]
        public Transform CompensationTransform;

        [Tooltip("If your animator controller have enabled 'Animate Physics' you should enable this option")]
        public bool AnimatePhysics = false;
        [Tooltip("If you want to make bones stimulator independent from the Time.time scale value")]
        public bool UnscaledDelta = false;
        [Tooltip("If you want to disable bones stimulator when target mesh is not being visible in camera view")]
        public Renderer HideWith;
        [Tooltip("If you want to smoothly disable bones stimulator when this object is far away from main camera object")]
        public float FadeOutDistance = 0f;
        [Tooltip("If you want measure fade out distance from other object than main camera put it here")]
        public Transform DistanceFrom;
        [Tooltip("Making muscles be able to work on single bone objects")]
        public bool AutoHelperOffset = true;
        public Vector3 HelperOffset = Vector3.zero;

        public Vector3 GravityEffectForce = Vector3.zero;
        [Tooltip("More heavyness when coming near to the last bone in chain")]
        public float GravityHeavyness = 2f;

        void Start()
        {
            Initialize();
        }


        private void OnEnable()
        {
            if (Initialized)
            {
                foreach (Bone bone in Bones)
                {
                    bone.MotionMuscle.OverrideProceduralPositionHard(bone.transform.position);
                    bone.MotionMuscle.OverrideProceduralRotation(bone.transform.rotation);
                    bone.RotationMuscle.OverrideProceduralRotation(bone.transform.rotation);
                }

                influenceOffset = Vector3.zero;
                if (CompensationTransform) previousPosition = CompensationTransform.position;
            }
        }


        void Update()
        {
            CheckIfCanUpdate();
            PrepareCustomUpdateRate();

#if UNITY_EDITOR
#if ENABLE_LEGACY_INPUT_MANAGER
            // Just for editor debug help
            if (Input.GetKey(KeyCode.BackQuote)) updateStimulator = false;
#endif
#endif

            if (!updateStimulator) return;
            if (!AnimatePhysics)
            {
                PreCalibrateBones();
            }
        }

        bool fixedUpdated = false;
        void FixedUpdate()
        {
            if (!updateStimulator) return;
            if (AnimatePhysics)
            {
                fixedUpdated = true;
                PreCalibrateBones();
            }
        }

        void LateUpdate()
        {
            if (!updateStimulator) return;

            if (AnimatePhysics )
            {
                if (fixedUpdated == false) return;
                fixedUpdated = false;
            }

            #region Performance Measure START
#if UNITY_EDITOR
            MeasurePerformanceMain(true);

#endif
            #endregion

            #region Custom Update Rate

            if ( UpdateRate > 0 )
            {
                CustomUpdateRate_LateUpdate();
                return;
            }

            #endregion

            BeginUpdate();
            PhysicsUpdate();

            UpdateVibrateLogics();
            UpdateSqueezeLogics();
            UpdateMusclesLogics();

            PostPhysics();
            PostProcessing();


            #region Performance Measure END
#if UNITY_EDITOR
            MeasurePerformanceMain(false);

#endif
            #endregion

        }
    }
}