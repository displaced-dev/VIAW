using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using VIAW.Async;
using VIAW.Async.Auth;
using VIAW.Data;
using Evo.UI;

namespace VIAW.UI
{
    public class InputRemapConstructor : MonoBehaviour
    {
        [Header("Input Remapping Settings")]
        [SerializeField] private string actionName = "Jump";
        [SerializeField] private string compositePart = "";
        [SerializeField] private Button buttonManager;
        [SerializeField] private Button resetButton;
        [SerializeField] private KeySpriteReference keySpriteReference;
        [SerializeField] private float timeoutDuration = 5f;

        public PlayerInputActions InputActions { get; set; }

        private InputAction targetAction;
        private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
        private bool isListening = false;
        private bool isSuccessfullyRegistered = false;

        private int lastControllerType = -1;
        private bool hasQueuedControllerChange = false;
        private int queuedControllerType = -1;

        public bool IsListening => isListening;
        public string ActionName => actionName;
        public string FullActionName => string.IsNullOrEmpty(compositePart) ? actionName : $"{actionName}_{compositePart}";

        private bool previousResetButtonState;

        private void Awake()
        {
            if(buttonManager == null) { Debug.LogError($"{name}: ButtonManager missing."); return; }

            buttonManager.onClick.AddListener(StartListening);
            if(resetButton != null) { resetButton.onClick.AddListener(ResetToDefault); }
        }

        private void Start()
        {
            lastControllerType = ControllerTabsManager.GetControllerType();
            InputRemapManager.Instance.RegisterConstructor(this);
            UpdateResetButtonState();
            SetInitialBindingIcon();
        }

        private void OnDestroy()
        {
            rebindingOperation?.Dispose();
            buttonManager?.onClick.RemoveListener(StartListening);
            resetButton?.onClick.RemoveListener(ResetToDefault);
            InputRemapManager.Instance?.UnregisterConstructor(this);
        }

        private void FixedUpdate()
        {
            int currentControllerType = ControllerTabsManager.GetControllerType();
            if(currentControllerType != lastControllerType)
            {
                Debug.Log($"{name}: Controller type changed from {lastControllerType} to {currentControllerType} for action: {FullActionName}");
                lastControllerType = currentControllerType;

                if(targetAction != null && InputActions != null)
                {
                    OnControllerTypeChanged();
                }
                else
                {
                    Debug.Log($"{name}: Queueing controller type change for {FullActionName} (not initialized yet)");
                    hasQueuedControllerChange = true;
                    queuedControllerType = currentControllerType;
                }
            }

            if(hasQueuedControllerChange && targetAction != null && InputActions != null)
            {
                Debug.Log($"{name}: Processing queued controller type change to {queuedControllerType} for {FullActionName}");
                hasQueuedControllerChange = false;
                OnControllerTypeChanged();
            }

            UpdateResetButtonState();

            if(isSuccessfullyRegistered && !IsProperlyInitialized())
            {
                Debug.LogWarning($"{name}: Lost initialization state, re-registering for action: {FullActionName}");
                isSuccessfullyRegistered = false;
                InputRemapManager.Instance?.RegisterConstructor(this);
            }
        }

        public void OnRegisteredByManager()
        {
            isSuccessfullyRegistered = true;
            StartCoroutine(PostRegistrationSetup());
        }

        private IEnumerator PostRegistrationSetup()
        {
            yield return new WaitForEndOfFrame();

            LoadSavedBindingsIfNeeded();
            UpdateResetButtonState();
            SetInitialBindingIcon();

            if(hasQueuedControllerChange)
            {
                Debug.Log($"{name}: Processing queued controller type change after registration for {FullActionName}");
                hasQueuedControllerChange = false;
                OnControllerTypeChanged();
            }
        }

        public bool IsProperlyInitialized()
        {
            return isSuccessfullyRegistered && InputActions != null && targetAction != null;
        }

        public void RefreshInitialization()
        {
            if(InputActions == null) { Debug.LogError($"{name}: InputActions missing for action: {FullActionName}"); return; }

            targetAction = InputActions.FindAction(actionName);
            if(targetAction == null) { Debug.LogError($"{name}: Action '{actionName}' not found in InputActions for {FullActionName}"); }
        }

        public void StartListening()
        {
            if(isListening || targetAction == null)
            {
                Debug.LogWarning($"{name}: Cannot start listening - already listening ({isListening}) or target action is null for action: {FullActionName}");
                return;
            }

            Debug.Log($"Starting to listen for new binding for action: {FullActionName}");

            int bindingIndex = GetBindingIndex();
            if(bindingIndex == -1) { Debug.LogError($"{name}: Binding index for '{FullActionName}' not found."); return; }

            targetAction.Disable();
            buttonManager.SetInteractable(false);
            isListening = true;

            var rebind = targetAction.PerformInteractiveRebinding(bindingIndex);
            rebind.WithTimeout(timeoutDuration);
            rebind.OnComplete(_ => CompleteRebind(bindingIndex));
            rebind.OnCancel(_ => CancelRebind());
            rebindingOperation = rebind.Start();
        }

        public void CancelListening() => rebindingOperation?.Cancel();

        private void CompleteRebind(int bindingIndex)
        {
            isListening = false;
            targetAction.Enable();
            buttonManager.SetInteractable(true);

            Debug.Log($"Completed rebind for action: {FullActionName}");

            string newPath = targetAction.bindings[bindingIndex].effectivePath;
            buttonManager.SetIcon(keySpriteReference != null ? keySpriteReference.GetSprite(newPath) : null);

            if(InputRemapManager.Instance != null)
                InputRemapManager.Instance.SaveBinding(FullActionName, newPath);
            else
                Debug.LogWarning($"{name}: InputRemapManager not available for saving binding for action: {FullActionName}");

            UpdateResetButtonState();
            rebindingOperation?.Dispose();
        }

        private void CancelRebind()
        {
            isListening = false;
            targetAction.Enable();
            buttonManager.SetInteractable(true);
            rebindingOperation?.Dispose();
        }

        public void ApplyBinding(string bindingPath)
        {
            if(targetAction == null) { Debug.LogWarning($"{name}: Cannot apply binding. TargetAction is null for {FullActionName}"); return; }

            int index = GetBindingIndex();
            if(index == -1) { Debug.LogWarning($"{name}: Cannot apply binding due to invalid index for {FullActionName}"); return; }

            try
            {
                if(index < targetAction.bindings.Count)
                {
                    targetAction.ApplyBindingOverride(index, bindingPath);
                    UpdateResetButtonState();

                    if(keySpriteReference != null)
                        buttonManager.SetIcon(keySpriteReference.GetSprite(bindingPath));

                    Debug.Log($"{name}: Successfully applied binding '{bindingPath}' for {FullActionName}");
                }
                else
                {
                    Debug.LogError($"{name}: Binding index {index} is out of range for action {FullActionName} (count: {targetAction.bindings.Count})");
                }
            }
            catch(System.Exception ex)
            {
                Debug.LogError($"{name}: Exception in ApplyBinding for action '{FullActionName}': {ex.Message}");
            }
        }

        public void ResetToDefault()
        {
            if(targetAction == null) { Debug.LogWarning($"{name}: Cannot reset to default as targetAction is null for {FullActionName}"); return; }

            int index = GetBindingIndex();
            if(index == -1) { Debug.LogWarning($"{name}: Cannot reset to default as invalid index for {FullActionName}"); return; }

            try
            {
                if(index < targetAction.bindings.Count)
                {
                    targetAction.RemoveBindingOverride(index);

                    if(InputRemapManager.Instance != null)
                        InputRemapManager.Instance.ClearBinding(FullActionName);

                    UpdateResetButtonState();
                    SetInitialBindingIcon();
                    Debug.Log($"{name}: Successfully reset binding to default for {FullActionName}");
                }
                else
                {
                    Debug.LogError($"{name}: Binding index {index} is out of range for action {FullActionName} (count: {targetAction.bindings.Count})");
                }
            }
            catch(System.Exception ex)
            {
                Debug.LogError($"{name}: Exception in ResetToDefault for action '{FullActionName}': {ex.Message}");
            }
        }

        public void UpdateResetButtonState()
        {
            if(resetButton == null) { return; }

            if(targetAction == null)
            {
                resetButton.SetInteractable(false);
                return;
            }

            bool currentState = HasCustomBinding();
            if(currentState != previousResetButtonState)
            {
                resetButton.SetInteractable(currentState);
                previousResetButtonState = currentState;
            }
        }

        private void OnControllerTypeChanged()
        {
            if(targetAction == null || InputActions == null)
            {
                Debug.LogWarning($"{name}: Cannot handle controller type change due to missing targetAction or InputActions for {FullActionName}");
                return;
            }

            Debug.Log($"{name}: Switching to {(ControllerTabsManager.IsUsingKeyboard() ? "Keyboard" : "Controller")} for action: {FullActionName}");

            try
            {
                int bindingIndex = GetBindingIndex();
                if(bindingIndex != -1 && bindingIndex < targetAction.bindings.Count)
                    targetAction.RemoveBindingOverride(bindingIndex);

                LoadSavedBindingsIfNeeded();
                SetInitialBindingIcon();
                UpdateResetButtonState();

                Debug.Log($"{name}: Successfully switched to {(ControllerTabsManager.IsUsingKeyboard() ? "Keyboard" : "Controller")} for action: {FullActionName}");
            }
            catch(System.Exception ex)
            {
                Debug.LogError($"{name}: Exception in OnControllerTypeChanged for action '{FullActionName}': {ex.Message}");
            }
        }

        private void LoadSavedBindingsIfNeeded()
        {
            if(InputRemapManager.Instance?.PrefController == null) { return; }

            string savedBinding = InputRemapManager.Instance.PrefController.LoadInputBinding(FullActionName);
            if(!string.IsNullOrEmpty(savedBinding))
                ApplyBinding(savedBinding);
            else
                SetInitialBindingIcon();
        }

        private void SetInitialBindingIcon()
        {
            if(targetAction == null)
            {
                RefreshInitialization();
                if(targetAction == null) { Debug.LogWarning($"{name}: Cannot set initial binding icon as targetAction is null for {FullActionName}"); return; }
            }

            int bindingIndex = GetBindingIndex();
            if(bindingIndex == -1) { Debug.LogWarning($"{name}: Cannot set initial binding icon due to invalid binding index for {FullActionName}"); return; }

            try
            {
                if(bindingIndex < targetAction.bindings.Count)
                {
                    string currentPath = targetAction.bindings[bindingIndex].effectivePath;

                    if(keySpriteReference != null)
                    {
                        buttonManager.SetIcon(keySpriteReference.GetSprite(currentPath));
                        Debug.Log($"{name}: Successfully set icon for {FullActionName}");
                    }
                    else
                    {
                        Debug.LogWarning($"{name}: keySpriteReference not assigned on {FullActionName} - cannot set icon.");
                    }
                }
                else
                {
                    Debug.LogError($"{name}: Binding index {bindingIndex} is out of range for action {FullActionName} (count: {targetAction.bindings.Count})");
                }
            }
            catch(System.Exception ex)
            {
                Debug.LogError($"{name}: Exception in SetInitialBindingIcon for action '{FullActionName}': {ex.Message}");
            }
        }

        private bool HasCustomBinding()
        {
            if(targetAction == null) { return false; }

            int index = GetBindingIndex();
            if(index == -1) { return false; }

            try
            {
                if(index < targetAction.bindings.Count)
                    return !string.IsNullOrEmpty(targetAction.bindings[index].overridePath);
            }
            catch(System.Exception ex)
            {
                Debug.LogError($"{name}: Exception in HasCustomBinding for action '{actionName}': {ex.Message}");
            }

            return false;
        }

        private int GetBindingIndex()
        {
            if(targetAction == null) { Debug.LogWarning($"{name}: targetAction is null for action '{actionName}'"); return -1; }
            if(targetAction.bindings.Count == 0) { Debug.LogWarning($"{name}: targetAction has no bindings for action '{actionName}'"); return -1; }

            string targetGroup = ControllerTabsManager.IsUsingKeyboard() ? "M&K" : "Gamepad";

            try
            {
                if(!string.IsNullOrEmpty(compositePart))
                {
                    for(int i = 0; i < targetAction.bindings.Count; i++)
                    {
                        var binding = targetAction.bindings[i];
                        if(!string.IsNullOrEmpty(binding.groups) && binding.groups.Contains(targetGroup) &&
                            !string.IsNullOrEmpty(binding.name) && binding.name.Equals(compositePart, System.StringComparison.OrdinalIgnoreCase))
                        {
                            return i;
                        }
                    }
                }
                else
                {
                    for(int i = 0; i < targetAction.bindings.Count; i++)
                    {
                        var binding = targetAction.bindings[i];
                        if(binding.isComposite) { continue; }
                        if(!string.IsNullOrEmpty(binding.groups) && binding.groups.Contains(targetGroup)) { return i; }
                    }
                }
            }
            catch(System.Exception ex)
            {
                Debug.LogError($"{name}: Exception in GetBindingIndex for action '{actionName}': {ex.Message}");
                return -1;
            }

            Debug.LogWarning($"{name}: Could not find binding index for action '{actionName}' with composite part '{compositePart}' and control scheme '{targetGroup}'");
            return -1;
        }

        public void DebugRegistrationStatus()
        {
            Debug.Log($"=== {name} Debug ===");
            Debug.Log($"Action: {FullActionName}");
            Debug.Log($"Successfully Registered: {isSuccessfullyRegistered}");
            Debug.Log($"Properly Initialized: {IsProperlyInitialized()}");
            Debug.Log($"InputActions: {(InputActions != null ? "Available" : "NULL")}");
            Debug.Log($"TargetAction: {(targetAction != null ? "Available" : "NULL")}");
            Debug.Log($"Manager Instance: {(InputRemapManager.Instance != null ? "Available" : "NULL")}");
            Debug.Log($"KeySpriteReference: {(keySpriteReference != null ? keySpriteReference.name : "NULL")}");
            Debug.Log($"Controller Type: {ControllerTabsManager.GetControllerType()} ({(ControllerTabsManager.IsUsingKeyboard() ? "Keyboard" : "Controller")})");
            Debug.Log($"Has Queued Controller Change: {hasQueuedControllerChange}");
        }

        public void DebugSpriteForCurrentBinding()
        {
            if(keySpriteReference == null) { Debug.LogError($"{name}: keySpriteReference not assigned."); return; }
            if(targetAction == null) { Debug.LogError($"{name}: targetAction is null - not yet initialized."); return; }

            int index = GetBindingIndex();
            if(index == -1) { Debug.LogError($"{name}: Could not find binding index."); return; }

            string path = targetAction.bindings[index].effectivePath;
            Sprite sprite = keySpriteReference.GetSprite(path);
            Debug.Log($"{name}: path='{path}' sprite={(sprite != null ? sprite.name : "NULL")}");
        }
    }
}