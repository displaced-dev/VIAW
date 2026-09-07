using UnityEngine;
using VIAW.Async.Auth;
using TinyInspector;

// Summary:
// A base movement class that allows for us to have any controller exist and work, as long as we follow base implementation.

namespace VIAW.Systems.Player
{
    public abstract class _MovementController : _InputAuth
    {
        [BoxGroup("_MovementController/Scene Refs")]
        public RigInfo _visualsRig;
        [BoxGroup("_MovementController/Scene Refs")]
        public Transform _visualSpawnPoint;
        
        [BoxGroup("_MovementController/Debug")]
        public bool _isInitialized;

        // Must inherit for basic controls
        public abstract void _Initialize(PlayerStateMachine psm);
        public abstract void _RemoteInit();
        public abstract void _UpdateBody(float deltaTime, Transform playerCam);

        // Visuals Spawn 
        public abstract void _SpawnVisuals();

        // Helpers
        public abstract Transform _GetCameraTarget();
        public abstract RigInfo _GetCurrentRigInfo();
        
        // Level Design Calls
        public abstract void _Teleport(Vector3 position);
        public abstract void _SetRotation(Quaternion rotation);
    }
}

