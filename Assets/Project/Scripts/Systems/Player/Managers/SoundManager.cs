using UnityEngine;
using VIAW.Data;
using TinyInspector;

namespace VIAW.Systems.Player
{
    public class SoundManager : MonoBehaviour
    {
        [BoxGroup("Debug")]
        [SerializeField] private CharacterDataSO currentCharData;

        public void UpdateData(CharacterDataSO charData, _MovementController characterController) {
            currentCharData = charData;
        }
    }
}
