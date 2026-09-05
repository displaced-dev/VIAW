using System.Collections.Generic;
using TinyInspector;
using UnityEngine;

/// Summary
/// A Controller for enabling and disabling the cursor lock. 
/// Any request will free the cursor
/// 
/// RequestUnlock - Unlocks
/// RelinquishRequest - Frees The Request

namespace VIAW.Async.Auth
{
    public class CursorStateManager : MonoBehaviour
    {
        public static CursorStateManager Instance { get; private set; }

        [BoxGroup("Debug")]
        [SerializeField] private bool cursorLocked;
        [BoxGroup("Debug")]
        public List<MonoBehaviour> scriptsRequestingUnlock = new List<MonoBehaviour>();

        #region Unity
        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(this); }
        
            Instance = this;
        }

        public void FixedUpdate()
        {
            scriptsRequestingUnlock.RemoveAll(script => script == null);

            if (scriptsRequestingUnlock.Count > 0)
            {
                cursorLocked = false;
                UnityEngine.Cursor.lockState = CursorLockMode.Confined;
                UnityEngine.Cursor.visible = true;
            }
            else
            {
                cursorLocked = true;
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                UnityEngine.Cursor.visible = false;
            }
        }
        #endregion

        #region Public Accessors
        public void RequestUnlock(MonoBehaviour requestingScript)
        {
            if (requestingScript != null && !scriptsRequestingUnlock.Contains(requestingScript))
            {
                scriptsRequestingUnlock.Add(requestingScript);
            }
        }

        public void RelinquishRequest(MonoBehaviour requestingScript)
        {
            if (requestingScript != null)
            {
                scriptsRequestingUnlock.Remove(requestingScript);
            }
        }
        #endregion
    }
}