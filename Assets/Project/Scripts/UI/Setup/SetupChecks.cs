using UnityEngine;
using TinyInspector;
using TMPro;

namespace VIAW.UI
{
    public class SetupChecks : MonoBehaviour
    {
        [BoxGroup("Scene Refs")]
        [SerializeField] private UsernameSetupController usernameController;
        [BoxGroup("Scene Refs")]
        [SerializeField] private UIFader uiFader;
        [BoxGroup("Scene Refs")]
        [SerializeField] private TMP_Text errorText;

        [BoxGroup("Config")]
        [SerializeField] private string sceneToLoad;

        public void RunChecks() {
            bool FreeToRun = true;

            if(usernameController.AllowedToUseName) {
                usernameController.SaveData();
            }
            else {
                FreeToRun = false;
                errorText.text = "Username Is Not Ready";
            }

            if(FreeToRun) { LoadGame(); }
        }  

        private void LoadGame() {
            uiFader.FadeToBlackAndLoadScene(sceneToLoad);
        }

    }
}
