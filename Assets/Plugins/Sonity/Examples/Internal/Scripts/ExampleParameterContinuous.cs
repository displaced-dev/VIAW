// Created by Victor Engström
// Copyright 2026 Sonigon AB
// http://www.sonity.org/

using UnityEngine;
using Sonity;

namespace ExampleSonity {

    [AddComponentMenu("")]
    public class ExampleParameterContinuous : MonoBehaviour {

        public SoundEvent soundEvent;
        public SoundParameterPitchSemitone parameterPitch = new SoundParameterPitchSemitone(0f, UpdateMode.Continuous);

        private void Start() {
            // Play Sound with the parameter
            soundEvent.Play(transform, parameterPitch);
        }

        void Update() {
            if (GetPressedMouseLeft()) {

                // SoundParameter lower pitch
                parameterPitch.PitchSemitone -= 1f;
            } else if (GetPressedMouseRight()) {

                // SoundParameter increase pitch
                parameterPitch.PitchSemitone += 1f;
            }

            // Setting gui text
            GetComponent<ExampleHelperGuiText>().textString = $"SoundParameter Continuous\nPress mouse left/right to lower/increase pitch\nCurrent pitch is {parameterPitch.PitchSemitone.ToString("0")} semitones";
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