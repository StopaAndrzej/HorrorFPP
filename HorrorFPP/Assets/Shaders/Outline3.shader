Shader "squidzoo/Outline"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_OutlineColor("Outline color", Color) = (0,0,0,1)
		_OutlineWidth("Outline width", Range(1.0,5.0)) = 1.05
	}

		CGINCLUDE
#include "UnityCG.cginc"

			struct appdata
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
		};

		struct v2f
		{
			float4 pos : POSITION;
			float3 normal : NORMAL;
		};

		float _OutlineWidth;
		float4 _OutlineColor;

		ENDCG

			SubShader
		{

			Pass
			{
				Zwrite Off

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				v2f vert(appdata v)
				{
					v.vertex.xyz *= _OutlineWidth;
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					return o;
				}

				half4 frag(v2f i) : COLOR
				{
					return _OutlineColor;
				}

				ENDCG
			}

			Pass
			{
				CGPROGRAM
				#pragma vertex vertPassTwo
				#pragma fragment fragPassTwo
				#pragma multi_compile_fog

				#include "UnityCG.cginc"

				struct appdataPassTwo
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2fPassTwo
				{
					float2 uv : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;

				v2fPassTwo vertPassTwo(appdataPassTwo v)
				{
					v2fPassTwo o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 fragPassTwo(v2fPassTwo i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.uv);
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG
			}
		}
}