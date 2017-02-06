Shader "Hidden/SSDistortion"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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
				o.uv = v.uv;
				o.uv_depth = v.uv.xy;
				o.interpolatedRay = v.ray;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _PUDepthTex;
			float3 _Position;
			float _Radius;
			float _Width;
			float _DistortionAmount;


			fixed4 frag (v2f i) : SV_Target
			{
				float4 col = tex2D(_MainTex, i.uv);

				float rawDepth = tex2D(_PUDepthTex, i.uv_depth);
				float linearDepth = Linear01Depth(rawDepth);
				float4 wsDir = linearDepth * i.interpolatedRay;
				float3 fragmentPosition = _WorldSpaceCameraPos + wsDir;

				float3 dir = fragmentPosition - _Position;
				float dist = length(dir);


				if (dist < _Radius && dist > _Radius - _Width )
				{
					//col = tex2D(_MainTex, i.uv +
					//	dir * _DistortionAmount *
					//dist - _Radius);

					float diff = (_Radius - dist);

					float fadeInner = lerp(1, 0, diff / _Width);
					float fadeOuter = lerp(0, 1, diff / _Width);

					float DistortionIntensity = fadeOuter * fadeInner;

					col = tex2D(_MainTex, i.uv +
						dir * -_DistortionAmount *
						DistortionIntensity);

				}
				
				return col;
			}
			ENDCG
		}
	}
}
