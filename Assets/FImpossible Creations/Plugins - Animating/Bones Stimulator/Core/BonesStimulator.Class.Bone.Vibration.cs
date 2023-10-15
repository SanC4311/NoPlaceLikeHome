using FIMSpace.FTools;
using UnityEngine;

namespace FIMSpace.BonesStimulation
{
    public partial class BonesStimulator
    {
        public partial class Bone
        {
            public float PosMul = 1f;
            public float RotMul = 1f;
            public float ScaleMul = 1f;

            public float[] randomFloats;
            public int[] randomInts;

            public void VibrateInitialize()
            {
                randomFloats = new float[6];
                randomInts = new int[6];

                for (int i = 0; i < randomFloats.Length; i++)
                {
                    randomFloats[i] = Random.Range(0.7f, 1.7f);
                    randomInts[i] = Random.Range(0, 3);
                }


            }

        }
    }
}