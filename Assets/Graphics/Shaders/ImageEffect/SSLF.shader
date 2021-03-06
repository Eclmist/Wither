﻿Shader "Hidden/SSLF"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "black" {}

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
			sampler2D _LensFlare;
			uniform sampler2D _PUDepthTex;
			float2 _sPosition;
			float _size;
			float _opacity;
			float _sDepth;

			half4 _color;

			fixed4 frag (v2f i) : SV_Target
			{
				half r = 0.003;
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 lens = tex2D(_LensFlare, ((i.uv) - (_sPosition)) / _size + 0.5);
				lens.a = 0.5;

				fixed depth = tex2D(_PUDepthTex, _sPosition).r;
				fixed depthTL = tex2D(_PUDepthTex, _sPosition + half2(-r, -r)).r;
				fixed depthTR = tex2D(_PUDepthTex, _sPosition + half2(r, -r)).r;
				fixed depthBL = tex2D(_PUDepthTex, _sPosition + half2(-r, r)).r;
				fixed depthBR = tex2D(_PUDepthTex, _sPosition + half2(r, r)).r;

				float linearDepth = LinearEyeDepth(depth);
				float linearDepthTL = LinearEyeDepth(depthTL);
				float linearDepthTR = LinearEyeDepth(depthTR);
				float linearDepthBL = LinearEyeDepth(depthBL);
				float linearDepthBR = LinearEyeDepth(depthBR);

				fixed oMul = 1;

				oMul -= (sign(_sDepth - linearDepth)) * 0.2;
				oMul -= (sign(_sDepth - linearDepthTL)) * 0.2;
				oMul -= (sign(_sDepth - linearDepthTR)) * 0.2;
				oMul -= (sign(_sDepth - linearDepthBL)) * 0.2;
				oMul -= (sign(_sDepth - linearDepthBR)) * 0.2;

				//col = LinearEyeDepth(tex2D(_PUDepthTex, i.uv).r) / 100;
				//if (length(i.uv - _sPosition) < 0.01) return _sDepth / 100;
				//return (tex2D(_PUDepthTex,i.uv)) * 5;
				return col + lens * _opacity *oMul * _color;
			}
			ENDCG
		}
	}
}
