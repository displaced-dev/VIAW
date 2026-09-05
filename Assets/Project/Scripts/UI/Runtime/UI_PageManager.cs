using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace VIAW.UI
{
    [System.Serializable]
    public class PageData
    {
        public string PageName;
        public GameObject Page;
        public List<GameObject> DependencyPages = new List<GameObject>();
    }

    public class UI_PageManager : MonoBehaviour
    {
        [SerializeField] private List<PageData> pages = new List<PageData>();

        private int currentPageIndex = -1;
        private List<GameObject> activeDependencies = new List<GameObject>();

        private void Start()
        {
            HideAllPages();
            if (pages.Count > 0) { SetPage(0); }
        }

        public void SetPage(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= pages.Count) {
                Debug.LogWarning($"Page index {pageIndex} out of range.");
                return;
            }

            HideAllPages();

            PageData target = pages[pageIndex];
            target.Page.SetActive(true);

            activeDependencies.Clear();
            foreach (GameObject dep in target.DependencyPages) {
                if (dep == null) { continue; }
                dep.SetActive(true);
                activeDependencies.Add(dep);
            }

            currentPageIndex = pageIndex;
        }

        public void SetPage(string pageName)
        {
            int index = pages.FindIndex(p => p.PageName == pageName);
            if (index == -1) {
                Debug.LogWarning($"No page named '{pageName}'.");
                return;
            }
            SetPage(index);
        }

        public void NextPage()
        {
            if (pages.Count == 0) { return; }
            SetPage((currentPageIndex + 1) % pages.Count);
        }

        public void PreviousPage()
        {
            if (pages.Count == 0) { return; }
            SetPage((currentPageIndex - 1 + pages.Count) % pages.Count);
        }

        public int GetCurrentPageIndex() => currentPageIndex;

        public string GetCurrentPageName()
        {
            if (currentPageIndex < 0 || currentPageIndex >= pages.Count) { return null; }
            return pages[currentPageIndex].PageName;
        }

        public IReadOnlyList<GameObject> GetActiveDependencies() => activeDependencies.AsReadOnly();

        private void HideAllPages()
        {
            foreach (PageData pd in pages) {
                if (pd.Page != null) { pd.Page.SetActive(false); }
            }

            foreach (GameObject dep in activeDependencies) {
                if (dep != null) { dep.SetActive(false); }
            }

            activeDependencies.Clear();
        }

    #if UNITY_EDITOR
        public void EditorPreviewPage(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= pages.Count) { return; }

            foreach (PageData pd in pages) {
                if (pd.Page != null) { pd.Page.SetActive(false); }

                foreach (GameObject dep in pd.DependencyPages) {
                    if (dep != null) { dep.SetActive(false); }
                }
            }

            PageData target = pages[pageIndex];
            if (target.Page != null) { target.Page.SetActive(true); }

            foreach (GameObject dep in target.DependencyPages) {
                if (dep != null) { dep.SetActive(true); }
            }

            currentPageIndex = pageIndex;
            EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
    #endif
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(UI_PageManager))]
    public class UI_PageManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            UI_PageManager manager = (UI_PageManager)target;

            SerializedProperty pagesProp = serializedObject.FindProperty("pages");
            if (pagesProp == null || pagesProp.arraySize == 0) { return; }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Quick Navigate", EditorStyles.boldLabel);

            int currentIndex = manager.GetCurrentPageIndex();

            for (int i = 0; i < pagesProp.arraySize; i++) {
                SerializedProperty entry = pagesProp.GetArrayElementAtIndex(i);
                SerializedProperty nameProp = entry.FindPropertyRelative("PageName");
                SerializedProperty goProp = entry.FindPropertyRelative("Page");

                string label = string.IsNullOrWhiteSpace(nameProp.stringValue) ? $"Page {i}" : nameProp.stringValue;
                bool isActive = (i == currentIndex);
                bool missingGO = (goProp.objectReferenceValue == null);

                EditorGUILayout.BeginHorizontal();

                GUI.enabled = !missingGO;
                GUI.color = isActive ? Color.cyan : Color.white;
                if (GUILayout.Button($"[{i}] {label}")) {
                    Undo.RecordObject(manager, "Preview Page");
                    manager.EditorPreviewPage(i);
                }
                GUI.color = Color.white;
                GUI.enabled = true;

                if (missingGO) {
                    EditorGUILayout.LabelField("Missing GO", GUILayout.Width(70));
                }

                EditorGUILayout.EndHorizontal();
            }
        }
    }
    #endif
}
