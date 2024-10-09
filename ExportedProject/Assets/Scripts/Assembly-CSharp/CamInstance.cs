using System.Collections;
using UnityEngine;

public class CamInstance : MonoBehaviour
{
	public Camera cam;

	public Transform camTransform;

	[HideInInspector]
	public static ArrayList allCams = new ArrayList();

	private CreatureEyes eyes;

	private void Start()
	{
		allCams.Add(this);
		eyes = GetComponent<CreatureEyes>();
		if (eyes == null)
		{
			Debug.LogWarning("camera have no Eyes!");
		}
	}

	private void OnDestroy()
	{
		allCams.Remove(this);
	}

	public bool IsCamLookSomebody()
	{
		if (eyes == null)
		{
			return false;
		}
		eyes.CheckVision();
		return eyes.inVision > 0;
	}
}
