using UnityEngine;
using VIAW.Systems.Player;
using TinyInspector;

namespace VIAW.Data
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/Character/CharacterData", order = 1)]
    public class CharacterDataSO : ScriptableObject
    {
        [BoxGroup("Gameplay Controllers")]
        public _MovementController gameplayController;
        
        [BoxGroup("Visuals")]
        public RigInfo thirdPersonVisuals;
        [BoxGroup("Visuals")]
        public _ArmController firstPersonArms;

        [BoxGroup("Sound")]
        public CharacterSoundData soundProfile;
    }
}
