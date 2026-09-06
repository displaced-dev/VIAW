using UnityEngine;
using TinyInspector;
using KinematicCharacterController;
using VIAW.Async.Auth;

namespace VIAW.Systems.Player
{
    public enum Stance { Stand, Crouch, Air }

    public struct CharacterState
    {
        public bool Grounded;
        public Stance Stance;
        public Vector3 Velocity;
    }

    public class PlayerCharacter : _MovementController, ICharacterController
    {
        [BoxGroup("Scene Refs")]
        [SerializeField] private KinematicCharacterMotor motor;
        [BoxGroup("Scene Refs")]
        [SerializeField] private Transform cameraTarget;

        [TabGroup("Controller", "Config")]
        [SerializeField] private float groundResponse = 25f;
        [TabGroup("Controller", "Config")]
        [SerializeField] private float gravity = -90f;

        [TabGroup("Controller", "Height")]
        [SerializeField] private float standHeight = 2f;
        [TabGroup("Controller", "Height")]
        [SerializeField] private float crouchHeight = 1.2f;
        [TabGroup("Controller", "Height")] [Range(0f, 1f)]
        [SerializeField] private float cameraStandHeight = .9f;
        [TabGroup("Controller", "Height")] [Range(0f, 1f)]
        [SerializeField] private float cameraCrouchHeight = .7f;
        [TabGroup("Controller", "Height")]
        [SerializeField] private float heightResponse = 15f;

        [TabGroup("Movement", "Speeds")] [Title("Grounded")]
        [SerializeField] private float walkSpeed = 20f;
        [TabGroup("Movement", "Speeds")]
        [SerializeField] private float crouchSpeed = 10f;
        [TabGroup("Movement", "Speeds")] [Separator(12, 24)] [Title("Air")]
        [SerializeField] private float airSpeed = 15f;
        [TabGroup("Movement", "Speeds")]
        [SerializeField] private float airAcceleration = 70f;

        [TabGroup("Movement", "Jumping")]
        [SerializeField] private float jumpSpeed = 20f;

        [BoxGroup("Debug")]
        [SerializeField] private Stance debugStance;
        
        public CharacterState state;

        private Quaternion cameraYaw = Quaternion.identity;
        private Quaternion requestedRotation = Quaternion.identity;
        private Vector3 requestedMovement;
        private bool requestedJump;
        private bool requestedCrouch;
        private bool isCrouched;
        
        private Transform playerCamera;

        private PlayerStateMachine psm;

        private const float MinPlanarSqrMagnitude = 0.0001f;

        public override void _Initialize(PlayerStateMachine psm)
        {
            if(_isInitialized){
                return;
            }

            this.psm = psm;
            motor.CharacterController = this;
            motor.enabled = true;
            state.Stance = Stance.Stand;

            _isInitialized = true;
            aInputInit(true);
            InputAuthManager.Instance.RequestInput(this);
        }

        public override void _RemoteInit() {
            motor.enabled = false;
            this.enabled = false;
        }

        public override void _UpdateBody(float deltaTime, Transform playerCam)
        {
            if(!_isInitialized || motor == null || cameraTarget == null){
                return;
            }

            playerCamera = playerCam;

            UpdateCameraYaw();
            UpdateInput();

            var cameraHeight = motor.Capsule.height * (isCrouched ? cameraCrouchHeight : cameraStandHeight);

            cameraTarget.localPosition = Vector3.Lerp(
                cameraTarget.localPosition,
                new Vector3(0f, cameraHeight, 0f),
                1f - Mathf.Exp(-heightResponse * deltaTime));
        }

        private void UpdateCameraYaw()
        {
            if(playerCamera == null){
                return;
            }

            var up = motor.CharacterUp;
            var forward = Vector3.ProjectOnPlane(playerCamera.forward, up);

            if(forward.sqrMagnitude < MinPlanarSqrMagnitude){
                forward = Vector3.ProjectOnPlane(playerCamera.up, up) * -Mathf.Sign(Vector3.Dot(playerCamera.forward, up));
            }

            if(forward.sqrMagnitude > MinPlanarSqrMagnitude){
                cameraYaw = Quaternion.LookRotation(forward.normalized, up);
            }
        }

        public void ClearInput()
        {
            requestedMovement = Vector3.zero;
            requestedJump = false;
            requestedCrouch = false;
        }

        public void UpdateInput()
        {
            if(!_isInitialized || !_inputAuthorized) { 
                ClearInput(); 
                return;
            }

            requestedRotation = cameraYaw;

            var move = _input.Move.ReadValue<Vector2>();
            requestedMovement = cameraYaw * Vector3.ClampMagnitude(new Vector3(move.x, 0f, move.y), 1f);

            requestedJump |= _input.Jump.WasPressedThisFrame();
            requestedCrouch = _input.Crouch.IsPressed();
        }

        public override Transform _GetCameraTarget() => cameraTarget;

        public override void _Teleport(Vector3 position)
        {
            if(motor == null){
                return;
            }
            motor.BaseVelocity = Vector3.zero;
            motor.SetPosition(position);
        }

        public override void _SetRotation(Quaternion rotation)
        {
            if(motor == null){
                return;
            }
            cameraYaw = rotation;
            requestedRotation = rotation;
            motor.SetRotation(rotation);
        }

        public void ResetMovementStates()
        {
            if(!_isInitialized || motor == null){
                return;
            }

            ClearInput();
            state = default;
            state.Stance = Stance.Stand;
            motor.BaseVelocity = Vector3.zero;
            SetCrouched(false);
        }

        public CharacterState GetState() => state;
        public Stance GetStance() => state.Stance;

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            var forward = Vector3.ProjectOnPlane(requestedRotation * Vector3.forward, motor.CharacterUp);
            if(forward != Vector3.zero){
                currentRotation = Quaternion.LookRotation(forward, motor.CharacterUp);
            }
        }

        public void BeforeCharacterUpdate(float deltaTime)
        {
            SetCrouched(requestedCrouch);
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            var grounded = motor.GroundingStatus.IsStableOnGround;

            if(grounded)
            {
                state.Stance = isCrouched ? Stance.Crouch : Stance.Stand;

                var groundedMovement = motor.GetDirectionTangentToSurface(
                    requestedMovement, motor.GroundingStatus.GroundNormal) * requestedMovement.magnitude;

                var speed = isCrouched ? crouchSpeed : walkSpeed;

                currentVelocity = Vector3.Lerp(
                    currentVelocity,
                    groundedMovement * speed,
                    1f - Mathf.Exp(-groundResponse * deltaTime));
            }
            else
            {
                state.Stance = Stance.Air;

                if(requestedMovement.sqrMagnitude > 0f)
                {
                    var planarMovement = Vector3.ProjectOnPlane(requestedMovement, motor.CharacterUp).normalized
                                         * requestedMovement.magnitude;
                    var planarVelocity = Vector3.ProjectOnPlane(currentVelocity, motor.CharacterUp);
                    var movementForce = planarMovement * airAcceleration * deltaTime;

                    if(planarVelocity.magnitude < airSpeed)
                    {
                        var target = Vector3.ClampMagnitude(planarVelocity + movementForce, airSpeed);
                        movementForce = target - planarVelocity;
                    }
                    else if(Vector3.Dot(planarVelocity, movementForce) > 0f)
                    {
                        movementForce = Vector3.ProjectOnPlane(movementForce, planarVelocity.normalized);
                    }

                    currentVelocity += movementForce;
                }

                currentVelocity += motor.CharacterUp * gravity * deltaTime;
            }

            if(requestedJump)
            {
                requestedJump = false;

                if(grounded)
                {
                    motor.ForceUnground();

                    var verticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
                    currentVelocity += motor.CharacterUp * (Mathf.Max(jumpSpeed, verticalSpeed) - verticalSpeed);
                }
            }
        }

        public void AfterCharacterUpdate(float deltaTime)
        {
            state.Grounded = motor.GroundingStatus.IsStableOnGround;
            state.Velocity = motor.Velocity;
            debugStance = state.Stance;
        }

        public void PostGroundingUpdate(float deltaTime) { }
        public bool IsColliderValidForCollisions(Collider coll) => true;
        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }
        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }
        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport) { }
        public void OnDiscreteCollisionDetected(Collider hitCollider) { }

        private void SetCrouched(bool crouch)
        {
            if(crouch == isCrouched){
                return;
            }
            isCrouched = crouch;

            var height = crouch ? crouchHeight : standHeight;
            motor.SetCapsuleDimensions(motor.Capsule.radius, height, height * .5f);
        }
    }
}