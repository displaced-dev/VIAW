#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine; // Add this line
using UnityEngine.SceneManagement;
using QFSW.QC;

[InitializeOnLoad]
public static class BootstrapLoader
{
    private const string OriginalSceneKey = "BootstrapLoader_OriginalScene";

    static BootstrapLoader()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if(state == PlayModeStateChange.ExitingEditMode)
        {
            if(EditorBuildSettings.scenes.Length == 0)
            {
                UnityEngine.Debug.LogWarning("No scenes in build settings!");
                return;
            }

            Scene currentScene = SceneManager.GetActiveScene();
            string currentScenePath = currentScene.path;
            
            string bootstrapScenePath = EditorBuildSettings.scenes[0].path;
            
            bool isCurrentSceneInBuild = false;
            foreach(var scene in EditorBuildSettings.scenes)
            {
                if(scene.path == currentScene.path && scene.enabled)
                {
                    isCurrentSceneInBuild = true;
                    break;
                }
            }

            if(isCurrentSceneInBuild && currentScene.path != bootstrapScenePath)
            {
                EditorPrefs.SetString(OriginalSceneKey, currentScenePath);
                
                if(currentScene.isDirty)
                {
                    EditorSceneManager.SaveScene(currentScene);
                }

                EditorSceneManager.OpenScene(bootstrapScenePath, OpenSceneMode.Single);
                
                UnityEngine.Debug.Log($"Bootstrap scene loaded: {bootstrapScenePath}. Will restore to: {currentScenePath}");
            }
            else
            {
                EditorPrefs.DeleteKey(OriginalSceneKey);
            }
        }
        else if(state == PlayModeStateChange.EnteredEditMode)
        {
            if(EditorPrefs.HasKey(OriginalSceneKey))
            {
                string originalScenePath = EditorPrefs.GetString(OriginalSceneKey);
                
                if(!string.IsNullOrEmpty(originalScenePath))
                {
                    UnityEngine.Debug.Log($"Restoring original scene: {originalScenePath}");
                    EditorSceneManager.OpenScene(originalScenePath, OpenSceneMode.Single);
                }
                
                EditorPrefs.DeleteKey(OriginalSceneKey);
            }
        }
    }

    [Command("debug_scene_loadcached")]
    private static void BootLoad() 
    {
        if(EditorPrefs.HasKey(OriginalSceneKey))
        {
            string originalScenePath = EditorPrefs.GetString(OriginalSceneKey);
            
            if(!string.IsNullOrEmpty(originalScenePath))
            {
                if(Application.isPlaying)
                {
                    string sceneName = System.IO.Path.GetFileNameWithoutExtension(originalScenePath);
                    SceneManager.LoadScene(sceneName);
                    UnityEngine.Debug.Log($"Loading scene at runtime: {sceneName}");
                }
                else
                {
                    EditorSceneManager.OpenScene(originalScenePath, OpenSceneMode.Single);
                    UnityEngine.Debug.Log($"Loading scene in editor: {originalScenePath}");
                }
            }
        }
        else
        {
            UnityEngine.Debug.LogWarning("No cached scene found. Original scene key not set.");
        }
    }

}
#endif