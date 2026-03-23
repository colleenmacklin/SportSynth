using UnityEngine;

namespace Synthic
{
    [System.Serializable]
    public class LFO
    {
        public enum LFOTarget { Frequency, Amplitude }
        public enum WaveformType { Sine, Saw, Square, Triangle }

        [SerializeField] public LFOTarget lfoTarget = LFOTarget.Frequency;
        [SerializeField] public WaveformType waveform = WaveformType.Sine;
        [SerializeField, Range(0.1f, 20f)] public float rate = 1f;
        [SerializeField, Range(0f, 1f)] public float depth = 0.5f;
        [SerializeField, Range(0f, 1f)] public float baseValue = 0.5f;

        private float _phase;

        public float Update(float deltaTime)
        {
            _phase = (_phase + rate * deltaTime) % 1f;

            float lfoValue = waveform switch
            {
                WaveformType.Sine     => Mathf.Sin(_phase * 2f * Mathf.PI),
                WaveformType.Saw      => _phase * 2f - 1f,
                WaveformType.Square   => _phase < 0.5f ? 1f : -1f,
                WaveformType.Triangle => _phase < 0.5f ? (_phase * 4f - 1f) : (3f - _phase * 4f),
                _                     => 0f
            };

            return baseValue + lfoValue * depth * baseValue;
        }
    }
}