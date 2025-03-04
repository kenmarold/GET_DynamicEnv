Shader "Custom/URP_Dissolve"
{
Properties
{
_Color ("Color", Color) = (1,1,1,1)
[HDR]_Emission ("Emission", Color) = (0,0,0,0)
_MainTex ("Albedo", 2D) = "white" {}
_Normal ("Normal", 2D) = "bump" {}
_MetallicSmooth ("Metallic (RGB) Smooth (A)", 2D) = "white" {}
_AO ("AO", 2D) = "white" {}
[HDR]_EdgeColor1 ("Edge Color", Color) = (1,1,1,1)
_Noise ("Noise", 2D) = "white" {}
[Toggle] _Use_Gradient ("Use Gradient?", Float) = 1
_Gradient ("Gradient", 2D) = "white" {}
_Glossiness ("Smoothness", Range(0,1)) = 0.5
_Metallic ("Metallic", Range(0,1)) = 0.0
_Cutoff ("Cutoff", Range(0,1)) = 0.0
_EdgeSize ("EdgeSize", Range(0,1)) = 0.2
_NoiseStrength ("Noise Strength", Range(0,1)) = 0.4
_DisplaceAmount ("Displace Amount", Range(0,1)) = 0.4
}
SubShader
{
    Tags { "RenderPipeline"="UniversalPipeline" "Queue"="AlphaTest" "RenderType"="TransparentCutout" "IgnoreProjector"="True" }
    Cull Off
    LOD 200

    Pass
    {
        Name "ForwardLit"
        Tags { "LightMode"="UniversalForward" }

        HLSLPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma shader_feature _USE_GRADIENT
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceData.hlsl"

        struct Attributes
        {
            float4 positionOS : POSITION;
            float2 uv : TEXCOORD0;
            float3 normalOS : NORMAL;
        };

        struct Varyings
        {
            float2 uv : TEXCOORD0;
            float3 worldPos : TEXCOORD1;
            float3 worldNormal : TEXCOORD2;
            float3 viewDir : TEXCOORD3;
            float4 positionCS : SV_POSITION;
        };

        sampler2D _MainTex, _Noise, _Gradient, _Normal, _MetallicSmooth, _AO;
        half _Glossiness, _Metallic, _Cutoff, _EdgeSize, _NoiseStrength, _DisplaceAmount;
        half4 _Color, _EdgeColor1, _Emission;
        
        Varyings vert(Attributes IN)
        {
            Varyings OUT;
            OUT.uv = IN.uv;
            OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
            OUT.worldNormal = TransformObjectToWorldNormal(IN.normalOS);
            OUT.viewDir = normalize(GetWorldSpaceViewDir(OUT.worldPos));
            OUT.positionCS = TransformObjectToHClip(IN.positionOS);
            return OUT;
        }

        half4 frag(Varyings IN) : SV_Target
        {
            half3 Noise = tex2D(_Noise, IN.uv).rgb;
            half4 MetallicSmooth = tex2D(_MetallicSmooth, IN.uv);
            half3 Gradient = tex2D(_Gradient, IN.uv).rgb;
            
            #ifdef _USE_GRADIENT
            half Edge = smoothstep(_Cutoff, _Cutoff - _EdgeSize, 1 - (Gradient + Noise.r * (1 - Gradient) * _NoiseStrength));
            #else
            half Edge = smoothstep(_Cutoff, _Cutoff - _EdgeSize, 1 - Noise.r);
            #endif
            
            half4 albedoColor = tex2D(_MainTex, IN.uv) * _Color;
            half3 EmissiveCol = albedoColor.a * _Emission.rgb;
            
            SurfaceData surface = (SurfaceData)0;
            surface.albedo = albedoColor.rgb;
            surface.metallic = MetallicSmooth.r * _Metallic;
            surface.smoothness = MetallicSmooth.a * _Glossiness;
            surface.specular = 0;
            surface.normalTS = UnpackNormal(tex2D(_Normal, IN.uv));
            surface.emission = EmissiveCol + _EdgeColor1.rgb * Edge;
            surface.occlusion = tex2D(_AO, IN.uv).r;
            surface.alpha = albedoColor.a;
            
            InputData inputData = (InputData)0;
            inputData.positionWS = IN.worldPos;
            inputData.normalWS = normalize(IN.worldNormal);
            inputData.viewDirectionWS = normalize(IN.viewDir);
            
            clip(1 - Edge - _Cutoff);
            return UniversalFragmentPBR(inputData, surface);
        }
        ENDHLSL
    }
}

FallBack "Hidden/InternalErrorShader"
}
 