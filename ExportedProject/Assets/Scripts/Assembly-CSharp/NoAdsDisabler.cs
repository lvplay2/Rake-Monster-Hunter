using UnityEngine;

public class NoAdsDisabler : MonoBehaviour
{
	public GameObject button;

	public bool inGame;

	public bool inMainMenu;

	private void Check()
	{
		bool flag = false;
		if (inGame)
		{
			if (AdverController.noAds || !AdverController.wasFirst5sec || flag)
			{
				VerifyButtonActive(false);
				return;
			}
		}
		else if (inMainMenu && (AdverController.noAds || flag))
		{
			VerifyButtonActive(false);
			return;
		}
		VerifyButtonActive(true);
	}

	private void VerifyButtonActive(bool value)
	{
		if (button == null)
		{
			button = base.gameObject;
		}
		if (button.activeSelf != value)
		{
			button.SetActive(value);
		}
	}

	private void Awake()
	{
		Check();
	}

	private void FixedUpdate()
	{
		Check();
	}
}
