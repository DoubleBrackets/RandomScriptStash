// Based on https://www.ronja-tutorials.com/post/010-triplanar-mapping/
Shader "Example/BasicTriplanar"
{
    Properties
    {
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        _TintColor("Tint", Color) = (1, 1, 1, 1)
        _Sharpness("Blend Sharpness", float) = 1
        _Offset("Blend Offset", float) = 0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 normal : NORMAL;
            };

            // This macro declares _BaseMap as a Texture2D object.
            TEXTURE2D(_BaseMap);
            // This macro declares the sampler for the _BaseMap texture.
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _TintColor;
                float _Sharpness;
                float _Offset;
            CBUFFER_END


            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.worldPos = mul(unity_ObjectToWorld, IN.positionOS);

                // Calculate normal in world space
                float3 worldNormal = TransformObjectToWorldNormal(IN.normalOS);
                OUT.normal = normalize(worldNormal);
                
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                //calculate UV coordinates for three projections
                float2 uv_front = TRANSFORM_TEX(IN.worldPos.xy, _BaseMap);
                float2 uv_side = TRANSFORM_TEX(IN.worldPos.zy, _BaseMap);
                float2 uv_top = TRANSFORM_TEX(IN.worldPos.xz, _BaseMap);

                // Prevent mirroring on opposite sides
                if(IN.normal.x < 0)
                {
                    uv_side.x *= -1;
                }
                if(IN.normal.y < 0)
                {
                    uv_top.x *= -1;
                }
                if(IN.normal.z >= 0)
                {
                    uv_front.x *= -1;
                }
                
                //read texture at uv position of the three projections
                float4 col_front = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv_front);
                float4 col_side = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv_side);
                float4 col_top = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv_top);
                
                float3 weights = IN.normal;
                weights = abs(weights);

                // Offset weights
                weights += float3(_Offset, _Offset, _Offset);
                weights = saturate(weights);

                // Raise weights to increase sharpness (reduce soft blending)
                weights = pow(weights, _Sharpness);

                // Scale so weights add to 1
                weights = weights / (weights.x + weights.y + weights.z);

                col_front *= weights.z;
                col_side *= weights.x;
                col_top *= weights.y;

                //combine the projected colors
                float4 col = col_front + col_side + col_top;

                //multiply texture color with tint color
                col *= _TintColor;
                return col;
            }
            ENDHLSL
        }
    }
}