using UnityEngine;

public class Rifle : Weapon
{
	public GameObject shootParticle;

	protected override void Start()
	{
		base.Start();
	}

	protected override void MakeShoot()
	{
		GameObject gameObject = Object.Instantiate(shootParticle, shootParticle.transform.parent);
		gameObject.SetActive(true);
		base.MakeShoot();
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
