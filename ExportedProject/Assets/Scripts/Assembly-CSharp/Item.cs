using UnityEngine;

public class Item : MonoBehaviour
{
	public int ammount;

	public int ammountMax;

	protected bool isEquiped;

	protected virtual void Start()
	{
	}

	public bool IsEquiped()
	{
		return isEquiped;
	}

	public virtual void Equip()
	{
		isEquiped = true;
	}

	public virtual void UnEquip()
	{
		isEquiped = false;
	}

	public virtual void UpdateCounter()
	{
	}
}
