using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.BonesStimulation
{
    public partial class BonesStimulator
    {
        private float _squeeze_time;

        public void SqueezeInitialize()
        {
            _squeeze_time = Random.Range(-Mathf.PI, Mathf.PI);
        }

        public void UpdateSqueezeLogics()
        {
            if (GetEffectBlendWeight() * SqueezingAmount <= 0f) return;

            _squeeze_time += elapsedDelta * SqueezingSpeed;
            float multiplier = SqueezingMultiply * SqueezingAmount * GetEffectBlendWeight();
            Vector3 newScale = new Vector3(1f + (Mathf.Sin(_squeeze_time) * SqueezingAxis.x) * multiplier, 1f + (Mathf.Cos(_squeeze_time) * SqueezingAxis.y) * multiplier, 1f + (Mathf.Sin(_squeeze_time) * SqueezingAxis.z) * multiplier);
            
            Bones[0].transform.localScale = Vector3.Scale(Bones[0].transform.localScale, newScale);
        }
    }
}