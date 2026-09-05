using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using VIAW.Async.Auth;
using VIAW.Async.Preferences;
using VIAW.Data;
using VIAW.UI;
using VIAW.Tags;
using Evo;
using Evo.UI;
using TinyInspector;

namespace VIAW.Async.Auth
{
    public class InputRemapManager : MonoBehaviour
    {
        public static InputRemapManager Instance { get; private set; }

        [TabGroup("Config", "Data")]
        [SerializeField] private bool loadBindingsOnStart = true;

        [TabGroup("Config", "Scene Refs")]
        [SerializeField] private KeySpriteReference keySpriteReference;

        private PlayerInputActions inputActions;
        private PlayerPreferences prefController => PlayerPreferences.Instance;

        private int lastControllerType = -1;

        private readonly List<InputRemapConstructor> activeConstructors = new();
        private readonly HashSet<int> registeredInstanceIDs = new();

        public PlayerInputActions InputActions => inputActions;
        public PlayerPreferences PrefController => prefController;
        public ControllerTabsManager ControllerTabsManager { get; set; }

        #region Unity
        private void Awake()
        {
            if(Instance != null && Instance != this) { Destroy(this); }
            Instance = this;

            inputActions = new PlayerInputActions();
            inputActions.Enable();

            lastControllerType = ControllerTabsManager.GetControllerType();

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        private void Start()
        {
            if(loadBindingsOnStart) { StartCoroutine(DelayedInitialBindingLoad()); }
        }

        private void Update()
        {
            int currentControllerType = ControllerTabsManager.GetControllerType();
            if(currentControllerType != lastControllerType)
            {
                lastControllerType = currentControllerType;
                StartCoroutine(DelayedControllerTypeChange());
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
            inputActions?.Disable();
        }
        #endregion

        #region Constructor Registration
        public void RegisterConstructor(InputRemapConstructor constructor)
        {
            if(!IsValidConstructor(constructor)) { Debug.LogWarning("InputRemapManager: Received registration from an invalid constructor."); return; }

            int id = constructor.GetInstanceID();

            if(registeredInstanceIDs.Contains(id))
            {
                if(!constructor.IsProperlyInitialized())
                {
                    constructor.InputActions = inputActions;
                    constructor.RefreshInitialization();
                    LoadBindingForConstructor(constructor);
                    constructor.UpdateResetButtonState();
                }
                return;
            }

            if(!IsInValidScene(constructor))
            {
                Debug.LogWarning($"InputRemapManager: Ignoring registration for '{constructor.FullActionName}' - not in active scene or DontDestroyOnLoad.");
                return;
            }

            constructor.InputActions = inputActions;
            constructor.RefreshInitialization();

            activeConstructors.Add(constructor);
            registeredInstanceIDs.Add(id);

            LoadBindingForConstructor(constructor);
            constructor.OnRegisteredByManager();
            RegisterAllResetButtons();
        }

        public void UnregisterConstructor(InputRemapConstructor constructor)
        {
            if(constructor == null) { return; }
            activeConstructors.Remove(constructor);
            registeredInstanceIDs.Remove(constructor.GetInstanceID());
        }
        #endregion

        #region Scene & Controller Events
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            StartCoroutine(DelayedResetButtonRefresh());
        }

        private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            PruneDestroyedConstructors();
            CancelAllListening();
        }

        private IEnumerator DelayedResetButtonRefresh()
        {
            yield return new WaitForSeconds(0.1f);
            RegisterAllResetButtons();
        }

        private IEnumerator DelayedControllerTypeChange()
        {
            yield return new WaitForSeconds(0.1f);
            OnControllerTypeChanged();
        }

        private IEnumerator DelayedInitialBindingLoad()
        {
            while(PlayerPreferences.Instance == null) { yield return null; }

            LoadAllBindings();
        }

        private void OnControllerTypeChanged()
        {
            CancelAllListening();

            foreach(var constructor in GetValidConstructors())
            {
                try
                {
                    var action = constructor.InputActions?.FindAction(constructor.ActionName);
                    if(action != null)
                    {
                        for(int i = 0; i < action.bindings.Count; i++){
                            action.RemoveBindingOverride(i);
                        }
                    }
                }
                catch(System.Exception ex)
                {
                    Debug.LogError($"InputRemapManager: Error clearing bindings during controller change: {ex.Message}");
                }
            }

            LoadAllBindings();
        }
        #endregion

        #region Binding Load / Save
        public void LoadAllBindings()
        {
            ApplySavedBindingsToActions();

            PruneDestroyedConstructors();

            foreach(var constructor in GetValidConstructors()){
                LoadBindingForConstructor(constructor);
            }

            UpdateAllResetButtonStates();
        }

        public void ApplySavedBindingsToActions()
        {
            if(inputActions == null) { return; }

            if(prefController == null)
            {
                Debug.LogError("InputRemapManager: PlayerPrefController.Instance is null - cannot apply saved bindings.");
                return;
            }

            foreach(var map in inputActions.asset.actionMaps)
            {
                foreach(var action in map.actions)
                {
                    ApplySavedBindingsForScheme(action, "M&K", keyboardScheme: true);
                    ApplySavedBindingsForScheme(action, "Gamepad", keyboardScheme: false);
                }
            }
        }

        private void ApplySavedBindingsForScheme(InputAction action, string group, bool keyboardScheme)
        {
            var appliedKeys = new HashSet<string>();

            for(int i = 0; i < action.bindings.Count; i++)
            {
                var binding = action.bindings[i];
                if(binding.isComposite) { continue; }
                if(string.IsNullOrEmpty(binding.groups) || !binding.groups.Contains(group)) { continue; }

                string fullActionName = binding.isPartOfComposite
                    ? $"{action.name}_{binding.name}"
                    : action.name;

                if(!appliedKeys.Add(fullActionName)) { continue; }

                try
                {
                    string saved = prefController.LoadInputBinding(fullActionName, keyboardScheme);
                    if(!string.IsNullOrEmpty(saved)){
                        action.ApplyBindingOverride(i, saved);
                    }
                }
                catch(System.Exception ex)
                {
                    Debug.LogError($"InputRemapManager: Error applying saved binding for {fullActionName}: {ex.Message}");
                }
            }
        }

        private bool LoadBindingForConstructor(InputRemapConstructor constructor)
        {
            if(!IsValidConstructor(constructor)) { return false; }

            if(prefController == null)
            {
                Debug.LogError($"InputRemapManager: PlayerPrefController.Instance is null - cannot load binding for {constructor.FullActionName}.");
                return false;
            }

            try
            {
                string saved = prefController.LoadInputBinding(constructor.FullActionName);
                if(!string.IsNullOrEmpty(saved))
                {
                    constructor.ApplyBinding(saved);
                    return true;
                }
            }
            catch(System.Exception ex)
            {
                Debug.LogError($"InputRemapManager: Error loading binding for {constructor.FullActionName}: {ex.Message}");
            }

            return false;
        }

        public void SaveBinding(string actionName, string bindingPath)
        {
            if(prefController == null)
            {
                Debug.LogError($"InputRemapManager: PlayerPrefController.Instance is null - binding for '{actionName}' was NOT saved!");
                return;
            }

            if(!string.IsNullOrEmpty(actionName)){
                prefController.SaveInputBinding(actionName, bindingPath);
            }
        }

        public void ClearBinding(string actionName)
        {
            if(prefController == null)
            {
                Debug.LogError($"InputRemapManager: PlayerPrefController.Instance is null - binding for '{actionName}' was NOT cleared!");
                return;
            }

            if(!string.IsNullOrEmpty(actionName)){
                prefController.ClearInputBinding(actionName);
            }
        }
        #endregion

        #region Constructor Control
        public void ResetAllBindingsToDefault()
        {
            CancelAllListening();

            foreach(var constructor in GetValidConstructors())
            {
                try { constructor.ResetToDefault(); }
                catch(System.Exception ex) { Debug.LogError($"InputRemapManager: Error resetting constructor: {ex.Message}"); }
            }

            UpdateAllResetButtonStates();
        }

        public void CancelAllListening()
        {
            foreach(var constructor in GetValidConstructors())
            {
                try
                {
                    if(constructor.IsListening) { constructor.CancelListening(); }
                }
                catch(System.Exception ex) { Debug.LogError($"InputRemapManager: Error canceling listening: {ex.Message}"); }
            }
        }

        public void UpdateAllResetButtonStates()
        {
            foreach(var constructor in GetValidConstructors())
            {
                try { constructor.UpdateResetButtonState(); }
                catch(System.Exception ex) { Debug.LogError($"InputRemapManager: Error updating reset button state: {ex.Message}"); }
            }
        }

        public void NotifySceneChangeStarting()
        {
            CancelAllListening();
            PruneDestroyedConstructors();
        }

        private void RegisterAllResetButtons()
        {
            try
            {
                var activeScene = SceneManager.GetActiveScene();
                var allObjects = FindObjectsOfType<GameObject>(true);
                if(allObjects == null) { return; }

                foreach(var go in allObjects)
                {
                    if(go != null && go.scene == activeScene &&
                        go.TryGetComponent<TAG_ResetButton>(out _) &&
                        go.TryGetComponent<Button>(out var btn))
                    {
                        btn.onClick.RemoveListener(ResetAllBindingsToDefault);
                        btn.onClick.AddListener(ResetAllBindingsToDefault);
                    }
                }
            }
            catch(System.Exception ex)
            {
                Debug.LogError($"InputRemapManager: Error registering reset buttons: {ex.Message}");
            }
        }
        #endregion

        #region Validation
        private static bool IsValidConstructor(InputRemapConstructor c)
        {
            if(c == null) { return false; }
            try { return c.gameObject != null; }
            catch { return false; }
        }

        private static bool IsInDontDestroyOnLoad(GameObject go)
        {
            return go != null && go.scene.name == "DontDestroyOnLoad";
        }

        private bool IsInValidScene(InputRemapConstructor c)
        {
            if(!IsValidConstructor(c)) { return false; }
            var activeScene = SceneManager.GetActiveScene();
            return c.gameObject.scene == activeScene || IsInDontDestroyOnLoad(c.gameObject);
        }

        private void PruneDestroyedConstructors()
        {
            for(int i = activeConstructors.Count - 1; i >= 0; i--)
            {
                var c = activeConstructors[i];
                if(!IsValidConstructor(c))
                {
                    registeredInstanceIDs.Remove(c != null ? c.GetInstanceID() : 0);
                    activeConstructors.RemoveAt(i);
                }
            }
        }

        private IEnumerable<InputRemapConstructor> GetValidConstructors()
        {
            foreach(var c in activeConstructors)
            {
                if(IsValidConstructor(c)) { yield return c; }
            }
        }
        #endregion

        #region Debug
        public string GetDebugInfo()
        {
            int valid = activeConstructors.Count(IsValidConstructor);
            return $"Active Constructors: {valid}, Controller: {ControllerTabsManager.GetControllerType()} ({(ControllerTabsManager.IsUsingKeyboard() ? "Keyboard" : "Controller")})";
        }

        public void ForceLoadAllBindings() => LoadAllBindings();

        public void ForceResetAllBindings() => ResetAllBindingsToDefault();

        public void DebugConstructorList()
        {
            Debug.Log($"InputRemapManager: {activeConstructors.Count} registered constructors");
            foreach(var c in activeConstructors)
            {
                if(IsValidConstructor(c)){
                    Debug.Log($" {c.FullActionName} initialized={c.IsProperlyInitialized()}");
                }
                else{
                    Debug.Log(" <destroyed>");
                }
            }
        }
        #endregion
    }
}