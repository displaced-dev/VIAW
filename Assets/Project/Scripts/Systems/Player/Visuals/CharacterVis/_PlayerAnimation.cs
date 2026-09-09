using UnityEngine;
using TinyInspector;
using VIAW.Async.Auth;

// Summary
// Base class responsible for managing the base movement states such as walk, run, jump, crouch.
// Will need to be extended when elements such as abilities modify the animations.

namespace VIAW.Systems.Player
{
    public abstract class _PlayerAnimation : _InputAuth
    {
        [BoxGroup("Scene Refs")]
        [SerializeField] protected RigInfo rigInfo;

        [BoxGroup("Scene Refs")]
        public Animator characterAnimator;
        [Space]
        [BoxGroup("Scene Refs")]
        public _MovementController m_Controller;

        [BoxGroup("Debug")]
        public float deltaTime;
        [BoxGroup("Debug")]
        public float speedModifier = 1f;

        private float currentXVal;
        private float currentYVal;

        private bool isGrounded = true;
        private bool isCrouching = false;
        private bool isMoving = false;

        private Stance characterStance;

        protected virtual void Awake()
        {
            FindComponent(ref rigInfo);
        }

        protected virtual void OnEnable()
        {
            aInputInit(true);
            InputAuthManager.Instance.RequestInput(this);
        }

        void FindComponent<T>(ref T field) where T : Component
        {
            if(field != null) { return; }
            field = GetComponent<T>() ?? GetComponentInChildren<T>();
        }

        protected virtual void Update()
        {
            deltaTime = Time.deltaTime;

            if(characterAnimator == null) { return; }
            if(!_inputAuthorized) { return; }
            if(!rigInfo.localPlayer) { return; }

            Vector2 requestedMovement = _input.Move.ReadValue<Vector2>();
            bool sprinting = !_input.Walk.IsPressed();

            float maxMovementValue = sprinting ? 1f : 0.5f;
            float targetX = Mathf.Clamp(requestedMovement.x, -maxMovementValue, maxMovementValue);
            float targetY = Mathf.Clamp(requestedMovement.y, -maxMovementValue, maxMovementValue);

            currentXVal = Mathf.Lerp(currentXVal, targetX, deltaTime * speedModifier);
            currentYVal = Mathf.Lerp(currentYVal, targetY, deltaTime * speedModifier);

            isMoving = Mathf.Abs(currentXVal) >= .01f || Mathf.Abs(currentYVal) >= .01f;

            UpdateAnimationStateFromStance();
            UpdateAnimatorValues();
        }

        protected virtual void UpdateAnimationStateFromStance()
        {
            if (m_Controller != null)
            {
                characterStance = m_Controller.stanceMirror;
            }

            switch (characterStance)
            {
                case Stance.Air:
                    isGrounded = false; isCrouching = false; break;
                case Stance.Stand:
                    isGrounded = true; isCrouching = false; break;
                case Stance.Crouch:
                    isGrounded = true; isCrouching = true; break;
            }
        }

        protected virtual void UpdateAnimatorValues()
        {
            if(characterAnimator == null) { return; }

            characterAnimator.SetFloat("xVal", currentXVal);
            characterAnimator.SetFloat("yVal", currentYVal);
            characterAnimator.SetBool("Grounded", isGrounded);
            characterAnimator.SetBool("Crouch", isCrouching);
            characterAnimator.SetBool("Moving", isMoving);
        }

        protected virtual void OnDestroy()
        {
            if(InputAuthManager.Instance != null)
            {
                InputAuthManager.Instance.RelinquishRequest(this);
            }
        }
    }
}