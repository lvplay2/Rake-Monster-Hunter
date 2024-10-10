Shader "Hidden/SSAO" {
	Properties {
		_MainTex ("", 2D) = "" {}
		_RandomTexture ("", 2D) = "" {}
		_SSAO ("", 2D) = "" {}
	}
	SubShader {
		Pass {
			ZTest Always
			ZWrite Off
			Cull Off
			GpuProgramID 13231
			// No subprograms found
		}
		Pass {
			ZTest Always
			ZWrite Off
			Cull Off
			GpuProgramID 112671
			// No subprograms found
		}
		Pass {
			ZTest Always
			ZWrite Off
			Cull Off
			GpuProgramID 174990
			// No subprograms found
		}
		Pass {
			ZTest Always
			ZWrite Off
			Cull Off
			GpuProgramID 254315
			// No subprograms found
		}
		Pass {
			ZTest Always
			ZWrite Off
			Cull Off
			GpuProgramID 318747
			// No subprograms found
		}
	}
}