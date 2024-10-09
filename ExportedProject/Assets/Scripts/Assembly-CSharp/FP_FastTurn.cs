using UnityEngine;
using UnityEngine.UI;

public class FP_FastTurn : MonoBehaviour
{
	public float turnSpeed = 5.5f;

	public float turnAngle = 180f;

	public Button leftTurn;

	public Button rightTurn;

	private Transform thisT;

	public static bool turn;

	private Quaternion targetRotation;

	private void Start()
	{
		thisT = base.transform;
		leftTurn.onClick.AddListener(LeftTurn);
		rightTurn.onClick.AddListener(RightTurn);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			LeftTurn();
		}
		else if (Input.GetKeyDown(KeyCode.E))
		{
			RightTurn();
		}
		if (thisT.rotation != targetRotation)
		{
			if (turn)
			{
				thisT.rotation = Quaternion.RotateTowards(thisT.rotation, targetRotation, turnSpeed * 100f * Time.deltaTime);
			}
		}
		else
		{
			turn = false;
		}
	}

	private void LeftTurn()
	{
		targetRotation = Quaternion.AngleAxis(turnAngle, base.transform.up) * thisT.rotation;
		turn = true;
	}

	private void RightTurn()
	{
		targetRotation = Quaternion.AngleAxis(0f - turnAngle, base.transform.up) * thisT.rotation;
		turn = true;
	}
}
