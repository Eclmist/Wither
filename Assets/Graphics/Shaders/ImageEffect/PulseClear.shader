Shader "Hidden/PulseClear"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 ray : TEXCOORD1;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float2 uv_depth : TEXCOORD1;
				float4 interpolatedRay : TEXCOORD2;

			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv.xy;
				o.uv_depth = v.uv.xy;
				o.interpolatedRay = v.ray;

				return o;
			}
			
			sampler2D _MainTex;
			sampler2D_float _CameraDepthTexture;
			float4 _PulsePosition;
			float _PulseDistance;

			fixed4 frag (v2f i) : SV_Target
			{
				float4 col = tex2D(_MainTex, i.uv);

				float rawDepth = DecodeFloatRG(tex2D(_CameraDepthTexture, i.uv_depth));
				float linearDepth = Linear01Depth(rawDepth);
				float4 wsDir = linearDepth * i.interpolatedRay;
				float3 fragmentPosition = _WorldSpaceCameraPos + wsDir;

				float dist = distance(fragmentPosition, _PulsePosition);

				if (dist < _PulseDistance && linearDepth < 1)
				{

					return 1;
				}

				return col;

			}
			ENDCG
		}
	}
}
