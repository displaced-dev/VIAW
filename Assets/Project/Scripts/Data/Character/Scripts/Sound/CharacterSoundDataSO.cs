using UnityEngine;
using VIAW.Systems.Player;
using FMODUnity;

namespace VIAW.Data
{
    [CreateAssetMenu(fileName = "CharacterSoundData", menuName = "ScriptableObjects/Character/CharacterSoundData", order = 2)]
    public class CharacterSoundData : ScriptableObject
    {
        [Header("Walking")]
        public EventReference walkingBasic;

        [Header("Jump and Land")]
        public EventReference jumpEvent;
        public EventReference landEvent;

    }
}
