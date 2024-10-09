using UnityEngine;

public class CreatureHitBox : MonoBehaviour
{
	protected Creature creature;

	public Creature useOtherCreature;

	public float hitValue = 1f;

	private void Start()
	{
		if (useOtherCreature != null)
		{
			creature = useOtherCreature;
		}
		else
		{
			creature = base.gameObject.GetComponentInParent<Creature>();
		}
		if (creature == null)
		{
			Debug.Log("hit box cant find a creature");
		}
	}

	private void Update()
	{
	}

	public void TakeDamage(float dmg, Transform damager)
	{
		if (creature != null)
		{
			creature.TakeDamage(dmg * hitValue, damager);
		}
	}

	public Creature GetCreature()
	{
		return creature;
	}
}
