using System.Collections;
using UnityEngine;

public class MeatInstance : MonoBehaviour
{
	public static ArrayList all = new ArrayList();

	private void Start()
	{
		all.Add(this);
	}

	private void OnDestroy()
	{
		all.Remove(this);
	}

	private void Update()
	{
	}
}
