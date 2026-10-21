Shader "OpenWaters/Harbor Water"
{
    Properties
    {
        _DeepColor ("Deep lagoon", Color) = (0.025,0.23,0.34,1)
        _ShallowColor ("Turquoise crests", Color) = (0.04,0.57,0.64,1)
        _FoamColor ("Sea glass foam", Color) = (0.72,0.94,0.88,1)
        _Pattern ("Designed ripple and foam field", 2D) = "gray" {}
        _WaveHeight ("Swell amplitude", Range(0,2)) = 1
        _Speed ("Ocean speed (0 for reduced motion)", Range(0,2)) = 1
        _FoamAmount ("Crest foam", Range(0,1)) = 0.36
        _TileSize ("Ocean repeat metres", Float) = 32
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }
        Pass
        {
            Name "HarborWater"
            Tags { "LightMode"="UniversalForward" }
            Cull Back ZWrite On
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_fog
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            TEXTURE2D(_Pattern); SAMPLER(sampler_Pattern);
            CBUFFER_START(UnityPerMaterial)
                float4 _DeepColor, _ShallowColor, _FoamColor, _Pattern_ST;
                float _WaveHeight, _Speed, _FoamAmount, _TileSize;
            CBUFFER_END
            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float crest : TEXCOORD2;
                float fog : TEXCOORD3;
            };
            Varyings Vert(Attributes v)
            {
                Varyings o;
                float3 p = TransformObjectToWorld(v.positionOS.xyz);
                float k = TWO_PI / max(_TileSize, 0.1);
                float t = _Time.y * _Speed * TWO_PI / 4.0;
                float a = dot(p.xz, float2(2,1)) * k + t;
                float b = dot(p.xz, float2(-1,3)) * k - 2*t;
                float c = dot(p.xz, float2(5,2)) * k + 3*t;
                float h = 0.12*sin(a) + 0.055*sin(b) + 0.025*sin(c);
                float2 slope = k * (0.12*cos(a)*float2(2,1) + 0.055*cos(b)*float2(-1,3) + 0.025*cos(c)*float2(5,2));
                p.y += h * _WaveHeight;
                o.positionWS=p; o.positionCS=TransformWorldToHClip(p);
                o.normalWS=normalize(float3(-slope.x*_WaveHeight,1,-slope.y*_WaveHeight));
                o.crest=h/0.2;
                o.fog=ComputeFogFactor(o.positionCS.z);
                return o;
            }
            half4 Frag(Varyings i) : SV_Target
            {
                float t = _Time.y * _Speed;
                float2 uv=i.positionWS.xz/max(_TileSize,0.1);
                float4 field=SAMPLE_TEXTURE2D(_Pattern,sampler_Pattern,uv*3+float2(t/96,-t/192));
                float detail=SAMPLE_TEXTURE2D(_Pattern,sampler_Pattern,uv*5+float2(-t/160,t/128)).g;
                float water=saturate(0.42+i.crest*.3+field.r*.22);
                float3 col=lerp(_DeepColor.rgb,_ShallowColor.rgb,water);
                float foam=smoothstep(0.77,0.96,field.b)*smoothstep(-.3,.8,i.crest+detail*.35)*_FoamAmount;
                col=lerp(col,_FoamColor.rgb,foam);
                Light light=GetMainLight();
                float3 n=normalize(i.normalWS+float3((field.r-.5)*.045,0,(detail-.5)*.045));
                float3 view=GetWorldSpaceNormalizeViewDir(i.positionWS);
                float sheen=pow(saturate(dot(n,normalize(light.direction+view))),64)*.12;
                float fresnel=pow(1-saturate(dot(n,view)),4);
                col*=.78+.22*saturate(dot(n,light.direction));
                col+=light.color*sheen+_ShallowColor.rgb*fresnel*.2;
                return half4(MixFog(col,i.fog),1);
            }
            ENDHLSL
        }
    }
    FallBack Off
}

