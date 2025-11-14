Shader "Custom/SH_Bubble_base"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.8, 0.9, 1.0, 0.3)
        _EdgeColor ("Edge Color", Color) = (1.0, 1.0, 1.0, 0.8)
        _FresnelPower ("Fresnel Power", Range(0.1, 10)) = 3.0
        _FresnelIntensity ("Fresnel Intensity", Range(0, 2)) = 1.0
        _RefractionStrength ("Refraction Strength", Range(0, 0.2)) = 0.05
        _SpecularColor ("Specular Color", Color) = (1,1,1,1)
        _SpecularSize ("Specular Size", Range(10, 500)) = 150.0
        _SpecularIntensity ("Specular Intensity", Range(0, 5)) = 2.0
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _NoiseStrength ("Noise Strength", Range(0, 0.1)) = 0.02
        _NoiseSpeed ("Noise Speed", Range(0, 2)) = 0.5

        // Inner Properties
        _InnerColor ("Inner Color", Color) = (1.0, 0.4, 1.0, 0.9)
        _InnerColorIntensity ("Inner Color Intensity", Range(0, 3)) = 1.5
        _ColorDensity ("Color Density", Range(0, 1)) = 0.7
        _InnerMovement ("Inner Movement", Range(0, 2)) = 0.5
        _ColorChangeSpeed ("Color Change Speed", Range(0, 2)) = 0.5
        _ColorVariation ("Color Variation", Range(0, 0.5)) = 0.2
        _FlowStrength ("Flow Strength", Range(0, 1)) = 0.3
        _FlowSpeed ("Flow Speed", Range(0, 2)) = 1.0

        _AlphaMultiplier ("Alpha Multiplier", Range(0, 2)) = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend One Zero
        ZWrite On
        Cull Back

        Pass
        {
            Name "BubblePass"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS    : TEXCOORD0;
                float3 viewDirWS   : TEXCOORD1;
                float4 screenPos   : TEXCOORD2;
                float2 uv          : TEXCOORD3;
            };

            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _EdgeColor;
                float _FresnelPower;
                float _FresnelIntensity;
                float _RefractionStrength;
                half4 _SpecularColor;
                float _SpecularSize;
                float _SpecularIntensity;
                float4 _NoiseTex_ST;
                float _NoiseStrength;
                float _NoiseSpeed;

                // Inner Properties
                half4 _InnerColor;
                float _InnerColorIntensity;
                float _ColorDensity;
                float _InnerMovement;
                float _ColorChangeSpeed;
                float _ColorVariation;
                float _FlowStrength;
                float _FlowSpeed;

                float _AlphaMultiplier;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);

                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.viewDirWS = GetWorldSpaceNormalizeViewDir(positionWS);

                OUT.screenPos = ComputeScreenPos(OUT.positionHCS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _NoiseTex);

                return OUT;
            }

            float CalculateInnerPattern(float2 flowUV)
            {
                float2 innerNoiseUV1 = flowUV * 3.0  + _Time.y * _InnerMovement * 0.1;
                float2 innerNoiseUV2 = flowUV * 1.5 + _Time.y * _InnerMovement * 0.3 + float2(0.2, 0.5);

                float innerPattern1 = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, innerNoiseUV1).r;
                float innerPattern2 = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, innerNoiseUV2).g;

                return (innerPattern1 * 0.7 + innerPattern2 * 0.3);
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 noiseUV = IN.uv + _Time.y * _NoiseSpeed * 0.1;
                float surfaceNoise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, noiseUV).r;

                float3 perturbedNormal =
                    normalize(IN.normalWS + (surfaceNoise - 0.5) * _NoiseStrength * 0.2);

                float2 screenUV = IN.screenPos.xy / IN.screenPos.w;

                float ndotv = saturate(dot(perturbedNormal, IN.viewDirWS));
                float fresnel = pow(1.0 - ndotv, _FresnelPower) * _FresnelIntensity;
                fresnel = saturate(fresnel);

                float refStrength01 = saturate(_RefractionStrength * 10.0);
                float2 refractionOffset = perturbedNormal.xz * _RefractionStrength * fresnel;
                float2 refractUV = screenUV + refractionOffset;

                half3 flatBg =
                    SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV).rgb;
              
                half3 refractedBg =
                    SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, refractUV).rgb;

                float refMix = refStrength01 * fresnel; 
                half3 bgInside = lerp(flatBg, refractedBg, refMix);

                half3 bubbleColor = lerp(_BaseColor.rgb, _EdgeColor.rgb, fresnel);

                float3 lightDir = normalize(float3(0.3, 1.0, 0.2));
                float3 halfVector = normalize(lightDir + IN.viewDirWS);
                float specular = pow(saturate(dot(perturbedNormal, halfVector)), _SpecularSize);
                specular *= _SpecularIntensity * fresnel;

                float2 flowUV = IN.uv;
                flowUV.x += sin(IN.uv.y * 5.0 + _Time.y * _FlowSpeed)      * _FlowStrength * 0.1;
                flowUV.y += cos(IN.uv.x * 5.0 + _Time.y * _FlowSpeed * 0.7) * _FlowStrength * 0.1;

                float innerPattern = CalculateInnerPattern(flowUV);
                innerPattern = saturate(innerPattern);

                float centerMask = pow(1.0 - fresnel, 2.0); 
                float innerMask  = centerMask * _ColorDensity;

                float3 innerColor =
                    _InnerColor.rgb * innerPattern * _InnerColorIntensity * innerMask;

                half3 contentColor = bubbleColor * 0.8 + innerColor * 1.2;

                float bgFactorCenter = 0.05; 
                float bgFactorEdge   = 0.25; 
                float bgFactor = lerp(bgFactorEdge, bgFactorCenter, centerMask);

                bgFactor = saturate(bgFactor * (2.0 - _AlphaMultiplier)); 

                float contentFactor = 1.0 - bgFactor;

                half3 finalColor = bgInside * bgFactor + contentColor * contentFactor;

                finalColor += specular * _SpecularColor.rgb;

                return half4(finalColor, 1.0);

            }

            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Unlit"
}
