using UnityEngine;
using UnityEngine.SceneManagement;
using TinyInspector;

namespace VIAW.Async
{
    public class BootstrapManager : MonoBehaviour
    {
        [BoxGroup("Scene Refs")]
        [SerializeField] private string mainSceneName;
        [BoxGroup("Scene Refs")]
        [SerializeField] private string setupSceneName;

        [BoxGroup("Debug")]
        [SerializeField] private bool Bypass;

        private const string FirstLaunchKey = "HasLaunchedBefore";
        
        private void Start() {
            Initialized();
        }

        // Summary: To Be Called Once the Bootstrap has finished loading 
        // Modifiers: Steam / Input System Loading / Other Processes
        public void Initialized() {
            if (!PlayerPrefs.HasKey(FirstLaunchKey))
            {
                PlayerPrefs.SetInt(FirstLaunchKey, 1);
                PlayerPrefs.Save();

                SceneManager.LoadScene(mainSceneName);
            }
            else if (Bypass) {
                SceneManager.LoadScene(setupSceneName);
            }
            else
            {
                SceneManager.LoadScene(setupSceneName);
            }
        }
    }
}
