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
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float2 _Position;
			float _Radius;
			float _DistortionAmount;
			float _AspectRatio;
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				// just invert the colors
				
				float2 fixedDistance = (i.uv * float2(_AspectRatio, 1) -
				_Position * float2(_AspectRatio, 1));


				if (length(fixedDistance) < _Radius)
				{

					col = tex2D(_MainTex, i.uv + float2(_AspectRatio, 1) * 
						lerp(0, _DistortionAmount * fixedDistance,
							sin(length(fixedDistance) / _Radius * 6.28)));

//					return lerp(fixed4(0, 0, 0, 0), fixed4(1, 0, 0, 0), sin(length(fixedDistance) / _Radius * 4) );
				}
				
				return col;
			}
			ENDCG
		}
	}
}
