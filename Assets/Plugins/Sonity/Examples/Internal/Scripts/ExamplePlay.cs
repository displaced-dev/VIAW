// Created by Victor Engström
// Copyright 2026 Sonigon AB
// http://www.sonity.org/

using UnityEngine;
using Sonity;

namespace ExampleSonity {

    [AddComponentMenu("")]
    public class ExamplePlay : MonoBehaviour {

        public SoundEvent soundEvent;

        void Update() {
            // Plays the sound on left mouse click
            if (GetPressedMouseLeft()) {
                soundEvent.Play(transform);
            }
            // Stops the sound on right mouse click
            if (GetPressedMouseRight()) {
                soundEvent.Stop(transform);
            }
        }

        private bool GetPressedMouseLeft() {
#if ENABLE_INPUT_SYSTEM
            return UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame;
#elif ENABLE_LEGACY_INPUT_MANAGER
            return Input.GetKey(KeyCode.Mouse0);
#endif
        }

        private bool GetPressedMouseRight() {
#if ENABLE_INPUT_SYSTEM
            return UnityEngine.InputSystem.Mouse.current.rightButton.wasPressedThisFrame;
#elif ENABLE_LEGACY_INPUT_MANAGER
            return Input.GetKeyDown(KeyCode.Mouse1);
#endif
        }
    }
}