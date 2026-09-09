using UnityEngine;
using VIAW.Data;
using TinyInspector;

namespace VIAW.Systems.Player
{
    public class CharacterDataManager : MonoBehaviour
    {
        [BoxGroup("Scene Refs")]
        [SerializeField] private Transform characterSpawnRoot;
        [BoxGroup("Scene Refs")]
        [SerializeField] private Transform handSpawnRoot;
        
        [BoxGroup("Config")]
        [SerializeField] private CharacterDataSO fallbackCharacterData;

        [BoxGroup("Debug")]
        public CharacterDataSO currentCharacterData;
        [BoxGroup("Debug")]
        public GameObject currentCharacterObject;
        [BoxGroup("Debug")]
        public _MovementController currentMovementController;
        [BoxGroup("Debug")]
        public _ArmController currentArmController;

        public void Initialize() {
            // TODO: Strip this code out if there ends up being a character select system
            // or need for no starting
            if(currentCharacterObject == null) {
                LoadCharacterData(fallbackCharacterData);
            }
        }

        public void LoadCharacterData(CharacterDataSO newCharacter){
            if(newCharacter == null || newCharacter.gameplayController == null) {
                Debug.LogError("Could Not Process Character Data Swap");
                return;
            }

            if(currentCharacterObject != null) {
                ClearCharacter();
            }

            // Instantiate Character which then spins up the third person visuals
            currentCharacterData = newCharacter;
            currentMovementController = Instantiate(newCharacter.gameplayController, characterSpawnRoot);
            if(currentMovementController != null) {
                currentCharacterObject = currentMovementController.gameObject;
            }

            // Instantiate Arms for the character
            currentArmController = Instantiate(newCharacter.firstPersonArms, handSpawnRoot);
        }

        private void ClearCharacter() {
            currentCharacterData = null;
            
            if(currentCharacterObject == null) {
                return;
            }
            else {
                Destroy(currentCharacterObject);
                currentCharacterObject = null;
            }
        }
    }
}
