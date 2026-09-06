using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private UI_PageManager pageManager;
        [BoxGroup("Scene Refs")]
        [SerializeField] private TMP_Text errorText;

        [BoxGroup("Scene Refs")]
        [SerializeField] private List<GameObject> completionImages;

        [BoxGroup("Config")]
        [SerializeField] private string sceneToLoad;

        void Update() {
            int currentPage = pageManager.GetCurrentPageIndex();
            if(currentPage > 1) {
                foreach(GameObject check in completionImages) {
                    check.SetActive(true);
                }
            }
            else if(currentPage == 1) {
                completionImages[0].SetActive(true);
                completionImages[1].SetActive(false);
            }
            else{ 
                foreach(GameObject check in completionImages) {
                    check.SetActive(false);
                }
            }
        }

        public void RunChecks() {
            bool FreeToRun = true;

            if(usernameController.AllowedToUseName) {
                usernameController.SaveData();
            }
            else {
                FreeToRun = false;
                errorText.text = "Error: Username is not ready";
            }

            if(FreeToRun) { LoadGame(); }
        }  

        private void LoadGame() {
            uiFader.FadeToBlackAndLoadScene(sceneToLoad);
        }

    }
}
