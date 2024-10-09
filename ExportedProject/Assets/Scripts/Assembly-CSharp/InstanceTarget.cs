using System.Collections;
using UnityEngine;

public class InstanceTarget : MonoBehaviour
{
	public Material positive;

	public Material negative;

	public MeshRenderer[] renderers;

	public BoxCollider[] forbidenZones;

	protected Transform thisTransform;

	private ArrayList collisions = new ArrayList();

	private bool isInited;

	protected bool isInForbidenZone;

	private void Init()
	{
		if (!isInited)
		{
			thisTransform = base.transform;
		}
	}

	private void Start()
	{
		Init();
	}

	private void OnEnable()
	{
		Init();
		collisions.Clear();
		UpdateStatus();
	}

	private void SetMaterial(Material mat)
	{
		MeshRenderer[] array = renderers;
		foreach (MeshRenderer meshRenderer in array)
		{
			meshRenderer.material = mat;
		}
	}

	private void UpdateStatus()
	{
		if (collisions.Count <= 0 && !isInForbidenZone)
		{
			SetMaterial(positive);
		}
		else
		{
			SetMaterial(negative);
		}
	}

	protected virtual bool IsInForbidenZone()
	{
		if (forbidenZones == null || forbidenZones.Length < 1)
		{
			return false;
		}
		BoxCollider[] array = forbidenZones;
		foreach (BoxCollider boxCollider in array)
		{
			if (boxCollider != null && JackUtils.PointInsideBox(thisTransform.position, boxCollider))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsCanBeInstalled()
	{
		return collisions.Count <= 0 && !isInForbidenZone;
	}

	protected virtual bool IsColliderMatch(Collider coll)
	{
		return coll.gameObject.GetComponent<TrapInstance>();
	}

	private void OnTriggerEnter(Collider coll)
	{
		if (IsColliderMatch(coll))
		{
			collisions.Add(coll);
			UpdateStatus();
		}
	}

	private void OnTriggerExit(Collider coll)
	{
		if (IsColliderMatch(coll))
		{
			collisions.Remove(coll);
			UpdateStatus();
		}
	}

	public void RemoveCollisionManualy(Collider coll)
	{
		Debug.Log("RemoveCollisionManualy ");
		Debug.Log(coll);
		collisions.Remove(coll);
		UpdateStatus();
		foreach (Collider collision in collisions)
		{
			Debug.Log(collision);
		}
	}

	public void FixedUpdate()
	{
		bool flag = IsInForbidenZone();
		if (isInForbidenZone != flag)
		{
			isInForbidenZone = flag;
			UpdateStatus();
		}
	}
}
