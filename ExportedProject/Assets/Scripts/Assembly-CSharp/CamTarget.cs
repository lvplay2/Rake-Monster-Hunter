using UnityEngine;

public class CamTarget : InstanceTarget
{
	protected override bool IsColliderMatch(Collider coll)
	{
		return coll.gameObject.GetComponent<CamInstance>();
	}
}
