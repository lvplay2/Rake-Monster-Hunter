using UnityEngine;

public class Heals : MonoBehaviour
{
	public float hp;

	public float hpMax = 100f;

	public Bar bar;

	private void Start()
	{
		SetValueToBar();
	}

	public virtual void TakeDamage(float damage)
	{
		hp -= damage;
		SetValueToBar();
	}

	public virtual void Heal(float heal)
	{
		hp = hpMax * heal;
		SetValueToBar();
	}

	public void SetValueToBar()
	{
		if ((bool)bar)
		{
			bar.SetValue(hp / hpMax);
		}
	}

	public virtual void IsAlive()
	{
	}
}
