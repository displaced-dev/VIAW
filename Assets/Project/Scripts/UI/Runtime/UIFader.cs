using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TinyInspector;
using UnityEngine.SceneManagement;

namespace VIAW.UI
{
    public class UIFader : MonoBehaviour
    {
        [BoxGroup("Scene Refs")]
        [SerializeField] private List<GameObject> setupObjects = new List<GameObject>();
        [BoxGroup("Scene Refs/Runtime")]
        [SerializeField] private Canvas overlayCanvas;
        [BoxGroup("Scene Refs/Runtime")]
        [SerializeField] private Image fadeImage;

        [BoxGroup("Fading Config")]
        [SerializeField] private List<int> pauseAtIndices = new List<int>();
        [BoxGroup("Fading Config")]
        [SerializeField] private string sceneNameFallback;

        [BoxGroup("Fade Data/Color")]
        [SerializeField] private Color fadeColor = Color.black;
        [BoxGroup("Fade Data/Timing")]
        [SerializeField] private float fadeInDuration = 1f;
        [BoxGroup("Fade Data/Timing")]
        [SerializeField] private float displayDuration = 0.5f;
        [BoxGroup("Fade Data/Timing")]
        [SerializeField] private float fadeOutDuration = 1f;
        [BoxGroup("Fade Data/Timing")]
        [SerializeField] private float sceneChangeFadeDuration = 1f;
        [BoxGroup("Fade Data/Animation")]
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [BoxGroup("Fade Data")]
        [SerializeField] private bool startOnAwake = true;

        private CanvasGroup overlayCanvasGroup;
        private Coroutine currentSequence;
        private bool isPaused = false;
        private bool proceedRequested = false;
        private HashSet<int> pauseIndexSet;

        public bool IsPaused => isPaused;

        private void Awake()
        {
            if(overlayCanvas == null || fadeImage == null) { CreateFadeOverlay(); }

            overlayCanvasGroup = overlayCanvas.GetComponent<CanvasGroup>();
            if(overlayCanvasGroup == null) { overlayCanvasGroup = overlayCanvas.gameObject.AddComponent<CanvasGroup>(); }

            pauseIndexSet = new HashSet<int>(pauseAtIndices);
        }

        private void Start()
        {
            if(startOnAwake) { StartSetupSequence(); }
        }

        public void StartSetupSequence()
        {
            if(currentSequence != null) { StopCoroutine(currentSequence); }

            isPaused = false;
            proceedRequested = false;
            currentSequence = StartCoroutine(FadeSequence());
        }

        public void ProceedToNext()
        {
            if(!isPaused) { return; }
            proceedRequested = true;
        }

        public void FadeToBlackAndLoadScene(string sceneName)
        {
            if(string.IsNullOrEmpty(sceneName)) { Debug.LogWarning("FadeToBlackAndLoadScene called with an empty scene name."); return; }

            if(currentSequence != null) { StopCoroutine(currentSequence); currentSequence = null; }

            StartCoroutine(FadeToBlackAndLoadSceneRoutine(sceneName));
        }

        public void CloseGame()
        {
            Application.Quit();
        }

        private IEnumerator FadeSequence()
        {
            // No objects: just fade the overlay away.
            if(setupObjects.Count == 0)
            {
                yield return Fade(1f, 0f, fadeInDuration);
                overlayCanvas.gameObject.SetActive(false);
                currentSequence = null;
                yield break;
            }

            for(int i = 0; i < setupObjects.Count; i++)
            {
                GameObject obj = setupObjects[i];
                bool isLastObject = (i == setupObjects.Count - 1);

                // Show object behind black, then fade in.
                overlayCanvasGroup.alpha = 1f;
                overlayCanvas.gameObject.SetActive(true);
                obj.SetActive(true);
                yield return null;

                yield return Fade(1f, 0f, fadeInDuration);

                if(isLastObject)
                {
                    overlayCanvas.gameObject.SetActive(false);
                    currentSequence = null;
                    yield break;
                }

                // Hold: either wait for ProceedToNext() or a timed display.
                if(pauseIndexSet.Contains(i)) { yield return WaitForProceed(); }
                else { yield return new WaitForSeconds(displayDuration); }

                // Fade back to black and hide the object.
                yield return Fade(0f, 1f, fadeOutDuration);
                obj.SetActive(false);
            }
        }

        private IEnumerator WaitForProceed()
        {
            isPaused = true;
            proceedRequested = false;

            while(!proceedRequested) { yield return null; }

            isPaused = false;
            proceedRequested = false;
        }

        private IEnumerator FadeToBlackAndLoadSceneRoutine(string sceneName = null)
        {
            if(string.IsNullOrEmpty(sceneName)) { sceneName = sceneNameFallback; }
            
            overlayCanvas.gameObject.SetActive(true);
            yield return Fade(overlayCanvasGroup.alpha, 1f, sceneChangeFadeDuration);
            SceneManager.LoadScene(sceneName);
        }

        private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
        {
            float elapsed = 0f;

            while(elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                overlayCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, fadeCurve.Evaluate(t));
                yield return null;
            }

            overlayCanvasGroup.alpha = endAlpha;
        }

        private void CreateFadeOverlay()
        {
            GameObject overlayGO = new GameObject("FadeOverlay");
            overlayCanvas = overlayGO.AddComponent<Canvas>();
            overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            overlayCanvas.sortingOrder = 9999;

            CanvasScaler scaler = overlayGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

            GameObject imageGO = new GameObject("FadeImage");
            imageGO.transform.SetParent(overlayCanvas.transform, false);

            fadeImage = imageGO.AddComponent<Image>();
            fadeImage.color = fadeColor;
            fadeImage.raycastTarget = false;

            RectTransform rt = fadeImage.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
            rt.anchoredPosition = Vector2.zero;
        }
    }
}
