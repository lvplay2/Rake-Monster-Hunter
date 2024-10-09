Shader "OceanShader/OceanLow" {
	Properties {
		_WaterTex ("Normal Map (RGB), Foam (A)", 2D) = "white" {}
		_WaterTex2 ("Normal Map (RGB), Foam (B)", 2D) = "white" {}
		_Tiling ("Wave Scale", Range(0.00025, 0.01)) = 0.25
		_WaveSpeed ("Wave Speed", Float) = 0.4
		_ReflectionIntensity ("Reflection Intensity", Range(0, 1)) = 0.1
		_SpecularRatio ("Specular Ratio", Range(10, 500)) = 200
		_BottomColor ("Bottom Color", Vector) = (0,0,0,0)
		_TopColor ("Top Color", Vector) = (0,0,0,0)
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
		}
		ENDCG
	}
	Fallback "Diffuse"
}