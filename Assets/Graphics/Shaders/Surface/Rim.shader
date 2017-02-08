// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Stencils/Rim"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		_RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
		_RimPower("Rim Power", Range(0.5,8.0)) = 3.0
		_Opacity("Opacity", Range(0,1)) = 1


	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		//Blend One One

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
				float2 uv : TEXCOORD2;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 normalDir : TEXCOORD1;
				float3 viewDir : TEXCOORD0;
				float2 uv : TEXCOORD2;
				float4 projPos : TEXCOORD3;
			};

			sampler2D _MainTex;
			uniform sampler2D _PUDepthTex;
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
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.projPos = ComputeScreenPos(o.pos);
				COMPUTE_EYEDEPTH(o.projPos.z);
				return o;
			}
			//TODO : fix depth culling
			fixed4 frag (v2f i) : SV_Target
			{
				float sceneZ = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_PUDepthTex, UNITY_PROJ_COORD(i.projPos))));

				//return i.projPos.z;

				
				if (i.projPos.z > sceneZ) discard;

				float ndotv = 1 - dot(i.normalDir, i.viewDir);
				return _RimColor * pow (ndotv, _RimPower) * _Opacity + tex2D(_MainTex, i.uv);
			}
			ENDCG
		}
	}
}
