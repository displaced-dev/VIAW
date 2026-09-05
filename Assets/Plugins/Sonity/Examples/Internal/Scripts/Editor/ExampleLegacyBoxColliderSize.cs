// Created by Victor Engström
// Copyright 2026 Sonigon AB
// http://www.sonity.org/

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace ExampleSonity {

    [ExecuteInEditMode]
    [AddComponentMenu("")]
    public class ExampleLegacyBoxColliderSize : MonoBehaviour {

        public Vector3 boxColliderSize = new Vector3(1f, 1f, 1f);

        // Unity has a bug where if you downgrade a project box colliders might set the box colliders to size 2,2,2 instead of 1,1,1.
        private void Start() {
            // Only run in editor
            if (Application.isPlaying) {
                return;
            }
            BoxCollider boxCollider = GetComponent<BoxCollider>();
            if (boxCollider != null && boxCollider.size != boxColliderSize) {
                boxCollider.size = boxColliderSize;
                // Sets Dirty in Editor so it will save
                EditorUtility.SetDirty(this);
            }
        }
    }
}
#endif