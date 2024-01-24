// SDF Functions taken from https://iquilezles.org/articles/distfunctions/
Shader "DoubleBrackets/SphereTraceSDF"
{
    Properties
    {
        _MaxSteps ("Max Steps", Integer) = 1
        _DrawDistThresh ("Threshold Dist", Float) = 0.01
        _Morph ("Morph value", Range(0.0, 1.0)) = 1
        _Smoothness ("Smoothness", Range(0.0, 1.0)) = 1
        _RepeatSpacing ("Repeat Spacing", Float) = 10
        _DistFade ("Distance Fade", Float) = 10

        _Shape1Pos ("Shape 1 Pos", Vector) = (.25, .5, .5, 1)
        _Shape2Pos ("Shape 2 Pos", Vector) = (.25, .5, .5, 1)
    }

    SubShader
    {
        // SubShader Tags define when and under which conditions a SubShader block or
        // a pass is executed.
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }


        Cull Off

        Pass
        {

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float _MaxSteps;
            float _DrawDistThresh;
            float _Morph;
            float _Smoothness;
            float _RepeatSpacing;
            float _DistFade;

            float4 _Shape1Pos;
            float4 _Shape2Pos;

            // SDF functions
            float sdRoundBox(float3 p, float3 b, float r)
            {
                float3 q = abs(p) - b;
                return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0) - r;
            }

            float sdTorus(float3 p, float2 t )
            {
              float2 q = float2(length(p.xz)-t.x,p.y);
              return length(q)-t.y;
            }

            float opSmoothUnion( float d1, float d2, float k )
            {
                float h = clamp( 0.5 + 0.5*(d2-d1)/k, 0.0, 1.0 );
                return lerp( d2, d1, h ) - k*h*(1.0-h);
            }

            // Blend between these two sets of shapes
            float morph1SDF(float3 p)
            {
                float obj1 = sdTorus(p - _Shape1Pos.xyz, _Shape1Pos.w);
                float obj2 = sdRoundBox(p - _Shape2Pos.xyz, float3(1,2,1), 0.5);
                return opSmoothUnion(obj1, obj2, _Smoothness);
            }

            float morph2SDF(float3 p)
            {
                float obj3 = sdRoundBox(p - _Shape1Pos.xyz, float3(0.5,1,0.5), 0.25);
                float obj4 = sdTorus(p - _Shape2Pos.xyz, _Shape1Pos.w);
                return opSmoothUnion(obj4, obj3, _Smoothness);
            }


            float calcSDF(float3 p)
            {
                // Repeat
                p = p - _RepeatSpacing * round(p / _RepeatSpacing);
 
                return lerp(morph1SDF(p), morph2SDF(p) , _Morph);
            }

            struct Attributes
            {
                float4 positionOS   : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float3 viewDir = normalize(IN.worldPos - _WorldSpaceCameraPos.xyz);
                float3 currentPos = IN.worldPos;
                float3 p = currentPos;
                
                // Sphere trace steps
                for(int i = 0;i < _MaxSteps;i++)
                {
                    float sdf = calcSDF(p);
                    
                    if(sdf <= _DrawDistThresh)
                    {
                        float depth = distance(_WorldSpaceCameraPos.xyz, p);
                        float c = 1 - saturate(depth/_DistFade);
                        return half4(c, c, c, 1);
                    }

                    p += viewDir * sdf;
                }
                discard;
                return half4(0,0,0,0);
            }
            ENDHLSL
        }
    }
}