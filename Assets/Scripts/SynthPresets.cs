using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace Synthic
{
    [System.Serializable]
    public class SynthPreset
    {
        public string name = "New Preset";

        [Header("Waveform")]
        public PolyphonicGenerator.WaveformType waveform = PolyphonicGenerator.WaveformType.Sine;

        [Header("ADSR")]
        [Range(0.001f, 5f)] public float attack  = 0.01f;
        [Range(0.001f, 5f)] public float decay   = 0.1f;
        [Range(0f, 1f)]     public float sustain = 0.7f;
        [Range(0.001f, 5f)] public float release = 0.3f;

        [Header("Frequency LFO")]
        public bool freqLfoEnabled = false;
        public LFO.WaveformType freqLfoWaveform = LFO.WaveformType.Sine;
        [Range(0.1f, 20f)] public float freqLfoRate      = 1f;
        [Range(0f, 1f)]    public float freqLfoDepth     = 0f;
        [Range(0f, 1f)]    public float freqLfoBaseValue = 0.5f;

        [Header("Amplitude LFO")]
        public bool ampLfoEnabled = false;
        public LFO.WaveformType ampLfoWaveform = LFO.WaveformType.Sine;
        [Range(0.1f, 20f)] public float ampLfoRate      = 1f;
        [Range(0f, 1f)]    public float ampLfoDepth     = 0f;
        [Range(0f, 1f)]    public float ampLfoBaseValue = 1f;
    }

    public class SynthPresets : MonoBehaviour
    {
[SerializeField] private PolyphonicGenerator generator; // point this to keyGenerator
        [SerializeField] private List<SynthPreset>   presets = new();

        private int _currentPresetIndex = -1;

        private void Awake()
        {
            InitializeDefaultPresets();
        }

        private void InitializeDefaultPresets()
        {
            // only add defaults if no presets defined in Inspector
            if (presets.Count > 0) return;

            presets = new List<SynthPreset>
            {
                new SynthPreset // 1 - clean sine
                {
                    name     = "Clean Sine",
                    waveform = PolyphonicGenerator.WaveformType.Sine,
                    attack   = 0.01f, decay = 0.1f, sustain = 0.7f, release = 0.3f,
                    freqLfoEnabled = false,
                    ampLfoEnabled  = false,
                },
                new SynthPreset // 2 - pluck
                {
                    name     = "Pluck",
                    waveform = PolyphonicGenerator.WaveformType.Saw,
                    attack   = 0.001f, decay = 0.3f, sustain = 0f, release = 0.1f,
                    freqLfoEnabled = false,
                    ampLfoEnabled  = false,
                },
                new SynthPreset // 3 - pad
                {
                    name     = "Pad",
                    waveform = PolyphonicGenerator.WaveformType.Sine,
                    attack   = 0.8f, decay = 0.5f, sustain = 0.8f, release = 1.5f,
                    freqLfoEnabled  = true,
                    freqLfoWaveform = LFO.WaveformType.Sine,
                    freqLfoRate     = 0.5f,
                    freqLfoDepth    = 0.02f,
                    freqLfoBaseValue = 0.5f,
                    ampLfoEnabled   = false,
                },
                new SynthPreset // 4 - wobble bass
                {
                    name     = "Wobble Bass",
                    waveform = PolyphonicGenerator.WaveformType.Saw,
                    attack   = 0.01f, decay = 0.2f, sustain = 0.8f, release = 0.2f,
                    freqLfoEnabled  = true,
                    freqLfoWaveform = LFO.WaveformType.Sine,
                    freqLfoRate     = 4f,
                    freqLfoDepth    = 0.3f,
                    freqLfoBaseValue = 0.5f,
                    ampLfoEnabled   = false,
                },
                new SynthPreset // 5 - tremolo
                {
                    name     = "Tremolo",
                    waveform = PolyphonicGenerator.WaveformType.Triangle,
                    attack   = 0.05f, decay = 0.1f, sustain = 0.9f, release = 0.3f,
                    freqLfoEnabled = false,
                    ampLfoEnabled   = true,
                    ampLfoWaveform  = LFO.WaveformType.Sine,
                    ampLfoRate      = 6f,
                    ampLfoDepth     = 0.8f,
                    ampLfoBaseValue = 0.8f,
                },
                new SynthPreset // 6 - organ
                {
                    name     = "Organ",
                    waveform = PolyphonicGenerator.WaveformType.Square,
                    attack   = 0.01f, decay = 0.01f, sustain = 1f, release = 0.05f,
                    freqLfoEnabled = false,
                    ampLfoEnabled  = false,
                },
                new SynthPreset // 7 - bell
                {
                    name     = "Bell",
                    waveform = PolyphonicGenerator.WaveformType.Sine,
                    attack   = 0.001f, decay = 1.5f, sustain = 0f, release = 0.5f,
                    freqLfoEnabled  = true,
                    freqLfoWaveform = LFO.WaveformType.Sine,
                    freqLfoRate     = 0.2f,
                    freqLfoDepth    = 0.01f,
                    freqLfoBaseValue = 0.5f,
                    ampLfoEnabled   = false,
                },
                new SynthPreset // 8 - stutter
                {
                    name     = "Stutter",
                    waveform = PolyphonicGenerator.WaveformType.Square,
                    attack   = 0.001f, decay = 0.05f, sustain = 0f, release = 0.01f,
                    freqLfoEnabled = false,
                    ampLfoEnabled   = true,
                    ampLfoWaveform  = LFO.WaveformType.Square,
                    ampLfoRate      = 8f,
                    ampLfoDepth     = 1f,
                    ampLfoBaseValue = 1f,
                },
                new SynthPreset // 9 - saw lead
                {
                    name     = "Saw Lead",
                    waveform = PolyphonicGenerator.WaveformType.Saw,
                    attack   = 0.05f, decay = 0.2f, sustain = 0.6f, release = 0.4f,
                    freqLfoEnabled  = true,
                    freqLfoWaveform = LFO.WaveformType.Sine,
                    freqLfoRate     = 5f,
                    freqLfoDepth    = 0.02f,
                    freqLfoBaseValue = 0.5f,
                    ampLfoEnabled   = false,
                },
            };
        }

        private void Update()
        {
            if (Keyboard.current == null) return;

            // 1-9 trigger presets
            if (Keyboard.current.digit1Key.wasPressedThisFrame) ApplyPreset(0);
            if (Keyboard.current.digit2Key.wasPressedThisFrame) ApplyPreset(1);
            if (Keyboard.current.digit3Key.wasPressedThisFrame) ApplyPreset(2);
            if (Keyboard.current.digit4Key.wasPressedThisFrame) ApplyPreset(3);
            if (Keyboard.current.digit5Key.wasPressedThisFrame) ApplyPreset(4);
            if (Keyboard.current.digit6Key.wasPressedThisFrame) ApplyPreset(5);
            if (Keyboard.current.digit7Key.wasPressedThisFrame) ApplyPreset(6);
            if (Keyboard.current.digit8Key.wasPressedThisFrame) ApplyPreset(7);
            if (Keyboard.current.digit9Key.wasPressedThisFrame) ApplyPreset(8);
        }

        public void ApplyPreset(int index)
        {
            if (generator == null) return;
            if (index < 0 || index >= presets.Count) return;

            SynthPreset preset   = presets[index];
            _currentPresetIndex  = index;

            // waveform
            generator.Waveform = preset.waveform;

            // ADSR
            generator.SetAttack(preset.attack);
            generator.SetDecay(preset.decay);
            generator.SetSustain(preset.sustain);
            generator.SetRelease(preset.release);

            // LFOs - find and update existing LFOs on the generator
            ApplyLFOSettings(preset);

            //Debug.Log($"Preset {index + 1} applied: {preset.name}");
        }

        private void ApplyLFOSettings(SynthPreset preset)
        {
            // get the LFO list from the generator via the public lfos field
            // we need to expose this — add a public property to PolyphonicGenerator
            var lfos = generator.GetLFOs();
            if (lfos == null) return;

            // find or create freq and amp LFOs
            LFO freqLfo = lfos.Find(l => l.lfoTarget == LFO.LFOTarget.Frequency);
            LFO ampLfo  = lfos.Find(l => l.lfoTarget == LFO.LFOTarget.Amplitude);

            if (preset.freqLfoEnabled)
            {
                if (freqLfo == null)
                {
                    freqLfo = new LFO();
                    lfos.Add(freqLfo);
                }
                freqLfo.lfoTarget  = LFO.LFOTarget.Frequency;
                freqLfo.waveform   = preset.freqLfoWaveform;
                freqLfo.rate       = preset.freqLfoRate;
                freqLfo.depth      = preset.freqLfoDepth;
                freqLfo.baseValue  = preset.freqLfoBaseValue;
            }
            else if (freqLfo != null)
            {
                lfos.Remove(freqLfo);
            }

            if (preset.ampLfoEnabled)
            {
                if (ampLfo == null)
                {
                    ampLfo = new LFO();
                    lfos.Add(ampLfo);
                }
                ampLfo.lfoTarget  = LFO.LFOTarget.Amplitude;
                ampLfo.waveform   = preset.ampLfoWaveform;
                ampLfo.rate       = preset.ampLfoRate;
                ampLfo.depth      = preset.ampLfoDepth;
                ampLfo.baseValue  = preset.ampLfoBaseValue;
            }
            else if (ampLfo != null)
            {
                lfos.Remove(ampLfo);
            }
        }

        public int CurrentPresetIndex => _currentPresetIndex;
        public SynthPreset CurrentPreset => _currentPresetIndex >= 0 && 
                                            _currentPresetIndex < presets.Count
            ? presets[_currentPresetIndex] : null;
    }
}