using UnityEngine;
using System.Collections.Generic;

namespace Synthic
{
    public class SynthOut : MonoBehaviour
    {
        [SerializeField] private List<SynthProvider> providers;

        private float[] _mixBuffer;

        private void OnAudioFilterRead(float[] data, int channels)
        {
            if (channels != 2) return;

            // ensure mix buffer is the right size
            if (_mixBuffer == null || _mixBuffer.Length != data.Length)
                _mixBuffer = new float[data.Length];

            // clear output
            System.Array.Clear(data, 0, data.Length);

            foreach (var provider in providers)
            {
                if (provider == null) continue;

                // clear mix buffer before each provider
                System.Array.Clear(_mixBuffer, 0, _mixBuffer.Length);

                // fill mix buffer from this provider
                provider.FillBuffer(_mixBuffer);

                // accumulate into output
                for (int i = 0; i < data.Length; i++)
                    data[i] += _mixBuffer[i];
            }
        }
    }
}