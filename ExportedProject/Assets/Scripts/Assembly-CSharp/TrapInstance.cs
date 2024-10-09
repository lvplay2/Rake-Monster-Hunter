using UnityEngine;

public class TrapInstance : MonoBehaviour
{
	public Animator anim;

	protected float damage = 20f;

	protected float freezeTime = 2f;

	protected bool opened = true;

	private void SetOpened(bool value)
	{
		opened = value;
		if (value)
		{
			anim.SetBool("Open", true);
		}
		else
		{
			anim.SetBool("Open", false);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (opened)
		{
			Creature component = other.gameObject.GetComponent<Creature>();
			if ((bool)component)
			{
				component.TakeDamage(damage, base.transform);
				component.Freeze(freezeTime);
				SetOpened(false);
			}
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnDestroy()
	{
	}
}
