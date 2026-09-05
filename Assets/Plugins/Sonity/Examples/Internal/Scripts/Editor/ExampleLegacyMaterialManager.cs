// Created by Victor Engström
// Copyright 2026 Sonigon AB
// http://www.sonity.org/

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ExampleSonity {

    [ExecuteInEditMode]
    [AddComponentMenu("")]
    public class ExampleLegacyMaterialManager : MonoBehaviour {

        [Header("Fixes material compability with BRP, URP, HDRP and SRP")]
        public RenderPipelineAsset renderPipelineAsset;
        // Better to save as string, because SRP can have custom names, so enum type might not be right
        public string renderPipelineName = "Not initialized";

        // Cannot downgrade from URP or HDRP to BRP again
        [Header("BRP Materials")]
        public Material material_BRP_Default;
        public Material material_BRP_Grey;
        public Material material_BRP_Red;

        [Header("URP Materials")]
        public Material material_URP_Default;
        public Material material_URP_Grey;
        public Material material_URP_Red;

        [Header("Shared Materials")]
        public Material material_Shared_Water;
        // Water is shared between the different versions

        // Fix for making materials work in SRP, URP, HDRP
        private void Start() {

            // Only run in editor
            if (Application.isPlaying) {
                return;
            }

            Initialize();
        }

        public void Initialize() {

            RenderPipelineAsset renderPipelineNew = GraphicsSettings.currentRenderPipeline;
            string renderPipelineNameNew = ExampleLegacyMaterialHelpers.ClassifyPipeline(renderPipelineNew);

            if (renderPipelineAsset != renderPipelineNew || renderPipelineName != renderPipelineNameNew) {
                Debug.Log($"Sonity: Render pipeline change from \"{renderPipelineName}\" to \"{renderPipelineNameNew}\"");
                renderPipelineAsset = renderPipelineNew;
                renderPipelineName = renderPipelineNameNew;
                EditorUtility.SetDirty(this);
            }
        }
    }
}
#endif