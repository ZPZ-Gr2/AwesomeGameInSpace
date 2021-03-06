﻿Shader "Custom/ExplosionMark" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "Queue"="Transparent-100" "RenderType"="Transparent" }
		LOD 200
		ZWrite Off
		CGPROGRAM
		#pragma surface surf Lambert alpha

		sampler2D _MainTex;
		sampler2D _CameraDepthTexture;

		struct Input {
			float2 uv_MainTex;
			float4 screenPos;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			float depth = DECODE_EYEDEPTH(tex2Dproj(_CameraDepthTexture, IN.screenPos).r);
			float sceneZ = IN.screenPos.z + _ProjectionParams.y;
			float transparency = saturate(1 - (depth - sceneZ) * 10);
			float4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a * transparency;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
