using UnityEngine;
using UnityEngine.UI;

public class PickUpUi : MonoBehaviour
{
	public Text txtItemName;

	private PickUpItem item;

	private GameObject thisGameObject;

	public void SetPickUpInfo(PickUpItem item_)
	{
		item = item_;
		if ((bool)txtItemName)
		{
			txtItemName.text = item.itemName;
		}
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	public void OnClick()
	{
		bool flag = false;
		if (item.itemCon != null)
		{
			flag = item.itemCon.PickUpItem(item);
		}
		else
		{
			Debug.LogError("Item don't know his item controller!");
		}
		if (flag)
		{
			Hide();
		}
	}

	private void Start()
	{
		thisGameObject = base.gameObject;
	}

	private void Update()
	{
	}
}
