using UnityEngine;

public class JumpAgent : MonoBehaviour
{
	public Vector3[] jumpPoints;

	public Vector3 prev;

	public Vector3 next;

	protected Transform thisTransform;

	private StateMachine jumpSm = new StateMachine();

	private int currNum;

	private float jumpSpeed = 12.5f;

	private float jumpCurrVal;

	private float timeForJump;

	private float jumpDistance;

	private float turnDefPart = 0.25f;

	private float turnMaxTime = 0.75f;

	private float turnPart;

	private float landDefPart = 0.25f;

	private float landMaxTime = 0.75f;

	private float landPart;

	private Quaternion jumpNextLook;

	private Quaternion jumpNextLand;

	private Quaternion jumpStartLook;

	private float waitAfterJumpFactor = 0.3f;

	private float waitAfterJumpStart;

	private float waitAfterJumpTime;

	private bool atNextPoint;

	private bool atEndOfTheWay;

	public Animator anim;

	private bool isInited;

	public float JumpSimple(float t)
	{
		t = t * 2f - 1f;
		return 1f - t * t;
	}

	private void ConfigureStateMachine()
	{
		StateMachine.State state = new StateMachine.State("jumpFromTo");
		jumpSm.AddState(state);
		state.OnEnter = delegate
		{
			prev = thisTransform.position;
			next = jumpPoints[currNum];
			jumpCurrVal = 0f;
			jumpDistance = Vector3.Distance(prev, next);
			timeForJump = jumpDistance / jumpSpeed;
			turnPart = Mathf.Min(turnDefPart, turnMaxTime / timeForJump);
			landPart = Mathf.Min(landDefPart, landMaxTime / timeForJump);
			Vector3 vector = next - thisTransform.position;
			vector = next - thisTransform.position;
			vector.y = 0f;
			vector = vector.normalized;
			jumpNextLook = Quaternion.LookRotation(vector);
			jumpNextLand = jumpNextLook * Quaternion.Euler(-90f, 0f, 0f);
			jumpStartLook = base.transform.rotation;
		};
		state.Update = delegate
		{
			Vector3 position = Vector3.Lerp(prev, next, jumpCurrVal);
			position += new Vector3(0f, JumpSimple(jumpCurrVal) * (jumpDistance / 4f), 0f);
			thisTransform.position = position;
			if (0f <= jumpCurrVal && jumpCurrVal <= turnPart)
			{
				float num = jumpCurrVal / turnPart;
				thisTransform.rotation = Quaternion.Lerp(jumpStartLook, jumpNextLook, num);
				anim.SetFloat("JumpTime", num / 2f);
			}
			if (1f - landPart <= jumpCurrVal && jumpCurrVal <= 1f)
			{
				float num2 = (jumpCurrVal - (1f - landPart)) / landPart;
				thisTransform.rotation = Quaternion.Lerp(jumpNextLook, jumpNextLand, num2);
				anim.SetFloat("JumpTime", num2 / 2f + 0.5f);
			}
			atNextPoint = jumpCurrVal >= 1f;
			atEndOfTheWay = currNum == jumpPoints.Length - 1;
			if (atNextPoint)
			{
				jumpCurrVal = 1f;
			}
			else
			{
				jumpCurrVal += 1f / timeForJump * Time.deltaTime;
			}
		};
		state.OnExit = delegate
		{
			if (currNum < jumpPoints.Length - 1)
			{
				currNum++;
			}
		};
		state.AddLink("waitAfterJump", () => atNextPoint && !atEndOfTheWay);
		state.AddLink("unActive", () => atNextPoint && atEndOfTheWay);
		StateMachine.State state2 = new StateMachine.State("unActive");
		jumpSm.AddState(state2);
		StateMachine.State state3 = new StateMachine.State("waitAfterJump");
		jumpSm.AddState(state3);
		state3.OnEnter = delegate
		{
			waitAfterJumpTime = jumpCurrVal * waitAfterJumpFactor;
			waitAfterJumpStart = Time.time;
		};
		state3.OnExit = delegate
		{
		};
		state3.AddLink("jumpFromTo", () => Time.time > waitAfterJumpStart + waitAfterJumpTime);
		jumpSm.isLoggingEnabled = true;
		jumpSm.logName = "JAg";
	}

	private void Start()
	{
		Init();
	}

	private void Init()
	{
		if (!isInited)
		{
			isInited = true;
			thisTransform = base.transform;
			ConfigureStateMachine();
		}
	}

	private void OnEnable()
	{
		Init();
		anim.SetTrigger("Jump");
		currNum = 0;
		jumpSm.SwitchStateTo(jumpSm.GetStateById("jumpFromTo"));
	}

	private void OnDisable()
	{
		anim.SetTrigger("OnGround");
	}

	private void Update()
	{
		jumpSm.Update();
	}
}
