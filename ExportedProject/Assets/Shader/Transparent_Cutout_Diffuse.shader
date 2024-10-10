Shader "Transparent/Cutout/Diffuse" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Range(0, 1)) = 0.5
	}
	SubShader {
		LOD 200
		Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "AlphaTest" "RenderType" = "TransparentCutout" }
		Pass {
			Name "FORWARD"
			LOD 200
			Tags { "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "FORWARDBASE" "QUEUE" = "AlphaTest" "RenderType" = "TransparentCutout" "SHADOWSUPPORT" = "true" }
			ColorMask RGB
			Cull Off
			GpuProgramID 15605
			// No subprograms found
		}
		Pass {
			Name "FORWARD"
			LOD 200
			Tags { "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "FORWARDADD" "QUEUE" = "AlphaTest" "RenderType" = "TransparentCutout" }
			Blend One One, One One
			ColorMask RGB
			ZWrite Off
			Cull Off
			GpuProgramID 90360
			// No subprograms found
		}
		Pass {
			Name "PREPASS"
			LOD 200
			Tags { "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "PREPASSBASE" "QUEUE" = "AlphaTest" "RenderType" = "TransparentCutout" }
			Cull Off
			GpuProgramID 169635
			// No subprograms found
		}
		Pass {
			Name "PREPASS"
			LOD 200
			Tags { "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "PREPASSFINAL" "QUEUE" = "AlphaTest" "RenderType" = "TransparentCutout" }
			ZWrite Off
			Cull Off
			GpuProgramID 224414
			// No subprograms found
		}
		Pass {
			Name "DEFERRED"
			LOD 200
			Tags { "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "DEFERRED" "QUEUE" = "AlphaTest" "RenderType" = "TransparentCutout" }
			Cull Off
			GpuProgramID 271490
			// No subprograms found
		}
		Pass {
			Name "META"
			LOD 200
			Tags { "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "META" "QUEUE" = "AlphaTest" "RenderType" = "TransparentCutout" }
			Cull Off
			GpuProgramID 377901
			// No subprograms found
		}
	}
	Fallback "Legacy Shaders/Transparent/Cutout/VertexLit"
}