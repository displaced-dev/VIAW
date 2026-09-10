using UnityEngine;
using VIAW.Systems.Player;
using TinyInspector;

namespace VIAW.Data
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/Weapons/WeaponData", order = 1)]
    public class WeaponDataSO : ScriptableObject
    {
        [BoxGroup("Config")]
        public float fireRate;
        [BoxGroup("Config")]
        public float reloadTime;
        [BoxGroup("Config")]
        public float ammoCount;

        [BoxGroup("Scene Refs")]
        public GameObject weaponWorldAsset;
        [BoxGroup("Scene Refs")]
        public WeaponController weaponController;
        [BoxGroup("Scene Refs")]
        public GameObject thirdPersonObject;

        [BoxGroup("Data")]
        public WeaponSoundDataSO weaponSound;
        [BoxGroup("Data")]
        public bool isMainSlot;
    }
}
