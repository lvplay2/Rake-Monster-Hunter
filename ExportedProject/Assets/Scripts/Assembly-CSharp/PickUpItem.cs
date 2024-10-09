using System;
using UnityEngine;
using UnityEngine.Events;

public class PickUpItem : MonoBehaviour
{
	[Serializable]
	public class CallEvent : UnityEvent
	{
	}

	public string id;

	public string itemName;

	public int ammount = 1;

	public GameObject target;

	public CallEvent callEvent;

	[HideInInspector]
	public Collider coll;

	[HideInInspector]
	public ItemController itemCon;

	private void Start()
	{
		coll = GetComponent<Collider>();
	}
}
