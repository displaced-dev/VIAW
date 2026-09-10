using UnityEngine;
using VIAW.Data;
using VIAW.Async.Auth;
using TinyInspector;

namespace VIAW.Systems.Player
{
    public class WeaponController : _InputAuth
    {
        [BoxGroup("Config")]
        [SerializeField] private WeaponDataSO weaponData;

        // [BoxGroup("Scene Refs")]

        private GameObject handObject;
        private GameObject thirdPersonObject;

        public void Initialize() {
            aInputInit(true);
            InputAuthManager.Instance.RequestInput(this);
        }

        public void Drop() {
            InputAuthManager.Instance.RelinquishRequest(this);
        }

        public void OnDestroy() {
            if(InputAuthManager.Instance != null) {
                InputAuthManager.Instance.RelinquishRequest(this);
            }
        }
    }
}
