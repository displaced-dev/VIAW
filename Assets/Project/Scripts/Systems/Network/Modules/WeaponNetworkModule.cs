using UnityEngine;
using System;
using VIAW.Data;
using PurrNet;

namespace VIAW.Network
{
    public class WeaponNetworkModule : NetworkBehaviour
    {
        public SyncVar<WeaponDataSO> mainSlot = new();
        public SyncVar<WeaponDataSO> offSlot  = new();
        public SyncVar<int> weaponShown = new();

        public event Action<WeaponDataSO> OnThirdPersonSync;

        [ObserversRpc(bufferLast: true)]
        public void RpcSyncThirdPerson(WeaponDataSO weapon) => OnThirdPersonSync?.Invoke(weapon);
    }
}