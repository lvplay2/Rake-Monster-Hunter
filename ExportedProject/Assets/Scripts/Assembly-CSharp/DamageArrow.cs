using UnityEngine;
using UnityEngine.UI;

public class DamageArrow : MonoBehaviour
{
	public Transform testTransform;

	public bool testMode;

	public bool updateDirection = true;

	public Transform playerTrans;

	public Transform rotateTrans;

	public Image image;

	public float showTime = 5f;

	private float arrowTimer;

	private Transform currTransform;

	private void Start()
	{
		image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
	}

	private void Update()
	{
		if (arrowTimer > 0f)
		{
			arrowTimer = Mathf.MoveTowards(arrowTimer, 0f, Time.deltaTime);
			image.color = new Color(image.color.r, image.color.g, image.color.b, arrowTimer / showTime * 1f);
			if (updateDirection)
			{
				RotateDamageArrowBy(currTransform);
			}
		}
		if (testMode && (bool)testTransform)
		{
			ShowDamageArrow(testTransform);
		}
	}

	public void ShowDamageArrow()
	{
		ShowDamageArrow(testTransform);
	}

	public void ShowDamageArrow(Transform target)
	{
		currTransform = target;
		arrowTimer = showTime;
		RotateDamageArrowBy(currTransform);
	}

	public void RotateDamageArrowBy(Transform target)
	{
		Quaternion quaternion = Quaternion.LookRotation(target.position - playerTrans.position);
		Vector3 vector = playerTrans.eulerAngles - quaternion.eulerAngles;
		rotateTrans.eulerAngles = new Vector3(0f, 0f, vector.y);
	}
}
