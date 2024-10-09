using System;
using UnityEngine;

[Serializable]
public class DayNightController : MonoBehaviour
{
	public float daySpeedMultiplier = 0.1f;

	public Light sunLight;

	public bool controlIntensity = true;

	public float startTime = 12f;

	private float currentTime;

	public string timeString = "00:00 AM";

	private float xValueOfSun = 90f;

	[SerializeField]
	public Transform[] cloudSpheres;

	public float cloudRotationSpeed = 1f;

	[SerializeField]
	public Transform[] starSpheres;

	public float twinkleFrequency = 5f;

	private float twinkleCounter;

	public float starRotationSpeed = 0.15f;

	public Camera cameraToFollow;

	private void Start()
	{
		currentTime = startTime;
	}

	private void Update()
	{
		currentTime += Time.deltaTime * daySpeedMultiplier;
		if (currentTime >= 24f)
		{
			currentTime %= 24f;
		}
		if ((bool)sunLight)
		{
			ControlLight();
		}
		if (cloudSpheres.Length > 0)
		{
			ControlClouds();
		}
		if (starSpheres.Length > 0)
		{
			StarSphere();
		}
		ControlCamera();
		CalculateTime();
	}

	private void ControlLight()
	{
		xValueOfSun = 0f - (90f + currentTime * 15f);
		sunLight.transform.eulerAngles = sunLight.transform.right * xValueOfSun;
		if (xValueOfSun >= 360f)
		{
			xValueOfSun = 0f;
		}
		if (controlIntensity && (bool)sunLight && (currentTime >= 18f || currentTime <= 5.5f))
		{
			sunLight.intensity = Mathf.MoveTowards(sunLight.intensity, 0f, Time.deltaTime * daySpeedMultiplier * 10f);
		}
		else if (controlIntensity && (bool)sunLight)
		{
			sunLight.intensity = Mathf.MoveTowards(sunLight.intensity, 1f, Time.deltaTime * daySpeedMultiplier * 10f);
		}
	}

	private void ControlClouds()
	{
		Transform[] array = cloudSpheres;
		foreach (Transform transform in array)
		{
			if ((bool)transform)
			{
				transform.transform.Rotate(Vector3.forward * cloudRotationSpeed * daySpeedMultiplier * Time.deltaTime);
			}
		}
	}

	private void StarSphere()
	{
		Transform[] array = starSpheres;
		foreach (Transform transform in array)
		{
			if ((bool)transform)
			{
				transform.transform.Rotate(Vector3.forward * starRotationSpeed * daySpeedMultiplier * Time.deltaTime);
				if (currentTime > 5.5f && currentTime < 18f && (bool)transform.GetComponent<Renderer>())
				{
					Color color = transform.GetComponent<Renderer>().material.color;
					transform.GetComponent<Renderer>().material.color = new Color(color.r, color.g, color.b, Mathf.Lerp(color.a, 0f, Time.deltaTime * 50f * daySpeedMultiplier));
				}
			}
		}
		int num = UnityEngine.Random.Range(0, starSpheres.Length);
		if ((bool)starSpheres[num] && twinkleCounter <= 0f && (currentTime >= 18f || currentTime <= 5.5f) && (bool)starSpheres[num].GetComponent<Renderer>())
		{
			twinkleCounter = 1f;
			Color color = starSpheres[num].GetComponent<Renderer>().material.color;
			starSpheres[num].GetComponent<Renderer>().material.color = new Color(color.r, color.g, color.b, UnityEngine.Random.Range(0.01f, 0.5f));
		}
		if (twinkleCounter > 0f)
		{
			twinkleCounter -= Time.deltaTime * daySpeedMultiplier * twinkleFrequency;
		}
	}

	private void ControlCamera()
	{
		if (!cameraToFollow)
		{
			cameraToFollow = Camera.main;
		}
		else if ((bool)cameraToFollow)
		{
			base.transform.position = cameraToFollow.transform.position;
		}
	}

	private void CalculateTime()
	{
		string empty = string.Empty;
		float num = (currentTime - Mathf.Floor(currentTime)) * 60f;
		empty = ((!(currentTime <= 12f)) ? "PM" : "AM");
		timeString = Mathf.Floor(currentTime) + " : " + num.ToString("F0") + " " + empty;
	}
}
