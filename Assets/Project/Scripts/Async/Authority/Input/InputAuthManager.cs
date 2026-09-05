using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VIAW.Async;
using TinyInspector;

/// Summary
/// Controller that authorizes or de-authorizes access to each _InputAuth
/// 
/// 
/// Note: This does not give global input auth actionable power, that is the responsibility of _InputAuth to limit it's own access.

namespace VIAW.Async.Auth
{
    public class InputAuthManager : MonoBehaviour
    {
        public static InputAuthManager Instance { get; private set; }

        // Root grouping for the Auth Managmenet
        [HorizontalGroup("Parsing")]

        [BoxGroup("Parsing/Request")]
        [SerializeField] private List<_InputAuth> scriptsRequestingInput = new List<_InputAuth>();
        
        [BoxGroup("Parsing/Active")]
        [SerializeField] private List<_InputAuth> currentlyAuthorized = new List<_InputAuth>();

        #region Unity
        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(this); }

            Instance = this;
        }

        public void FixedUpdate()
        {
            scriptsRequestingInput.RemoveAll(script => script == null);

            if (scriptsRequestingInput.Count != 0)
            {
                EvaluateInputAuthorization();
            }
        }
        #endregion

        #region Public Accessors
        public void RequestInput(_InputAuth requestingScript)
        {
            if (requestingScript != null && !scriptsRequestingInput.Contains(requestingScript))
            {
                scriptsRequestingInput.Add(requestingScript);
                EvaluateInputAuthorization();
            }
        }

        public void RelinquishRequest(_InputAuth requestingScript)
        {
            if (requestingScript != null && scriptsRequestingInput.Remove(requestingScript))
            {
                EvaluateInputAuthorization();
            }
        }
        #endregion

        #region Helpers
        public bool IsFilterPermitted(float filterValue)
        {
            if (scriptsRequestingInput.Count == 0)
                return true;

            float highestFilter = GetHighestRequestedFilter();
            return filterValue >= highestFilter;
        }

        /// The highest filter value across every channel every requesting script currently holds.
        private float GetHighestRequestedFilter()
        {
            float highest = float.NegativeInfinity;

            foreach (var script in scriptsRequestingInput)
            {
                if (script._inputChannels == null)
                    continue;

                foreach (var channel in script._inputChannels)
                {
                    if (channel != null && channel.filterVal > highest)
                    {
                        highest = channel.filterVal;
                    }
                }
            }

            return highest;
        }

        /// it only takes one high-priority channel to carry the whole script.
        private bool ScriptHasChannelAtFilter(_InputAuth script, float filterValue)
        {
            if (script._inputChannels == null)
                return false;

            foreach (var channel in script._inputChannels)
            {
                if (channel != null && Mathf.Approximately(channel.filterVal, filterValue))
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region Core
        private void EvaluateInputAuthorization()
        {
            currentlyAuthorized.Clear();

            if (scriptsRequestingInput.Count == 0)
                return;

            float highestFilter = GetHighestRequestedFilter();

            foreach (var script in scriptsRequestingInput)
            {
                if (ScriptHasChannelAtFilter(script, highestFilter))
                {
                    script.aGrantInput();
                    currentlyAuthorized.Add(script);
                }
                else
                {
                    script.aDenyInput();
                }
            }
            #endregion
        }
    }
}