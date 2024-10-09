using UnityEngine;

public class Weapon : Item
{
	public FP_Input playerInput;

	public Animator anim;

	public Transform cam;

	public AudioSource audioSource;

	public AudioClip sndFire;

	public float scareRadius = 50f;

	public int ammo = 40;

	public int ammoMax = 40;

	public int magazineMax = 8;

	public int magazine;

	protected float damage = 15f;

	public TextCounter counter;

	private bool fireButtonStatus;

	protected bool reloadWasInterrupted;

	private StateMachine sm = new StateMachine();

	private float shootRange = 100f;

	private int shootableMask;

	private Ray shootRay = default(Ray);

	private RaycastHit shootHit;

	private bool currAnimIsEnded;

	public override void UpdateCounter()
	{
		counter.SetValue(magazine + "/" + ammo);
	}

	protected override void Start()
	{
		base.Start();
		shootableMask = LayerMask.GetMask("Shootable") + LayerMask.GetMask("Terrain");
		SimpleDelegation simpleDelegation = anim.gameObject.AddComponent<SimpleDelegation>();
		simpleDelegation.Add("ConfirmCurrAnimIsEnded", ConfirmCurrAnimIsEnded);
		StateMachine.State state = new StateMachine.State("Idle");
		sm.AddState(state);
		state.OnEnter = Idle_OnEnter;
		state.Update = Idle_Update;
		state.OnExit = Idle_OnExit;
		state.AddLink("Fire", ShootIsPreesdAndWeaponLoaded);
		state.AddLink("Reload", MagazineIsEmptyButYouHaveAmmo);
		state.AddLink("Reload", ReloadPressed_MagazineNotFull_YouHaveAmmo);
		StateMachine.State state2 = new StateMachine.State("Fire");
		sm.AddState(state2);
		state2.OnEnter = Fire_OnEnter;
		state2.Update = Fire_Update;
		state2.OnExit = Fire_OnExit;
		state2.AddLink("Idle", CurrAnimIsEnded);
		StateMachine.State state3 = new StateMachine.State("Reload");
		sm.AddState(state3);
		state3.OnEnter = Reload_OnEnter;
		state3.Update = Reload_Update;
		state3.OnExit = Reload_OnExit;
		state3.AddLink("Idle", CurrAnimIsEnded);
		sm.SwitchStateTo(state);
		counter.SetValue(magazine + "/" + ammo);
	}

	public int TakeAmmoAll()
	{
		int num = ammoMax - ammo;
		ammo += num;
		counter.SetValue(magazine + "/" + ammo);
		return num;
	}

	private bool ReloadPressed_MagazineNotFull_YouHaveAmmo()
	{
		return playerInput.Reload() && magazine < magazineMax && ammo > 0;
	}

	private bool MagazineIsEmptyButYouHaveAmmo()
	{
		return magazine <= 0 && ammo > 0;
	}

	private bool MagazineLoaded()
	{
		return currAnimIsEnded;
	}

	private void ScareCreatures()
	{
	}

	protected virtual void MakeShoot()
	{
		ScareCreatures();
		audioSource.clip = sndFire;
		audioSource.Play();
		shootRay.origin = cam.position;
		shootRay.direction = cam.forward;
		if (Physics.Raycast(shootRay, out shootHit, shootRange, shootableMask))
		{
			CreatureHitBox component = shootHit.collider.GetComponent<CreatureHitBox>();
			if ((bool)component)
			{
				component.TakeDamage(damage, base.transform);
			}
		}
		magazine--;
	}

	protected virtual void Idle_OnEnter()
	{
		anim.SetTrigger("Idle");
	}

	protected virtual void Idle_Update()
	{
	}

	protected virtual void Idle_OnExit()
	{
	}

	private bool ShootIsPreesdAndWeaponLoaded()
	{
		if (playerInput.Shoot() && magazine > 0)
		{
			return true;
		}
		return false;
	}

	protected virtual void Fire_OnEnter()
	{
		anim.SetTrigger("Fire");
		currAnimIsEnded = false;
		MakeShoot();
		counter.SetValue(magazine + "/" + ammo);
	}

	protected virtual void Fire_Update()
	{
	}

	protected virtual void Fire_OnExit()
	{
	}

	private bool CurrAnimIsEnded()
	{
		return currAnimIsEnded;
	}

	public void ConfirmCurrAnimIsEnded()
	{
		currAnimIsEnded = true;
	}

	protected virtual void Reload_OnEnter()
	{
		anim.SetTrigger("Reload");
		currAnimIsEnded = false;
		counter.SetValue("--/" + ammo);
	}

	protected virtual void Reload_Update()
	{
	}

	protected virtual void Reload_OnExit()
	{
		int a = magazineMax - magazine;
		a = Mathf.Min(a, ammo);
		magazine += a;
		ammo -= a;
		counter.SetValue(magazine + "/" + ammo);
	}

	public override void Equip()
	{
		if (reloadWasInterrupted)
		{
			sm.SwitchStateTo(sm.GetStateById("Reload"));
			reloadWasInterrupted = false;
		}
		else
		{
			sm.SwitchStateTo(sm.GetStateById("Idle"));
		}
		base.Equip();
	}

	public override void UnEquip()
	{
		if (sm.GetCurrState().id == "Reload")
		{
			reloadWasInterrupted = true;
		}
		base.gameObject.SetActive(false);
		base.UnEquip();
	}

	protected virtual void Update()
	{
		sm.Update();
	}
}
