Shader "Stencils/StandardMask" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		_RimColor("Rim Color", Color) = (1,1,1,1)
		_RimPower("Rim Power", Range(0.5,8)) = 0
		_Opacity("Rim Opacity", Range(0,1)) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Stencil
		{
			Ref 1
			Comp always
			Pass replace
			ZFail keep
		}


		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
			float3 normalDir;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG

		ZTest Greater

		Stencil
		{
			Ref 1
			Comp NotEqual
			Pass replace
		}

		CGPROGRAM
		#pragma surface surf Standard vertex:vert alpha:fade
		#pragma target 3.0
		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
			float3 normalDir;
		};

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.normalDir = UnityObjectToWorldNormal(v.normal);
			o.viewDir = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, v.vertex).xyz);
		}
		sampler2D _MainTex;
		float4 _RimColor;
		float _RimPower;
		float _Opacity;

		void surf(Input IN, inout SurfaceOutputStandard o) {
			float ndotv = 1 - dot(IN.normalDir, IN.viewDir);

			o.Emission = _RimColor * pow(ndotv, _RimPower) * _Opacity;
			o.Alpha = _Opacity;
		}

		ENDCG

	}
	FallBack "Diffuse"
}
