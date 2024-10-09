using System;
using UnityEngine;

public class MonsterBigFoot : Monster
{
	public float damagePerAnim = 15f;

	public AudioSource voiceSource;

	public PlaySound monsterFarSounds;

	public AudioClip nearGnarling;

	public float gnarlingDistance = 15f;

	protected float damageForScare = 20f;

	protected float takenDamage;

	protected float strikesForRetreate = 1f;

	protected float strikesMaked;

	protected StateMachine attackMachine = new StateMachine();

	protected MonsterPaw[] paws;

	protected DifficultLevel currDiff;

	protected float firstScreamAfter = 15f;

	protected bool wasFirstScream;

	protected float forceAttackAfter = 120f;

	protected bool wasForeceAttack;

	protected bool watchNearFire;

	private bool isRunAwayNow;

	private bool isAttackState;

	private void SetMonsterParameters()
	{
		currDiff = Difficult.Instance.GetSelectedLevel();
		damagePerAnim *= currDiff.monsterMakeDamage;
		runSpeed = 10f;
		walkSpeed = 2.5f;
		runAwayRange = 100f;
		lookRange = 45f;
		AgentAnimDriver component = GetComponent<AgentAnimDriver>();
		component.walk.realSpd = walkSpeed;
		component.run.realSpd = runSpeed;
	}

	protected override void Start()
	{
		firstScreamAfter += Time.time;
		forceAttackAfter += Time.time;
		SetMonsterParameters();
		SelectSpawnPoint();
		base.Start();
		stateMachine.isLoggingEnabled = true;
		stateMachine.logName = base.gameObject.name;
		StateMachine.State stateById = stateMachine.GetStateById("idlePatrolling");
		stateById.AddLink("walkPatrolling", IdlePatrolling_WalkPatrolling);
		stateById.AddLink("watchTarget", Patrolling_RunToTarget);
		stateById.AddLink("runAwayFromTarget", IsMonsterScared);
		StateMachine.State stateById2 = stateMachine.GetStateById("walkPatrolling");
		stateById2.AddLink("idlePatrolling", WalkPatrolling_IdlePatrolling);
		stateById2.AddLink("watchTarget", Patrolling_RunToTarget);
		stateById2.AddLink("runAwayFromTarget", IsMonsterScared);
		stateById2.AddLink("gotoDecoy", delegate
		{
			Transform nearestDecoyInRange2 = GetNearestDecoyInRange(lookRange);
			if ((bool)nearestDecoyInRange2)
			{
				currDecoy = nearestDecoyInRange2;
			}
			return nearestDecoyInRange2 != null;
		});
		StateMachine.State stateById3 = stateMachine.GetStateById("runToTarget");
		stateById3.OnEnter = (StateMachine.OnEvent)Delegate.Combine(stateById3.OnEnter, (StateMachine.OnEvent)delegate
		{
			isAttackState = true;
			PlayerPrefs.SetInt("WatchBF", 1);
		});
		stateById3.OnExit = (StateMachine.OnEvent)Delegate.Combine(stateById3.OnExit, (StateMachine.OnEvent)delegate
		{
			isAttackState = false;
		});
		stateById3.AddLink("attackTarget", RunToTarget_AttackTarget);
		stateById3.AddLink("runAwayFromTarget", MonsterRetreate);
		StateMachine.State stateById4 = stateMachine.GetStateById("attackTarget");
		stateById4.OnEnter = (StateMachine.OnEvent)Delegate.Combine(stateById4.OnEnter, (StateMachine.OnEvent)delegate
		{
			isAttackState = true;
		});
		stateById4.OnExit = (StateMachine.OnEvent)Delegate.Combine(stateById4.OnExit, (StateMachine.OnEvent)delegate
		{
			isAttackState = false;
		});
		stateById4.AddLink("runToTarget", AttackTarget_RunToTarget);
		stateById4.AddLink("runAwayFromTarget", MonsterRetreate);
		StateMachine.State stateById5 = stateMachine.GetStateById("runAwayFromTarget");
		stateById5.OnEnter = (StateMachine.OnEvent)Delegate.Combine(stateById5.OnEnter, (StateMachine.OnEvent)delegate
		{
			isRunAwayNow = true;
			ResetMonsterScare();
		});
		stateById5.OnExit = RunAwayFromTarget_OnExit;
		stateById5.OnExit = (StateMachine.OnEvent)Delegate.Combine(stateById5.OnExit, (StateMachine.OnEvent)delegate
		{
			isRunAwayNow = false;
			ResetMonsterScare();
		});
		stateById5.AddLink("changePosByTarget", RunAwayFromTarget_HuntTarget);
		stateById5.AddLink("runAwayFromTarget", RunAwayFromTarget_RunAwayFromTarget);
		StateMachine.State stateById6 = stateMachine.GetStateById("huntTarget");
		stateById6.AddLink("watchTarget", HuntTarget_RunToTarget);
		stateById6.AddLink("huntTarget", HuntTarget_HuntTarget);
		stateById6.AddLink("runAwayFromTarget", IsMonsterScared);
		stateById6.AddLink("gotoDecoy", delegate
		{
			Transform nearestDecoyInRange = GetNearestDecoyInRange(lookRange);
			if ((bool)nearestDecoyInRange)
			{
				currDecoy = nearestDecoyInRange;
			}
			return nearestDecoyInRange != null;
		});
		StateMachine.State stateById7 = stateMachine.GetStateById("watchTarget");
		stateById7.OnEnter = (StateMachine.OnEvent)Delegate.Combine(stateById7.OnEnter, (StateMachine.OnEvent)delegate
		{
			takenDamage = 0f;
			if (UnityEngine.Random.Range(0, 3) == 0 || PlayerPrefs.GetInt("WatchBF") == 0)
			{
				monsterFarSounds.PlayRand("farScream");
			}
		});
		stateById7.AddLink("runAwayFromTarget", IsMonsterScared);
		stateById7.AddLink("runToTarget", () => TargetInRadius(lookRange - 5f) || !TargetInRadius(lookRange + 5f) || Time.time > watchBegin + watchTime || TakedAnyDamage());
		StateMachine.State stateById8 = stateMachine.GetStateById("gotoDecoy");
		stateById8.AddLink("attackTarget", () => TargetInRadius(lookRange / 2f));
		stateById8.AddLink("eatDecoy", () => TransformInRadius(currDecoy, 0.1f) ? true : false);
		stateById8.AddLink("runAwayFromTarget", () => IsMonsterScared());
		StateMachine.State stateById9 = stateMachine.GetStateById("eatDecoy");
		stateById9.AddLink("attackTarget", () => TargetInRadius(lookRange / 2f));
		stateById9.AddLink("runAwayFromTarget", () => IsMonsterScared());
		stateById9.AddLink("walkPatrolling", () => Time.time > eatDecoyStart + eatDecoyTime);
		StateMachine.State stateById10 = stateMachine.GetStateById("frozen");
		stateById10.OnEnter = (StateMachine.OnEvent)Delegate.Combine(stateById10.OnEnter, (StateMachine.OnEvent)delegate
		{
			if (TransformInRadius(currDecoy, 2f))
			{
				UnityEngine.Object.Destroy(currDecoy.gameObject);
			}
			if (TargetInRadius(lookRange / 2f))
			{
				monsterFarSounds.PlayRand("trollScream");
			}
			else
			{
				monsterFarSounds.PlayRand("painScream");
			}
		});
		stateById10.AddLink("runAwayFromTarget", () => Time.time > freezeBegin + freezeMax && TargetInRadius(lookRange));
		stateById10.AddLink("walkPatrolling", () => Time.time > freezeBegin + freezeMax && !TargetInRadius(lookRange));
		StateMachine.State stateById11 = stateMachine.GetStateById("changePosByTarget");
		stateById11.AddLink("runAwayFromTarget", () => TransformInRadius(target, lookRange));
		stateById11.AddLink("huntTarget", () => PositionInRadius(changePosSelectedPos, 1f) || changePosNoPosSelected);
		stateMachine.SwitchStateTo(stateById2);
		StateMachine.State state = new StateMachine.State("waitForAttack");
		attackMachine.AddState(state);
		state.OnEnter = WaitForAttack_OnEnter;
		state.AddLink("attackingNow", () => MonsterCanAttack());
		StateMachine.State state2 = new StateMachine.State("attackingNow");
		attackMachine.AddState(state2);
		state2.OnEnter = AttackingNow_OnEnter;
		state2.AddLink("waitForAttack", () => !MonsterCanAttack());
		attackMachine.SwitchStateTo(state);
		SetAnimDelegates();
		paws = GetComponentsInChildren<MonsterPaw>();
	}

	private bool MonsterCanAttack()
	{
		return isAttackState && !isRunAwayNow && heals.hp > 0f && TargetInRadius(5f);
	}

	private void SetAnimDelegates()
	{
		SimpleDelegation simpleDelegation = anim.gameObject.AddComponent<SimpleDelegation>();
		simpleDelegation.Add("OnAnimAttackStart", OnAnimAttackStart);
		simpleDelegation.Add("OnAnimAttackEnd", OnAnimAttackEnd);
	}

	private void OnAnimAttackStart()
	{
		Debug.Log("OnAnimAttackStart");
		MonsterPaw[] array = paws;
		foreach (MonsterPaw monsterPaw in array)
		{
			monsterPaw.SetPower(damagePerAnim);
		}
	}

	private void OnAnimAttackEnd()
	{
		Debug.Log("OnAnimAttackEnd");
		MonsterPaw[] array = paws;
		foreach (MonsterPaw monsterPaw in array)
		{
			monsterPaw.SetPower(0f);
		}
	}

	public override void StrikeSucces()
	{
		strikesMaked += 1f;
	}

	private Transform GetNearestDecoyInRange(float maxRadius)
	{
		Transform result = null;
		float num = maxRadius;
		foreach (MeatInstance item in MeatInstance.all)
		{
			float magnitude = (item.transform.position - thisTransform.position).magnitude;
			if (magnitude < num)
			{
				num = magnitude;
				result = item.transform;
			}
		}
		return result;
	}

	private void WaitForAttack_OnEnter()
	{
		anim.SetBool("ToAttack", false);
	}

	private void AttackingNow_OnEnter()
	{
		anim.SetBool("ToAttack", true);
	}

	private void AttackTarget_OnExit()
	{
		strikesMaked = 0f;
		takenDamage = 0f;
	}

	private void RunAwayFromTarget_OnExit()
	{
		strikesMaked = 0f;
		takenDamage = 0f;
		watchNearFire = false;
	}

	private bool RunAwayFromTarget_HuntTarget()
	{
		return RunAwayFromTarget_IdlePatrolling();
	}

	private bool HuntTarget_RunToTarget()
	{
		return Patrolling_RunToTarget();
	}

	private bool HuntTarget_HuntTarget()
	{
		if ((thisTransform.position - huntPoint).sqrMagnitude < huntPointRadius * huntPointRadius)
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

	private bool IdlePatrolling_WalkPatrolling()
	{
		if (Time.time >= idlePatrollBegin + idlePatrollMax)
		{
			return true;
		}
		return false;
	}

	private bool RunToTarget_AttackTarget()
	{
		if ((thisTransform.position - target.position).sqrMagnitude < minRangeForAttack * minRangeForAttack)
		{
			return true;
		}
		return false;
	}

	private bool AttackTarget_RunToTarget()
	{
		if ((thisTransform.position - target.position).sqrMagnitude > maxRangeForAttack * maxRangeForAttack)
		{
			return true;
		}
		return false;
	}

	private bool RunAwayFromTarget_IdlePatrolling()
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

	private bool Patrolling_RunToTarget()
	{
		if ((thisTransform.position - target.position).sqrMagnitude < lookRange * lookRange)
		{
			return true;
		}
		return false;
	}

	private bool MonsterRetreate()
	{
		if (IsMonsterScared() || strikesMaked >= strikesForRetreate)
		{
			return true;
		}
		return false;
	}

	private bool IsMonsterScared()
	{
		if (takenDamage >= damageForScare || watchNearFire)
		{
			return true;
		}
		return false;
	}

	private void ResetMonsterScare()
	{
		watchNearFire = false;
		takenDamage = 0f;
	}

	private bool TakedAnyDamage()
	{
		if (takenDamage > 0f)
		{
			return true;
		}
		return false;
	}

	public override void TakeDamage(float damage, Transform damager)
	{
		if (currDiff != null)
		{
			damage *= currDiff.monsterTakeDamage;
		}
		else
		{
			Debug.Log("!currDiff");
		}
		base.TakeDamage(damage, damager);
	}

	public override void OnNotLethalStrike(float damage, Transform damager)
	{
		base.OnNotLethalStrike(damage, damager);
		takenDamage += damage;
	}

	public override void Die()
	{
		Player component = target.GetComponent<Player>();
		component.OnMonsterDie();
		base.Die();
	}

	private void FixedUpdate()
	{
		if (PlayerPrefs.GetInt("WatchBF") == 0)
		{
			if (Time.time > forceAttackAfter && !wasForeceAttack)
			{
				Debug.Log("First play -- force attack!" + Time.time);
				stateMachine.SwitchStateTo(stateMachine.GetStateById("runToTarget"));
				wasForeceAttack = true;
			}
			if (Time.time > firstScreamAfter && !wasFirstScream)
			{
				Debug.Log("First play -- scream!" + Time.time);
				monsterFarSounds.PlayRand("farScream");
				wasFirstScream = true;
			}
		}
		if ((target.position - thisTransform.position).sqrMagnitude < gnarlingDistance * gnarlingDistance)
		{
			if (!voiceSource.isPlaying)
			{
				voiceSource.clip = nearGnarling;
				voiceSource.loop = true;
				voiceSource.Play();
			}
		}
		else if (voiceSource.isPlaying)
		{
			voiceSource.Stop();
		}
	}

	private void Update()
	{
		stateMachine.Update();
		attackMachine.Update();
	}

	public override void ScareByRocket()
	{
		watchNearFire = true;
	}
}
