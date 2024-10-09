using UnityEngine;

public class JackUtils
{
	public delegate void SimpleFunc();

	public static void ControllActive(GameObject obj, bool value, SimpleFunc onValueDifferent = null)
	{
		if (!(obj == null) && obj.activeSelf != value)
		{
			obj.SetActive(value);
			if (onValueDifferent != null)
			{
				onValueDifferent();
			}
		}
	}

	public static void ControllEnabled(Behaviour comp, bool value, SimpleFunc onValueDifferent = null)
	{
		if (!(comp == null) && comp.enabled != value)
		{
			comp.enabled = value;
			if (onValueDifferent != null)
			{
				onValueDifferent();
			}
		}
	}

	public static bool PointInsideBox(Vector3 point, BoxCollider box)
	{
		Vector3 vector = box.transform.InverseTransformPoint(point);
		vector -= box.center;
		bool flag = ValueInRange(vector.x, (0f - box.size.x) / 2f, box.size.x / 2f);
		bool flag2 = ValueInRange(vector.y, (0f - box.size.y) / 2f, box.size.y / 2f);
		bool flag3 = ValueInRange(vector.z, (0f - box.size.z) / 2f, box.size.z / 2f);
		return flag && flag2 && flag3;
	}

	public static bool PointInAngle(Vector3 lookPos, Vector3 lookDir, float angle, Vector3 point)
	{
		bool flag = false;
		float num = Vector3.Dot((point - lookPos).normalized, lookDir.normalized);
		if (num < 1f)
		{
			float num2 = Mathf.Acos(num);
			float num3 = num2 * 57.29578f;
			return num3 <= angle;
		}
		return true;
	}

	public static bool PointInAngle_XZ(Vector3 lookPos, Vector3 lookDir, float angle, Vector3 point)
	{
		lookPos.y = (lookDir.y = (point.y = 0f));
		return PointInAngle(lookPos, lookDir, angle, point);
	}

	public static Vector3 NorthDir()
	{
		return Vector3.forward;
	}

	public static float AnglePositive(float a)
	{
		if (Mathf.Abs(a) > 360f)
		{
			a %= 360f;
		}
		if (a < 0f)
		{
			a = 180f - a + 180f;
		}
		return a;
	}

	public static float AzimuthOfPointByTarget(Vector3 point, Vector3 tar)
	{
		Vector3 rhs = NorthDir();
		Vector3 lhs = point - tar;
		lhs.y = 0f;
		lhs.Normalize();
		float f = Vector3.Dot(lhs, rhs);
		float num = Mathf.Acos(f) * 57.29578f;
		if (lhs.x < 0f)
		{
			num *= -1f;
		}
		return AnglePositive(num);
	}

	public static bool AngleInRage(float angle, float center, float range)
	{
		angle = AnglePositive(angle);
		center = AnglePositive(center);
		float num = center - range / 2f;
		float num2 = center + range / 2f;
		if (num <= angle && angle <= num2)
		{
			return true;
		}
		return false;
	}

	public static bool ValueInRange(float v, float min, float max)
	{
		if (min <= v && v <= max)
		{
			return true;
		}
		return false;
	}
}
