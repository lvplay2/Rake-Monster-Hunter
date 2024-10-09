using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Creature : MonoBehaviour
{
	private class AvailablePoint
	{
		private Vector3 pos;

		private float targetAzimuthAngle;

		public AvailablePoint(Vector3 p, float t)
		{
			pos = p;
			targetAzimuthAngle = t;
		}
	}

	public static ArrayList allCreatures = new ArrayList();

	private static Transform navHelp = null;

	protected Transform walkPoints;

	protected Transform boxes;

	public Renderer[] renderers;

	public Animator anim;

	public Transform target;

	protected NavMeshAgent agent;

	protected StateMachine stateMachine = new StateMachine();

	protected float rayCastHeight = 100f;

	protected Vector3 walkingPos;

	protected float walkSpeed = 3.5f;

	protected float idlePatrollBegin;

	protected float idlePatrollMax = 3f;

	protected float runSpeed = 16f;

	protected float runAwayRange = 50f;

	protected Vector3 runAwayPoint;

	protected float runAwayPointRadius = 10f;

	protected Vector3 huntPoint;

	protected float huntPointRadius = 10f;

	protected float watchTimeMin = 20f;

	protected float watchTimeMax = 60f;

	protected float watchTime;

	protected float watchBegin;

	protected float freezeBegin;

	protected float freezeMax = 5f;

	protected float changePosForbidenRadius = 75f;

	protected bool changePosNoPosSelected;

	protected Vector3 changePosSelectedPos;

	protected float lookRange = 10f;

	protected float minRangeForAttack = 1.5f;

	protected float maxRangeForAttack = 2f;

	protected float eatDecoyStart;

	protected float eatDecoyTime = 10f;

	protected float carefulPatrolForbidenRadius;

	protected Vector3 lastValidDestination;

	protected bool isAlive = true;

	protected Transform thisTransform;

	protected Heals heals;

	private Ray rayToTerrain = default(Ray);

	protected bool relocationEnd;

	public Transform currDecoy;

	private void OnDestroy()
	{
		allCreatures.Remove(this);
	}

	protected virtual void Start()
	{
		AgentAnimDriver component = GetComponent<AgentAnimDriver>();
		if ((bool)component)
		{
			component.walk.realSpd = walkSpeed;
			component.run.realSpd = runSpeed;
		}
		allCreatures.Add(this);
		if (navHelp == null)
		{
			navHelp = GameObject.Find("NavigationHelp").transform;
		}
		walkPoints = navHelp.Find("TravelPoints");
		boxes = navHelp.Find("Boxes");
		heals = GetComponent<Heals>();
		thisTransform = base.transform;
		agent = GetComponent<NavMeshAgent>();
		StateMachine.State state = new StateMachine.State("waitForPlayerRess");
		stateMachine.AddState(state);
		state.OnEnter = delegate
		{
			agent.enabled = false;
		};
		StateMachine.State state2 = new StateMachine.State("goAwayFromTarget");
		stateMachine.AddState(state2);
		state2.OnEnter = GoAwayFromTarget_OnEnter;
		state2.Update = GoAwayFromTarget_Update;
		StateMachine.State state3 = new StateMachine.State("carefulPatrolling");
		stateMachine.AddState(state3);
		state3.OnEnter = CarefulPatrolling_OnEnter;
		StateMachine.State state4 = new StateMachine.State("walkPatrolling");
		stateMachine.AddState(state4);
		state4.OnEnter = WalkPatrolling_OnEnter;
		StateMachine.State state5 = new StateMachine.State("idlePatrolling");
		stateMachine.AddState(state5);
		state5.OnEnter = IdlePatrolling_OnEnter;
		state5.OnExit = IdlePatrolling_OnExit;
		StateMachine.State state6 = new StateMachine.State("runToTarget");
		stateMachine.AddState(state6);
		state6.OnEnter = RunToTarget_OnEnter;
		state6.Update = RunToTarget_Update;
		StateMachine.State state7 = new StateMachine.State("attackTarget");
		stateMachine.AddState(state7);
		state7.OnEnter = AttackTarget_OnEnter;
		state7.Update = AttackTarget_Update;
		StateMachine.State state8 = new StateMachine.State("runAwayFromTarget");
		stateMachine.AddState(state8);
		state8.OnEnter = RunAwayFromTarget_OnEnter;
		state8.Update = RunAwayFromTarget_Update;
		StateMachine.State state9 = new StateMachine.State("huntTarget");
		stateMachine.AddState(state9);
		state9.OnEnter = HuntTarget_OnEnter;
		StateMachine.State state10 = new StateMachine.State("watchTarget");
		stateMachine.AddState(state10);
		state10.OnEnter = WatchTarget_OnEnter;
		state10.Update = WatchTarget_Update;
		StateMachine.State state11 = new StateMachine.State("frozen");
		stateMachine.AddState(state11);
		state11.OnEnter = Frozen_OnEnter;
		StateMachine.State state12 = new StateMachine.State("gotoDecoy");
		stateMachine.AddState(state12);
		state12.OnEnter = GotoDecoy_OnEnter;
		StateMachine.State state13 = new StateMachine.State("eatDecoy");
		stateMachine.AddState(state13);
		state13.OnEnter = EatDecoy_OnEnter;
		state13.OnExit = EatDecoy_OnExit;
		StateMachine.State state14 = new StateMachine.State("changePosByTarget");
		stateMachine.AddState(state14);
		state14.OnEnter = ChangePosByTarget_OnEnter;
		state14.Update = ChangePosByTarget_Update;
		state14.Update = ChangePosByTarget_OnExit;
	}

	public bool IsAlive()
	{
		return isAlive;
	}

	public bool PositionInRadius(Vector3 pos, float radius)
	{
		if ((pos - thisTransform.position).sqrMagnitude < radius * radius)
		{
			return true;
		}
		return false;
	}

	public bool TransformInRadius(Transform t, float radius)
	{
		if (!t)
		{
			return false;
		}
		if ((t.position - thisTransform.position).sqrMagnitude < radius * radius)
		{
			return true;
		}
		return false;
	}

	public bool TargetInRadius(float radius)
	{
		return TransformInRadius(target, radius);
	}

	private void ChangePosByTarget_OnEnter()
	{
		float angle = 90f - Mathf.Acos(changePosForbidenRadius / Vector3.Distance(thisTransform.position, target.position)) * 57.29578f;
		ArrayList arrayList = new ArrayList();
		Vector3 lookDir = target.position - thisTransform.position;
		float center = JackUtils.AzimuthOfPointByTarget(thisTransform.position, target.position);
		for (int i = 0; i < walkPoints.childCount; i++)
		{
			Transform child = walkPoints.GetChild(i);
			bool flag = JackUtils.PointInAngle(thisTransform.position, lookDir, angle, child.position);
			float angle2 = JackUtils.AzimuthOfPointByTarget(child.position, target.position);
			if (!flag && !JackUtils.AngleInRage(angle2, center, 90f))
			{
				arrayList.Add(child);
			}
		}
		if (arrayList.Count > 0)
		{
			changePosNoPosSelected = false;
			int index = Random.Range(0, arrayList.Count);
			Transform transform = (Transform)arrayList[index];
			changePosSelectedPos = transform.position;
			agent.enabled = true;
			agent.speed = runSpeed;
			agent.SetDestination(changePosSelectedPos);
		}
		else
		{
			changePosNoPosSelected = true;
			agent.enabled = false;
		}
	}

	private void ChangePosByTarget_Update()
	{
	}

	private void ChangePosByTarget_OnExit()
	{
	}

	private void CarefulPatrolling_OnEnter()
	{
		float angle = 90f - Mathf.Acos(carefulPatrolForbidenRadius / Vector3.Distance(thisTransform.position, target.position)) * 57.29578f;
		ArrayList arrayList = new ArrayList();
		Vector3 lookDir = target.position - thisTransform.position;
		for (int i = 0; i < walkPoints.childCount; i++)
		{
			Transform child = walkPoints.GetChild(i);
			if (!JackUtils.PointInAngle(thisTransform.position, lookDir, angle, child.position))
			{
				arrayList.Add(child);
			}
		}
		int count = arrayList.Count;
		if (count > 0)
		{
			int index = Random.Range(0, count);
			walkingPos = ((Transform)arrayList[index]).position;
		}
		else
		{
			int childCount = walkPoints.childCount;
			int index2 = Random.Range(0, childCount);
			walkingPos = walkPoints.GetChild(index2).position;
		}
		agent.enabled = true;
		agent.speed = walkSpeed;
		agent.SetDestination(walkingPos);
		currDecoy = null;
	}

	private void WalkPatrolling_OnEnter()
	{
		int childCount = walkPoints.childCount;
		int index = Random.Range(0, childCount);
		walkingPos = walkPoints.GetChild(index).position;
		agent.enabled = true;
		agent.speed = walkSpeed;
		agent.SetDestination(walkingPos);
		currDecoy = null;
	}

	private void IdlePatrolling_OnEnter()
	{
		agent.enabled = false;
		idlePatrollBegin = Time.time;
	}

	private void IdlePatrolling_OnExit()
	{
	}

	private void RunToTarget_OnEnter()
	{
		agent.enabled = true;
		agent.speed = runSpeed;
		agent.SetDestination(target.position);
	}

	private void RunToTarget_Update()
	{
		agent.SetDestination(target.position);
	}

	private void AttackTarget_OnEnter()
	{
		agent.enabled = false;
	}

	private void AttackTarget_Update()
	{
		Vector3 forward = target.position - thisTransform.position;
		forward.y = 0f;
		thisTransform.rotation = Quaternion.LookRotation(forward);
	}

	private void GoAwayFromTarget_OnEnter()
	{
		agent.enabled = true;
		agent.speed = walkSpeed;
		Vector3 destination = (runAwayPoint = GetRunAwayPoint());
		agent.SetDestination(destination);
	}

	private void GoAwayFromTarget_Update()
	{
	}

	public Vector3 GetRunAwayPoint()
	{
		Vector3 a = thisTransform.position - target.position;
		a.Normalize();
		a = Vector3.Scale(a, new Vector3(runAwayRange + 20f, runAwayRange + 20f, runAwayRange + 20f));
		a += thisTransform.position;
		if (!DestinationVerifyByBoxes(a) || !DestinationVerifyByTerrain(a, out a))
		{
			a = GetOverRunAwayRangeWalkPoint();
		}
		return a;
	}

	private void RunAwayFromTarget_OnEnter()
	{
		agent.enabled = true;
		agent.speed = runSpeed;
		Vector3 destination = (runAwayPoint = GetRunAwayPoint());
		agent.SetDestination(destination);
	}

	private bool DestinationVerifyByBoxes(Vector3 point)
	{
		BoxCollider[] componentsInChildren = boxes.GetComponentsInChildren<BoxCollider>();
		BoxCollider[] array = componentsInChildren;
		foreach (BoxCollider boxCollider in array)
		{
			if (boxCollider.bounds.Contains(point))
			{
				return true;
			}
		}
		return false;
	}

	private bool DestinationVerifyByTerrain(Vector3 point, out Vector3 hit)
	{
		rayToTerrain.origin = new Vector3(point.x, point.y + rayCastHeight, point.z);
		rayToTerrain.direction = Vector3.down;
		int mask = LayerMask.GetMask("Terrain");
		RaycastHit hitInfo;
		bool result = Physics.Raycast(rayToTerrain, out hitInfo, rayCastHeight * 2f, mask);
		hit = hitInfo.point;
		return result;
	}

	private Vector3 GetMostFarWalkPoint()
	{
		int childCount = walkPoints.childCount;
		Vector3 vector = walkPoints.GetChild(0).position;
		float num = (vector - thisTransform.position).sqrMagnitude;
		for (int i = 0; i < childCount; i++)
		{
			Vector3 position = walkPoints.GetChild(i).position;
			float sqrMagnitude = (thisTransform.position - position).sqrMagnitude;
			if (sqrMagnitude > num)
			{
				num = sqrMagnitude;
				vector = position;
			}
		}
		return vector;
	}

	private Vector3 GetOverRunAwayRangeWalkPoint()
	{
		int childCount = walkPoints.childCount;
		float num = runAwayRange + 20f;
		ArrayList arrayList = new ArrayList();
		for (int i = 0; i < childCount; i++)
		{
			Vector3 position = walkPoints.GetChild(i).position;
			float magnitude = (thisTransform.position - position).magnitude;
			if (magnitude > num)
			{
				arrayList.Add(position);
			}
		}
		Vector3 result = Vector3.zero;
		if (arrayList.Count > 0)
		{
			result = (Vector3)arrayList[Random.Range(0, arrayList.Count)];
		}
		return result;
	}

	private void RunAwayFromTarget_Update()
	{
	}

	private void HuntTarget_OnEnter()
	{
		agent.enabled = true;
		agent.speed = walkSpeed;
		huntPoint = target.position;
		agent.SetDestination(huntPoint);
		currDecoy = null;
	}

	private void HidenRelocation_OnEnter()
	{
		relocationEnd = false;
	}

	private void HidenRelocation_Check()
	{
	}

	private void WatchTarget_OnEnter()
	{
		agent.enabled = false;
		watchTime = Random.Range(watchTimeMin, watchTimeMax);
		watchBegin = Time.time;
	}

	private void WatchTarget_Update()
	{
	}

	private void Frozen_OnEnter()
	{
		agent.enabled = false;
		freezeBegin = Time.time;
	}

	private bool Frozen_RunAwasFromTarget()
	{
		if (freezeBegin + freezeMax < Time.time)
		{
			return true;
		}
		return false;
	}

	private void GotoDecoy_OnEnter()
	{
		agent.enabled = true;
		agent.SetDestination(currDecoy.position);
		agent.speed = walkSpeed;
	}

	private void EatDecoy_OnEnter()
	{
		agent.enabled = false;
		anim.SetTrigger("Eat");
		eatDecoyStart = Time.time;
	}

	private void EatDecoy_OnExit()
	{
		anim.SetTrigger("OnGround");
		if ((bool)currDecoy)
		{
			Object.Destroy(currDecoy.gameObject);
		}
	}

	public virtual void TakeDamage(float damage, Transform damager)
	{
		if (!(heals.hp <= 0f))
		{
			Debug.Log(base.gameObject.name + " take damage " + damage);
			heals.TakeDamage(damage);
			if (heals.hp > 0f)
			{
				OnNotLethalStrike(damage, damager);
			}
			else
			{
				OnLethalStrike();
			}
		}
	}

	public virtual void ScareByRocket()
	{
	}

	public virtual void ScareBySound()
	{
	}

	public virtual void OnNotLethalStrike(float damage, Transform damager)
	{
	}

	public virtual void OnLethalStrike()
	{
		isAlive = false;
		Die();
	}

	public virtual void Die()
	{
		stateMachine.SwitchStateTo(null);
		anim.SetTrigger("Death");
		agent.enabled = false;
		Collider component = GetComponent<Collider>();
		if ((bool)component)
		{
			component.isTrigger = true;
		}
	}

	public virtual void Heal(float heal)
	{
		heals.Heal(heal);
	}

	public virtual void Freeze(float time)
	{
		if (!(heals.hp <= 0f))
		{
			freezeMax = time;
			stateMachine.SwitchStateTo(stateMachine.GetStateById("frozen"));
		}
	}
}
