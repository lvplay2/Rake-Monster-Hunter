Shader "OceanShader/OceanShoreLine" {
	Properties {
		_WaterTex ("Normal Map (RGB), Foam (A)", 2D) = "white" {}
		_WaterTex2 ("Normal Map (RGB), Foam (B)", 2D) = "white" {}
		_ShoreLineTex ("ShoreLine Foam", 2D) = "white" {}
		_ShoreGray ("Shore Gray(gray)", 2D) = "white" {}
		_ShoreLineIntensity ("ShoreLine Intensity", Float) = 2
		_Tiling ("Wave Scale", Range(0.00025, 0.01)) = 0.25
		_WaveSpeed ("Wave Speed", Float) = 0.4
		_SpecularRatio ("Specular Ratio", Range(10, 500)) = 200
		_BottomColor ("Bottom Color", Color) = (0,0,0,0)
		_TopColor ("Top Color", Color) = (0,0,0,0)
		_Alpha ("Alpha", Range(0, 1)) = 1
		_ReflectionIntensity ("Reflection Intensity", Range(0, 1)) = 0.1
		_OceanWidth ("Ocean Width", Float) = 10240
		_OceanHeight ("Ocean Height", Float) = 10240
	}
	SubShader {
		LOD 250
		Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent-200" "RenderType" = "Transparent" }
		Pass {
			LOD 250
			Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent-200" "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			Lighting On
			GpuProgramID 12878
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
			GpuProgramID 127226
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
			GpuProgramID 178030
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
			GpuProgramID 212794
			// No subprograms found
		}
	}
	Fallback "Diffuse"
}