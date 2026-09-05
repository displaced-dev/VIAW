using UnityEngine;

namespace VIAW.Data
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/Character/CharacterData", order = 1)]
    public class CharacterDataSO : ScriptableObject
    {
        public GameObject GameplayController;
    }
}
