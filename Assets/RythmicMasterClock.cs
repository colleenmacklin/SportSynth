using UnityEngine;
using System.Collections.Generic;

namespace Synthic
{
    public class RhythmicMasterClock : MonoBehaviour
    {
        public static RhythmicMasterClock Instance { get; private set; }

        [SerializeField] private float bpm = 120f;
        [SerializeField] private Sequencer sequencer;

        private float _masterTimer = 0f;
        private List<RhythmicBounceSphere> _spheres = new();

        public float BPM         => bpm;
        public float MasterTimer => _masterTimer;
public float BeatDuration     => 60f / bpm;
public float SixteenthDuration => 60f / (bpm * 4f); // matches sequencer step duration

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            // find all rhythmic spheres in the scene
            RefreshSphereList();
        }

private void Update()
{
    if (sequencer != null)
        bpm = sequencer.BPM;

    _masterTimer += Time.deltaTime;

    // sync spheres on quarter note boundaries
    float quarterNote = 60f / bpm;
    if (_masterTimer >= quarterNote)
    {
        _masterTimer -= quarterNote;
        foreach (var sphere in _spheres)
        {
            if (sphere != null)
                sphere.Resync(_masterTimer);
        }
    }
}
        public void RefreshSphereList()
        {
            _spheres.Clear();
            _spheres.AddRange(FindObjectsByType<RhythmicBounceSphere>(FindObjectsSortMode.None));
            foreach (var sphere in _spheres)
                sphere.SetBPM(bpm);
        }

        public void SetBPM(float newBpm)
        {
            bpm = newBpm;
            foreach (var sphere in _spheres)
                sphere.SetBPM(bpm);
        }
    }
}