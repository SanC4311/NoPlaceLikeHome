using UnityEngine;

namespace FIMSpace.BonesStimulation
{
    public partial class BonesStimulator
    {
        private float delta = 0.01f;
        private float smoothDelta = 0.01f;

        private bool updateStimulator = true;

        /// <summary> Internal effect weight control for smoothly fade out bones stimulator effect when object is far away from the camera </summary>
        private float fadeOutBlend = 1f;
        /// <summary> Blending amount of all Bones Stimulator features to use in custom fade out / fade in methods </summary>
        private float CustomBlendAmount = 1f;
        public float CurrentFadeDistance { get; private set; }
        public Transform ArtificialHelper { get; private set; }
        public Bone ArtificialHelpBone { get; private set; }
        public bool Initialized { get; private set; }

        public void Initialize()
        {
            InitialStimulatorAmount = StimulatorAmount;

            if (Bones.Count == 0) Bones.Add(new Bone() { transform = transform });

            for (int i = Bones.Count - 1; i >= 0; i--) if (Bones[i] == null || Bones[i].transform == null) Bones.RemoveAt(i);

            //if (MovementMuscles > 0f || RotationSpaceMuscles > 0f)
            {
                // Helper last bone generating
                Transform lastBn = Bones[Bones.Count - 1].transform;
                ArtificialHelper = new GameObject(Bones[Bones.Count - 1].transform.name + "-Helper").transform;
                ArtificialHelper.SetParent(lastBn);
                ArtificialHelper.rotation = lastBn.rotation;
                ArtificialHelper.position = lastBn.TransformPoint(HelperOffset);
                ArtificialHelpBone = new Bone() { transform = ArtificialHelper };
                Bones.Add(ArtificialHelpBone);
            }

            // Initializing features
            for (int i = 0; i < Bones.Count; i++)
            {
                Bones[i].UpdateInitialCoords();
                Bones[i].InitMuscles();
                Bones[i].VibrateInitialize();

                if (Bones.Count > 1)
                    Bones[i].Evaluation = (float)i / (float)(Bones.Count - 1);
                else
                    Bones[i].Evaluation = 1f;
            }

            // Setting relation
            for (int i = 0; i < Bones.Count; i++)
            {
                if (i < Bones.Count - 1)
                    Bones[i].SetChild(Bones[i + 1]);
                if (i > 0)
                    Bones[i].SetParent(Bones[i - 1]);

                Bones[i].RefreshFinalPose();
            }

            Vibrate_Initialize();
            SqueezeInitialize();
            USER_CheckExtraSettingsAfterManualChange();


            if (CompensationTransform) previousPosition = CompensationTransform.position;
            Initialized = true;
        }

        void OnValidate()
        {
            USER_CheckExtraSettingsAfterManualChange();
        }


        void OnDisable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif

            if (Initialized)
            {
                if (updateStimulator) PreCalibrateBones();
            }
        }

        public void CheckIfCanUpdate()
        {
            // Check distance blend
            if (FadeOutDistance > 0f)
            {
                Transform from = GetDistanceMeasureTransform();
                if (from != null)
                {
                    CurrentFadeDistance = Vector3.Distance(transform.position, from.transform.position);
                    if (CurrentFadeDistance > FadeOutDistance)
                    {
                        if (fadeOutBlend <= 0f) fadeOutBlend = 0f;
                        else
                            fadeOutBlend -= Time.unscaledDeltaTime;
                    }
                    else
                    {
                        if (fadeOutBlend >= 1f) fadeOutBlend = 1f;
                        else
                            fadeOutBlend += Time.unscaledDeltaTime;
                    }
                }
            }


            // Check effect weight
            if (GetEffectBlendWeight() <= Mathf.Epsilon)
            {
                if (updateStimulator)
                {
                    PreCalibrateBones();
                    updateStimulator = false;
                }

                return;
            }

            // Check mesh visibility
            if (HideWith != null) if (HideWith.isVisible == false)
                {
                    if (updateStimulator)
                    {
                        PreCalibrateBones();
                        updateStimulator = false;
                    }

                    return;
                }

            //if (UnscaledDelta)
            //{
            //    if (!AnimatePhysics) delta = Time.unscaledDeltaTime; else delta = Time.fixedUnscaledDeltaTime;
            //}
            //else
            //{
            //    if (!AnimatePhysics) delta = Time.deltaTime; else delta = Time.fixedDeltaTime;
            //}

            if (UnscaledDelta)
            {
                delta = Time.unscaledDeltaTime;
                if (Time.timeScale == 0f) smoothDelta = 0f; else smoothDelta = Time.smoothDeltaTime / Time.timeScale;
            }
            else
            {
                delta = Time.deltaTime;
                smoothDelta = Time.smoothDeltaTime;
            }

            elapsedDelta += delta;
            updateStimulator = true;
        }


        public Transform GetDistanceMeasureTransform()
        {
            Transform from = DistanceFrom;
            if (from == null) if (Camera.main != null) from = Camera.main.transform;
            return from;
        }

        public void PreCalibrateBones()
        {

            #region Performance Measure START
#if UNITY_EDITOR
            MeasurePerformanceUpdate(true);

#endif
            #endregion


            if (ArtificialHelpBone != null)
            {
                ArtificialHelpBone.transform.localPosition = ArtificialHelpBone.initLocalPos;
                ArtificialHelpBone.transform.localRotation = ArtificialHelpBone.initLocalRot;
            }

            Bone b = Bones[0];
            if (MovementMuscles <= 0f && RotationSpaceMuscles <= 0f)
            {
                if (VibrateScale != 0f || SqueezingAmount != 0f)
                {
                    while (b != null) { b.PreCalibrateWithScale(); b = b.Child; }
                }
                else
                {
                    while (b != null) { b.PreCalibrate(); b = b.Child; }
                }
            }
            else
            {
                if (VibrateScale != 0f || SqueezingAmount != 0f)
                {
                    while (b != null)
                    {
                        b.PreCalibrateWithScale();
                        UpdateMuscleSettings(b);
                        b = b.Child;
                    }
                }
                else
                {
                    while (b != null)
                    {
                        b.PreCalibrate();
                        UpdateMuscleSettings(b);
                        b = b.Child;
                    }
                }
            }

            #region Performance Measure END
#if UNITY_EDITOR
            MeasurePerformanceUpdate(false);

#endif
            #endregion
        }


        float elapsedDelta = 0f;
        public void BeginUpdate()
        {
            Motion_MotionInfluence();

            if (MovementMuscles > 0f)
            {
                Bone b = Bones[0];
                while (b != null)
                {
                    b.CaptureAnimation();
                    b = b.Child;
                }
            }
        }


        public void UpdateMuscleSettings(Bone bone)
        {
            bone.MotionMuscle.PositionMuscle.Acceleration = MusclesRapidity * 10000f;
            bone.MotionMuscle.PositionMuscle.AccelerationLimit = MusclesRapidity * 5000f;

            bone.MotionMuscle.RotationRapidness = 1f - MildRotation;

            bone.MotionMuscle.PositionMuscle.BrakePower = 1f - Smoothing;

            bone.MotionMuscle.PositionMuscle.Damping = Damping * 40f;

            // Rotation Muscles -----------------------------------

            if (!UseEulerRotation)
            {
                bone.RotationMuscle.Acceleration = RotationsRapidity * 10000f;
                bone.RotationMuscle.AccelerationLimit = RotationsRapidity * 5000f;

                bone.RotationMuscle.BrakePower = 1f - RotationsSwinginess;
                bone.RotationMuscle.Damping = RotationsDamping * 40f;
            }
            else
            {
                bone.EulerAnglesMuscle.Acceleration = RotationsRapidity * 10000f;
                bone.EulerAnglesMuscle.AccelerationLimit = RotationsRapidity * 5000f;

                bone.EulerAnglesMuscle.BrakePower = 1f - RotationsSwinginess;
                bone.EulerAnglesMuscle.Damping = RotationsDamping * 40f;
            }

            if (bone.MomentDampen > 0f)
            {
                bone.MotionMuscle.PositionMuscle.Damping += 40f * bone.MomentDampen;
                if (!UseEulerRotation) bone.RotationMuscle.Damping += 40f * bone.MomentDampen;
                else bone.EulerAnglesMuscle.Damping += 40f * bone.MomentDampen;
                bone.MomentDampen -= delta * 4f;
            }
        }


        public float GetEffectBlendWeight()
        {
            return StimulatorAmount * fadeOutBlend * CustomBlendAmount;
        }

        public Vector3 GetEndTipWorldOffset()
        {
            Transform firstTr;
            if (Bones.Count == 0) firstTr = transform; else firstTr = Bones[0].transform;

            if (firstTr.parent)
                if (Bones.Count > 0)
                    return GetLastTransform().position - firstTr.parent.position;

            return Vector3.zero;
        }

        public Transform GetLastTransform()
        {
            if (Bones.Count == 0) return transform;
            if (Bones[0].transform == null) return transform;
            return Bones[Bones.Count - 1].transform;
        }


        // Motion Influence -------------
        private Vector3 previousPosition;
        private Vector3 influenceOffset = Vector3.zero;

        private void Motion_MotionInfluence()
        {
            if (CompensationTransform == null) return;
            if (MotionInfluence >= 1f) return;
            influenceOffset = (CompensationTransform.position - previousPosition) * (1f - MotionInfluence);
            previousPosition = CompensationTransform.position;
        }
    }
}