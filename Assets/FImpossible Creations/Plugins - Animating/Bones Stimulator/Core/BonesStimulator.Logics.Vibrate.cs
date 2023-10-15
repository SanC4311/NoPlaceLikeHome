using UnityEngine;

namespace FIMSpace.BonesStimulation
{
    public partial class BonesStimulator
    {
        #region Private variables for vibration calculations

        protected float[] _vib_RandomFloats;
        protected float _vib_Time;
        protected float _vib_TrueSpeed;

        protected float _vib_Speed;
        protected float _vib_Range;
        protected float _vib_Intensity;

        private float[] _vib_sinuses;
        private float[] _vib_cosinuses;
        private float _vib_blend;

        #endregion

        private void Vibrate_Initialize()
        {
            Vibrate_ChooseNewSeed();

            _vib_Speed = VibrateSpeed;
            _vib_Range = VibrateRange;
            _vib_Intensity = 0f;

            _vib_sinuses = new float[3];
            _vib_cosinuses = new float[3];
        }

        public void UpdateVibrateLogics()
        {
            if (GetEffectBlendWeight() * VibrateAmount <= 0f) return;
            _vib_blend = GetEffectBlendWeight() * VibrateAmount;


            // Calculating values for trigonometric animation
            // It will result in vector calculated of randomized but smart timed values thanks to trigonometric functions
            float intensityLog = Mathf.Max(0f, Mathf.Log(_vib_Intensity) / 5f);
            _vib_Intensity -= Mathf.Max(0f, intensityLog);

            _vib_Speed = Mathf.Min(75f, VibrateSpeed + _vib_Intensity * 4f - intensityLog * 5f);
            _vib_Range = VibrateRange + _vib_Intensity * 0.01f;

            _vib_Time += elapsedDelta * _vib_TrueSpeed;

            if (!DetailedVibrate)
            {
                _vib_sinuses[0] = Mathf.Sin(_vib_Time * _vib_RandomFloats[0] + _vib_RandomFloats[1]) * _vib_Range;
                _vib_sinuses[1] = Mathf.Sin(_vib_Time / 2.2f * _vib_RandomFloats[4] + _vib_RandomFloats[1]) * _vib_Range;
                _vib_sinuses[2] = Mathf.Sin(_vib_Time * _vib_RandomFloats[2] + _vib_RandomFloats[0]) * _vib_Range;

                _vib_cosinuses[0] = Mathf.Pow(Mathf.Cos(_vib_Time / 1.5f * _vib_RandomFloats[2]), 2) * _vib_Range;
                _vib_cosinuses[1] = Mathf.Cos(_vib_Time * _vib_RandomFloats[1] + _vib_RandomFloats[2]) * _vib_Range;
                _vib_cosinuses[2] = Mathf.Cos(_vib_Time * 1.24f * _vib_RandomFloats[3] + _vib_RandomFloats[0]) * _vib_Range;
            }

            _vib_TrueSpeed = _vib_Speed;


            // Computing vibrate for each bone
            Bone b = Bones[0];
            while(b != null)
            {
                Vibrate_Calculation(b);
                b = b.Child;
            }
        }


        /// <summary>
        /// Increasing animation intensity for a moment
        /// </summary>
        public void Vibrate_ExplosionShake(float value = 12f)
        {
            float newIntensity = Mathf.Min(_vib_Intensity + value, value * 3f);
            _vib_Time += value / 3f;
            if (newIntensity > _vib_Intensity) _vib_Intensity = newIntensity;
        }


        /// <summary>
        /// Choosing randomized animation offsets
        /// </summary>
        protected void Vibrate_ChooseNewSeed()
        {
            Random.InitState(Random.Range(0, 999999));
            _vib_RandomFloats = new float[6];
            for (int i = 0; i < 3; i++) _vib_RandomFloats[i] = Random.Range(0.8f, 1.0f);
            for (int i = 3; i < 6; i++) _vib_RandomFloats[i] = Random.Range(1.0f, 2.5f);
            _vib_Time = Random.Range(0f, 4f);
            _vib_TrueSpeed = _vib_Speed;
        }

        private void Vibrate_Calculation(Bone target)
        {
            Vector3 vibrationFactor = Vector3.zero;

            if (DetailedVibrate)
            {
                _vib_sinuses[0] = Mathf.Sin(_vib_Time * target.randomFloats[0] + target.randomFloats[1]) * _vib_Range;
                _vib_sinuses[1] = Mathf.Sin(_vib_Time / 2.2f * target.randomFloats[4] + target.randomFloats[1]) * _vib_Range;
                _vib_sinuses[2] = Mathf.Sin(_vib_Time * target.randomFloats[2] + target.randomFloats[0]) * _vib_Range;

                _vib_cosinuses[0] = Pow2(Mathf.Cos(_vib_Time / 1.5f * target.randomFloats[2])) * _vib_Range;
                _vib_cosinuses[1] = Mathf.Cos(_vib_Time * target.randomFloats[1] + target.randomFloats[2]) * _vib_Range;
                _vib_cosinuses[2] = Mathf.Cos(_vib_Time * 1.24f * target.randomFloats[3] + target.randomFloats[0]) * _vib_Range;
            }

            // Calculating vector value for each axis
            if (VibrateAxis.x != 0)
            {
                vibrationFactor.x = _vib_sinuses[target.randomInts[0]] * _vib_Range * target.randomFloats[0];
                vibrationFactor.x += _vib_cosinuses[target.randomInts[1]] * _vib_Range * target.randomFloats[1];
                vibrationFactor.x *= VibrateAxis.x;
            }

            if (VibrateAxis.y != 0)
            {
                vibrationFactor.y = _vib_cosinuses[target.randomInts[2]] * _vib_Range * target.randomFloats[2];
                vibrationFactor.y += _vib_sinuses[target.randomInts[3]] * _vib_Range * target.randomFloats[3];
                vibrationFactor.y *= VibrateAxis.y;
            }

            if (VibrateAxis.z != 0)
            {
                vibrationFactor.z = _vib_cosinuses[target.randomInts[4]] * _vib_Range * target.randomFloats[4];
                vibrationFactor.z += _vib_sinuses[target.randomInts[5]] * _vib_Range * target.randomFloats[5];
                vibrationFactor.z *= VibrateAxis.z;
            }

            if (VibratePosition != 0f)
            {
                float posBlend = _vib_blend * VibratePosBlend.Evaluate(target.Evaluation);
                if (posBlend > 0f) target.transform.localPosition += vibrationFactor * 0.005f * VibratePosition * posBlend;
            }

            if (VibrateRotation != 0f)
            {
                float rotBlend = _vib_blend * VibrateRotBlend.Evaluate(target.Evaluation);
                if (rotBlend > 0f)
                {
                    if (UniRotation == false)
                        target.transform.localRotation *= Quaternion.Euler(vibrationFactor * VibrateRotation * rotBlend);
                    else
                    {
                        target.transform.rotation = Quaternion.Euler(vibrationFactor * VibrateRotation * rotBlend) * target.transform.parent.rotation * target.transform.localRotation;
                    }
                }
            }

            if (VibrateScale != 0f)
            {
                float scaleBlend = _vib_blend * VibrateScaleBlend.Evaluate(target.Evaluation);
                if (scaleBlend > 0f) target.transform.localScale += vibrationFactor * 0.003f * VibrateScale * scaleBlend;
            }
        }

        private float Pow2(float toPower) { return toPower * toPower; }
    }
}
