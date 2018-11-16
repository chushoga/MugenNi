// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/CelShader"
{
    Properties
    {
    	_Color ("Color Tint", Color) = (0,0,0,0)
        _MainTex ("Texture", 2D) = "white" {}
        _Treshold ("Cel Threshold", Range(1., 20.)) = 5.
        _Ambient ("Ambient Intensity", Range(0., 1.0)) = 0.1
        _Border("Border Size", Range(0.0,0.1)) = 0.01
		_BorderColor("Border Color", Color) = (0,0,0,1)
		[Toggle(SPECULAR_ENABLE)] _SpecularEnable("Enable Specular", Int) = 0
		[Toggle(OUTLINE_FRONT)] _OutlineFront("Enable Outline", Int) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "LightMode"="ForwardBase" }
        Cull Off
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : NORMAL;
            };

            float _Treshold;

            float LightToonShading(float3 normal, float3 lightDir)
            {
                float NdotL = max(0.0, dot(normalize(normal), normalize(lightDir)));
                return floor(NdotL * _Treshold) / (_Treshold - 0.5);
            }

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            v2f vert (appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldNormal = mul(v.normal.xyz, (float3x3) unity_WorldToObject);
                return o;
            }

            fixed4 _LightColor0;
            half _Ambient;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb *= saturate(LightToonShading(i.worldNormal, _WorldSpaceLightPos0.xyz) + _Ambient) * _LightColor0.rgb;
                col.rgb *= _Color;
                return col;
            }
            ENDCG
        }
        Pass
		{
			Name "OUTLINE"
			Tags{ "LightMode" = "Always" }
			Cull Front
			ZWrite Off
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#pragma shader_feature OUTLINE_FRONT

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

					struct appdata {
						float4 vertex : POSITION;
						float3 normal : NORMAL;
					};

					struct v2f {
						float4 pos : POSITION;
						float4 color : COLOR;
					};

					float _Border;
					float4 _BorderColor;

					v2f vert(appdata v)
					{
						v2f o;
			#if OUTLINE_FRONT
						o.pos = UnityObjectToClipPos(v.vertex);

						float3 norm = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
						float2 offset = TransformViewToProjection(norm.xy);

						o.pos.xy += offset * UNITY_Z_0_FAR_FROM_CLIPSPACE(o.pos.z) * _Border;
						o.color = _BorderColor;
			#else
						o.pos = float4(0, 0, 0, 0);
						o.color = float4(0, 0, 0, 0);
			#endif
						return o;
					}

					half4 frag(v2f i) :COLOR{ return i.color; }
						ENDCG
		}
    }
    Fallback "Diffuse"
}