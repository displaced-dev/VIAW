using UnityEngine;
using VIAW.Async.Auth;
using VIAW.Data;
using TinyInspector;

// Summary:
// A base movement class that allows for us to have any controller exist and work, as long as we follow base implementation.

namespace VIAW.Systems.Player
{
    public enum Stance { Stand, Crouch, Air }

    public abstract class _MovementController : _InputAuth
    {
        [BoxGroup("_MovementController/Scene Refs")]
        public Transform _visualSpawnPoint;
        [Space]
        [BoxGroup("_MovementController/Scene Refs")]
        public CharacterDataSO _characterData;

        [BoxGroup("_MovementController/Debug")]
        public bool _isInitialized;
        public Stance stanceMirror;

        // Must inherit for basic controls
        public abstract void _Initialize(PlayerStateMachine psm, CharacterDataSO characterdata);
        public abstract void _RemoteInit();
        public abstract void _UpdateBody(float deltaTime, Transform playerCam);

        // Visuals Spawn 
        public abstract void _SpawnVisuals();

        // Helpers
        public abstract Transform _GetCameraTarget();
        public abstract RigInfo _GetCurrentRigInfo();
        public abstract bool _ShouldGenerateSound();
        
        // Level Design Calls
        public abstract void _Teleport(Vector3 position);
        public abstract void _SetRotation(Quaternion rotation);
    }
}

