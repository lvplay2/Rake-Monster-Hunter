using UnityEngine;

public class MirrorReflection : MonoBehaviour
{
	private int m_TextureSize = 512;

	private float m_ClipPlaneOffset = 0.1f;

	public LayerMask m_ReflectLayers = -1;

	private Camera m_ReflectionCamera;

	public static RenderTexture m_ReflectionTexture;

	public Material oceanMaterial;

	public Camera cam;

	public bool enableMirrorReflection = true;

	public float alpha = 1f;

	private static MirrorReflection _instance;

	public static MirrorReflection instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType(typeof(MirrorReflection)) as MirrorReflection;
			}
			return _instance;
		}
	}

	private void Awake()
	{
		if (oceanMaterial == null)
		{
			oceanMaterial = GetComponent<Renderer>().sharedMaterial;
		}
	}

	private void LateUpdate()
	{
		int num = 100;
		if (enableMirrorReflection)
		{
			num += 100;
		}
		alpha = Mathf.Clamp01(alpha);
		if (alpha < 1f)
		{
			num += 50;
		}
		oceanMaterial.shader.maximumLOD = num;
		oceanMaterial.SetFloat("_Alpha", alpha);
		if (enableMirrorReflection)
		{
			if (cam == null)
			{
				cam = Camera.main.GetComponent<Camera>();
			}
			if ((bool)oceanMaterial && (bool)cam)
			{
				Camera reflectionCamera;
				CreateMirrorObjects(cam, out reflectionCamera);
				Vector3 position = base.transform.position;
				Vector3 up = base.transform.up;
				UpdateCameraModes(cam, reflectionCamera);
				float w = 0f - Vector3.Dot(up, position) - m_ClipPlaneOffset;
				Vector4 plane = new Vector4(up.x, up.y, up.z, w);
				Matrix4x4 reflectionMat = Matrix4x4.zero;
				CalculateReflectionMatrix(ref reflectionMat, plane);
				Vector3 position2 = cam.transform.position;
				Vector3 position3 = reflectionMat.MultiplyPoint(position2);
				reflectionCamera.worldToCameraMatrix = cam.worldToCameraMatrix * reflectionMat;
				Vector4 clipPlane = CameraSpacePlane(reflectionCamera, position, up, 1f);
				Matrix4x4 projection = cam.projectionMatrix;
				CalculateObliqueMatrix(ref projection, clipPlane);
				reflectionCamera.projectionMatrix = projection;
				reflectionCamera.cullingMask = -17 & m_ReflectLayers.value;
				reflectionCamera.transform.position = position3;
				Vector3 eulerAngles = cam.transform.eulerAngles;
				reflectionCamera.transform.eulerAngles = new Vector3(0f, eulerAngles.y, eulerAngles.z);
				reflectionCamera.transform.position = position2;
				oceanMaterial.SetTexture("_ReflectionTex", m_ReflectionTexture);
				Matrix4x4 matrix4x = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, new Vector3(0.5f, 0.5f, 0.5f));
				Vector3 lossyScale = base.transform.lossyScale;
				Matrix4x4 matrix4x2 = Matrix4x4.Scale(new Vector3(1f / lossyScale.x, 1f / lossyScale.y, 1f / lossyScale.z));
				matrix4x2 = matrix4x * cam.projectionMatrix * cam.worldToCameraMatrix * matrix4x2;
				oceanMaterial.SetMatrix("_ProjMatrix", matrix4x2);
			}
		}
	}

	private void OnDisable()
	{
		if ((bool)m_ReflectionTexture)
		{
			Object.DestroyImmediate(m_ReflectionTexture);
			m_ReflectionTexture = null;
		}
		if ((bool)m_ReflectionCamera)
		{
			Object.DestroyImmediate(m_ReflectionCamera.gameObject);
		}
	}

	private void UpdateCameraModes(Camera src, Camera dest)
	{
		if (dest == null)
		{
			return;
		}
		dest.clearFlags = src.clearFlags;
		dest.backgroundColor = src.backgroundColor;
		if (src.clearFlags == CameraClearFlags.Skybox)
		{
			Skybox skybox = src.GetComponent(typeof(Skybox)) as Skybox;
			Skybox skybox2 = dest.GetComponent(typeof(Skybox)) as Skybox;
			if (!skybox || !skybox.material)
			{
				skybox2.enabled = false;
			}
			else
			{
				skybox2.enabled = true;
				skybox2.material = skybox.material;
			}
		}
		dest.farClipPlane = src.farClipPlane;
		dest.nearClipPlane = src.nearClipPlane;
		dest.orthographic = src.orthographic;
		dest.fieldOfView = src.fieldOfView;
		dest.aspect = src.aspect;
		dest.orthographicSize = src.orthographicSize;
	}

	private void CreateMirrorObjects(Camera currentCamera, out Camera reflectionCamera)
	{
		if (!m_ReflectionTexture)
		{
			m_ReflectionTexture = new RenderTexture(m_TextureSize, m_TextureSize, 16);
			m_ReflectionTexture.name = "__MirrorReflection" + GetInstanceID();
			m_ReflectionTexture.isPowerOfTwo = true;
			m_ReflectionTexture.hideFlags = HideFlags.DontSave;
		}
		reflectionCamera = m_ReflectionCamera;
		if (!reflectionCamera)
		{
			GameObject gameObject = new GameObject("Mirror Refl Camera id" + GetInstanceID() + " for " + currentCamera.GetInstanceID(), typeof(Camera), typeof(Skybox));
			reflectionCamera = gameObject.GetComponent<Camera>();
			reflectionCamera.enabled = true;
			gameObject.hideFlags = HideFlags.DontSave;
			m_ReflectionCamera = reflectionCamera;
			reflectionCamera.gameObject.AddComponent<ReflectionCameraControl>();
		}
		reflectionCamera.depth = currentCamera.depth - 1f;
		reflectionCamera.fieldOfView = currentCamera.fieldOfView;
		reflectionCamera.transform.position = base.transform.position;
		reflectionCamera.transform.rotation = base.transform.rotation;
	}

	private static float sgn(float a)
	{
		if (a > 0f)
		{
			return 1f;
		}
		if (a < 0f)
		{
			return -1f;
		}
		return 0f;
	}

	private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
	{
		Vector3 point = pos + normal * m_ClipPlaneOffset;
		Matrix4x4 worldToCameraMatrix = cam.worldToCameraMatrix;
		Vector3 lhs = worldToCameraMatrix.MultiplyPoint(point);
		Vector3 rhs = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
		return new Vector4(rhs.x, rhs.y, rhs.z, 0f - Vector3.Dot(lhs, rhs));
	}

	private static void CalculateObliqueMatrix(ref Matrix4x4 projection, Vector4 clipPlane)
	{
		Vector4 b = projection.inverse * new Vector4(sgn(clipPlane.x), sgn(clipPlane.y), 1f, 1f);
		Vector4 vector = clipPlane * (2f / Vector4.Dot(clipPlane, b));
		projection[2] = vector.x - projection[3];
		projection[6] = vector.y - projection[7];
		projection[10] = vector.z - projection[11];
		projection[14] = vector.w - projection[15];
	}

	private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
	{
		reflectionMat.m00 = 1f - 2f * plane[0] * plane[0];
		reflectionMat.m01 = -2f * plane[0] * plane[1];
		reflectionMat.m02 = -2f * plane[0] * plane[2];
		reflectionMat.m03 = -2f * plane[3] * plane[0];
		reflectionMat.m10 = -2f * plane[1] * plane[0];
		reflectionMat.m11 = 1f - 2f * plane[1] * plane[1];
		reflectionMat.m12 = -2f * plane[1] * plane[2];
		reflectionMat.m13 = -2f * plane[3] * plane[1];
		reflectionMat.m20 = -2f * plane[2] * plane[0];
		reflectionMat.m21 = -2f * plane[2] * plane[1];
		reflectionMat.m22 = 1f - 2f * plane[2] * plane[2];
		reflectionMat.m23 = -2f * plane[3] * plane[2];
		reflectionMat.m30 = 0f;
		reflectionMat.m31 = 0f;
		reflectionMat.m32 = 0f;
		reflectionMat.m33 = 1f;
	}
}
