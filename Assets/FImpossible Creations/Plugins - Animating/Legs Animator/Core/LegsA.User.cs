using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FIMSpace.FProceduralAnimation.LegsAnimator;

namespace FIMSpace.FProceduralAnimation
{

    public partial class LegsAnimator
    {

        bool _grounded = true;
        //public bool IsGrounded { get { return Debug_IsGrounded; } }
        public bool IsGrounded { get { return _grounded; } }
        //public bool IsInAir { get { return !Debug_IsGrounded; } }
        public bool IsInAir { get { return !_grounded; } }
        /// <summary> Value which smoothly transitions to value =zero if grounded param is false </summary>
        public float IsGroundedBlend { get; private set; }
        /// <summary> When Ragdolled then goes to zero, when not ragdolled goes to value = 1 </summary>
        public float RadgolledDisablerBlend { get; protected set; } = 1f;

        public void User_SetIsGrounded(bool grounded)
        {
            if (grounded != _grounded)
            {
                if (grounded == true) // On landing
                {
                    Control_OnLand();
                }
                else // On loosing ground
                {
                    Control_OnLooseGround();
                }
            }

            _grounded = grounded;
        }


        bool _moving = false;
        //public bool IsMoving { get { return Debug_IsMoving; } }
        public bool IsMoving { get { return _moving; } }
        //public bool IsIdling { get { return !Debug_IsMoving; } }
        public bool IsIdling { get { return !_moving; } }
        /// <summary> Value which smoothly transitions to value =one if moving param is true </summary>
        public float IsMovingBlend { get; private set; }

        public void User_SetIsMoving(bool moving)
        {
            if (moving != _moving)
            {
                if (moving == true) // On Start Moving
                {
                    Control_OnStartMoving();
                }
                else // On End Moving
                {
                    Control_OnStopMoving();
                }
            }

            _moving = moving;
        }



        bool _sliding = false;
        //public bool IsMoving { get { return Debug_IsMoving; } }
        public bool IsSliding { get { return _sliding; } }
        /// <summary> Value which smoothly transitions to value =one if moving param is true </summary>
        public float IsSlidingBlend { get { return 1f - NotSlidingBlend; } }
        protected float NotSlidingBlend = 1f;

        public void User_SetIsSliding(bool moving)
        {
            _sliding = moving;
        }

        // Do it with modules
        //public void User_ForceLegIKOnPosition(int legIndex, Vector3 position, bool useIdleGlue = false)
        //{
        //    if (Legs.ContainsIndex(legIndex) == false) return;
        //    User_ForceLegIKOnPosition(legIndex, position, Legs[legIndex]._PreviousFinalIKRot, useIdleGlue);
        //}
        //public void User_ForceLegIKOnPosition(int legIndex, Vector3 position, Quaternion rotation, bool useIdleGlue = false)
        //{
        //    if (Legs.ContainsIndex(legIndex) == false) return;
        //}

        /// <summary> Fading single leg animation weight. Consider using custom modules for it. </summary>
        public void User_FadeLeg(int legIndex, float blend, float duration)
        {
            if (Legs.ContainsIndex(legIndex) == false) return;
            StartCoroutine(IEFadeLegTo(Legs[legIndex], 0f, duration));
        }

        /// <summary> Fading legs animation weight to disabled state. </summary>
        public void User_FadeToDisabled(float duration)
        {
            StopAllCoroutines();
            StartCoroutine(IEFadeLegsAnimatorTo(0f, duration));
        }

        /// <summary> Fading legs animation weight to fully enabled state. </summary>
        public void User_FadeEnabled(float duration)
        {
            if (enabled == false) enabled = true;
            StopAllCoroutines();
            StartCoroutine(IEFadeLegsAnimatorTo(1f, duration));
            for (int l = 0; l < Legs.Count; l++) Legs[l].LegBlendWeight = 1f;
        }


        /// <summary> For Now, Working only with IDLE GLUING. Make custom step for the leg. </summary>
        public void User_MoveLegTo(int legIndex, Transform transform)
        {
            if (Legs.ContainsIndex(legIndex) == false) return;
            var leg = Legs[legIndex];
            leg.User_OverrideRaycastHit(transform);
        }

        /// <summary> For Now, Working only with IDLE GLUING. Make custom step for the leg. </summary>
        public void User_MoveLegTo(int legIndex, Vector3 position, Vector3 normal)
        {
            RaycastHit newHit = new RaycastHit();
            newHit.point = position;
            newHit.normal = normal;
            User_MoveLegTo(legIndex, newHit);
        }

        /// <summary> For Now, Working only with IDLE GLUING. Make custom step for the leg. </summary>
        public void User_MoveLegTo(int legIndex, RaycastHit hit)
        {
            if (Legs.ContainsIndex(legIndex) == false) return;
            var leg = Legs[legIndex];
            leg.User_OverrideRaycastHit(hit);
        }

        /// <summary> Release custom leg step. </summary>
        public void User_MoveLegTo_Restore(int legIndex)
        {
            if (Legs.ContainsIndex(legIndex) == false) return;
            Legs[legIndex].User_RestoreRaycasting();
        }


        #region Coroutines

        protected IEnumerator IEFadeLegsAnimatorTo(float blend, float duration)
        {
            float startBlend = LegsAnimatorBlend;
            float elapsed = 0f;
            
            while(elapsed < duration)
            {
                elapsed += Time.deltaTime;
                LegsAnimatorBlend = Mathf.Lerp(startBlend, blend, elapsed / duration);
                yield return null;
            }

            LegsAnimatorBlend = blend;
            if (blend <= 0f) enabled = false;
            yield break;
        }

        protected IEnumerator IEFadeLegTo(Leg leg, float blend, float duration)
        {
            float startBlend = leg.LegBlendWeight;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                leg.LegBlendWeight = Mathf.Lerp(startBlend, blend, elapsed / duration);
                yield return null;
            }

            leg.LegBlendWeight = blend;
            yield break;
        }

        #endregion

    }

}