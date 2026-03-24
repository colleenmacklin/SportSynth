using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace Synthic
{
    public class SphereSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PolyphonicGenerator generator;
        [SerializeField] private NoteInput           noteInput;
        [SerializeField] private GameObject          spherePrefab;
        [SerializeField] private Camera              playerCamera;

        [Header("Throw Settings")]
        [SerializeField, Range(1f, 30f)]  private float throwForce    = 10f;
        [SerializeField, Range(0f, 1f)]   private float throwArcAngle = 0.8f;
        [SerializeField, Range(0.1f, 2f)] private float sphereRadius  = 0.2f;

        [Header("Sphere Settings")]
        [SerializeField] private float minCollisionVelocity = 0.05f;
        [SerializeField] private float maxCollisionVelocity = 10f;

        [Header("Caps Lock Mode")]
        [SerializeField, Range(0f, 5f)]  private float capsLockAttack  = 0.05f;
        [SerializeField, Range(0f, 1f)]  private float capsLockSustain = 1f;
        [SerializeField, Range(0f, 5f)]  private float capsLockRelease = 0.5f;

        [Header("Rhythmic Bounce")]
[SerializeField] private bool  rhythmicBounce  = true;
[SerializeField, Range(1, 16)] private int beatsPerCycle = 1;
[SerializeField, Range(0f, 1f)] private float sphereBounciness = 0.8f;


        // note key layout - one octave, bottom row only
        private static readonly Dictionary<Key, Note.Name> NoteKeys =
            new Dictionary<Key, Note.Name>
        {
            { Key.Z, Note.Name.C      },
            { Key.S, Note.Name.CSharp },
            { Key.X, Note.Name.D      },
            { Key.D, Note.Name.DSharp },
            { Key.C, Note.Name.E      },
            { Key.V, Note.Name.F      },
            { Key.G, Note.Name.FSharp },
            { Key.B, Note.Name.G      },
            { Key.H, Note.Name.GSharp },
            { Key.N, Note.Name.A      },
            { Key.J, Note.Name.ASharp },
            { Key.M, Note.Name.B      },
        };

        private int                    _currentOctave  = 3;
        private readonly List<GameObject> _spawnedSpheres = new();
        private readonly HashSet<Key>     _heldNoteKeys   = new();

        // track which frequencies are held in caps lock mode
        private readonly Dictionary<Key, float> _heldCapsLockNotes = new();

private void Awake()
{
    SynthSphere.SetPlayerTransform(playerCamera.transform);
    RhythmicSynthSphere.SetPlayerTransform(playerCamera.transform);
}

        private void Update()
        {
            if (Keyboard.current == null) return;

            HandleOctaveKeys();
            HandleNoteKeys();
            HandleSphereManagement();
        }

        private void HandleOctaveKeys()
        {
            // comma = octave down, period = octave up
            if (Keyboard.current.commaKey.wasPressedThisFrame)
                _currentOctave = Mathf.Max(0, _currentOctave - 1);
            if (Keyboard.current.periodKey.wasPressedThisFrame)
                _currentOctave = Mathf.Min(8, _currentOctave + 1);
        }

        private void HandleNoteKeys()
        {
            bool capsLockOn = Keyboard.current.capsLockKey.isPressed;

            foreach (var kvp in NoteKeys)
            {
                Key       inputKey  = kvp.Key;
                Note.Name noteName  = kvp.Value;
                float     frequency = Note.Get(noteName, _currentOctave);

                bool isDown = Keyboard.current[inputKey].wasPressedThisFrame;
                bool isUp   = Keyboard.current[inputKey].wasReleasedThisFrame;

                if (capsLockOn)
                {
                    // caps lock mode - hold note, no sphere spawning
                    if (isDown && !_heldNoteKeys.Contains(inputKey))
{
    _heldNoteKeys.Add(inputKey);
    _heldCapsLockNotes[inputKey] = frequency;

    // save current ADSR before overriding
    generator.SaveADSR();

    generator.SetAttack(capsLockAttack);
    generator.SetSustain(capsLockSustain);
    generator.SetRelease(capsLockRelease);
    generator.NoteOn(frequency, 0.8f);
}
else if (isUp && _heldNoteKeys.Contains(inputKey))
{
    _heldNoteKeys.Remove(inputKey);
    if (_heldCapsLockNotes.TryGetValue(inputKey, out float heldFreq))
    {
        generator.NoteOff(heldFreq);
        _heldCapsLockNotes.Remove(inputKey);
    }

    // restore ADSR only when all caps lock notes are released
    if (_heldCapsLockNotes.Count == 0)
        generator.RestoreADSR();
}
                }
                else
                {
                    // normal mode - release any held caps lock notes first
                    if (_heldNoteKeys.Contains(inputKey) && isUp)
                    {
                        _heldNoteKeys.Remove(inputKey);
                        if (_heldCapsLockNotes.TryGetValue(inputKey, out float heldFreq))
                        {
                            generator.NoteOff(heldFreq);
                            _heldCapsLockNotes.Remove(inputKey);
                        }
                    }

                    // spawn sphere on key down
                    if (isDown && !_heldNoteKeys.Contains(inputKey))
                    {
                        _heldNoteKeys.Add(inputKey);
                        SpawnSphere(frequency);
                    }
                    else if (isUp)
                    {
                        _heldNoteKeys.Remove(inputKey);
                    }
                }
            }
        }

private void HandleSphereManagement()
{
    if (Keyboard.current == null) return;

    bool shiftHeld = Keyboard.current.leftShiftKey.isPressed ||
                     Keyboard.current.rightShiftKey.isPressed;

    // on Mac, backspace = delete key, deleteKey = Fn+Delete
    bool deletePressed = Keyboard.current.backspaceKey.wasPressedThisFrame;

    if (deletePressed)
    {
        if (shiftHeld)
            ClearAllSpheres();
        else
            ClearLastSphere();
    }
}

        private void SpawnSphere(float frequency)
{
    if (spherePrefab == null || playerCamera == null) return;

    Vector3 spawnPosition = playerCamera.transform.position
                          + playerCamera.transform.forward * (sphereRadius * 2f + 0.1f);

    GameObject sphere = Instantiate(spherePrefab, spawnPosition, Quaternion.identity);
    sphere.transform.localScale = Vector3.one * sphereRadius * 2f;

    if (rhythmicBounce)
    {
        // remove SynthSphere if present and add RhythmicSynthSphere
        var existing = sphere.GetComponent<SynthSphere>();
        if (existing != null) Destroy(existing);

        var rhythmicSphere = sphere.GetComponent<RhythmicSynthSphere>();
        if (rhythmicSphere == null)
            rhythmicSphere = sphere.AddComponent<RhythmicSynthSphere>();

        rhythmicSphere.Initialize(generator, frequency, beatsPerCycle,
                                  sphereBounciness, minCollisionVelocity,
                                  maxCollisionVelocity);
    }
    else
    {
        var synthSphere = sphere.GetComponent<SynthSphere>();
        if (synthSphere == null)
            synthSphere = sphere.AddComponent<SynthSphere>();

        synthSphere.Initialize(generator, frequency);

        var minVelField = typeof(SynthSphere).GetField("minCollisionVelocity",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var maxVelField = typeof(SynthSphere).GetField("maxCollisionVelocity",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        minVelField?.SetValue(synthSphere, minCollisionVelocity);
        maxVelField?.SetValue(synthSphere, maxCollisionVelocity);
    }

    var rb = sphere.GetComponent<Rigidbody>();
    if (rb != null)
    {
        Vector3 throwDirection = playerCamera.transform.forward
                               + playerCamera.transform.up * throwArcAngle;
        rb.linearVelocity = throwDirection.normalized * throwForce;
    }

    _spawnedSpheres.Add(sphere);
}

        private void ClearLastSphere()
        {
            if (_spawnedSpheres.Count == 0) return;
            GameObject last = _spawnedSpheres[_spawnedSpheres.Count - 1];
            _spawnedSpheres.RemoveAt(_spawnedSpheres.Count - 1);
            if (last != null) Destroy(last);
        }

        public void ClearAllSpheres()
        {
            foreach (var sphere in _spawnedSpheres)
                if (sphere != null) Destroy(sphere);
            _spawnedSpheres.Clear();
        }
    }
}