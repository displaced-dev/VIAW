// Created by Victor Engström
// Copyright 2026 Sonigon AB
// http://www.sonity.org/

using UnityEngine;

namespace ExampleSonity {

    [AddComponentMenu("")]
    public class ExampleMusicSwitcher : MonoBehaviour {

        private void Update() {

            if (GetPressedMouseLeft()) {
                // Play main menu music
                SonityTemplate.TemplateSoundMusicManager.Instance.PlayMainMenu();
            } else if (GetPressedMouseRight()) {
                // Play ingame music
                SonityTemplate.TemplateSoundMusicManager.Instance.PlayIngame();
            }

            // Setting gui text
            GetComponent<ExampleHelperGuiText>().textString = $"Press left/right mouse buttons\nto play main menu/ingame music\nCurrent music is {SonityTemplate.TemplateSoundMusicManager.Instance.GetMusicPlaying()}";
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