// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Stencils/XRay"
{
	Properties
	{
		_RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
		_RimPower("Rim Power", Range(0.5,8.0)) = 3.0
		_Opacity("Opacity", Range(0,1)) = 1


	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Blend One One
		ZTest always

		//Stencil
		//{
		//	Ref 1
		//	Comp always
		//	Pass replace
		//}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 normalDir : TEXCOORD1;
				float3 viewDir : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _RimColor;
			float _RimPower;
			float _Opacity;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.normalDir = UnityObjectToWorldNormal(v.normal);
				o.viewDir = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, v.vertex).xyz); 
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{

				float ndotv = 1 - dot(i.normalDir, i.viewDir);
				return _RimColor * pow (ndotv, _RimPower) * _Opacity;
			}
			ENDCG
		}
	}
}
