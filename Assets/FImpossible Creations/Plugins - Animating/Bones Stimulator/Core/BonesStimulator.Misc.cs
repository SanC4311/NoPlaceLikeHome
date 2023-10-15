using UnityEngine;

namespace FIMSpace.BonesStimulation
{
    public partial class BonesStimulator
    {
        [Tooltip("If you notice different animation behaviour on high FPS games, change this value for example to 60 to unify animation motion.\n(Zero is unlimited update rate)")]
        [HideInInspector] public int UpdateRate = 0;

        /// <summary> How many udpate loops should be done according to stable update rate </summary>
        protected int framesToSimulate = 1;
        float rateDelta = 0.016f;

        /// <summary> Helper for calculating stable delta calculations </summary>
        protected float collectedCustomRateDelta = 0f;


        void PrepareCustomUpdateRate()
        {
            if ( UpdateRate <= 0 ) return;
            if (updateStimulator == false) return;

            rateDelta = 1f / (float)UpdateRate;

            collectedCustomRateDelta += delta; // Collecting delta time from game frames
            framesToSimulate = 0; // Collecting delta for [one second] div by [UpdateRate] update so for one frame in static defined time rate


            while (collectedCustomRateDelta >= rateDelta) // Collected delta is big enough to do tail motion frame
            {
                collectedCustomRateDelta -= rateDelta;
                    
                framesToSimulate += 1;
                if (framesToSimulate >= 4) { collectedCustomRateDelta = 0; break; } // Simulating up to 4 frames update in one unity game frame
            }
        }

        void CustomUpdateRate_LateUpdate()
        {


            if (framesToSimulate <= 0)
            {
                for (int i = Bones.Count - 1; i >= 0; i--)
                {
                    Bones[i].RestoreFinalPose();
                }

                return;
            }

            BeginUpdate();
            PhysicsUpdate();

            delta = rateDelta;
            elapsedDelta = delta;
            smoothDelta = delta;

            for (int i = 0; i < framesToSimulate; i++)
            {
                UpdateVibrateLogics();
                UpdateSqueezeLogics();
                UpdateMusclesLogics();
            }

            PostPhysics();
            PostProcessing();
        }

    }
}
