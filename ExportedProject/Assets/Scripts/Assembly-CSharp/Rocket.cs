using System.Collections;
using UnityEngine;

public class Rocket : MonoBehaviour
{
	public bool Activate;

	private float lifetimeout;

	public Transform Smoke;

	public Transform Fire;

	public Rigidbody rig;

	public float scareDist = 10f;

	private bool atFirstTime = true;

	protected Transform thisTransform;

	private void Start()
	{
		thisTransform = base.transform;
	}

	private void Pinok()
	{
		Vector3 vector = new Vector3(base.transform.forward.x, base.transform.forward.y, base.transform.forward.z);
		vector *= 5000f;
		Debug.Log("v " + vector);
		rig.AddForce(vector);
	}

	private void ScareCreatures()
	{
		ArrayList allCreatures = Creature.allCreatures;
		foreach (Creature item in allCreatures)
		{
			if ((item.transform.position - thisTransform.position).sqrMagnitude < scareDist * scareDist)
			{
				item.ScareByRocket();
			}
		}
	}

	private void FixedUpdate()
	{
		if (Activate)
		{
			ScareCreatures();
		}
	}

	private void Update()
	{
		if (Activate)
		{
			if (atFirstTime)
			{
				atFirstTime = false;
				Invoke("Pinok", 0f);
				EnvirometSettings.sets.ConfirmRocketFire(true);
			}
			if (rig.isKinematic)
			{
				rig.isKinematic = false;
			}
			lifetimeout += Time.deltaTime;
			if (lifetimeout > 10f)
			{
				Dead();
				EnvirometSettings.sets.ConfirmRocketFire(false);
			}
		}
	}

	public void Dead()
	{
		Smoke.GetComponent<ParticleEmitter>().emit = false;
		Smoke.parent = null;
		Object.Destroy(Smoke.gameObject, 7f);
		Object.Destroy(base.gameObject);
	}
}
