using UnityEngine;
using VIAW.Async.Auth;

// Summary:
// A base movement class that allows for us to have any controller exist and work, as long as we follow base implementation.

namespace VIAW.Systems.Player
{
    public abstract class _MovementController : _InputAuth
    {
        public bool _isInitialized;

        // Must inherit for basic controls
        public abstract void _Initialize(PlayerStateMachine psm);
        public abstract void _UpdateBody(float deltaTime);

        // Helpers
        public abstract Transform _GetCameraTarget();
        
        // Level Design Calls
        public abstract void _Teleport(Vector3 position);
        public abstract void _SetRotation(Quaternion rotation);
    }
}

