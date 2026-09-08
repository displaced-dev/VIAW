using UnityEngine;
using VIAW.Async.Auth;
using TinyInspector;
using FMODUnity;

namespace VIAW.Systems.Sound
{
    public class DebugSoundEvent : _InputAuth
    {
        [BoxGroup("Scene Refs")]
        [SerializeField] private StudioEventEmitter soundEmitter;

        void Start() {
            aInputInit(true);
            InputAuthManager.Instance.RequestInput(this);
        }
        void Update()
        {
            if(_input.Fire.WasPressedThisFrame() && _inputAuthorized) {
                soundEmitter.Play();
            }
        }

        void OnDestroy() {
            if(InputAuthManager.Instance != null) {
                InputAuthManager.Instance.RelinquishRequest(this);
            }
        }
    }
}
