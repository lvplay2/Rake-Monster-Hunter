using System;
using System.Collections;
using UnityEngine;

public class MonsterRake : Monster
{
	public float damagePerAnim = 15f;

	public AudioSource voiceSource;

	public PlaySound monsterSounds;

	public AudioClip nearGnarling;

	public float gnarlingDistance = 15f;

	protected float getoutReactionRange;

	protected float damageForRetreate = 20f;

	protected float takenDamage;

	protected float strikesForRetreate = 1f;

	protected float strikesMaked;

	protected float decoyLookRange;

	protected StateMachine attackMachine = new StateMachine();

	protected MonsterPaw[] paws;

	protected DifficultLevel currDiff;

	protected float firstScreamAfter = 15f;

	protected bool wasFirstScream;

	protected float firstAttackAfter = 90f;

	protected bool wasFirstAttack;

	protected bool watchNearFire;

	protected Heals targetHeals;

	protected bool jumpEscapeNoWay;

	protected Vector3 jumpEscapeFinalPos;

	protected JumpAgent jagent;

	protected int trappingLimit = 3;

	protected float trappingTimeout = 60f;

	protected float trappingLittleTimeout = 30f;

	protected int wasTrapedInLastTime;

	protected float timerForMeat;

	protected float minTimedScreamDelay = 180f;

	protected float maxTimedScreamDelay = 240f;

	protected float delayTimedScream;

	protected float lastTimedScreamTime;

	protected float minTimedAttackDelay = 300f;

	protected float maxTimedAttackDelay = 420f;

	protected float delayTimedAttack;

	protected float lastTimedAttackTime;

	private bool isRunAwayNow;

	private bool isAttackState;

	private void SetMonsterParameters()
	{
		jagent = GetComponent<JumpAgent>();
		currDiff = Difficult.Instance.GetSelectedLevel();
		damagePerAnim *= currDiff.monsterMakeDamage;
		runSpeed = 16f;
		walkSpeed = 2.5f;
		decoyLookRange = 200f;
		lookRange = 40f;
		getoutReactionRange = 47f;
		runAwayRange = 150f;
		changePosForbidenRadius = 130f;
		carefulPatrolForbidenRadius = 75f;
		eatDecoyTime = 120f;
		minRangeForAttack = 2f;
		maxRangeForAttack = 3f;
		AgentAnimDriver component = GetComponent<AgentAnimDriver>();
		component.walk.realSpd = walkSpeed;
		component.run.realSpd = runSpeed;
		targetHeals = target.GetComponent<Heals>();
		wasTrapedInLastTime = 0;
		timerForMeat = Time.time;
	}

	protected override void Start()
	{
		lastTimedScreamTime = Time.time;
		delayTimedScream = UnityEngine.Random.Range(minTimedScreamDelay, maxTimedScreamDelay);
		lastTimedAttackTime = Time.time;
		delayTimedAttack = UnityEngine.Random.Range(minTimedAttackDelay, maxTimedAttackDelay);
		firstScreamAfter += Time.time;
		firstAttackAfter += Time.time;
		SetMonsterParameters();
		SelectSpawnPoint();
		base.Start();
		stateMachine.isLoggingEnabled = true;
		stateMachine.logName = base.gameObject.name;
		StateMachine.State stateById = stateMachine.GetStateById("idlePatrolling");
		stateById.AddLink("carefulPatrolling", IdlePatrolling_WalkPatrolling);
		stateById.AddLink("runToTarget", Common_Aggr);
		stateById.AddLink("jumpEscaping", IsMonsterScared);
		StateMachine.State stateById2 = stateMachine.GetStateById("carefulPatrolling");
		stateById2.Update = delegate
		{
			if (PlayerPrefs.GetInt("WatchBF") == 1)
			{
				if (!TargetInRadius(100f) && Time.time > lastTimedScreamTime + delayTimedScream)
				{
					Debug.LogWarning("TimedScream " + Time.time);
					monsterSounds.PlayRand("farScream");
					lastTimedScreamTime = Time.time;
					delayTimedScream = UnityEngine.Random.Range(minTimedScreamDelay, maxTimedScreamDelay);
				}
				if (!TargetInRadius(100f) && Time.time > lastTimedAttackTime + delayTimedAttack)
				{
					delayTimedAttack = UnityEngine.Random.Range(minTimedAttackDelay, maxTimedAttackDelay);
					Debug.LogWarning("TimedAttack " + Time.time);
					stateMachine.SwitchStateTo(stateMachine.GetStateById("fastFindTarget"));
				}
			}
		};
		stateById2.AddLink("idlePatrolling", WalkPatrolling_IdlePatrolling);
		stateById2.AddLink("runToTarget", Common_Aggr);
		stateById2.AddLink("jumpEscaping", IsMonsterScared);
		stateById2.AddLink("gotoDecoy", delegate
		{
			if (Time.time >= timerForMeat)
			{
				Transform nearestDecoyInRange3 = GetNearestDecoyInRange(decoyLookRange);
				if ((bool)nearestDecoyInRange3)
				{
					currDecoy = nearestDecoyInRange3;
				}
				return nearestDecoyInRange3 != null;
			}
			return false;
		});
		StateMachine.State stateById3 = stateMachine.GetStateById("goAwayFromTarget");
		stateById3.AddLink("carefulPatrolling", () => !TransformInRadius(target, runAwayRange));
		stateById3.AddLink("runToTarget", Common_Aggr);
		stateById3.AddLink("jumpEscaping", IsMonsterScared);
		stateById3.AddLink("gotoDecoy", delegate
		{
			Transform nearestDecoyInRange2 = GetNearestDecoyInRange(decoyLookRange);
			if ((bool)nearestDecoyInRange2)
			{
				currDecoy = nearestDecoyInRange2;
			}
			return nearestDecoyInRange2 != null;
		});
		StateMachine.State stateById4 = stateMachine.GetStateById("runToTarget");
		stateById4.OnEnter = (StateMachine.OnEvent)Delegate.Combine(stateById4.OnEnter, (StateMachine.OnEvent)delegate
		{
			isAttackState = true;
			PlayerPrefs.SetInt("WatchBF", 1);
			lastTimedScreamTime = Time.time;
			lastTimedAttackTime = Time.time;
		});
		stateById4.OnExit = (StateMachine.OnEvent)Delegate.Combine(stateById4.OnExit, (StateMachine.OnEvent)delegate
		{
			isAttackState = false;
		});
		stateById4.AddLink("attackTarget", RunToTarget_AttackTarget);
		stateById4.AddLink("jumpEscaping", MonsterRetreate);
		StateMachine.State stateById5 = stateMachine.GetStateById("attackTarget");
		stateById5.OnEnter = (StateMachine.OnEvent)Delegate.Combine(stateById5.OnEnter, (StateMachine.OnEvent)delegate
		{
			isAttackState = true;
		});
		stateById5.OnExit = (StateMachine.OnEvent)Delegate.Combine(stateById5.OnExit, (StateMachine.OnEvent)delegate
		{
			isAttackState = false;
		});
		stateById5.AddLink("runToTarget", AttackTarget_RunToTarget);
		stateById5.AddLink("jumpEscaping", MonsterRetreate);
		StateMachine.State stateById6 = stateMachine.GetStateById("runAwayFromTarget");
		stateById6.OnEnter = (StateMachine.OnEvent)Delegate.Combine(stateById6.OnEnter, (StateMachine.OnEvent)delegate
		{
			isRunAwayNow = true;
		});
		stateById6.OnExit = RunAwayFromTarget_OnExit;
		stateById6.OnExit = (StateMachine.OnEvent)Delegate.Combine(stateById6.OnExit, (StateMachine.OnEvent)delegate
		{
			isRunAwayNow = false;
		});
		stateById6.AddLink("waitForPlayerRess", () => !TransformInRadius(target, runAwayRange) && IsTargetDead());
		stateById6.AddLink("changePosByTarget", () => !TransformInRadius(target, runAwayRange));
		stateById6.AddLink("runAwayFromTarget", RunAwayFromTarget_RunAwayFromTarget);
		StateMachine.State stateById7 = stateMachine.GetStateById("waitForPlayerRess");
		stateById7.AddLink("carefulPatrolling", () => !IsTargetDead());
		StateMachine.State state = new StateMachine.State("fastFindTarget");
		stateMachine.AddState(state);
		state.OnEnter = delegate
		{
			if (!agent.enabled)
			{
				agent.enabled = true;
			}
			agent.speed = runSpeed;
		};
		state.Update = delegate
		{
			agent.destination = target.position;
		};
		state.AddLink("runToTarget", Common_Aggr);
		StateMachine.State stateById8 = stateMachine.GetStateById("gotoDecoy");
		stateById8.OnEnter = (StateMachine.OnEvent)Delegate.Combine(stateById8.OnEnter, (StateMachine.OnEvent)delegate
		{
			if (wasTrapedInLastTime == 0)
			{
				Debug.LogWarning("MEAT YEAH!!!");
			}
		});
		stateById8.AddLink("attackTarget", Common_Aggr);
		stateById8.AddLink("eatDecoy", () => TransformInRadius(currDecoy, 0.15f));
		stateById8.AddLink("idlePatrolling", () => currDecoy == null && Vector3.Distance(agent.destination, thisTransform.position) <= 20f);
		StateMachine.State stateById9 = stateMachine.GetStateById("eatDecoy");
		stateById9.AddLink("attackTarget", Common_Aggr);
		stateById9.AddLink("carefulPatrolling", () => Time.time > eatDecoyStart + eatDecoyTime);
		StateMachine.State stateById10 = stateMachine.GetStateById("frozen");
		stateById10.OnEnter = (StateMachine.OnEvent)Delegate.Combine(stateById10.OnEnter, (StateMachine.OnEvent)delegate
		{
			lastTimedScreamTime = Time.time;
			lastTimedAttackTime = Time.time;
			Transform nearestDecoyInRange = GetNearestDecoyInRange(2f);
			if (nearestDecoyInRange != null)
			{
				UnityEngine.Object.Destroy(nearestDecoyInRange.gameObject);
				wasTrapedInLastTime++;
				if (wasTrapedInLastTime >= trappingLimit)
				{
					Debug.LogWarning("Pain! NO MORE meat!");
					wasTrapedInLastTime = 0;
					timerForMeat = Time.time + trappingTimeout;
				}
				else
				{
					Debug.LogWarning("Pain! WTF?");
					if (Time.time >= timerForMeat)
					{
						timerForMeat = Time.time + trappingLittleTimeout;
					}
				}
			}
			if (TargetInRadius(lookRange / 2f))
			{
				monsterSounds.PlayRand("trollScream");
			}
			else
			{
				monsterSounds.PlayRand("painScream");
			}
		});
		stateById10.AddLink("runToTarget", () => Time.time > freezeBegin + freezeMax && TargetInRadius(lookRange));
		stateById10.AddLink("carefulPatrolling", () => Time.time > freezeBegin + freezeMax && !TargetInRadius(lookRange));
		StateMachine.State stateById11 = stateMachine.GetStateById("changePosByTarget");
		stateById11.AddLink("runAwayFromTarget", () => TransformInRadius(target, lookRange));
		stateById11.AddLink("carefulPatrolling", () => changePosNoPosSelected || PositionInRadius(changePosSelectedPos, 1f));
		StateMachine.State state2 = new StateMachine.State("jumpEscaping");
		stateMachine.AddState(state2);
		state2.OnEnter = delegate
		{
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = new ArrayList();
			Vector3 position = target.position;
			RaycastHit[] array = Physics.SphereCastAll(position, 40f, Vector3.up, 0f, LayerMask.GetMask("CastShadows"), QueryTriggerInteraction.UseGlobal);
			Debug.Log("SphereCastAll");
			Vector3 position2 = target.position;
			Vector3 forward = target.forward;
			if (array != null && array.Length > 0)
			{
				RaycastHit[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					RaycastHit raycastHit = array2[i];
					GameObject gameObject = raycastHit.collider.gameObject;
					Transform transform = raycastHit.collider.transform;
					Vector3 position3 = transform.position;
					if (gameObject.tag == "Tree")
					{
						if (JackUtils.PointInAngle(position2, forward, 20f, position3) && Vector3.Distance(position2, position3) > 10f)
						{
							arrayList.Add(transform);
						}
						arrayList2.Add(transform);
					}
				}
			}
			Transform transform2 = Unical.Get("t4sample").transform;
			if (arrayList2.Count >= 3 && arrayList.Count >= 1)
			{
				Transform transform3 = null;
				float num = 100f;
				foreach (Transform item in arrayList)
				{
					float num2 = Vector3.Distance(item.position, thisTransform.position);
					if (num2 < num)
					{
						transform3 = item;
						num = num2;
					}
				}
				arrayList2.Remove(transform3);
				Vector3 vector = transform3.TransformPoint(transform2.Find("J1").localPosition);
				int index = UnityEngine.Random.Range(0, arrayList2.Count);
				Vector3 vector2 = ((Transform)arrayList2[index]).TransformPoint(transform2.Find("J2").localPosition);
				arrayList2.RemoveAt(index);
				index = UnityEngine.Random.Range(0, arrayList2.Count);
				Vector3 vector3 = ((Transform)arrayList2[index]).TransformPoint(transform2.Find("J3").localPosition);
				arrayList2.RemoveAt(index);
				agent.enabled = false;
				jumpEscapeFinalPos = GetRunAwayPoint();
				jagent.jumpPoints = new Vector3[4] { vector, vector2, vector3, jumpEscapeFinalPos };
				jagent.enabled = true;
				jumpEscapeNoWay = false;
			}
			else
			{
				jumpEscapeNoWay = true;
			}
		};
		state2.Update = delegate
		{
		};
		state2.OnExit = delegate
		{
			lastTimedScreamTime = Time.time;
			lastTimedAttackTime = Time.time;
			agent.enabled = true;
			jagent.enabled = false;
		};
		state2.AddLink("runAwayFromTarget", () => jumpEscapeNoWay);
		state2.AddLink("waitForPlayerRess", () => PositionInRadius(jumpEscapeFinalPos, 1f) && IsTargetDead());
		state2.AddLink("changePosByTarget", () => PositionInRadius(jumpEscapeFinalPos, 1f));
		stateMachine.SwitchStateTo(stateById2);
		StateMachine.State state3 = new StateMachine.State("waitForAttack");
		attackMachine.AddState(state3);
		state3.OnEnter = WaitForAttack_OnEnter;
		state3.AddLink("attackingNow", () => MonsterCanAttack());
		StateMachine.State state4 = new StateMachine.State("attackingNow");
		attackMachine.AddState(state4);
		state4.OnEnter = AttackingNow_OnEnter;
		state4.AddLink("waitForAttack", () => !MonsterCanAttack());
		attackMachine.SwitchStateTo(state3);
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
	}

	private void RunAwayFromTarget_OnExit()
	{
	}

	private bool RunAwayFromTarget_HuntTarget()
	{
		return !TransformInRadius(target, runAwayRange);
	}

	private bool HuntTarget_RunToTarget()
	{
		return Common_Aggr();
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

	private bool Common_Aggr()
	{
		if (TransformInRadius(target, lookRange) || TakedAnyDamage())
		{
			monsterSounds.PlayRand("aggrScream");
			return true;
		}
		return false;
	}

	private bool MonsterRetreate()
	{
		bool flag = IsMonsterScared();
		if (takenDamage >= damageForRetreate || flag || IsTargetDead())
		{
			ResetDamage();
			ResetMonsterScare();
			return true;
		}
		return false;
	}

	private bool IsTargetDead()
	{
		return targetHeals.hp <= 0f;
	}

	private bool IsMonsterScared()
	{
		if (watchNearFire)
		{
			ResetMonsterScare();
			return true;
		}
		return false;
	}

	private void ResetMonsterScare()
	{
		watchNearFire = false;
	}

	private void ResetDamage()
	{
		takenDamage = 0f;
	}

	private bool TakedAnyDamage()
	{
		if (takenDamage > 0f)
		{
			ResetDamage();
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
		bool flag = false;
		if (damage >= heals.hp && damager != null && damager.GetComponent<TrapInstance>() != null)
		{
			flag = true;
		}
		bool flag2 = false;
		StateMachine.State currState = stateMachine.GetCurrState();
		if (damage >= heals.hp && currState != null && currState.id == "jumpEscaping")
		{
			flag2 = true;
		}
		if (!flag && !flag2)
		{
			base.TakeDamage(damage, damager);
		}
	}

	public override void OnNotLethalStrike(float damage, Transform damager)
	{
		base.OnNotLethalStrike(damage, damager);
		StateMachine.State currState = stateMachine.GetCurrState();
		bool flag = damager != null && damager.gameObject.GetComponent<TrapInstance>() != null;
		bool flag2 = currState.id == "jumpEscaping";
		bool flag3 = currState.id == "changePosByTarget";
		bool flag4 = currState.id == "runAwayFromTarget";
		bool flag5 = currState.id == "frozen";
		if (!flag4 && !flag5 && !flag3 && !flag2 && !flag)
		{
			takenDamage += damage;
		}
	}

	public override void Die()
	{
		Player component = target.GetComponent<Player>();
		component.OnMonsterDie();
		base.Die();
	}

	private void FixedUpdate()
	{
		StateMachine.State currState = stateMachine.GetCurrState();
		if (PlayerPrefs.GetInt("WatchBF") == 0)
		{
			if (Time.time > firstAttackAfter && !wasFirstAttack)
			{
				Debug.Log("First play -- force attack!" + Time.time);
				stateMachine.SwitchStateTo(stateMachine.GetStateById("fastFindTarget"));
				wasFirstAttack = true;
			}
			if (Time.time > firstScreamAfter && !wasFirstScream)
			{
				Debug.Log("First play -- scream!" + Time.time);
				monsterSounds.PlayRand("farScream");
				wasFirstScream = true;
			}
		}
	}

	private void Update()
	{
		stateMachine.Update();
		attackMachine.Update();
	}

	public override void ScareByRocket()
	{
		StateMachine.State currState = stateMachine.GetCurrState();
		bool flag = currState.id == "jumpEscaping";
		bool flag2 = currState.id == "changePosByTarget";
		bool flag3 = currState.id == "runAwayFromTarget";
		bool flag4 = currState.id == "frozen";
		if (!flag3 && !flag4 && !flag2 && !flag)
		{
			watchNearFire = true;
		}
	}
}
