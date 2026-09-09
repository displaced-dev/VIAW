using UnityEngine;
using VIAW.Async.Auth;

// Summary
// Script responsible for manaing logic such as weapons, melees, etc

namespace VIAW.Systems.Player
{
    public abstract class _ArmController : _InputAuth
    {
        public void Start(){
            aInputInit(true);
            InputAuthManager.Instance.RequestInput(this);
        }

        void OnDestroy(){
            if(InputAuthManager.Instance != null) {
                InputAuthManager.Instance.RelinquishRequest(this);
            }
        }
    }
}
