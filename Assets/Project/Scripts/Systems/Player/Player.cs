using UnityEngine;
using VIAW.Systems.Network;

namespace VIAW.Systems.Player
{
    public class Player : MonoBehaviour
    {
        [Header("State Machine")]
        [SerializeField] private PlayerStateMachine PSM;
        [SerializeField] private LocalPlayerState LPS;
        
        [Header("Controllers")]
        [SerializeField] private PlayerCamera playerCamera;
        [Space]
        [SerializeField] private _MovementController playerCharacter;

        [Header("Managers")]
        [SerializeField] private CharacterDataManager characterDataM;

        private Transform cameraFocalTarget;
        private Transform spectatorCameraTarget;

        #region Unity Calls
        public void Initialize() {
            if(LPS.isLocalPlayer) { LocalInit(); }
            else { RemoteInit(); }
        }

        private void Update() {
            RoutineChecks();
            ProcessControllers();
        }

        private void LateUpdate()
        {
            UpdateCameraTarget();
        }
        #endregion

        #region Initializations
        private void LocalInit() {
            playerCamera.Initialize(PSM);
            characterDataM.Initialize();
        }
        private void RemoteInit() {
            playerCamera.RemoteInit();
        }
        #endregion

        #region Controllers
        // Both
        public void ProcessControllers() {
            if(playerCamera != null)
            {
                playerCamera.UpdateCameraInput();
                playerCamera.UpdateRotation();
            }
            if(playerCharacter != null && playerCamera != null)
            {
                playerCharacter._UpdateBody(Time.deltaTime, playerCamera.gameObject.transform);
            }
        }

        // Camera Controller
        public void UpdateCameraTarget()
        {
            if (playerCharacter == null || playerCamera == null)
                return;

            if (!PSM.isCinematic)
            {
                spectatorCameraTarget = null;
                if(playerCharacter != null)
                {
                    cameraFocalTarget = playerCharacter._GetCameraTarget();
                }
                playerCamera.UpdatePosition(cameraFocalTarget);
            }
            else
            {
                if (spectatorCameraTarget == null)
                {
                    return;
                }

                if (spectatorCameraTarget != null)
                {
                    playerCamera.UpdatePositionSmooth(spectatorCameraTarget);
                    playerCamera.UpdateRotationSmooth(spectatorCameraTarget.transform.eulerAngles);
                }
            }
        }

        // Movement
        public void ClearMovement() { }
        #endregion

        #region Checks
        private void RoutineChecks() {
            if(!LPS.isLocalPlayer) {
                if(playerCharacter == null) { 
                    playerCharacter = GetComponentInChildren<_MovementController>();
                    
                    if(playerCharacter.enabled) {
                       playerCharacter._RemoteInit(); 
                    }
                }
            }
            
            // Character Data
            if(characterDataM.currentMovementController != null && LPS.isLocalPlayer) {
                if(!characterDataM.currentMovementController._isInitialized) {
                    playerCharacter = characterDataM.currentMovementController;

                    playerCharacter._Initialize(PSM);
                 }
            }
        }
        #endregion

        #region Public Accessors
        // Cinematic Controls
        public void SetAndLoadCinematic(Transform cinematicPosition) {
            spectatorCameraTarget = cinematicPosition;
            PSM.isCinematic = true;
        }

        public void ExitCinematic() {
            PSM.isCinematic = false;
        }
        #endregion
    }
}
