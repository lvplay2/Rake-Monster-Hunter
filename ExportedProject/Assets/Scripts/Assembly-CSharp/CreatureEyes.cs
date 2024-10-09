using System.Collections;
using UnityEngine;

public class CreatureEyes : MonoBehaviour
{
	public Transform testTarget;

	public Transform eyesTransform;

	public Vector3 dirAxis = Vector3.forward;

	public float horAngle = 90f;

	public float distance = 10f;

	public bool checkVisionAuto;

	public int inVision;

	public ArrayList visibleCreatures;

	public LayerMask layers;

	private Transform _thisTransform;

	private Transform thisTransform
	{
		get
		{
			if (_thisTransform == null)
			{
				_thisTransform = base.transform;
			}
			return _thisTransform;
		}
		set
		{
			_thisTransform = value;
		}
	}

	public Transform GetEyesTransfrom()
	{
		return (!eyesTransform) ? thisTransform : eyesTransform;
	}

	public bool IsCreatureVisible(Transform target)
	{
		CreatureVisible component = target.GetComponent<CreatureVisible>();
		if (component == null)
		{
			Debug.LogError("CreatureVisible is not exiss on target");
			return false;
		}
		return component.IsCreatureVisibleFor(this);
	}

	public void CheckVision()
	{
		visibleCreatures = new ArrayList();
		foreach (CreatureVisible item in CreatureVisible.all)
		{
			if (item.IsCreatureVisibleFor(this))
			{
				visibleCreatures.Add(item);
			}
		}
		inVision = ((visibleCreatures != null) ? visibleCreatures.Count : 0);
	}

	private void Update()
	{
		if ((bool)testTarget)
		{
			Debug.Log(">> " + IsCreatureVisible(testTarget));
		}
		if (checkVisionAuto)
		{
			CheckVision();
		}
	}
}
