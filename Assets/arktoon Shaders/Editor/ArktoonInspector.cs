using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

// Arktoon-Shaders
//
// Copyright (c) 2018 synqark
//
// This code and repos（https://github.com/synqark/Arktoon-Shader) is under MIT licence, see LICENSE
//
// 本コードおよびリポジトリ（https://github.com/synqark/Arktoon-Shader) は MIT License を使用して公開しています。
// 詳細はLICENSEか、https://opensource.org/licenses/mit-license.php を参照してください。
namespace ArktoonShaders
{
    public class ArktoonInspector : ShaderGUI
    {
        #region MaterialProperties
        MaterialProperty BaseTexture;
        MaterialProperty BaseColor;
        MaterialProperty Normalmap;
        MaterialProperty BumpScale;
        MaterialProperty EmissionMap;
        MaterialProperty EmissionColor;
        MaterialProperty Shadowborder;
        MaterialProperty ShadowborderBlur;
        MaterialProperty ShadowStrength;
        MaterialProperty ShadowStrengthMask;
        MaterialProperty ShadowIndirectIntensity;
        MaterialProperty ShadowUseStep;
        MaterialProperty ShadowSteps;
        MaterialProperty PointAddIntensity;
        MaterialProperty PointShadowStrength;
        MaterialProperty PointShadowborder;
        MaterialProperty PointShadowborderBlur;
        MaterialProperty PointShadowUseStep;
        MaterialProperty PointShadowSteps;
        MaterialProperty CutoutCutoutAdjust;
        MaterialProperty ShadowPlanBUsePlanB;
        MaterialProperty ShadowPlanBDefaultShadowMix;
        MaterialProperty ShadowPlanBUseCustomShadowTexture;
        MaterialProperty ShadowPlanBHueShiftFromBase;
        MaterialProperty ShadowPlanBSaturationFromBase;
        MaterialProperty ShadowPlanBValueFromBase;
        MaterialProperty ShadowPlanBCustomShadowTexture;
        MaterialProperty ShadowPlanBCustomShadowTextureRGB;
        MaterialProperty CustomShadow2nd;
        MaterialProperty ShadowPlanB2border;
        MaterialProperty ShadowPlanB2borderBlur;
        MaterialProperty ShadowPlanB2HueShiftFromBase;
        MaterialProperty ShadowPlanB2SaturationFromBase;
        MaterialProperty ShadowPlanB2ValueFromBase;
        MaterialProperty ShadowPlanB2UseCustomShadowTexture;
        MaterialProperty ShadowPlanB2CustomShadowTexture;
        MaterialProperty ShadowPlanB2CustomShadowTextureRGB;
        MaterialProperty UseGloss;
        MaterialProperty GlossBlend;
        MaterialProperty GlossBlendMask;
        MaterialProperty GlossPower;
        MaterialProperty GlossColor;
        MaterialProperty UseOutline;
        MaterialProperty OutlineWidth;
        MaterialProperty OutlineMask;
        MaterialProperty OutlineCutoffRange;
        MaterialProperty OutlineColor;
        MaterialProperty OutlineShadeMix;
        MaterialProperty OutlineTextureColorRate;
        MaterialProperty UseOutlineWidthMask;
        MaterialProperty OutlineWidthMask;
        MaterialProperty MatcapBlendMode;
        MaterialProperty MatcapBlend;
        MaterialProperty MatcapTexture;
        MaterialProperty MatcapColor;
        MaterialProperty MatcapBlendMask;
        MaterialProperty MatcapNormalMix;
        MaterialProperty MatcapShadeMix;
        MaterialProperty UseReflection;
        MaterialProperty UseReflectionProbe;
        MaterialProperty ReflectionReflectionPower;
        MaterialProperty ReflectionReflectionMask;
        MaterialProperty ReflectionNormalMix;
        MaterialProperty ReflectionShadeMix;
        MaterialProperty ReflectionCubemap;
        MaterialProperty ReflectionSuppressBaseColorValue;
        MaterialProperty UseRim;
        MaterialProperty RimBlend;
        MaterialProperty RimBlendMask;
        MaterialProperty RimShadeMix;
        MaterialProperty RimFresnelPower;
        MaterialProperty RimUpperSideWidth;
        MaterialProperty RimColor;
        MaterialProperty RimTexture;
        MaterialProperty RimUseBaseTexture;
        MaterialProperty ShadowCapBlendMode;
        MaterialProperty ShadowCapBlend;
        MaterialProperty ShadowCapBlendMask;
        MaterialProperty ShadowCapNormalMix;
        MaterialProperty ShadowCapTexture;
        MaterialProperty StencilNumber;
        MaterialProperty StencilCompareAction;
        MaterialProperty StencilMaskTex;
        MaterialProperty StencilMaskAdjust;
        MaterialProperty UseDoubleSided;
        MaterialProperty DoubleSidedFlipBackfaceNormal;
        MaterialProperty DoubleSidedBackfaceLightIntensity;
        MaterialProperty ShadowCasterCulling;
        MaterialProperty ZWrite;
        MaterialProperty VertexColorBlendDiffuse;
        MaterialProperty VertexColorBlendEmissive;
        MaterialProperty OtherShadowBorderSharpness;
        MaterialProperty OtherShadowAdjust;
        MaterialProperty UseVertexLight;
        MaterialProperty BackfaceColorMultiply;
        MaterialProperty LightSampling;
        MaterialProperty UsePositionRelatedCalc;

        #endregion

        static bool IsShowAdvanced = false;
        GUIStyle style = new GUIStyle();


        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            Material material = materialEditor.target as Material;
            Shader shader = material.shader;

            // shader.nameによって調整可能なプロパティを制御する。
            bool isOpaque = shader.name.Contains("Opaque");
            bool isFade = shader.name.Contains("Fade");
            bool isCutout = shader.name.Contains("Cutout");
            bool isStencilWriter = shader.name.Contains("Stencil/Writer");
            bool isStencilReader = shader.name.Contains("Stencil/Reader");
            bool isStencilWriterMask = shader.name.Contains("Stencil/WriterMask");

            // FindProperties
            BaseTexture = FindProperty("_MainTex", props);
            BaseColor = FindProperty("_Color", props);
            Normalmap = FindProperty("_BumpMap", props);
            BumpScale = FindProperty("_BumpScale", props);
            EmissionMap = FindProperty("_EmissionMap", props);
            EmissionColor = FindProperty("_EmissionColor", props);
            if(isCutout) CutoutCutoutAdjust = FindProperty("_CutoutCutoutAdjust", props);
            Shadowborder = FindProperty("_Shadowborder", props);
            ShadowborderBlur = FindProperty("_ShadowborderBlur", props);
            ShadowStrength = FindProperty("_ShadowStrength", props);
            ShadowStrengthMask = FindProperty("_ShadowStrengthMask", props);
            ShadowIndirectIntensity = FindProperty("_ShadowIndirectIntensity", props);
            ShadowUseStep = FindProperty("_ShadowUseStep", props);
            ShadowSteps = FindProperty("_ShadowSteps", props);
            PointAddIntensity = FindProperty("_PointAddIntensity", props);
            PointShadowStrength = FindProperty("_PointShadowStrength", props);
            PointShadowborder = FindProperty("_PointShadowborder", props);
            PointShadowborderBlur= FindProperty("_PointShadowborderBlur", props);
            PointShadowUseStep = FindProperty("_PointShadowUseStep", props);
            PointShadowSteps = FindProperty("_PointShadowSteps", props);
            ShadowPlanBUsePlanB = FindProperty("_ShadowPlanBUsePlanB", props);
            ShadowPlanBDefaultShadowMix = FindProperty("_ShadowPlanBDefaultShadowMix", props);
            ShadowPlanBUseCustomShadowTexture = FindProperty("_ShadowPlanBUseCustomShadowTexture", props);
            ShadowPlanBHueShiftFromBase = FindProperty("_ShadowPlanBHueShiftFromBase", props);
            ShadowPlanBSaturationFromBase = FindProperty("_ShadowPlanBSaturationFromBase", props);
            ShadowPlanBValueFromBase = FindProperty("_ShadowPlanBValueFromBase", props);
            ShadowPlanBCustomShadowTexture = FindProperty("_ShadowPlanBCustomShadowTexture", props);
            ShadowPlanBCustomShadowTextureRGB = FindProperty("_ShadowPlanBCustomShadowTextureRGB", props);

            CustomShadow2nd = FindProperty("_CustomShadow2nd", props);

            ShadowPlanB2border = FindProperty("_ShadowPlanB2border", props);
            ShadowPlanB2borderBlur = FindProperty("_ShadowPlanB2borderBlur", props);
            ShadowPlanB2HueShiftFromBase = FindProperty("_ShadowPlanB2HueShiftFromBase", props);
            ShadowPlanB2SaturationFromBase = FindProperty("_ShadowPlanB2SaturationFromBase", props);
            ShadowPlanB2ValueFromBase = FindProperty("_ShadowPlanB2ValueFromBase", props);
            ShadowPlanB2UseCustomShadowTexture = FindProperty("_ShadowPlanB2UseCustomShadowTexture", props);
            ShadowPlanB2CustomShadowTexture = FindProperty("_ShadowPlanB2CustomShadowTexture", props);
            ShadowPlanB2CustomShadowTextureRGB = FindProperty("_ShadowPlanB2CustomShadowTextureRGB", props);

            UseGloss = FindProperty("_UseGloss", props);
            GlossBlend = FindProperty("_GlossBlend", props);
            GlossBlendMask = FindProperty("_GlossBlendMask", props);
            GlossPower = FindProperty("_GlossPower", props);
            GlossColor = FindProperty("_GlossColor", props);
            UseOutline = FindProperty("_UseOutline", props);
            OutlineWidth = FindProperty("_OutlineWidth", props);
            if(!isOpaque) OutlineMask = FindProperty("_OutlineMask", props);
            if(!isOpaque) OutlineCutoffRange = FindProperty("_OutlineCutoffRange", props);
            OutlineColor = FindProperty("_OutlineColor", props);
            OutlineShadeMix = FindProperty("_OutlineShadeMix", props);
            OutlineTextureColorRate = FindProperty("_OutlineTextureColorRate", props);
            UseOutlineWidthMask = FindProperty("_UseOutlineWidthMask", props);
            OutlineWidthMask = FindProperty("_OutlineWidthMask", props);
            MatcapBlendMode = FindProperty("_MatcapBlendMode", props);
            MatcapBlend = FindProperty("_MatcapBlend", props);
            MatcapTexture = FindProperty("_MatcapTexture", props);
            MatcapColor = FindProperty("_MatcapColor", props);
            MatcapBlendMask = FindProperty("_MatcapBlendMask", props);
            MatcapNormalMix = FindProperty("_MatcapNormalMix", props);
            MatcapShadeMix = FindProperty("_MatcapShadeMix", props);
            UseReflection = FindProperty("_UseReflection", props);
            UseReflectionProbe = FindProperty("_UseReflectionProbe", props);
            ReflectionReflectionPower = FindProperty("_ReflectionReflectionPower", props);
            ReflectionReflectionMask = FindProperty("_ReflectionReflectionMask", props);
            ReflectionNormalMix = FindProperty("_ReflectionNormalMix", props);
            ReflectionShadeMix = FindProperty("_ReflectionShadeMix", props);
            ReflectionCubemap = FindProperty("_ReflectionCubemap", props);
            ReflectionSuppressBaseColorValue = FindProperty("_ReflectionSuppressBaseColorValue", props);
            UseRim = FindProperty("_UseRim", props);
            RimBlend = FindProperty("_RimBlend", props);
            RimBlendMask = FindProperty("_RimBlendMask", props);
            RimShadeMix = FindProperty("_RimShadeMix", props);
            RimFresnelPower = FindProperty("_RimFresnelPower", props);
            RimUpperSideWidth = FindProperty("_RimUpperSideWidth", props);
            RimColor = FindProperty("_RimColor", props);
            RimTexture = FindProperty("_RimTexture", props);
            RimUseBaseTexture = FindProperty("_RimUseBaseTexture", props);
            ShadowCapBlendMode = FindProperty("_ShadowCapBlendMode", props);
            ShadowCapBlend = FindProperty("_ShadowCapBlend", props);
            ShadowCapBlendMask = FindProperty("_ShadowCapBlendMask", props);
            ShadowCapNormalMix = FindProperty("_ShadowCapNormalMix", props);
            ShadowCapTexture = FindProperty("_ShadowCapTexture", props);
            if(isStencilWriter || isStencilReader) StencilNumber = FindProperty("_StencilNumber", props);
            if(isStencilWriterMask) StencilMaskTex = FindProperty("_StencilMaskTex", props);
            if(isStencilWriterMask) StencilMaskAdjust = FindProperty("_StencilMaskAdjust", props);
            if(isStencilReader) StencilCompareAction = FindProperty("_StencilCompareAction", props);
            // Cull = FindProperty("_Cull", props);
            UseDoubleSided = FindProperty("_UseDoubleSided", props);
            DoubleSidedFlipBackfaceNormal = FindProperty("_DoubleSidedFlipBackfaceNormal", props);
            DoubleSidedBackfaceLightIntensity = FindProperty("_DoubleSidedBackfaceLightIntensity", props);
            ShadowCasterCulling = FindProperty("_ShadowCasterCulling", props);
            VertexColorBlendDiffuse = FindProperty("_VertexColorBlendDiffuse", props);
            VertexColorBlendEmissive = FindProperty("_VertexColorBlendEmissive", props);
            OtherShadowBorderSharpness = FindProperty("_OtherShadowBorderSharpness", props);
            OtherShadowAdjust = FindProperty("_OtherShadowAdjust", props);
            UseVertexLight = FindProperty("_UseVertexLight", props);
            LightSampling = FindProperty("_LightSampling", props);
            UsePositionRelatedCalc = FindProperty("_UsePositionRelatedCalc", props);
            if(isFade) ZWrite = FindProperty("_ZWrite", props);
            // BackfaceColorMultiply = FindProperty("_BackfaceColorMultiply", props);

            EditorGUIUtility.labelWidth = 0f;

            EditorGUI.BeginChangeCheck();
            {
                UIHelper.ShurikenHeader("Common");
                {
                    EditorGUI.indentLevel ++;
                    materialEditor.TexturePropertySingleLine(new GUIContent("Main Texture", "Base Color Texture (RGB)"), BaseTexture, BaseColor);
                    materialEditor.TexturePropertySingleLine(new GUIContent("Normal Map", "Normal Map (RGB)"), Normalmap, BumpScale);
                    materialEditor.TexturePropertySingleLine(new GUIContent("Emission", "Emission (RGB)"), EmissionMap, EmissionColor);
                    // materialEditor.ShaderProperty(Cull, "Cull");
                    materialEditor.ShaderProperty(UseDoubleSided, "Is Double Sided");
                    var doublesided = UseDoubleSided.floatValue;
                    if(doublesided > 0){
                        ShadowCasterCulling.floatValue = 0;
                        EditorGUI.indentLevel ++;
                        materialEditor.ShaderProperty(DoubleSidedFlipBackfaceNormal, "Flip backface normal");
                        materialEditor.ShaderProperty(DoubleSidedBackfaceLightIntensity, "Backface Light Intensity");
                        EditorGUI.indentLevel --;
                    } else {
                        ShadowCasterCulling.floatValue = 2;
                    }
                    if(isFade) materialEditor.ShaderProperty(ZWrite, "ZWrite");
                    EditorGUI.indentLevel --;
                }

                if(isCutout){
                    UIHelper.ShurikenHeader("Alpha Cutout");
                    {
                        EditorGUI.indentLevel ++;
                        materialEditor.ShaderProperty(CutoutCutoutAdjust, "Cutoff Adjust");
                        EditorGUI.indentLevel --;
                    }
                }

                UIHelper.ShurikenHeader("Shadow");
                {
                    EditorGUI.indentLevel ++;
                    materialEditor.ShaderProperty(Shadowborder, "Border pos");
                    materialEditor.ShaderProperty(ShadowborderBlur, "Border blur");
                    if(ShadowPlanBUsePlanB.floatValue > 0)
                    {
                        EditorGUILayout.LabelField("Strength"," (disabled)", EditorStyles.centeredGreyMiniLabel);
                    } else {
                        materialEditor.ShaderProperty(ShadowStrength, "Strength");
                    }
                    materialEditor.ShaderProperty(ShadowStrengthMask, "Strength Mask");
                    materialEditor.ShaderProperty(ShadowUseStep, "Use Steps");
                    var useStep = ShadowUseStep.floatValue;
                    if(useStep > 0)
                    {
                        materialEditor.ShaderProperty(ShadowSteps, " ");
                    }

                    materialEditor.ShaderProperty(ShadowPlanBUsePlanB, "Use Custom Shade");
                    var usePlanB = ShadowPlanBUsePlanB.floatValue;
                    if(usePlanB > 0)
                    {

                        EditorGUI.indentLevel ++;
                        materialEditor.ShaderProperty(ShadowPlanBDefaultShadowMix, "Mix Default Shade");
                        EditorGUILayout.LabelField("1st shade", EditorStyles.boldLabel);
                        EditorGUI.indentLevel ++;
                        materialEditor.ShaderProperty(ShadowPlanBUseCustomShadowTexture, "Use Shade Texture");
                        var useShadeTexture = ShadowPlanBUseCustomShadowTexture.floatValue;
                        if(useShadeTexture > 0)
                        {
                            materialEditor.ShaderProperty(ShadowPlanBCustomShadowTexture, "Shade Texture");
                            materialEditor.ShaderProperty(ShadowPlanBCustomShadowTextureRGB, "Shade Texture RGB");
                        }
                        else
                        {
                            materialEditor.ShaderProperty(ShadowPlanBHueShiftFromBase, "Hue Shift");
                            materialEditor.ShaderProperty(ShadowPlanBSaturationFromBase, "Saturation");
                            materialEditor.ShaderProperty(ShadowPlanBValueFromBase, "Value");
                        }
                        EditorGUI.indentLevel --;
                        EditorGUILayout.LabelField("2nd shade", EditorStyles.boldLabel);
                        EditorGUI.indentLevel ++;
                        materialEditor.ShaderProperty(CustomShadow2nd, "Use");
                        var customshadow2nd = CustomShadow2nd.floatValue;
                        if(customshadow2nd > 0)
                        {
                            materialEditor.ShaderProperty(ShadowPlanB2border, "Border pos");
                            materialEditor.ShaderProperty(ShadowPlanB2borderBlur, "Border blur");
                            materialEditor.ShaderProperty(ShadowPlanB2UseCustomShadowTexture, "Use Shade Texture");
                            var useShadeTexture2 = ShadowPlanB2UseCustomShadowTexture.floatValue;
                            if(useShadeTexture2 > 0)
                            {
                                materialEditor.ShaderProperty(ShadowPlanB2CustomShadowTexture,  "Shade Texture");
                                materialEditor.ShaderProperty(ShadowPlanB2CustomShadowTextureRGB,  "Shade Texture RGB");
                            }else{
                                materialEditor.ShaderProperty(ShadowPlanB2HueShiftFromBase, "Hue Shift");
                                materialEditor.ShaderProperty(ShadowPlanB2SaturationFromBase, "Saturation");
                                materialEditor.ShaderProperty(ShadowPlanB2ValueFromBase, "Value");
                            }
                        }

                        EditorGUI.indentLevel --;
                        EditorGUI.indentLevel --;
                    }
                    EditorGUI.indentLevel --;
                }

                UIHelper.ShurikenHeader("Gloss");
                {
                    EditorGUI.indentLevel ++;
                    materialEditor.ShaderProperty(UseGloss, "Use");
                    var useGloss = UseGloss.floatValue;
                    if(useGloss > 0)
                    {
                        materialEditor.ShaderProperty(GlossBlend, "Smoothness");
                        materialEditor.ShaderProperty(GlossBlendMask, "Smoothness Mask");
                        materialEditor.ShaderProperty(GlossPower, "Metallic");
                        materialEditor.ShaderProperty(GlossColor, "Color");
                    }
                    EditorGUI.indentLevel --;
                }

                UIHelper.ShurikenHeader("Outline");
                {
                    EditorGUI.indentLevel++;
                    materialEditor.ShaderProperty(UseOutline, "Use");
                    var useOutline = UseOutline.floatValue;
                    if(useOutline > 0)
                    {
                        materialEditor.ShaderProperty(OutlineWidth,"Width");
                        if(!isOpaque) {
                            materialEditor.ShaderProperty(OutlineMask,"Cutoff Mask");
                            materialEditor.ShaderProperty(OutlineCutoffRange,"Cutoff Range");
                        }else{
                            EditorGUILayout.LabelField("Cutoff Mask","Available in", EditorStyles.centeredGreyMiniLabel);
                            EditorGUILayout.LabelField("Cutoff Range","AlphaCutout/Fade Shader", EditorStyles.centeredGreyMiniLabel);
                        }
                        materialEditor.ShaderProperty(UseOutlineWidthMask,"Use Width Mask");
                        var useOutlineWidthMask = UseOutlineWidthMask.floatValue;
                        if (useOutlineWidthMask > 0) materialEditor.ShaderProperty(OutlineWidthMask,"Width Mask");

                        materialEditor.ShaderProperty(OutlineColor,"Color");
                        materialEditor.ShaderProperty(OutlineShadeMix,"Shadow mix");
                        materialEditor.ShaderProperty(OutlineTextureColorRate,"Base Color Mix");
                    }
                    EditorGUI.indentLevel--;
                }

                UIHelper.ShurikenHeader("MatCap");
                {
                    EditorGUI.indentLevel++;
                    materialEditor.ShaderProperty(MatcapBlendMode,"MatCap Mode");
                    var useMatcap = MatcapBlendMode.floatValue;
                    if(useMatcap != 3) // Not 'Unused'
                    {
                        materialEditor.ShaderProperty(MatcapBlend,"Blend");
                        materialEditor.ShaderProperty(MatcapBlendMask,"Blend Mask");
                        materialEditor.ShaderProperty(MatcapNormalMix, "Normal Map mix");
                        materialEditor.ShaderProperty(MatcapShadeMix,"Shadow mix");
                        materialEditor.ShaderProperty(MatcapTexture,"Texture");
                        materialEditor.ShaderProperty(MatcapColor,"Color");
                    }
                    EditorGUI.indentLevel--;
                }

                UIHelper.ShurikenHeader("Reflection");
                {
                    EditorGUI.indentLevel++;
                    materialEditor.ShaderProperty(UseReflection, "Use");
                    var useReflection = UseReflection.floatValue;
                    if(useReflection > 0)
                    {
                        materialEditor.ShaderProperty(UseReflectionProbe,"Use Reflection Probe");
                        var useProbe = UseReflectionProbe.floatValue;
                        materialEditor.ShaderProperty(ReflectionReflectionPower,"Smoothness");
                        materialEditor.ShaderProperty(ReflectionReflectionMask,"Smoothness Mask");
                        materialEditor.ShaderProperty(ReflectionNormalMix,"Normal Map mix");
                        materialEditor.ShaderProperty(ReflectionShadeMix, "Shadow mix");
                        materialEditor.ShaderProperty(ReflectionSuppressBaseColorValue,"Suppress Base Color");
                        if(useProbe <= 0) {
                            materialEditor.ShaderProperty(ReflectionCubemap,"Cubemap");
                        } else {
                            materialEditor.ShaderProperty(ReflectionCubemap,"Cubemap(falllback)");
                        }
                    }
                    EditorGUI.indentLevel--;
                }

                UIHelper.ShurikenHeader("Rim");
                {
                    EditorGUI.indentLevel++;
                    materialEditor.ShaderProperty(UseRim, "Use");
                    var useRim = UseRim.floatValue;
                    if(useRim > 0)
                    {
                        materialEditor.ShaderProperty(RimBlend,"Blend");
                        materialEditor.ShaderProperty(RimBlendMask,"Blend Mask");
                        materialEditor.ShaderProperty(RimShadeMix,"Shadow mix");
                        materialEditor.ShaderProperty(RimFresnelPower,"Fresnel Power");
                        materialEditor.ShaderProperty(RimUpperSideWidth,"Upper side width");
                        materialEditor.ShaderProperty(RimUseBaseTexture,"Use Base Color");
                        materialEditor.ShaderProperty(RimColor,"Color");
                        materialEditor.ShaderProperty(RimTexture,"Texture");
                    }
                    EditorGUI.indentLevel--;
                }

                UIHelper.ShurikenHeader("Shade Cap");
                {
                    EditorGUI.indentLevel++;
                    materialEditor.ShaderProperty(ShadowCapBlendMode,"Shade Cap Mode");
                    var useShadowCap = ShadowCapBlendMode.floatValue;
                    if(useShadowCap != 3) // Not 'Unused'
                    {
                        materialEditor.ShaderProperty(ShadowCapBlend,"Blend");
                        materialEditor.ShaderProperty(ShadowCapBlendMask,"Blend Mask");
                        materialEditor.ShaderProperty(ShadowCapNormalMix,"Normal Map mix");
                        materialEditor.ShaderProperty(ShadowCapTexture,"Texture");
                    }
                    EditorGUI.indentLevel--;
                }

                if(isStencilWriter)
                {
                    UIHelper.ShurikenHeader("Stencil Writer");
                    {
                        EditorGUI.indentLevel++;
                        materialEditor.ShaderProperty(StencilNumber,"Number");
                        if(isStencilWriterMask) materialEditor.ShaderProperty(StencilMaskTex, "Mask Texture");
                        if(isStencilWriterMask) materialEditor.ShaderProperty(StencilMaskAdjust, "Mask Adjust");
                        EditorGUI.indentLevel--;
                    }
                }

                if(isStencilReader)
                {
                    UIHelper.ShurikenHeader("Stencil Reader");
                    {
                        EditorGUI.indentLevel++;
                        materialEditor.ShaderProperty(StencilNumber,"Number");
                        materialEditor.ShaderProperty(StencilCompareAction,"Compare Action");
                        EditorGUI.indentLevel--;
                    }
                }

                IsShowAdvanced = UIHelper.ShurikenFoldout("Advanced / Experimental (Click to Open)", IsShowAdvanced);
                if (IsShowAdvanced) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.HelpBox("These are some shade tweaking. no need to change usually." + Environment.NewLine + "ほとんどのケースで触る必要のないやつら。",MessageType.Info);
                    if (GUILayout.Button("Revert advanced params.")){
                        PointAddIntensity.floatValue = 1f;
                        PointShadowStrength.floatValue = 0.5f;
                        PointShadowborder.floatValue = 0.5f;
                        PointShadowborderBlur.floatValue = 0.01f;
                        OtherShadowAdjust.floatValue = -0.1f;
                        OtherShadowBorderSharpness.floatValue = 3;
                        PointShadowUseStep.floatValue = 0;
                        PointShadowSteps.floatValue = 2;
                        ShadowIndirectIntensity.floatValue = 0.25f;
                        VertexColorBlendDiffuse.floatValue = 0f;
                        VertexColorBlendEmissive.floatValue = 0f;
                        UseVertexLight.floatValue = 1f;
                        material.EnableKeyword("USE_VERTEX_LIGHT");
                        LightSampling.floatValue = 0f;
                        material.EnableKeyword("_LIGHTSAMPLING_ARKTOON");
                        material.DisableKeyword("_LIGHTSAMPLING_CUBED");
                        UsePositionRelatedCalc.floatValue = 0f;
                        material.DisableKeyword("USE_POSITION_RELATED_CALC");
                    }
                    {
                        EditorGUI.indentLevel ++;
                        EditorGUILayout.LabelField("Lights", EditorStyles.boldLabel);
                        {
                            EditorGUI.indentLevel ++;
                            materialEditor.ShaderProperty(LightSampling, "Sampling Style (def:arktoon)");
                            EditorGUI.indentLevel --;
                        }
                        EditorGUILayout.LabelField("Directional Shadow", EditorStyles.boldLabel);
                        {
                            EditorGUI.indentLevel ++;
                            materialEditor.ShaderProperty(ShadowIndirectIntensity, "Indirect face Intensity (0.25)");
                            EditorGUI.indentLevel --;
                        }
                        EditorGUILayout.LabelField("Vertex Colors", EditorStyles.boldLabel);
                        {
                            EditorGUI.indentLevel ++;
                            materialEditor.ShaderProperty(VertexColorBlendDiffuse, "Color blend to diffuse (def:0) ");
                            materialEditor.ShaderProperty(VertexColorBlendEmissive, "Color blend to emissive (def:0) ");
                            EditorGUI.indentLevel --;
                        }
                        EditorGUILayout.LabelField("PointLights", EditorStyles.boldLabel);
                        {
                            EditorGUI.indentLevel ++;
                            materialEditor.ShaderProperty(PointAddIntensity, "Intensity (def:1)");
                            materialEditor.ShaderProperty(PointShadowStrength, "Shadow Strength (def:0.5)");
                            materialEditor.ShaderProperty(PointShadowborder, "Shadow Border (def:0.5)");
                            materialEditor.ShaderProperty(PointShadowborderBlur, "Shadow Border blur (def:0.01)");
                            materialEditor.ShaderProperty(PointShadowUseStep, "Use Shadow Steps");
                            var usePointStep = PointShadowUseStep.floatValue;
                            if(usePointStep > 0)
                            {
                                materialEditor.ShaderProperty(PointShadowSteps, " ");
                            }
                            materialEditor.ShaderProperty(UseVertexLight, "Use Per-vertex Light");
                            EditorGUI.indentLevel --;
                        }
                        EditorGUILayout.LabelField("Shade from other meshes", EditorStyles.boldLabel);
                        {
                            EditorGUI.indentLevel ++;
                            materialEditor.ShaderProperty(OtherShadowAdjust, "Adjust (def:-0.1)");
                            materialEditor.ShaderProperty(OtherShadowBorderSharpness, "Sharpness(def:3)");
                            EditorGUI.indentLevel --;
                        }
                        EditorGUILayout.LabelField("MatCap / ShadeCap", EditorStyles.boldLabel);
                        {
                            EditorGUI.indentLevel ++;
                            materialEditor.ShaderProperty(UsePositionRelatedCalc, "Use Position Related Calc(def: no)");
                            EditorGUI.indentLevel --;
                        }
                        EditorGUI.indentLevel --;
                        // materialEditor.ShaderProperty(BackfaceColorMultiply, "Backface Color Multiply (def:white)");
                    }
                    EditorGUI.indentLevel--;
                }

                // Arktoon version info
                {
                    string localVersion =  EditorUserSettings.GetConfigValue ("arktoon_version_local");
                    string remoteVersion = EditorUserSettings.GetConfigValue ("arktoon_version_remote");

                    UIHelper.ShurikenHeader("Arktoon-Shaders");
                    style.alignment = TextAnchor.MiddleRight;
                    style.normal.textColor = Color.black;
                    EditorGUILayout.LabelField("Your Version : " + localVersion, style);

                    if (!string.IsNullOrEmpty(remoteVersion))
                    {
                        Version local_v = new Version(localVersion);
                        Version remote_v = new Version(remoteVersion);

                        if(remote_v > local_v)  {
                            style.normal.textColor = Color.blue;
                            EditorGUILayout.LabelField("Remote Version : " + remoteVersion, style);
                            EditorGUILayout.BeginHorizontal( GUI.skin.box );
                            {
                                style.alignment = TextAnchor.MiddleLeft;
                                EditorGUILayout.LabelField("New version available : ", style);
                                if(GUILayout.Button("Open download page."))
                                {
                                    System.Diagnostics.Process.Start("https://github.com/synqark/Arktoon-Shaders/releases");
                                }
                            }
                            GUILayout.EndHorizontal();
                        } else {
                            EditorGUILayout.LabelField("Remote Version : " + remoteVersion, style);
                        }
                    }
                }

                // Docs
                {
                    EditorGUILayout.BeginHorizontal( GUI.skin.box );
                    if(GUILayout.Button("How to use (Japanese)"))
                    {
                        System.Diagnostics.Process.Start("https://docs.google.com/document/d/15qR1ixw7YO1vKqaJXp5ul3Yvsgvv2cWa3YlXM07iQlo/edit?usp=sharing");
                    }
                    EditorGUI.BeginDisabledGroup(true);
                    GUILayout.Button("(english - wip)");
                    EditorGUI.EndDisabledGroup();

                    GUILayout.EndHorizontal();
                }
            }
            EditorGUI.EndChangeCheck();
        }
    }

    class UIHelper
    {        private static Rect DrawShuriken(string title, Vector2 contentOffset) {
            var style = new GUIStyle("ShurikenModuleTitle");
            style.margin = new RectOffset(0, 0, 10, 0);
            style.font = new GUIStyle(EditorStyles.boldLabel).font;
            style.border = new RectOffset(15, 7, 4, 4);
            style.fixedHeight = 22;
            style.contentOffset = contentOffset;
            var rect = GUILayoutUtility.GetRect(16f, 22f, style);
            GUI.Box(rect, title, style);
            return rect;
        }
        public static void ShurikenHeader(string title)
        {
            DrawShuriken(title,new Vector2(6f, -2f));
        }
        public static bool ShurikenFoldout(string title, bool display)
        {
            var rect = DrawShuriken(title,new Vector2(20f, -2f));

            var e = Event.current;

            var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
            if (e.type == EventType.Repaint) {
                EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
            }

            if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition)) {
                display = !display;
                e.Use();
            }
            return display;
        }
    }
}