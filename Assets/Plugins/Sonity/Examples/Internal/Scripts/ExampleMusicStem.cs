// Created by Victor Engström
// Copyright 2026 Sonigon AB
// http://www.sonity.org/

using UnityEngine;
using Sonity;

namespace ExampleSonity {

    [AddComponentMenu("")]
    public class ExampleMusicStem : MonoBehaviour {

        public SoundEvent musicStem;
        private SoundParameterIntensity parameterIntensity = new SoundParameterIntensity(0f, UpdateMode.Continuous);

        private void Start() {
            musicStem.MusicPlay(true, true, parameterIntensity);
        }

        private void Update() {

            if (GetPressedMouseLeft()) {
                // Set intensity to low
                parameterIntensity.Intensity = 0f;
            } else if (GetPressedMouseRight()) {
                // Set intensity to high
                parameterIntensity.Intensity = 1f;
            }

            // Setting gui text
            if (parameterIntensity.Intensity > 0.5f) {
                GetComponent<ExampleHelperGuiText>().textString = $"Press left/right mouse buttons\nto change from high to low intensity\nCurrently it is high intensity";
            } else {
                GetComponent<ExampleHelperGuiText>().textString = $"Press left/right mouse buttons\nto change from high to low intensity\nCurrently it is low intensity";
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