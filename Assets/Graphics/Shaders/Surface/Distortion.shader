Shader "Unlit/Distortion"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Pattern("Pattern", 2D) = "white" {}
		_Speed("Speed", Range(-1,1)) = 0
		_DistortionAmt("Distortion Amount", Range(0,1)) = 0
		_Opacity("Base Opacity", Range(0,1)) = 0
		
		_IntersectionColor("IntersectionColor", Color) = (1,1,1,1)
		_InvFade("Intersection Factor", Range(0.01,1)) = 1.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		//Cull Off

		GrabPass
		{
			"_BackgroundTexture"
		}


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
				float4 projPos : TEXCOORD1;
				float4 grabPos : TEXCOORD2;
			};

			sampler2D _MainTex;
			sampler2D _PUDepthTex;
			sampler2D _Pattern;
			sampler2D _BackgroundTexture;
			float4 _IntersectionColor;
			float4 _MainTex_ST;
			float _Opacity;
			float _InvFade;
			float _DistortionAmt;
			float _Speed;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.projPos = ComputeScreenPos(o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.grabPos = ComputeGrabScreenPos(o.vertex);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{

				//Distortion
				float4 distortion = tex2D(_Pattern, i.uv +
					(_Time.rg * _Speed));
				float4 distortedTex = tex2Dproj(_BackgroundTexture, i.grabPos + distortion * _DistortionAmt);
				distortedTex.a = 1;

				//Intersection Highlight
				float sceneZ = LinearEyeDepth(
					UNITY_SAMPLE_DEPTH(tex2Dproj(_PUDepthTex, UNITY_PROJ_COORD(i.projPos)))
				);
				float fragZ = i.projPos.z;
				float fade = saturate(_InvFade* (sceneZ - fragZ));

				if (sceneZ < fragZ) discard;

				fixed4 intersectColor = _IntersectionColor *
					tex2D(_Pattern, i.uv + _Time.rg * _Speed);
				intersectColor.a *= (1 - fade);

				distortedTex.rgb += intersectColor.rgb * intersectColor.a;


				return distortedTex;

			}
			ENDCG
		}
	}
}
