using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
	public Item[] items;

	public TextCounter counter;

	public Animator messageAnim;

	protected int itemByDeffault;

	public void SwitchItemTo(int indx)
	{
		EquipItemByNum(indx);
	}

	private Item GetItem<T>()
	{
		Item[] array = items;
		foreach (Item item in array)
		{
			if (item is T)
			{
				return item;
			}
		}
		return null;
	}

	public bool PickUpItem(PickUpItem item)
	{
		Debug.Log("PickUpItem " + item.id);
		if (item.id == "idTrap")
		{
			TrapInHands trapInHands = (TrapInHands)GetItem<TrapInHands>();
			if (trapInHands.TakeOne())
			{
				if (trapInHands.IsEquiped() && (bool)item.coll)
				{
					trapInHands.itemTarget.RemoveCollisionManualy(item.coll);
				}
				Object.Destroy(item.gameObject);
				return true;
			}
			ShowMessage("You can't take more.");
		}
		else if (item.id == "idCamera")
		{
			CameraInHands cameraInHands = (CameraInHands)GetItem<CameraInHands>();
			if (cameraInHands.TakeOne())
			{
				if (cameraInHands.IsEquiped() && (bool)item.coll)
				{
					cameraInHands.itemTarget.RemoveCollisionManualy(item.coll);
				}
				Object.Destroy(item.gameObject);
				return true;
			}
			ShowMessage("You can't take more.");
		}
		else if (item.id == "idMeat")
		{
			MeatInHands meatInHands = (MeatInHands)GetItem<MeatInHands>();
			if (meatInHands.TakeOne())
			{
				if (meatInHands.IsEquiped() && (bool)item.coll)
				{
					meatInHands.itemTarget.RemoveCollisionManualy(item.coll);
				}
				Object.Destroy(item.gameObject);
				return true;
			}
			ShowMessage("You can't take more.");
		}
		else if (item.id == "idSignalRocket")
		{
			SignalLauncher signalLauncher = (SignalLauncher)GetItem<SignalLauncher>();
			if (signalLauncher.TakeAmmoAll() == 0)
			{
				ShowMessage("You can't take more.");
			}
		}
		else if (item.id == "idMedKit")
		{
			MedKitUsing medKitUsing = (MedKitUsing)GetItem<MedKitUsing>();
			if (medKitUsing.TakeOne())
			{
				Object.Destroy(item.gameObject);
				return true;
			}
			ShowMessage("You can't take more.");
		}
		else if (item.id == "idRifleAmmo")
		{
			Rifle rifle = (Rifle)GetItem<Rifle>();
			if (rifle.TakeAmmoAll() == 0)
			{
				ShowMessage("You can't take more.");
			}
		}
		else
		{
			if (item.id == "idConsumables")
			{
				Rifle rifle2 = (Rifle)GetItem<Rifle>();
				rifle2.TakeAmmoAll();
				MedKitUsing medKitUsing2 = (MedKitUsing)GetItem<MedKitUsing>();
				medKitUsing2.TakeAll();
				return true;
			}
			if (item.id == "idDeerBody")
			{
				MeatInHands meatInHands2 = (MeatInHands)GetItem<MeatInHands>();
				if (meatInHands2.TakeOne())
				{
					item.ammount--;
					if (item.ammount <= 0)
					{
						Object.Destroy(item.target);
					}
				}
				else
				{
					ShowMessage("You can't take more.");
				}
			}
		}
		return false;
	}

	public void ShowMessage(string mess)
	{
		messageAnim.SetTrigger("Show");
		messageAnim.GetComponent<Text>().text = mess;
	}

	private void EquipItemByNum(int num)
	{
		for (int i = 0; i < items.Length; i++)
		{
			if (!(items[i] == null) && items[i].IsEquiped())
			{
				items[i].UnEquip();
			}
		}
		items[num].Equip();
	}

	private void UpdateAllCounters()
	{
		Item[] array = items;
		foreach (Item item in array)
		{
			if ((bool)item)
			{
				item.UpdateCounter();
			}
		}
	}

	private void Start()
	{
		UpdateAllCounters();
		EquipItemByNum(itemByDeffault);
	}
}
