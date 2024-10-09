using System.Collections;
using UnityEngine;

public class WeaponExample : MonoBehaviour
{
	public FP_Input playerInput;

	public float shootRate = 0.15f;

	public float reloadTime = 1f;

	public int ammoCount = 15;

	private int ammo;

	private float delay;

	private bool reloading;

	private void Start()
	{
		ammo = ammoCount;
	}

	private void Update()
	{
		if (playerInput.Shoot() && Time.time > delay)
		{
			Shoot();
		}
		if (playerInput.Reload() && !reloading && ammoCount < ammo)
		{
			StartCoroutine("Reload");
		}
	}

	private void Shoot()
	{
		if (ammoCount > 0)
		{
			Debug.Log("Shoot");
			ammoCount--;
		}
		else
		{
			Debug.Log("Empty");
		}
		delay = Time.time + shootRate;
	}

	private IEnumerator Reload()
	{
		reloading = true;
		Debug.Log("Reloading");
		yield return new WaitForSeconds(reloadTime);
		ammoCount = ammo;
		Debug.Log("Reloading Complete");
		reloading = false;
	}

	private void OnGUI()
	{
		GUILayout.Label("AMMO: " + ammoCount);
	}
}
