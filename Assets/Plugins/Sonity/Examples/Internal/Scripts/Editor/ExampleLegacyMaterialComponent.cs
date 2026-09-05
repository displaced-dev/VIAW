// Created by Victor Engström
// Copyright 2026 Sonigon AB
// http://www.sonity.org/

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace ExampleSonity {

    [ExecuteInEditMode]
    [AddComponentMenu("")]
    public class ExampleLegacyMaterialComponent : MonoBehaviour {
        
        public string renderPipelineName = "Not initialized";

        public MaterialType objectMaterial = MaterialType.Default;

        public enum MaterialType {
            Default,
            Grey,
            Red,
            Water,
            // Add new after the old ones
        }

        // Fix for making materials work in SRP, URP, HDRP
        private void Start() {
            // Only run in editor
            if (Application.isPlaying) {
                return;
            }
            FixMaterial();
        }

        private void FixMaterial() {

#if UNITY_6000_4_OR_NEWER || SONITY_DLL_RUNTIME
            // Unity 6000.4 removes FindObjectsSortMode
            List<ExampleLegacyMaterialManager> managers = UnityEngine.Object.FindObjectsByType<ExampleLegacyMaterialManager>(FindObjectsInactive.Include).ToList();
#elif UNITY_2022_3_OR_NEWER
            List<ExampleLegacyMaterialManager> managers = UnityEngine.Object.FindObjectsByType<ExampleLegacyMaterialManager>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
#else
            List<ExampleLegacyMaterialManager> managers = UnityEngine.Object.FindObjectsOfType<ExampleLegacyMaterialManager>().ToList();
#endif

            ExampleLegacyMaterialManager manager = null;

            // There should only be 1 manager but hey
            for (int i = 0; i < managers.Count; i++) {
                manager = managers[i];
                if (manager != null) {
                    manager = managers[i];
                    // Only need to use 1 manager
                    break;
                }
            }

            if (manager == null) {
                return;
            }

            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (renderer != null) {
                
                // Make sure its initialized first
                manager.Initialize();

                string managerRenderPipelineName = manager.renderPipelineName;

                if (renderPipelineName != managerRenderPipelineName) {

                    RenderPipelineType renderPipelineType = ExampleLegacyMaterialHelpers.GetRenderPipelineType(managerRenderPipelineName);

                    if (renderPipelineType == RenderPipelineType.BRP_Built_In_Render_Pipeline) {
                        // BRP has no currentRenderPipeline so it will be null
                        // Therefore we need to to use a saved BRP material
                        switch (objectMaterial) {
                            case MaterialType.Default:
                                renderer.material = manager.material_BRP_Default;
                                break;
                            case MaterialType.Grey:
                                renderer.material = manager.material_BRP_Grey;
                                break;
                            case MaterialType.Red:
                                renderer.material = manager.material_BRP_Red;
                                break;
                            case MaterialType.Water:
                                // Water doesnt really need to change or have a component, but hey
                                renderer.material = manager.material_Shared_Water;
                                break;
                        }
                    } else {
                        if (renderPipelineType == RenderPipelineType.URP_Universal_Render_Pipeline) {
                            // URP has saved materials we can use instead of currentRenderPipeline.defaultMaterial
                            switch (objectMaterial) {
                                case MaterialType.Default:
                                    renderer.material = manager.material_URP_Default;
                                    break;
                                case MaterialType.Grey:
                                    renderer.material = manager.material_URP_Grey;
                                    break;
                                case MaterialType.Red:
                                    renderer.material = manager.material_URP_Red;
                                    break;
                                case MaterialType.Water:
                                    // Water doesnt really need to change or have a component, but hey
                                    renderer.material = manager.material_Shared_Water;
                                    break;
                            }
                        } else {

                            // Only upgrade material when going to SRP, HDRP (not URP, is saved materials)
                            // But dont upgrade material when its water 
                            if (objectMaterial == MaterialType.Water) {

                                // Unity Warning:
                                // Instantiating material due to calling renderer.material during edit mode.
                                // This will leak materials into the scene.
                                // You most likely want to use renderer.sharedMaterial instead.

                                // Comment:
                                // renderer.sharedMaterial color will make everything be the same color though
                                // So that wont work either

                                // Copy color from old material to new (dont use sharedMaterial)
                                Color color = renderer.material.color;
                                renderer.material = GraphicsSettings.currentRenderPipeline.defaultMaterial;
                                renderer.material.color = color;
                            }
                        }
                    }

                    Debug.Log($"Sonity: Switch material on: \"{renderer.gameObject.name}\" from \"{renderPipelineName}\" to \"{managerRenderPipelineName}\"", renderer.gameObject);

                    // Set after debug
                    renderPipelineName = managerRenderPipelineName;

                    // Sets Dirty in Editor so it will save
                    EditorUtility.SetDirty(this);
                }
            }
        }
    }
}
#endif