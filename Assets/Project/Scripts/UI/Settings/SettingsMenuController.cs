using UnityEngine;
using TinyInspector;
using VIAW.Async.Auth;

namespace VIAW.UI
{
    public class SettingsMenuController : _InputAuth
    {
        [BoxGroup("Scene Refs")]
        public GameObject settingsMenuToggle;
        private bool settingsOpen;
    
        void Start() {
            Initialize();
        }

        void Update() {
            if(_inputActions == null) {
                Initialize();
                return;
            }
            
            if(_input.Pause.WasPressedThisFrame()) {
                if(settingsOpen) {
                    CloseSettings();
                }
                else {
                    OpenSettings();
                }
            }
        }

        void Initialize() {
            aInputInit();
            _input = _inputActions.Gameplay;
        }

        void OpenSettings() {
            settingsOpen = true;
            CursorStateManager.Instance.RequestUnlock(this);
            InputAuthManager.Instance.RequestInput(this);
        }

        void CloseSettings() {
            settingsOpen = false;
            CursorStateManager.Instance.RelinquishRequest(this);
            InputAuthManager.Instance.RelinquishRequest(this);
        }
    }
}
