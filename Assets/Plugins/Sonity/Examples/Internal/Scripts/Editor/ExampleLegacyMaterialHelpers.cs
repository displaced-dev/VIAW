// Created by Victor Engström
// Copyright 2026 Sonigon AB
// http://www.sonity.org/

#if UNITY_EDITOR

using UnityEngine.Rendering;
using System;

namespace ExampleSonity {

    public enum RenderPipelineType {
        None,
        BRP_Built_In_Render_Pipeline,
        URP_Universal_Render_Pipeline,
        HDRP_High_Definition_Render_Pipeline,
        SRP_Custom_Scriptable_Render_Pipeline,
    }

    public static class ExampleLegacyMaterialHelpers {

        // Examples:
        //RenderPipelineAsset current = GraphicsSettings.currentRenderPipeline;
        //RenderPipelineAsset defaultPipeline = GraphicsSettings.defaultRenderPipeline;
        //RenderPipelineAsset qualityOverride = QualitySettings.renderPipeline;
        //Debug.Log($"[Render Pipeline] Active (GraphicsSettings.currentRenderPipeline): {DescribeAsset(current)}");
        //Debug.Log($"[Render Pipeline] Classification: {ClassifyPipeline(current)}");
        //Debug.Log($"[Render Pipeline] Default (GraphicsSettings.defaultRenderPipeline): {DescribeAsset(defaultPipeline)}");
        //Debug.Log($"[Render Pipeline] Quality override (QualitySettings.renderPipeline, current level): {DescribeAsset(qualityOverride)}");
        //Debug.Log($"[Render Pipeline] Current quality level: {QualitySettings.GetQualityLevel()} — {QualitySettings.names[QualitySettings.GetQualityLevel()]}");

        public static readonly string BRP_Built_In_Render_Pipeline = "Built-in Render Pipeline (BRP)";
        public static readonly string URP_Universal_Render_Pipeline = "Universal Render Pipeline (URP)";
        public static readonly string HDRP_High_Definition_Render_Pipeline = "High Definition Render Pipeline (HDRP)";
        public static readonly string SRP_Custom_Scriptable_Render_Pipeline = "Scriptable Render Pipeline - custom or unrecognized (SRP)";

        public static RenderPipelineType GetRenderPipelineType(string renderPipelineName) {
            if (renderPipelineName == BRP_Built_In_Render_Pipeline) {
                return RenderPipelineType.BRP_Built_In_Render_Pipeline;
            } else if (renderPipelineName == URP_Universal_Render_Pipeline) {
                return RenderPipelineType.URP_Universal_Render_Pipeline;
            } else if (renderPipelineName == HDRP_High_Definition_Render_Pipeline) {
                return RenderPipelineType.HDRP_High_Definition_Render_Pipeline;
            } else if (renderPipelineName.Contains(SRP_Custom_Scriptable_Render_Pipeline)) {
                return RenderPipelineType.SRP_Custom_Scriptable_Render_Pipeline;
            }
            return RenderPipelineType.None;
        }

        /// <summary>
        /// Identifies URP/HDRP by the runtime type of the assigned asset, without compiling against those packages.
        /// </summary>
        public static string ClassifyPipeline(RenderPipelineAsset current) {
            if (current == null) {
                return BRP_Built_In_Render_Pipeline;
            }

            Type t = current.GetType();
            string ns = t.Namespace ?? string.Empty;
            string name = t.Name;

            if (name == "UniversalRenderPipelineAsset" || ns.StartsWith("UnityEngine.Rendering.Universal", StringComparison.Ordinal)) {
                return URP_Universal_Render_Pipeline;
            }

            if (name == "HDRenderPipelineAsset" || ns.StartsWith("UnityEngine.Rendering.HighDefinition", StringComparison.Ordinal)) {
                return HDRP_High_Definition_Render_Pipeline;
            }

            return $"{SRP_Custom_Scriptable_Render_Pipeline} - {t.FullName}";
        }

        public static string DescribeAsset(RenderPipelineAsset asset) {
            if (asset == null) {
                return "null (Built-in Render Pipeline is active when current is null)";
            }
            Type t = asset.GetType();
            return $"{t.FullName} — asset name: \"{asset.name}\"";
        }
    }
}
#endif