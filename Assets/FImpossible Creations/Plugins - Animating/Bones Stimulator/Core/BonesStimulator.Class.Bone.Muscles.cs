using FIMSpace.FTools;
using UnityEngine;

namespace FIMSpace.BonesStimulation
{
    public partial class BonesStimulator
    {
        public partial class Bone
        {
            public FElasticTransform MotionMuscle { get; private set; }
            public FMuscle_Quaternion RotationMuscle { get; private set; }
            public FMuscle_Eulers EulerAnglesMuscle { get; private set; }
            public float MomentDampen = 0f;

            public void InitMuscles()
            {
                MotionMuscle = new FElasticTransform();
                MotionMuscle.Initialize(transform);

                RotationMuscle = new FMuscle_Quaternion();
                if (transform != null) RotationMuscle.Initialize(transform.rotation);

                EulerAnglesMuscle = new FMuscle_Eulers();
                if (transform != null) EulerAnglesMuscle.Initialize(transform.rotation.eulerAngles);
            }

            public void CaptureAnimation()
            {
                if (transform == null) return;
                if (MotionMuscle != null) MotionMuscle.CaptureSourceAnimation();

                srcAnimatorPosition = transform.position;
                srcAnimatorRotation = transform.rotation;
                srcAnimatorLocRotation = transform.localRotation;
            }
        }
    }
}