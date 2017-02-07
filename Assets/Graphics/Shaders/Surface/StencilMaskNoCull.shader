﻿Shader "Stencils/StencilMaskNoCull"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "green" {}
		_Color("Color", Color) = (1,1,1,1)

	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue"="Geometry-100" }
		LOD 100

		Pass
		{
			Stencil
			{
				Ref 1
				Comp always
				Pass replace
				Fail replace
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
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
				
				if (col.a == 0) discard;
				col.a = col.rgb;
				col = (col + _Color) / 2;

				return col;
			}
			ENDCG
		}
	}
}
