using UnityEngine;

public class MeatTarget : InstanceTarget
{
	protected override bool IsColliderMatch(Collider coll)
	{
		return coll.gameObject.GetComponent<MeatInstance>();
	}
}
