using UnityEngine;
using VIAW.Systems.Player;
using TinyInspector;

namespace VIAW.Data
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/Character/WeaponData", order = 1)]
    public class WeaponDataSO : ScriptableObject
    {
        [BoxGroup("Config")]
        public float fireRate;
        [BoxGroup("Config")]
        public float reloadTime;
        [BoxGroup("Config")]
        public float ammoCount;

        [BoxGroup("Scene Refs")]
        public GameObject weaponAsset;
        // public _ArmsController _armsController;
    }
}
