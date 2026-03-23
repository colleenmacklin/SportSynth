using UnityEngine;

namespace Synthic
{
    public class PlatterSpinner : MonoBehaviour
    {
        [Header("BPM Settings")]
        [SerializeField, Range(60f, 180f)] private float bpm = 120f;
        [SerializeField] private int beatsPerRevolution = 4; // one full rotation = one measure

        [Header("Rotation")]
        [SerializeField] private Vector3 rotationAxis = Vector3.up; // spin on Y axis
        [SerializeField] private bool clockwise = true;

        // public property so the BPM slider can set it
        public float BPM
        {
            get => bpm;
            set => bpm = Mathf.Clamp(value, 60f, 180f);
        }

        // current rotation speed in degrees per second
        public float DegreesPerSecond => (360f / (beatsPerRevolution * (60f / bpm)));

        private void Update()
        {
            float direction       = clockwise ? -1f : 1f;
            float degreesPerFrame = DegreesPerSecond * Time.deltaTime * direction;
            transform.Rotate(rotationAxis, degreesPerFrame, Space.Self);
        }

        // sync BPM from sequencer or master clock
        public void SetBPM(float newBpm)
        {
            bpm = Mathf.Clamp(newBpm, 60f, 180f);
        }

        // returns current rotation as 0-1 progress through one revolution
        // useful for syncing other elements to the platter position
        public float RevolutionProgress =>
            (transform.localEulerAngles.y % 360f) / 360f;

        // returns which 16th note position the platter is currently at (0-15)
        public int Current16thStep =>
            Mathf.FloorToInt(RevolutionProgress * 16f) % 16;
    }
}