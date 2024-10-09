using UnityEngine;

public class RewardForMedKit : MonoBehaviour
{
	public MedKitUsing medKitUsing;

	public string placementId = "rewardedVideo";

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	public void OnOkCallback()
	{
		medKitUsing.TakeOne();
		Hide();
	}

	public void OnNo()
	{
		Hide();
	}
}
