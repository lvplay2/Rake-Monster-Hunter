Shader "VacuumShaders/Terrain To Mesh/Legacy Shaders/Diffuse/3 Textures" {
	Properties {
		_Color ("Tint Color", Color) = (1,1,1,1)
		_V_T2M_Control ("Control Map (RGBA)", 2D) = "black" {}
		[V_T2M_Layer] _V_T2M_Splat1 ("Layer 1 (R)", 2D) = "white" {}
		[HideInInspector] _V_T2M_Splat1_uvScale ("", Float) = 1
		[V_T2M_Layer] _V_T2M_Splat2 ("Layer 2 (G)", 2D) = "white" {}
		[HideInInspector] _V_T2M_Splat2_uvScale ("", Float) = 1
		[V_T2M_Layer] _V_T2M_Splat3 ("Layer 3 (B)", 2D) = "white" {}
		[HideInInspector] _V_T2M_Splat3_uvScale ("", Float) = 1
	}
	SubShader {
		LOD 200
		Tags { "RenderType" = "Opaque" }
		Pass {
			Name "FORWARD"
			LOD 200
			Tags { "LIGHTMODE" = "FORWARDBASE" "RenderType" = "Opaque" "SHADOWSUPPORT" = "true" }
			GpuProgramID 19731
			// No subprograms found
		}
		Pass {
			Name "FORWARD"
			LOD 200
			Tags { "LIGHTMODE" = "FORWARDADD" "RenderType" = "Opaque" }
			Blend One One, One One
			ZWrite Off
			GpuProgramID 118260
			// No subprograms found
		}
		Pass {
			Name "PREPASS"
			LOD 200
			Tags { "LIGHTMODE" = "PREPASSBASE" "RenderType" = "Opaque" }
			GpuProgramID 157236
			// No subprograms found
		}
		Pass {
			Name "PREPASS"
			LOD 200
			Tags { "LIGHTMODE" = "PREPASSFINAL" "RenderType" = "Opaque" }
			ZWrite Off
			GpuProgramID 258825
			// No subprograms found
		}
		Pass {
			Name "DEFERRED"
			LOD 200
			Tags { "LIGHTMODE" = "DEFERRED" "RenderType" = "Opaque" }
			GpuProgramID 284091
			// No subprograms found
		}
		Pass {
			Name "META"
			LOD 200
			Tags { "LIGHTMODE" = "META" "RenderType" = "Opaque" }
			Cull Off
			GpuProgramID 355172
			// No subprograms found
		}
	}
	Fallback "Legacy Shaders/Diffuse"
}