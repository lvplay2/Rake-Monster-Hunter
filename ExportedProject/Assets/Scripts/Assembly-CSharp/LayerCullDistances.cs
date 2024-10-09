using System;
using UnityEngine;

public class LayerCullDistances : MonoBehaviour
{
	[Serializable]
	public class Dist
	{
		public string name;

		public float distance;
	}

	public Dist[] layerDistances;

	private void SetDistances(Camera cam)
	{
		float[] array = new float[32];
		Dist[] array2 = layerDistances;
		foreach (Dist dist in array2)
		{
			int num = LayerMask.NameToLayer(dist.name);
			float distance = dist.distance;
			array[num] = distance;
		}
		cam.layerCullDistances = array;
	}

	private void Start()
	{
		Camera camera = GetComponent<Camera>();
		if (!camera)
		{
			camera = Camera.main;
		}
		SetDistances(camera);
	}

	private void Update()
	{
	}
}
