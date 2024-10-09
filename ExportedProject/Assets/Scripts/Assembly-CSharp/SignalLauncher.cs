using UnityEngine;

public class SignalLauncher : Weapon
{
	public GameObject rocketTemplate;

	protected override void Start()
	{
		base.Start();
	}

	protected override void MakeShoot()
	{
		audioSource.clip = sndFire;
		audioSource.Play();
		GameObject gameObject = Object.Instantiate(rocketTemplate, null, true);
		gameObject.transform.position = rocketTemplate.transform.position;
		gameObject.transform.rotation = rocketTemplate.transform.rotation;
		gameObject.SetActive(true);
		magazine--;
	}

	protected override void Update()
	{
		base.Update();
	}

	public override void Equip()
	{
		base.gameObject.SetActive(true);
		base.Equip();
	}

	public override void UnEquip()
	{
		base.gameObject.SetActive(false);
		base.UnEquip();
	}
}
