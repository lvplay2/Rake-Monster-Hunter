using UnityEngine;

public class TrapTarget : InstanceTarget
{
	protected override bool IsColliderMatch(Collider coll)
	{
		return coll.gameObject.GetComponent<TrapInstance>();
	}
}
