using UnityEngine;
using VIAW.Systems.Player;
using FMODUnity;

namespace VIAW.Data
{
    [CreateAssetMenu(fileName = "WeaponSoundData", menuName = "ScriptableObjects/Weapons/WeaponSoundData", order = 2)]
    public class WeaponSoundDataSO : ScriptableObject
    {
        public EventReference fire;
        public EventReference reload;
    }
}
