Shader "OceanShader/OceanLow" {
	Properties {
		_WaterTex ("Normal Map (RGB), Foam (A)", 2D) = "white" {}
		_WaterTex2 ("Normal Map (RGB), Foam (B)", 2D) = "white" {}
		_Tiling ("Wave Scale", Range(0.00025, 0.01)) = 0.25
		_WaveSpeed ("Wave Speed", Float) = 0.4
		_ReflectionIntensity ("Reflection Intensity", Range(0, 1)) = 0.1
		_SpecularRatio ("Specular Ratio", Range(10, 500)) = 200
		_BottomColor ("Bottom Color", Color) = (0,0,0,0)
		_TopColor ("Top Color", Color) = (0,0,0,0)
	}
	SubShader {
		LOD 250
		Tags { "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "FORWARDBASE" "QUEUE" = "Transparent-200" "RenderType" = "Transparent" }
		Pass {
			LOD 250
			Tags { "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "FORWARDBASE" "QUEUE" = "Transparent-200" "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			Lighting On
			GpuProgramID 1950
			// No subprograms found
		}
	}
	SubShader {
		LOD 200
		Tags { "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "FORWARDBASE" "QUEUE" = "Geometry" "RenderType" = "Opaque" }
		Pass {
			LOD 200
			Tags { "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "FORWARDBASE" "QUEUE" = "Geometry" "RenderType" = "Opaque" }
			Lighting On
			GpuProgramID 88058
			// No subprograms found
		}
	}
	SubShader {
		LOD 150
		Tags { "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "FORWARDBASE" "QUEUE" = "Transparent-200" "RenderType" = "Transparent" }
		Pass {
			LOD 150
			Tags { "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "FORWARDBASE" "QUEUE" = "Transparent-200" "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			Lighting On
			GpuProgramID 157153
			// No subprograms found
		}
	}
	SubShader {
		LOD 100
		Tags { "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "FORWARDBASE" "QUEUE" = "Geometry" "RenderType" = "Opaque" }
		Pass {
			LOD 100
			Tags { "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "FORWARDBASE" "QUEUE" = "Geometry" "RenderType" = "Opaque" }
			Lighting On
			GpuProgramID 255363
			// No subprograms found
		}
	}
	Fallback "Diffuse"
}