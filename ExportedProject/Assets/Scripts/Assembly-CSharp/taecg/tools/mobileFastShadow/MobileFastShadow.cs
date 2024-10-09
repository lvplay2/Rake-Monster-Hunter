using UnityEngine;

namespace taecg.tools.mobileFastShadow
{
	[RequireComponent(typeof(Camera))]
	[RequireComponent(typeof(Projector))]
	public class MobileFastShadow : MonoBehaviour
	{
		public enum AntiAliasing
		{
			None = 1,
			Samples2 = 2,
			Samples4 = 4,
			Samples8 = 8
		}

		[Header("v1.03")]
		[Header("Follow Camera")]
		[Tooltip("The shadow following camera automatically identifies the tag \"MainCamera\" camera if it is empty.")]
		public Camera FollowCam;

		[Header("Shadow Layer")]
		[Tooltip(" It is used to identify which objects need to cast shadows.")]
		public LayerMask LayerCaster;

		[Tooltip("It is used to identify which objects need to receive shadows.")]
		public LayerMask LayerIgnoreReceiver;

		[Header("Shadow Detail (In Editor Mode)")]
		[Tooltip("The size of the generated RenderTexture. ")]
		public Vector2 Size = new Vector2(1024f, 1024f);

		[Tooltip("Shaded sampling, if you want to make the edge as smooth as possible to choose a higher sample, the same performance will decline.")]
		public AntiAliasing RTAntiAliasing = AntiAliasing.None;

		[Tooltip(" In order to prevent the shadow of the RenderTarget edge from stretching, it is necessary to use a kind of transition picture to deal with it so that it is more natural.")]
		public Texture2D FalloffTex;

		[Range(0f, 1f)]
		[Tooltip("It is used to adjust the transparency of shadow.")]
		public float Intensity = 0.5f;

		[Header("Shadow Direction (Runtime)")]
		[Tooltip("To adjust the direction of the shadow.")]
		public Vector3 Direction = new Vector3(50f, -30f, -20f);

		[Header("Projection Orthographic Size (In Editor Mode)")]
		[Tooltip("The bigger the value, the more objects will be shadowed. It can solve the problem of blurred shadows within the same screen, but the excessive value will also cause the quality of the shadow to drop, so find a suitable balance for you. In order to maximize efficiency, there is no support for adjusting Size of Projector and camera at runtime, and these two values will be initialized after running, so this value can be used to adjust initialization value.")]
		public float ProjectionSize = 10f;

		private Camera shadowCam;

		private Transform shadowCamTrans;

		private Projector projector;

		private Material shadowMat;

		private RenderTexture shadowRT;

		public void Start()
		{
			if (FollowCam == null)
			{
				if (Camera.main != null)
				{
					FollowCam = Camera.main;
				}
				else
				{
					Debug.LogWarning("Please specify the main camera to follow！");
				}
			}
			projector = GetComponent<Projector>();
			if (projector == null)
			{
				Debug.LogError("Projector Component Missing!!");
			}
			projector.orthographic = true;
			projector.orthographicSize = ProjectionSize;
			projector.aspectRatio = Size.x / Size.y;
			shadowMat = new Material(Shader.Find("ONEMT/Projector/ProjectorShadow"));
			projector.material = shadowMat;
			shadowMat.SetTexture("_FalloffTex", FalloffTex);
			shadowMat.SetFloat("_Intensity", Intensity);
			projector.ignoreLayers = LayerIgnoreReceiver;
			shadowCam = GetComponent<Camera>();
			if (shadowCam == null)
			{
				Debug.LogError("Camera Component Missing!!");
			}
			shadowCamTrans = shadowCam.transform;
			shadowCam.clearFlags = CameraClearFlags.Color;
			shadowCam.backgroundColor = new Color(0f, 0f, 0f, 0f);
			shadowCam.orthographic = true;
			shadowCam.orthographicSize = ProjectionSize;
			shadowCam.depth = -2.1474836E+09f;
			shadowCam.cullingMask = LayerCaster;
			shadowRT = new RenderTexture((int)Size.x, (int)Size.y, 0, RenderTextureFormat.ARGB32);
			shadowRT.name = "ShadowRT";
			shadowRT.antiAliasing = (int)RTAntiAliasing;
			shadowRT.filterMode = FilterMode.Bilinear;
			shadowRT.wrapMode = TextureWrapMode.Clamp;
			shadowCam.targetTexture = shadowRT;
			shadowMat.SetTexture("_ShadowTex", shadowRT);
		}

		private void LateUpdate()
		{
			Vector3 forward = base.transform.forward;
			forward *= Direction.z;
			base.transform.position = FollowCam.transform.position + forward;
			shadowCamTrans.rotation = Quaternion.Euler(Direction);
		}
	}
}
