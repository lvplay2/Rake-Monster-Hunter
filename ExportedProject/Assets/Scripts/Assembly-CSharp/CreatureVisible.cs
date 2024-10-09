using System.Collections.Generic;
using UnityEngine;

public class CreatureVisible : MonoBehaviour
{
	public static HashSet<CreatureVisible> all = new HashSet<CreatureVisible>();

	public Transform[] visiblePoints;

	public static bool IsVisibleUnit<T>(T unit, Transform from, Vector3 dir, float angle, float distance, LayerMask mask) where T : CreatureVisible
	{
		bool result = false;
		if (unit != null)
		{
			Transform[] array = unit.visiblePoints;
			foreach (Transform transform in array)
			{
				if (IsVisibleObject(from, dir, transform.position, unit.gameObject, angle, distance, mask))
				{
					result = true;
					break;
				}
			}
		}
		return result;
	}

	public static bool IsVisibleObject(Transform from, Vector3 dir, Vector3 point, GameObject target, float angle, float distance, LayerMask mask)
	{
		bool result = false;
		if (IsAvailablePoint(from, dir, point, angle, distance))
		{
			Vector3 direction = point - from.position;
			Ray ray = new Ray(from.position, direction);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, distance, mask.value) && hitInfo.collider.gameObject == target)
			{
				result = true;
			}
		}
		return result;
	}

	public static bool IsAvailablePoint(Transform from, Vector3 dir, Vector3 point, float angle, float distance)
	{
		bool result = false;
		if (from != null && Vector3.Distance(from.position, point) <= distance)
		{
			Vector3 vector = point - from.position;
			float num = Vector3.Dot(from.forward, vector.normalized);
			if (num < 1f)
			{
				float num2 = Mathf.Acos(num);
				float num3 = num2 * 57.29578f;
				result = num3 <= angle;
			}
			else
			{
				result = true;
			}
		}
		return result;
	}

	public bool IsCreatureVisibleFor(CreatureEyes eyes)
	{
		if (visiblePoints == null || visiblePoints.Length < 1)
		{
			visiblePoints = new Transform[1] { base.transform };
		}
		return IsVisibleUnit(this, eyes.GetEyesTransfrom(), eyes.dirAxis, eyes.horAngle / 2f, eyes.distance, eyes.layers);
	}

	private void OnEnable()
	{
		all.Add(this);
	}

	private void OnDisable()
	{
		all.Remove(this);
	}
}
