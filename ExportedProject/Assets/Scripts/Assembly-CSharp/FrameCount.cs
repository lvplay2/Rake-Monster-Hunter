using UnityEngine;

public class FrameCount : MonoBehaviour
{
	public Light directLight;

	public MirrorReflection mirrorReflection;

	public Material oceanMat;

	private float frameCount;

	private int lastFrameCount;

	private int currenFrameCount;

	private float intevalTime = 1f;

	private Color lightColor;

	private GUIStyle style;

	private float reflectionIntensity = 0.3f;

	private float shoreLineIntensity = 4.8f;

	private float alpha = 0.7f;

	private void Awake()
	{
		mirrorReflection = Object.FindObjectOfType(typeof(MirrorReflection)) as MirrorReflection;
	}

	private void Start()
	{
		lightColor = directLight.color;
		style = new GUIStyle();
		style.fontSize = 25;
		style.normal.textColor = Color.white;
		InvokeRepeating("GetFrameCount", 0f, intevalTime);
	}

	private void Update()
	{
		directLight.color = lightColor;
		oceanMat.SetFloat("_ReflectionIntensity", reflectionIntensity);
		oceanMat.SetFloat("_ShoreLineIntensity", shoreLineIntensity);
	}

	private void GetFrameCount()
	{
		currenFrameCount = Time.frameCount;
		frameCount = (float)(currenFrameCount - lastFrameCount) / intevalTime;
		lastFrameCount = currenFrameCount;
	}

	private void OnGUI()
	{
		GUILayout.Label("frame:" + frameCount, style);
		GUILayout.Label("press w,s,a,d to move.");
		GUILayout.Label("press UpArrow,LeftArrow,RightArrow,DownArrow to rotate.");
		GUILayout.Label("light red");
		lightColor.r = GUILayout.HorizontalScrollbar(lightColor.r, 0.1f, 0f, 1f, GUILayout.Width(200f));
		GUILayout.Label("light green");
		lightColor.g = GUILayout.HorizontalScrollbar(lightColor.g, 0.1f, 0f, 1f, GUILayout.Width(200f));
		GUILayout.Label("light blue");
		lightColor.b = GUILayout.HorizontalScrollbar(lightColor.b, 0.1f, 0f, 1f, GUILayout.Width(200f));
		GUILayout.Label("Shore line intensity:");
		shoreLineIntensity = GUILayout.HorizontalScrollbar(shoreLineIntensity, 0.2f, 0f, 5f, GUILayout.Width(200f));
		GUILayout.Label("alpha:");
		alpha = GUILayout.HorizontalScrollbar(alpha, 0.1f, 0f, 1f, GUILayout.Width(200f));
		mirrorReflection.enableMirrorReflection = GUILayout.Toggle(mirrorReflection.enableMirrorReflection, "Mirror reflection");
		if (mirrorReflection.enableMirrorReflection)
		{
			GUILayout.Label("Reflection intensity:");
			reflectionIntensity = GUILayout.HorizontalScrollbar(reflectionIntensity, 0.1f, 0f, 1f, GUILayout.Width(200f));
		}
		mirrorReflection.alpha = alpha;
	}
}
