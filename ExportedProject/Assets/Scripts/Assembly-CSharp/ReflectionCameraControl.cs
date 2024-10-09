using UnityEngine;

public class ReflectionCameraControl : MonoBehaviour
{
	private void OnPreRender()
	{
		GL.SetRevertBackfacing(true);
	}

	private void OnPostRender()
	{
		GetComponent<Camera>().targetTexture = MirrorReflection.m_ReflectionTexture;
		GL.SetRevertBackfacing(false);
	}

	private void OnDestroy()
	{
		GL.SetRevertBackfacing(false);
	}
}
