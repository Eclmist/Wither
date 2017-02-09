Shader "Particles/Distortion"
{
	Properties
	{
		_MainTex ("Mask", 2D) = "white" {}
		_Pattern("Pattern", 2D) = "white" {}
		_Speed("Speed", Range(-1,1)) = 0
		_DistortionAmt("Distortion Amount", Range(0,1)) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" }
		LOD 100
		ZWrite Off
		Cull Off

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
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float4 projPos : TEXCOORD1;
				float4 grabPos : TEXCOORD2;
			};

			sampler2D _MainTex;
			sampler2D _PUDepthTex;
			sampler2D _Pattern;
			sampler2D _BackgroundTexture;
			float4 _MainTex_ST;
			float _DistortionAmt;
			float _Speed;
			v2f vert (appdata v)
			{
				v2f o;
				o.color = v.color;
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

				distortion *= tex2D(_MainTex, i.uv) * i.color.a;
				distortion *= tex2D(_MainTex, i.uv).r;

				float4 distortedTex = tex2Dproj(_BackgroundTexture, i.grabPos + distortion * _DistortionAmt);
				distortedTex.a = 1;


				return distortedTex;

			}
			ENDCG
		}
	}
}
