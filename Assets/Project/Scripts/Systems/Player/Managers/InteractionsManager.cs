using UnityEngine;
using VIAW.Data;
using VIAW.Tags;
using VIAW.Async.Auth;
using TinyInspector;

namespace VIAW.Systems.Player
{
    public class InteractionsManager : _InputAuth
    {
        [BoxGroup("Config")]
        [SerializeField] private Transform raycastOrigin;
        [BoxGroup("Config")]
        [SerializeField] private float raycastDistance;

        [BoxGroup("Scene Refs")]
        [SerializeField] private WeaponManager weaponM;

        private bool initialized = false;
        private WeaponWorldController weaponController;

        public void Initialize() {
            aInputInit(true);
            InputAuthManager.Instance.RequestInput(this);

            initialized = true;
        }

        public void Update() {
            if(!initialized) { return; }
            
            if(_input.Interact.WasPressedThisFrame()) {
                WeaponDataSO temp = TryGetWeaponData();

                if(temp != null) {
                    weaponM.Pickup(temp);
                    weaponController.Pickup();
                }
            }
        }

        public WeaponDataSO TryGetWeaponData() {
            Ray ray = new Ray(raycastOrigin.position, raycastOrigin.forward);

            if(Physics.Raycast(ray, out RaycastHit hit, raycastDistance)) {
                if(hit.collider.TryGetComponent(out WeaponWorldController weapon)) {
                    weaponController = weapon;
                    return weapon.weaponData;
                }
            }

            return null;
        }

        public void OnDestroy() {
            if(InputAuthManager.Instance != null) {
                InputAuthManager.Instance.RelinquishRequest(this);
            }
        }
    }
}
