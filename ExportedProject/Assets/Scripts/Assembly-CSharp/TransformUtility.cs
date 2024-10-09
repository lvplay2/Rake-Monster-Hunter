using UnityEngine;

public static class TransformUtility
{
	public static bool IsLooked(this Transform transform, Vector3 target, float lookAngle, float radius = float.PositiveInfinity)
	{
		Vector3 from = target - transform.position;
		if (from.magnitude > radius)
		{
			return false;
		}
		return lookAngle / 2f > Vector3.Angle(from, transform.forward);
	}

	public static bool IsLookedLocal(this Transform transform, Vector3 localTarget, float lookAngle, float radius = float.PositiveInfinity)
	{
		return transform.IsLooked(transform.position + localTarget, lookAngle, radius);
	}

	public static bool IsLooked(this Transform transform, Transform target, float lookAngle, float radius = float.PositiveInfinity)
	{
		return transform.IsLooked(target.position, lookAngle, radius);
	}

	public static bool IsLooked(this Camera camera, Vector3 target)
	{
		return camera.transform.IsLooked(target, camera.fieldOfView, camera.farClipPlane);
	}
}
