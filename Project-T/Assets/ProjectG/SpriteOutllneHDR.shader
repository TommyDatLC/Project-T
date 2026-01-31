Shader "Custom/SpriteOutlineOnlyHDR"
{
    Properties
    {
        [MainTexture] _MainTex ("Sprite Texture", 2D) = "white" {}
        [HDR] _OutlineColor ("Outline Color", Color) = (1,1,1,1) 
        _OutlineSize ("Outline Thickness", Range(0, 10)) = 1
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Name "SpriteOutlineOnly"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR; 
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize; 

            CBUFFER_START(UnityPerMaterial)
                float4 _OutlineColor;
                float _OutlineSize;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.color = input.color;
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                // Lấy màu tại pixel hiện tại (chủ yếu để lấy Alpha)
                float4 mainColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                
                // Tính toán kích thước pixel
                float2 texelSize = _MainTex_TexelSize.xy * _OutlineSize;

                // Lấy mẫu Alpha xung quanh
                float alphaUp    = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + float2(0, texelSize.y)).a;
                float alphaDown  = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv - float2(0, texelSize.y)).a;
                float alphaLeft  = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv - float2(texelSize.x, 0)).a;
                float alphaRight = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + float2(texelSize.x, 0)).a;

                // Tổng hợp alpha các vùng lân cận
                float combinedAlpha = saturate(alphaUp + alphaDown + alphaLeft + alphaRight);

                // --- LOGIC QUAN TRỌNG ĐÃ THAY ĐỔI ---
                
                // Mask outline: 
                // combinedAlpha cao (có pixel bên cạnh) TRỪ ĐI mainColor.a (pixel hiện tại).
                // Kết quả:
                // - Nếu là vùng bên ngoài sát cạnh: combinedAlpha(1) - mainColor.a(0) = 1 (VẼ VIỀN)
                // - Nếu là vùng bên trong hình: combinedAlpha(1) - mainColor.a(1) = 0 (KHÔNG VẼ/TRONG SUỐT)
                // - Nếu là vùng xa bên ngoài: combinedAlpha(0) - mainColor.a(0) = 0 (KHÔNG VẼ)
                
                float outlineMask = saturate(combinedAlpha - mainColor.a);
                
                // Khởi tạo màu đầu ra là màu Outline
                float4 finalColor = _OutlineColor;
                
                // Gán Alpha của màu đầu ra bằng đúng cái Mask vừa tính
                // Điều này khiến phần bên trong trở nên trong suốt
                finalColor.a *= outlineMask;

                return finalColor;
            }
            ENDHLSL
        }
    }
}