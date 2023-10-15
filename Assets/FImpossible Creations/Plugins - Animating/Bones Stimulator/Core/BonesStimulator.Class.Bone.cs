using FIMSpace.FTools;
using UnityEngine;

namespace FIMSpace.BonesStimulation
{
    public partial class BonesStimulator
    {
        [System.Serializable]
        public partial class Bone
        {
            public Transform transform;

            public Bone Child { get; private set; }
            public Bone Parent { get; private set; }
            public float Evaluation = 0f;

            public Vector3 initLocalPos;
            public Quaternion initLocalRot;
            public Vector3 initLocalScale;

            public Vector3 srcAnimatorLocPosition;
            public Vector3 srcAnimatorPosition;
            public Quaternion srcAnimatorLocRotation;
            public Quaternion srcAnimatorRotation;


            #region Physical

            public bool EnableCollisions = true;
            public float CollisionRadius = 0.1f;
            public Vector3 CollisionHelperVector;
            public float GetCollisionRadiusScaled() { return CollisionRadius * transform.lossyScale.x; }

            #endregion


            public void SetChild(Bone child)
            {
                Child = child;
                MotionMuscle.SetChild(child.MotionMuscle);
            }

            public void SetParent(Bone parent)
            {
                Parent = parent;
                MotionMuscle.SetParent(parent.MotionMuscle);
            }

            public void UpdateInitialCoords()
            {
                if (transform == null) return;

                initLocalPos = transform.localPosition;
                initLocalRot = transform.localRotation;
                initLocalScale = transform.localScale;
            }

            public void PreCalibrate()
            {
                if (transform == null) return;

                transform.localPosition = initLocalPos;
                transform.localRotation = initLocalRot;
            }            
            
            public void PreCalibrateWithScale()
            {
                if (transform == null) return;

                transform.localPosition = initLocalPos;
                transform.localRotation = initLocalRot;
                transform.localScale = initLocalScale;
            }



            public Vector3 lastFinalLocPosition;
            public Quaternion lastFinalLocRotation;
            public void RefreshFinalPose()
            {
                lastFinalLocPosition = transform.localPosition;
                lastFinalLocRotation = transform.localRotation;
            }

            public void RestoreFinalPose()
            {
                transform.localPosition = lastFinalLocPosition;
                transform.localRotation = lastFinalLocRotation;
            }

        }
    }
}