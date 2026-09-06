using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using VIAW.Data;
using TinyInspector;
using System;

namespace VIAW.Async.Auth {
    public abstract class _InputAuth : MonoBehaviour
    {
        [BoxGroup("InputAuth/Config")]
        public List<InputFIlterSO> _inputChannels = new List<InputFIlterSO>();
        
        [BoxGroup("InputAuth/Debug")]
        public bool _inputAuthorized;

        private bool _requestingInput;
        
        protected PlayerInputActions _inputActions;
        protected PlayerInputActions.GameplayActions _input;

        #region Input Auth Integration
        public void aGrantInput()
        {
            _inputAuthorized = true;
            aOnInputGranted();
        }

        public void aDenyInput()
        {
            _inputAuthorized = false;
            aOnInputDenied();
        }

        protected virtual void aOnInputGranted() { }
        protected virtual void aOnInputDenied() { }
        #endregion

        #region Input Auth Manager Integration
        public void aInputInit(bool autoPopulateGame = false)
        {
            if (InputAuthManager.Instance != null)
            {
                _inputActions = InputRemapManager.Instance.InputActions;

                if(autoPopulateGame == true) { _input = _inputActions.Gameplay; }
            }
            else
            {
                Debug.LogError("Input Auth Manager could not be found.");
            }
        }
        #endregion
    }
}