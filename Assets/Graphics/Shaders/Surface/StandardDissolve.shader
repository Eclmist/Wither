Shader "Custom/StandardDissolve" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_MaskTex("Dissolve Pattern", 2D) = "" {}
		_Opacity("Opacity", Range(0,1)) = 1
		_Cutoff("Cutoff", Range(0,1)) = 0

		_ColorRamp("Color Ramp", 2D) = "" {}
		_BurnSize("Burn Size", Range(0,1)) = 0.196

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Cull off

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard addshadow 

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		sampler2D _MaskTex;


		float _Opacity;
		float _Cutoff;

		sampler2D _ColorRamp;
		float _BurnSize;


		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color

			fixed4 mask = tex2D(_MaskTex, IN.uv_MainTex);
			mask.rgb -= 1 - _Opacity - 0.1;
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

			if (mask.r < _Cutoff) discard;
			// Add burning edge
			fixed4 ramp = tex2D(_ColorRamp, float2(mask.r *(1 / _BurnSize), 0));

			o.Albedo = c.rgb + ramp.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
