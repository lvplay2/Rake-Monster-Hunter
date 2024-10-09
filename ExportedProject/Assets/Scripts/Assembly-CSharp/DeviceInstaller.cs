using UnityEngine;
using UnityEngine.UI;

public class DeviceInstaller : Item
{
	public FP_Input playerInput;

	public Transform itemMulage;

	public InstanceTarget itemTarget;

	public Transform rotationObject;

	public GameObject itemPfb;

	public GameObject shootBtn;

	public GameObject putBtn;

	public PickUpRay disablePickUpRay;

	public Animator txtCantPut;

	public TextCounter counter;

	protected float distToTrap = 4f;

	protected Ray correctRay;

	protected RaycastHit correctHit;

	protected float correctRange = 6f;

	protected int correctMask;

	public override void UpdateCounter()
	{
		counter.SetValue(ammount + "/" + ammountMax);
	}

	protected override void Start()
	{
		correctMask = LayerMask.GetMask("Terrain");
		base.Start();
		counter.SetValue(ammount + "/" + ammountMax);
	}

	public bool TakeOne()
	{
		if (ammount < ammountMax)
		{
			ammount++;
			counter.SetValue(ammount + "/" + ammountMax);
			return true;
		}
		return false;
	}

	public override void Equip()
	{
		if ((bool)disablePickUpRay)
		{
			disablePickUpRay.enabled = false;
		}
		base.gameObject.SetActive(true);
		putBtn.SetActive(true);
		Button component = putBtn.GetComponent<Button>();
		component.onClick.RemoveAllListeners();
		component.onClick.AddListener(InstallItem);
		shootBtn.SetActive(false);
		base.Equip();
	}

	public override void UnEquip()
	{
		if ((bool)disablePickUpRay)
		{
			disablePickUpRay.enabled = true;
		}
		base.gameObject.SetActive(false);
		putBtn.SetActive(false);
		shootBtn.SetActive(true);
		base.UnEquip();
	}

	private bool CorrectByRay(Vector3 pos, out Vector3 corrected)
	{
		correctRay.origin = pos + new Vector3(0f, correctRange / 2f, 0f);
		correctRay.direction = Vector3.down;
		if (Physics.Raycast(correctRay, out correctHit, 40f, correctMask))
		{
			corrected = correctHit.point;
			return true;
		}
		corrected = pos;
		return false;
	}

	private Vector3 GetPositionForTrap()
	{
		Vector3 corrected = rotationObject.position + rotationObject.forward * distToTrap;
		CorrectByRay(corrected, out corrected);
		return corrected;
	}

	private void Update()
	{
		Vector3 positionForTrap = GetPositionForTrap();
		itemMulage.position = positionForTrap;
		itemMulage.rotation = rotationObject.rotation;
	}

	private void FixedUpdate()
	{
	}

	private void InstallItem()
	{
		if (itemTarget.IsCanBeInstalled() && ammount > 0)
		{
			Object.Instantiate(itemPfb, itemMulage.position, itemMulage.rotation);
			ammount--;
			counter.SetValue(ammount + "/" + ammountMax);
		}
		else if (!itemTarget.IsCanBeInstalled())
		{
			txtCantPut.SetTrigger("Show");
			txtCantPut.GetComponent<Text>().text = "You can not put it here";
		}
		else if (ammount < 1)
		{
			txtCantPut.SetTrigger("Show");
			txtCantPut.GetComponent<Text>().text = "You have no items.";
		}
	}
}
