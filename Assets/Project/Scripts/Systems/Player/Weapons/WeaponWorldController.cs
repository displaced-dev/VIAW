using UnityEngine;
using VIAW.Data;
using VIAW.Async.Auth;
using TinyInspector;

namespace VIAW.Systems.Player
{
    [RequireComponent(typeof(BoxCollider))] [RequireComponent(typeof(Rigidbody))]
    public class WeaponWorldController : MonoBehaviour
    {
        [BoxGroup("Config")]
        public WeaponDataSO weaponData;

        [BoxGroup("Data")]
        public int ammoLoaded;
        [BoxGroup("Data")]
        public int reserveAmmo;

        public void Pickup() {
            Destroy(this.gameObject);
        }
    }
}
