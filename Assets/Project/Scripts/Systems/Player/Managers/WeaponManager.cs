using UnityEngine;
using VIAW.Data;
using VIAW.Tags;
using VIAW.Network;
using VIAW.Async.Auth;
using TinyInspector;
using PurrNet;
using QFSW.QC;

namespace VIAW.Systems.Player
{
    public class WeaponManager : _InputAuth
    {
        [BoxGroup("Runtime Refs")]
        public Transform handRoot;
        [BoxGroup("Runtime Refs")]
        public Transform thirdPersonRoot;
        [BoxGroup("Runtime Refs")]
        public Animator thirdPersonAnimator;
        [BoxGroup("Runtime Refs")]
        public Animator firstPersonAnimator;

        [BoxGroup("Scene Refs/Components")]
        [SerializeField] private Player player;
        [BoxGroup("Scene Refs/Components")]
        [SerializeField] private WeaponNetworkModule weaponNet;

        [BoxGroup("Config")]
        [SerializeField] private Vector3 drop = new Vector3(1f,0f,0f);

        private bool initialized = false;
        private GameObject thirdWeapon;
        private GameObject firstWeapon;

        public void Initialize() {
            initialized = true;
            if(weaponNet == null) {
                weaponNet = GetComponent<WeaponNetworkModule>();
            }
            weaponNet.OnThirdPersonSync += HandleThirdPersonSync;
            aInputInit(true);
            InputAuthManager.Instance.RequestInput(this);
        }

        public void Update() {
            if(!initialized) { return; }

            if(firstPersonAnimator != null) {
                firstPersonAnimator.SetInteger("Weapon", weaponNet.weaponShown);
            }
            if(thirdPersonAnimator != null) {
                thirdPersonAnimator.SetInteger("Weapon", weaponNet.weaponShown);
            }

            if(!_inputAuthorized) { return; }
            if(_input.Drop.WasPressedThisFrame()) {
                Drop();
            }
        }

        public void UpdateAnimators(Animator firstP, Animator thirdP) {
            firstPersonAnimator = firstP;
            thirdPersonAnimator = thirdP;
        }

        public void UpdateFirstHand(Transform hand) {
            if(!initialized) { return; }
            handRoot = hand;
        }

        public void UpdateThirdHand(Transform hand) {
            thirdPersonRoot = hand;
        }

        public void Pickup(WeaponDataSO weapon) {
            if(!initialized) { return; }

            if(weapon.isMainSlot && weaponNet.mainSlot != null) {
                weaponNet.mainSlot.value = null;
            }

            if(!weapon.isMainSlot && weaponNet.offSlot != null) {
                weaponNet.offSlot.value = null;
            }

            Clear();

            if(weapon.isMainSlot) {
                weaponNet.mainSlot.value = weapon;
                weaponNet.weaponShown.value = 2;
                Populate(weapon);
            }
            else {
                weaponNet.offSlot.value = weapon;
                weaponNet.weaponShown.value = 1;
                Populate(weapon);
            }
        }

        [Command("debug_weapon_drop")]
        public void Drop() {
            if(weaponNet.weaponShown.value == 0 || !initialized) { return; }

            Vector3 dropPosition = handRoot.position + handRoot.TransformDirection(drop);
            Quaternion dropRotation = handRoot.rotation;

            if(weaponNet.weaponShown.value == 2) {
                Instantiate(weaponNet.mainSlot.value.weaponWorldAsset, dropPosition, dropRotation);
            }
            if(weaponNet.weaponShown.value == 1) {
                Instantiate(weaponNet.offSlot.value.weaponWorldAsset, dropPosition, dropRotation);
            }

            Clear();
        }

        [Command("debug_weapon_equip")]
        public void Equip(int val) {
            weaponNet.weaponShown.value = val;
        }

        public void LoadMainSlot() {
            if(!initialized) { return; }
            weaponNet.weaponShown.value = 2;
            Clear();
            Populate(weaponNet.mainSlot);
        }

        public void LoadOffSlot() {
            if(!initialized) { return; }
            weaponNet.weaponShown.value = 1;
            Clear();
            Populate(weaponNet.offSlot);
        }

        public void LoadUnarmed() {
            if(!initialized) { return; }
            weaponNet.weaponShown.value = 0;
            Clear();
        }

        public void Clear()
        {
            if(initialized && firstWeapon != null)
            {
                Destroy(firstWeapon);
                firstWeapon = null;
                weaponNet.weaponShown.value = 0;
            }

            weaponNet.RpcSyncThirdPerson(null);
        }

        private void Populate(WeaponDataSO weapon)
        {
            if(weapon == null) { return; }

            if (initialized)
            {
                PopulateF(weapon);
            }

            weaponNet.RpcSyncThirdPerson(weapon);
        }

        private void HandleThirdPersonSync(WeaponDataSO weapon)
        {
            if(initialized) { return; }

            if(thirdWeapon != null)
            {
                Destroy(thirdWeapon);
                thirdWeapon = null;
            }

            if(weapon != null)
            {
                PopulateT(weapon);
            }
        }

        public void PopulateF(WeaponDataSO weapon) {
            firstWeapon = Instantiate(weapon.weaponController.gameObject, handRoot);
        }

        public void PopulateT(WeaponDataSO weapon) {
            thirdWeapon = Instantiate(weapon.thirdPersonObject, thirdPersonRoot);
        }

        void OnDestroy() {
            InputAuthManager.Instance.RelinquishRequest(this);
            if(weaponNet != null) {
                weaponNet.OnThirdPersonSync -= HandleThirdPersonSync;
            }
        }
    }
}