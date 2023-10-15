using System.Collections;
using UnityEngine;

namespace FIMSpace.BonesStimulation
{
    public partial class BonesStimulator
    {

        float sd_amount = 0f;
        /// <summary>
        /// Smoothly changing Stimulator Amount to target value - must be called every frame
        /// </summary>
        public void User_TransitionAmount(float targetAmount, float duration)
        {
            float newValue = Mathf.SmoothDamp(StimulatorAmount, targetAmount, ref sd_amount, duration, Mathf.Infinity, Time.deltaTime);

            if (targetAmount == 0f)
            {
                if (newValue < 0.001f) newValue = 0f;
            }
            else if (targetAmount == 1f)
            {
                if (newValue > 0.999f) newValue = 1f;
            }

            StimulatorAmount = newValue;
        }


        Coroutine _coroBlend = null;
        public void User_ChangeStimulatorBlendTo(float targetBlend, float duration)
        {
            if (_coroBlend != null) StopCoroutine(_coroBlend);
            _coroBlend = StartCoroutine(IEUser_FadeStimulator(targetBlend, duration, 0f));
        }


        public void User_FadeOffStimulatorAndDisable()
        {
            User_FadeOffStimulatorAndDisable(0.4f, 0f);
        }
        public void User_FadeOffAndDisable(float duration)
        {
            User_FadeOffStimulatorAndDisable(duration, 0f);
        }
        public void User_FadeOffStimulatorAndDisable(float duration, float delay, bool disableComponent = true)
        {
            if (_coroBlend != null) StopCoroutine(_coroBlend);
            _coroBlend = StartCoroutine(IEUser_FadeStimulator(0f, duration, delay, disableComponent));
        }

        private IEnumerator IEUser_FadeStimulator(float targetBlend, float duration, float delay, bool disableComponent = false) 
        { 
            if ( delay > 0f) yield return new WaitForSeconds(delay);

            if (duration > 0f)
            {
                float preAmount = StimulatorAmount;

                float elapsed = 0f;
                while (elapsed < duration)
                {
                    elapsed += delta;

                    StimulatorAmount = Mathf.Lerp(preAmount, targetBlend, elapsed / duration);

                    yield return null;
                }
            }

            StimulatorAmount = targetBlend;
            if (disableComponent) enabled = false;
        }

    }
}
