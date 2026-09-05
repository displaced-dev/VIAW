// Created by Victor Engström
// Copyright 2026 Sonigon AB
// http://www.sonity.org/

using UnityEngine;

namespace ExampleSonity {

    [AddComponentMenu("")]
    public class ExampleFlyingCamera : MonoBehaviour {

        // Use WASD to move forward, left, back, right
        // Use Q/E to move up/down
        // Use Shift to move faster
        // Use the Right Mouse Button to look around

        private float mainSpeed = 5000f;
        private float shiftMultiply = 10;
        private float shiftMultiplyCurrent = 1f;
#if ENABLE_INPUT_SYSTEM
        private float mouseSpeed = 0.1f;
#elif ENABLE_LEGACY_INPUT_MANAGER
        private float mouseSpeed = 500f;
#endif
        private float yaw = 0.0f;
        private float pitch = 0.0f;
        private Rigidbody cachedRigidbody;

        void Start() {
            // Sets gravity, in case it was changed in the preferences
            Physics.gravity = new Vector3(0f, -9.81f, 0f);
            cachedRigidbody = gameObject.AddComponent<Rigidbody>();
            cachedRigidbody.useGravity = false;
#if UNITY_6000_0_OR_NEWER
            cachedRigidbody.linearDamping = 4f;
#else
            cachedRigidbody.drag = 4f;
#endif
        }

        void Update() {
            // For looking around
            if (GetPressedMouseRight()) {
                yaw += GetMouseAxisX() * mouseSpeed;
                pitch -= GetMouseAxisY() * mouseSpeed;
                transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
            }
        }

        void FixedUpdate() {
            // For moving the camera
            if (GetKeyPressed("LeftShift")) {
                shiftMultiplyCurrent = shiftMultiply;
            } else {
                shiftMultiplyCurrent = 1f;
            }
            if (GetKeyPressed("W")) {
                cachedRigidbody.AddRelativeForce(Vector3.forward * mainSpeed * shiftMultiplyCurrent * Time.fixedDeltaTime);
            }
            if (GetKeyPressed("A")) {
                cachedRigidbody.AddRelativeForce(Vector3.left * mainSpeed * shiftMultiplyCurrent * Time.fixedDeltaTime);
            }
            if (GetKeyPressed("S")) {
                cachedRigidbody.AddRelativeForce(Vector3.back * mainSpeed * shiftMultiplyCurrent * Time.fixedDeltaTime);
            }
            if (GetKeyPressed("D")) {
                cachedRigidbody.AddRelativeForce(Vector3.right * mainSpeed * shiftMultiplyCurrent * Time.fixedDeltaTime);
            }
            if (GetKeyPressed("Q")) {
                cachedRigidbody.AddForce(Vector3.up * mainSpeed * shiftMultiplyCurrent * Time.fixedDeltaTime);
            }
            if (GetKeyPressed("E")) {
                cachedRigidbody.AddForce(Vector3.down * mainSpeed * shiftMultiplyCurrent * Time.fixedDeltaTime);
            }
        }

        private bool GetKeyPressed(string key) {
#if ENABLE_INPUT_SYSTEM
            return UnityEngine.InputSystem.Keyboard.current[(UnityEngine.InputSystem.Key)System.Enum.Parse(typeof(UnityEngine.InputSystem.Key), key)].isPressed;
#elif ENABLE_LEGACY_INPUT_MANAGER
            return Input.GetKey((UnityEngine.KeyCode)System.Enum.Parse(typeof(UnityEngine.KeyCode), key));
#endif
        }

        private bool GetPressedMouseRight() {
#if ENABLE_INPUT_SYSTEM
            return UnityEngine.InputSystem.Mouse.current.rightButton.isPressed;
#elif ENABLE_LEGACY_INPUT_MANAGER
            return Input.GetKey(KeyCode.Mouse1);
#endif
        }

        private float GetMouseAxisX() {
#if ENABLE_INPUT_SYSTEM
            return UnityEngine.InputSystem.Mouse.current.delta.ReadValue().x;
#elif ENABLE_LEGACY_INPUT_MANAGER
            return Input.GetAxis("Mouse X") * Time.deltaTime;
#endif
        }

        private float GetMouseAxisY() {
#if ENABLE_INPUT_SYSTEM
            return UnityEngine.InputSystem.Mouse.current.delta.ReadValue().y;
#elif ENABLE_LEGACY_INPUT_MANAGER
            return Input.GetAxis("Mouse Y") * Time.deltaTime;
#endif
        }
    }
}