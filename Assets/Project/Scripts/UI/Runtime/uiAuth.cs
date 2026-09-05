using System.Collections;
using System.Collections.Generic;
using VIAW.Async.Auth;
using TinyInspector;
using UnityEngine;

namespace VIAW.UI {
    public class uiAuth : _InputAuth
    {
        [BoxGroup("Config Auth")]
        public bool requestInput;
        [BoxGroup("Config Auth")]
        public bool requestCursor;

        void OnEnable()
        {
            if(requestCursor) { CursorStateManager.Instance.RequestUnlock(this); }
            if(requestInput) { InputAuthManager.Instance.RequestInput(this); }
        }

        void OnDisable()
        {
            if(requestCursor) { CursorStateManager.Instance.RelinquishRequest(this); }
            if(requestInput) { InputAuthManager.Instance.RelinquishRequest(this); }
        }
    }
}