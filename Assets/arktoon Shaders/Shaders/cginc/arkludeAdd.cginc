float4 frag(
    #ifdef ARKTOON_OUTLINE
        g2f i
    #else
        VertexOutput i
    #endif
    ,  bool isFrontFace : SV_IsFrontFace
    ) : SV_Target
{
    // 表裏・アウトライン
    fixed faceSign = isFrontFace ? 1 : -1;
    bool isOutline = i.color.a;

    // アウトラインの裏面は常に削除
    clip(1 - isOutline + isFrontFace - 0.001);

    float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir * lerp(1, faceSign, _DoubleSidedFlipBackfaceNormal));
    float3 viewDirection = normalize(UnityWorldSpaceViewDir(i.posWorld.xyz));
    float3 _BumpMap_var = UnpackScaleNormal(tex2D(REF_BUMPMAP,TRANSFORM_TEX(i.uv0, REF_BUMPMAP)), REF_BUMPSCALE);
    float3 normalLocal = _BumpMap_var.rgb;
    float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
    float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
    float3 lightColor = _LightColor0.rgb;
    float3 halfDirection = normalize(viewDirection+lightDirection);
    float3 cameraSpaceViewDir = mul((float3x3)unity_WorldToCamera, viewDirection);

    UNITY_LIGHT_ATTENUATION(attenuation,i, i.posWorld.xyz);
    float4 _MainTex_var = UNITY_SAMPLE_TEX2D(REF_MAINTEX, TRANSFORM_TEX(i.uv0, REF_MAINTEX));
    float3 Diffuse = (_MainTex_var.rgb*REF_COLOR.rgb);
    Diffuse = lerp(Diffuse, Diffuse * i.color, _VertexColorBlendDiffuse);

    #ifdef ARKTOON_CUTOUT
        clip((_MainTex_var.a * REF_COLOR.a) - _CutoutCutoutAdjust);
    #endif

    #ifdef ARKTOON_OUTLINE
        if (isOutline) {
            #if defined(ARKTOON_CUTOUT) || defined(ARKTOON_FADE)
                float _OutlineMask_var = UNITY_SAMPLE_TEX2D_SAMPLER(_OutlineMask, REF_MAINTEX, TRANSFORM_TEX(i.uv0, _OutlineMask)).r;
                clip(_OutlineMask_var.r - _OutlineCutoffRange);
            #endif

            // アウトラインであればDiffuseとColorを混ぜる
            float4 _OutlineTexture_var = UNITY_SAMPLE_TEX2D_SAMPLER(_OutlineTexture, REF_MAINTEX, TRANSFORM_TEX(i.uv0, _OutlineTexture));
            float3 outlineColor = lerp(float3(_OutlineColor.rgb * _OutlineTexture_var.rgb), Diffuse, _OutlineTextureColorRate);
            if (_OutlineUseColorShift) {
                float3 Outline_Diff_HSV = CalculateHSV(outlineColor, _OutlineHueShiftFromBase, _OutlineSaturationFromBase, _OutlineValueFromBase);
                Diffuse = Outline_Diff_HSV;
            } else {
                Diffuse = outlineColor;
            }
        }
    #endif

    fixed _PointShadowborderBlur_var = UNITY_SAMPLE_TEX2D_SAMPLER(_PointShadowborderBlurMask, REF_MAINTEX, TRANSFORM_TEX(i.uv0, _PointShadowborderBlurMask)).r * _PointShadowborderBlur;
    float ShadowborderMin = saturate(-_PointShadowborderBlur_var*0.5 + _PointShadowborder);
    float ShadowborderMax = saturate( _PointShadowborderBlur_var*0.5 + _PointShadowborder);

    float lightContribution = dot(lightDirection, normalDirection)*attenuation;
    float directContribution = 1.0 - ((1.0 - saturate(( (saturate(lightContribution) - ShadowborderMin)) / (ShadowborderMax - ShadowborderMin))));
    directContribution = lerp(directContribution, saturate(floor(directContribution * _PointShadowSteps) / (_PointShadowSteps - 1)), _PointShadowUseStep);

    // 光の受光に関する更なる補正
    // ・LightIntensityIfBackface(裏面を描画中に変動する受光倍率)
    // ・ShadowCapのModeがLightShutterの時にかかるマスク乗算
    float additionalContributionMultiplier = 1;
    additionalContributionMultiplier *= lerp(_DoubleSidedBackfaceLightIntensity, 1, isFrontFace);

    if (_ShadowCapBlendMode == 2) { // Light Shutter
        float3 normalDirectionShadowCap = normalize(mul( float3(normalLocal.r*_ShadowCapNormalMix,normalLocal.g*_ShadowCapNormalMix,normalLocal.b), tangentTransform )); // Perturbed normals
        float2 transformShadowCap = float2(0,0);
        //ここだけ他のComputeTransformCapと式が違った
        if (_UsePositionRelatedCalc) {
            float3 transformShadowCapViewDir = cameraSpaceViewDir - float3(0,0,1);
            float3 transformShadowCapNormal = mul((float3x3)unity_WorldToCamera, normalDirectionShadowCap);
            float3 transformShadowCapCombined = transformShadowCapViewDir * (dot(transformShadowCapViewDir, transformShadowCapNormal) / transformShadowCapViewDir.z) + transformShadowCapNormal;
            transformShadowCap = ((transformShadowCapCombined.rg*0.5)+0.5);
        } else {
            transformShadowCap = (mul((float3x3)unity_WorldToCamera, normalDirectionShadowCap).rg*0.5+0.5);
        }
        float4 _ShadowCapTexture_var = UNITY_SAMPLE_TEX2D_SAMPLER(_ShadowCapTexture, REF_MAINTEX, TRANSFORM_TEX(transformShadowCap, _ShadowCapTexture));
        float4 _ShadowCapBlendMask_var = UNITY_SAMPLE_TEX2D_SAMPLER(_ShadowCapBlendMask, REF_MAINTEX, TRANSFORM_TEX(i.uv0, _ShadowCapBlendMask));
        additionalContributionMultiplier *= (1.0 - ((1.0 - (_ShadowCapTexture_var.rgb))*_ShadowCapBlendMask_var.rgb)*_ShadowCapBlend);
    }

    directContribution *= additionalContributionMultiplier;
    float _ShadowStrengthMask_var = tex2D(_ShadowStrengthMask, TRANSFORM_TEX(i.uv0, _ShadowStrengthMask));
    float3 finalLight = saturate(directContribution + ((1 - (_PointShadowStrength * _ShadowStrengthMask_var)) * attenuation));
    float3 coloredLight = saturate(lightColor*finalLight*_PointAddIntensity);
    float3 toonedMap = Diffuse * coloredLight;

    float3 specular = float3(0,0,0);
    float3 matcap = float3(0,0,0);
    float3 RimLight = float3(0,0,0);
    float3 shadowcap = float3(1000,1000,1000);

    #if !defined(ARKTOON_REFRACTED) && defined(ARKTOON_OUTLINE)
    if (!isOutline) {
    #endif
        // オプション：Gloss
        if(_UseGloss) {
            float glossNdotV = abs(dot( normalDirection, viewDirection ));
            float _GlossBlendMask_var = UNITY_SAMPLE_TEX2D_SAMPLER(_GlossBlendMask, REF_MAINTEX, TRANSFORM_TEX(i.uv0, _GlossBlendMask));
            float gloss = _GlossBlend * _GlossBlendMask_var;
            float perceptualRoughness = 1.0 - gloss;
            float roughness = perceptualRoughness * perceptualRoughness;
            float specPow = exp2( gloss * 10.0+1.0);
            float NdotL = saturate(dot( normalDirection, lightDirection ));
            float LdotH = saturate(dot(lightDirection, halfDirection));
            float3 specularColor = _GlossPower;
            float specularMonochrome;
            float3 diffuseColor = Diffuse;
            diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
            specularMonochrome = 1.0-specularMonochrome;
            float NdotH = saturate(dot( normalDirection, halfDirection ));
            float VdotH = saturate(dot( viewDirection, halfDirection ));
            float visTerm = SmithJointGGXVisibilityTerm( NdotL, glossNdotV, roughness );
            float normTerm = GGXTerm(NdotH, roughness);
            float specularPBL = (visTerm*normTerm) * UNITY_PI;
            #ifdef UNITY_COLORSPACE_GAMMA
                specularPBL = sqrt(max(1e-4h, specularPBL));
            #endif
            specularPBL = max(0, specularPBL * NdotL);
            #if defined(_SPECULARHIGHLIGHTS_OFF)
                specularPBL = 0.0;
            #endif
            specularPBL *= any(specularColor) ? 1.0 : 0.0;
            float3 attenColor = attenuation * _LightColor0.xyz;
            float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
            half grazingTerm = saturate( gloss + specularMonochrome );
            specular = attenuation * directSpecular * _GlossColor.rgb;
        }

        // オプション:ShadeCap
        if (_ShadowCapBlendMode < 2) {
            float3 normalDirectionShadowCap = normalize(mul( float3(normalLocal.r*_ShadowCapNormalMix,normalLocal.g*_ShadowCapNormalMix,normalLocal.b), tangentTransform )); // Perturbed normals
            float2 transformShadowCap = ComputeTransformCap(cameraSpaceViewDir,normalDirectionShadowCap);
            float4 _ShadowCapTexture_var =  UNITY_SAMPLE_TEX2D_SAMPLER(_ShadowCapTexture, REF_MAINTEX, TRANSFORM_TEX(transformShadowCap, _ShadowCapTexture));
            float4 _ShadowCapBlendMask_var = UNITY_SAMPLE_TEX2D_SAMPLER(_ShadowCapBlendMask, REF_MAINTEX, TRANSFORM_TEX(i.uv0, _ShadowCapBlendMask));
            shadowcap = (1.0 - ((1.0 - (_ShadowCapTexture_var.rgb))*_ShadowCapBlendMask_var.rgb)*_ShadowCapBlend);
        }

        // オプション：MatCap
        if (_MatcapBlendMode < 3) {
            float3 normalDirectionMatcap = normalize(mul( float3(normalLocal.r*_MatcapNormalMix,normalLocal.g*_MatcapNormalMix,normalLocal.b), tangentTransform )); // Perturbed normals
            float2 transformMatcap = ComputeTransformCap(cameraSpaceViewDir,normalDirectionMatcap);
            float4 _MatcapTexture_var = UNITY_SAMPLE_TEX2D_SAMPLER(_MatcapTexture, REF_MAINTEX, TRANSFORM_TEX(transformMatcap, _MatcapTexture));
            float4 _MatcapBlendMask_var = UNITY_SAMPLE_TEX2D_SAMPLER(_MatcapBlendMask, REF_MAINTEX, TRANSFORM_TEX(i.uv0, _MatcapBlendMask));
            float3 matcapResult = ((_MatcapColor.rgb*_MatcapTexture_var.rgb)*_MatcapBlendMask_var.rgb*_MatcapBlend);
            matcap = min(matcapResult, matcapResult * (coloredLight * _MatcapShadeMix));
        }

        // オプション：Rim
        if (_UseRim) {
            float _RimBlendMask_var = UNITY_SAMPLE_TEX2D_SAMPLER(_RimBlendMask, REF_MAINTEX, TRANSFORM_TEX(i.uv0, _RimBlendMask));
            float4 _RimTexture_var = UNITY_SAMPLE_TEX2D_SAMPLER(_RimTexture, REF_MAINTEX, TRANSFORM_TEX(i.uv0, _RimTexture));

            float rimNdotV = abs(dot( normalDirection, viewDirection ));
            float oneMinusRimNdotV = 1 - rimNdotV; // 0:正面 ~ 1:真横
            float value = (oneMinusRimNdotV - _RimBlendStart) / (_RimBlendEnd - _RimBlendStart);
            float rimPow3 = value*value*value;
            float rimPow5 = Pow5(value);
            float valueTotal = min(1, lerp(value, lerp(rimPow3, rimPow5, max(0, _RimPow-1)), min(1,_RimPow)));

            RimLight = (
                    lerp( _RimTexture_var.rgb, Diffuse, _RimUseBaseTexture )
                    * valueTotal
                    * _RimBlend
                    * _RimColor.rgb
                    * _RimBlendMask_var
            );
            RimLight = min(RimLight, RimLight * (coloredLight * _RimShadeMix));
        }
    #if !defined(ARKTOON_REFRACTED) && defined(ARKTOON_OUTLINE)
    }
    #endif

    float3 finalColor = max(toonedMap, RimLight) + specular;

    // ShadeCapのブレンドモード
    if (_ShadowCapBlendMode == 0) { // Darken
        finalColor = min(finalColor, shadowcap);
    } else if  (_ShadowCapBlendMode == 1) { // Multiply
        finalColor = finalColor * shadowcap;
    }

    // MatCapのブレンドモード
    if (_MatcapBlendMode == 0) { // Add
        finalColor = finalColor + matcap;
    } else if (_MatcapBlendMode == 1) { // Lighten
        finalColor = max(finalColor, matcap);
    } else if (_MatcapBlendMode == 2) { // Screen
        finalColor = 1-(1-finalColor) * (1-matcap);
    }

    #ifdef ARKTOON_FADE
        fixed _AlphaMask_var = UNITY_SAMPLE_TEX2D_SAMPLER(_AlphaMask, REF_MAINTEX, TRANSFORM_TEX(i.uv0, _AlphaMask)).r;
        fixed4 finalRGBA = fixed4(finalColor * (_MainTex_var.a * REF_COLOR.a * _AlphaMask_var),0);
    #else
        fixed4 finalRGBA = fixed4(finalColor, 0);
    #endif
    UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
    return finalRGBA;
}