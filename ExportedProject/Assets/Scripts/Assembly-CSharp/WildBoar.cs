using UnityEngine;

public class WildBoar : Creature
{
	public Collider[] hitBoxes;

	public Collider[] meatNodes;

	protected float tiredness;

	protected Vector3 runPos;

	private void SetBoarParameters()
	{
		idlePatrollMax = 5f;
		walkSpeed = 1.15f;
		runSpeed = 10f;
		runAwayRange = 100f;
		lookRange = 20f;
	}

	public void EnableMeatNodes(bool value)
	{
		Collider[] array = meatNodes;
		foreach (Collider collider in array)
		{
			collider.enabled = value;
		}
		Collider[] array2 = hitBoxes;
		foreach (Collider collider2 in array2)
		{
			collider2.enabled = !value;
		}
	}

	protected override void Start()
	{
		SetBoarParameters();
		base.Start();
		EnableMeatNodes(false);
		StateMachine.State stateById = stateMachine.GetStateById("idlePatrolling");
		stateById.AddLink("walkPatrolling", IdlePatrolling_WalkPatrolling);
		stateById.AddLink("runAwayFromTarget", AnyPatroll_RunAwayFromTarget);
		StateMachine.State stateById2 = stateMachine.GetStateById("walkPatrolling");
		stateById2.AddLink("idlePatrolling", WalkPatrolling_IdlePatrolling);
		stateById2.AddLink("runAwayFromTarget", AnyPatroll_RunAwayFromTarget);
		StateMachine.State stateById3 = stateMachine.GetStateById("runAwayFromTarget");
		stateById3.AddLink("walkPatrolling", RunAwayFromTarget_WalkPatrolling);
		stateById3.AddLink("runAwayFromTarget", RunAwayFromTarget_RunAwayFromTarget);
		stateMachine.SwitchStateTo(stateById);
	}

	private bool IdlePatrolling_WalkPatrolling()
	{
		if (Time.time >= idlePatrollBegin + idlePatrollMax)
		{
			return true;
		}
		return false;
	}

	private bool WalkPatrolling_IdlePatrolling()
	{
		if ((thisTransform.position - walkingPos).sqrMagnitude < 9f)
		{
			return true;
		}
		return false;
	}

	private bool AnyPatroll_RunAwayFromTarget()
	{
		GameObject nearestCreatureInRange = GetNearestCreatureInRange(lookRange);
		if (nearestCreatureInRange != null)
		{
			target = nearestCreatureInRange.transform;
			return true;
		}
		return false;
	}

	public GameObject GetNearestCreatureInRange(float range)
	{
		GameObject result = null;
		float num = range;
		foreach (Creature allCreature in Creature.allCreatures)
		{
			if (allCreature != this)
			{
				float num2 = Vector3.Distance(allCreature.transform.position, thisTransform.position);
				if (num2 < num)
				{
					result = allCreature.gameObject;
					num = num2;
				}
			}
		}
		return result;
	}

	private bool RunAwayFromTarget_WalkPatrolling()
	{
		if ((thisTransform.position - target.position).sqrMagnitude > runAwayRange * runAwayRange)
		{
			return true;
		}
		return false;
	}

	private bool RunAwayFromTarget_RunAwayFromTarget()
	{
		Vector3 vector = thisTransform.position - target.position;
		Vector3 vector2 = thisTransform.position - runAwayPoint;
		bool flag = vector.sqrMagnitude < runAwayRange * runAwayRange;
		bool flag2 = vector2.sqrMagnitude < runAwayPointRadius * runAwayPointRadius;
		if (flag && flag2)
		{
			return true;
		}
		return false;
	}

	public override void Die()
	{
		base.Die();
		EnableMeatNodes(true);
	}

	public override void OnNotLethalStrike(float damage, Transform damager)
	{
		base.OnNotLethalStrike(damage, damager);
		if (stateMachine.GetCurrState().id != "runAwayFromTarget")
		{
			stateMachine.SwitchStateTo(stateMachine.GetStateById("runAwayFromTarget"));
		}
	}

	private void Update()
	{
		stateMachine.Update();
	}

	public override void ScareBySound()
	{
	}
}
