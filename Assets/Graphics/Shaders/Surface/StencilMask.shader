Shader "Stencils/StencilMask"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		//_InvFade("Soft Particles Factor", Range(0.01,3.0)) = 1.0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Geometry-100" }
		LOD 100

		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			Stencil
			{
				Ref 1
				Comp always
				Pass replace
				Fail replace
				ZFail keep
			}

			ZWrite Off
			ColorMask 0	
			Offset -50, -100

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
				//float4 projPos : TEXCOORD1;
			};

			sampler2D _MainTex;
			sampler2D _PUDepthTex;
			float4 _MainTex_ST;
			float _InvFade;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				//o.projPos = ComputeScreenPos(o.vertex);
				//COMPUTE_EYEDEPTH(o.projPos.z);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);

				//float sceneZ = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_PUDepthTex, UNITY_PROJ_COORD(i.projPos))));
				//float partZ = i.projPos.z;
				//float fade = saturate(_InvFade * (sceneZ - partZ));
				//col.a *= fade;

				if (col.a == 0) discard;
				
				return col;
			}
			ENDCG
		}
	}
}
