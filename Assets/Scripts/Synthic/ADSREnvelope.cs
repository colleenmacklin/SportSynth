using UnityEngine;

namespace Synthic
{
    [System.Serializable]
    public struct ADSREnvelope
    {
        [Range(0.001f, 5f)] public float attack;
        [Range(0.001f, 5f)] public float decay;
        [Range(0f, 1f)]     public float sustain;
        [Range(0.001f, 5f)] public float release;
    }
}