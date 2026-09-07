using UnityEngine;
using VIAW.Systems.Player;

namespace VIAW.Data
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/Character/CharacterData", order = 1)]
    public class CharacterDataSO : ScriptableObject
    {
        public _MovementController gameplayController;
        // public _ArmsController armsController;
    }
}
