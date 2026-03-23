using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace Synthic
{
    public class SphereSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PolyphonicGenerator generator;
        [SerializeField] private GameObject spherePrefab;
        [SerializeField] private Camera playerCamera;

        [Header("Throw Settings")]
        [SerializeField, Range(1f, 30f)]  private float throwForce     = 10f;
        [SerializeField, Range(0f, 1f)]   private float throwArcAngle  = 0.8f; // upward angle 0=straight 1=45deg
        [SerializeField, Range(0.1f, 2f)] private float sphereRadius   = 0.2f;
        [SerializeField, Range(0f, 1f)]   private float spawnVelocity  = 0.8f; // velocity of the throw note

        [Header("Sphere Settings")]
        [SerializeField] private float minCollisionVelocity = 0.05f;
        [SerializeField] private float maxCollisionVelocity = 10f;

        private readonly List<GameObject> _spawnedSpheres = new();
        private readonly HashSet<KeyCode> _heldKeys       = new();
private void Awake()
{
    SynthSphere.SetPlayerTransform(playerCamera.transform);
}
        private void Update()
{
    foreach (var kvp in Note.KeyboardMap)
    {
        KeyCode key     = kvp.Key;
        float frequency = kvp.Value;

        // convert legacy KeyCode to new Input System Key
        Key inputKey = KeyCodeToKey(key);
        if (inputKey == Key.None) continue;

        bool isDown = Keyboard.current != null && Keyboard.current[inputKey].wasPressedThisFrame;
        bool isUp   = Keyboard.current != null && Keyboard.current[inputKey].wasReleasedThisFrame;

        if (isDown && !_heldKeys.Contains(key))
        {
            _heldKeys.Add(key);
            SpawnSphere(frequency);
        }
        else if (isUp && _heldKeys.Contains(key))
        {
            _heldKeys.Remove(key);
        }
    }

    // clear all spheres with backspace
    if (Keyboard.current != null && Keyboard.current.backspaceKey.wasPressedThisFrame)
        ClearAllSpheres();
}

private Key KeyCodeToKey(KeyCode keyCode)
{
    return keyCode switch
    {
        KeyCode.A => Key.A, KeyCode.B => Key.B, KeyCode.C => Key.C,
        KeyCode.D => Key.D, KeyCode.E => Key.E, KeyCode.F => Key.F,
        KeyCode.G => Key.G, KeyCode.H => Key.H, KeyCode.I => Key.I,
        KeyCode.J => Key.J, KeyCode.K => Key.K, KeyCode.L => Key.L,
        KeyCode.M => Key.M, KeyCode.N => Key.N, KeyCode.O => Key.O,
        KeyCode.P => Key.P, KeyCode.Q => Key.Q, KeyCode.R => Key.R,
        KeyCode.S => Key.S, KeyCode.T => Key.T, KeyCode.U => Key.U,
        KeyCode.V => Key.V, KeyCode.W => Key.W, KeyCode.X => Key.X,
        KeyCode.Y => Key.Y, KeyCode.Z => Key.Z,
        KeyCode.Alpha0 => Key.Digit0, KeyCode.Alpha1 => Key.Digit1,
        KeyCode.Alpha2 => Key.Digit2, KeyCode.Alpha3 => Key.Digit3,
        KeyCode.Alpha4 => Key.Digit4, KeyCode.Alpha5 => Key.Digit5,
        KeyCode.Alpha6 => Key.Digit6, KeyCode.Alpha7 => Key.Digit7,
        KeyCode.Alpha8 => Key.Digit8, KeyCode.Alpha9 => Key.Digit9,
        _ => Key.None
    };
}

        private void SpawnSphere(float frequency)
        {
            if (spherePrefab == null || playerCamera == null) return;

            // spawn slightly in front of the camera
            Vector3 spawnPosition = playerCamera.transform.position
                                  + playerCamera.transform.forward * (sphereRadius * 2f + 0.1f);

            GameObject sphere = Instantiate(spherePrefab, spawnPosition, Quaternion.identity);
            sphere.transform.localScale = Vector3.one * sphereRadius * 2f;

            // set up SynthSphere component
            var synthSphere = sphere.GetComponent<SynthSphere>();
            if (synthSphere == null)
                synthSphere = sphere.AddComponent<SynthSphere>();

            // pass settings through to the sphere
            synthSphere.Initialize(generator, frequency);

            // set collision velocity thresholds via serialized fields
            var minVelField = typeof(SynthSphere).GetField("minCollisionVelocity",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var maxVelField = typeof(SynthSphere).GetField("maxCollisionVelocity",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            minVelField?.SetValue(synthSphere, minCollisionVelocity);
            maxVelField?.SetValue(synthSphere, maxCollisionVelocity);

            // launch with throw arc
            var rb = sphere.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 throwDirection = playerCamera.transform.forward
                                       + playerCamera.transform.up * throwArcAngle;
                rb.linearVelocity = throwDirection.normalized * throwForce;
            }

            // play the throw note
            //synthSphere.PlayNote(spawnVelocity);

            _spawnedSpheres.Add(sphere);
        }

        public void ClearAllSpheres()
        {
            foreach (var sphere in _spawnedSpheres)
            {
                if (sphere != null)
                    Destroy(sphere);
            }
            _spawnedSpheres.Clear();
        }
    }
}