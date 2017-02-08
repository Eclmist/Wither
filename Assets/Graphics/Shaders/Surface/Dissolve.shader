Shader "Custom/Dissolve"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MaskTex("Texture", 2D) = "" {}
		_Opacity("Opacity", Range(0,1)) = 1
		_Cutoff("Cutoff", Range(0,1)) = 0

		_ColorRamp("Texture", 2D) = "" {}
		_BurnSize("Burn Size", Range(0,1)) = 0.196

	}
	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha

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

			sampler2D _MainTex;
			sampler2D _MaskTex;

			float _Opacity;
			float _Cutoff;
			float4 _MainTex_ST;
			sampler2D _ColorRamp;
			float _BurnSize;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 mask = tex2D(_MaskTex, i.uv);
				mask.rgb -= 1 - _Opacity - 0.1;
				
				if (mask.r < _Cutoff ) discard;

				// Add burning edge
				fixed4 ramp = tex2D(_ColorRamp, float2(mask.r *(1 / _BurnSize), 0));
				ramp.a *= col.a;

				return col + ramp;
			}
			ENDCG
		}
	}
}
