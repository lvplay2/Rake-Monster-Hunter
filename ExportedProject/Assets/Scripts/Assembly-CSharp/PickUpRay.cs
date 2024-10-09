using System;
using UnityEngine;

public class PickUpRay : MonoBehaviour
{
	[Serializable]
	public class ItemCongruence
	{
		public string name;

		public string defItemName = "Take item";

		public PickUpUi pickUpUi;

		public ItemController itemController;
	}

	public ItemCongruence[] items;

	public Transform lookCamera;

	public float pickUpRange = 5f;

	protected PickUpItem lastItem;

	protected PickUpItem currItem;

	private bool maskIsInited;

	private Ray pickRay = default(Ray);

	private RaycastHit pickHit;

	private PickUpUi lastUi;

	private bool objOnPrevFrame;

	private PickUpItem CheckItemByRay()
	{
		if (!maskIsInited)
		{
			maskIsInited = true;
		}
		pickRay.origin = lookCamera.position;
		pickRay.direction = lookCamera.forward;
		if (Physics.Raycast(pickRay, out pickHit, pickUpRange))
		{
			PickUpItem component = pickHit.collider.GetComponent<PickUpItem>();
			return (!component || !component.isActiveAndEnabled) ? null : component;
		}
		return null;
	}

	private ItemCongruence GetItemCongruence(PickUpItem item)
	{
		ItemCongruence[] array = items;
		foreach (ItemCongruence itemCongruence in array)
		{
			if (itemCongruence.name == item.id)
			{
				return itemCongruence;
			}
		}
		return null;
	}

	private void ShowItemDialog(PickUpItem item)
	{
		ItemCongruence itemCongruence = GetItemCongruence(item);
		PickUpUi pickUpUi = itemCongruence.pickUpUi;
		if ((bool)pickUpUi)
		{
			lastUi = pickUpUi;
			pickUpUi.Show();
			if (item.itemName == string.Empty)
			{
				item.itemName = itemCongruence.defItemName;
			}
			if (item.itemCon == null)
			{
				item.itemCon = itemCongruence.itemController;
			}
			pickUpUi.SetPickUpInfo(item);
		}
	}

	private void HideItemDialog(PickUpItem item)
	{
		ItemCongruence itemCongruence = GetItemCongruence(item);
		PickUpUi pickUpUi = itemCongruence.pickUpUi;
		if ((bool)pickUpUi)
		{
			pickUpUi.Hide();
		}
	}

	private void FixedUpdate()
	{
		PickUpItem pickUpItem = CheckItemByRay();
		bool flag = pickUpItem != null && lastItem != null;
		bool flag2 = objOnPrevFrame && !flag;
		objOnPrevFrame = flag;
		if (pickUpItem != null && pickUpItem != lastItem)
		{
			if (lastItem != null)
			{
				HideItemDialog(lastItem);
			}
			ShowItemDialog(pickUpItem);
		}
		else if (pickUpItem == null && lastItem != null)
		{
			HideItemDialog(lastItem);
		}
		else if (flag2)
		{
			lastUi.Hide();
		}
		lastItem = pickUpItem;
	}

	private void OnEnable()
	{
		objOnPrevFrame = false;
		currItem = null;
		lastItem = null;
	}

	private void OnDisable()
	{
		if ((bool)lastItem)
		{
			HideItemDialog(lastItem);
		}
	}
}
