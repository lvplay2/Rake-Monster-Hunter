using UnityEngine;

public class RateUs : MonoBehaviour
{
	public Animator anim;

	public GameObject blocker;

	public string hipperlink = "https://play.google.com/store/apps/details?id=com.onetongames.bigfootmh";

	private const string prefNotShow = "doNotShowRateUs";

	private const string prefWasGameplay = "wasGamePlay";

	public void Process()
	{
		bool flag = PlayerPrefs.GetInt("doNotShowRateUs") == 1;
		bool flag2 = PlayerPrefs.GetInt("wasGamePlay") == 1;
		if (!flag && flag2)
		{
			SetAnimDelegates();
			Show();
		}
	}

	private void SetAnimDelegates()
	{
		SimpleDelegation simpleDelegation = anim.gameObject.AddComponent<SimpleDelegation>();
		simpleDelegation.Add("OnShow", OnShow);
		simpleDelegation.Add("OnHide", OnHide);
	}

	public void ResetRateUs()
	{
		PlayerPrefs.SetInt("doNotShowRateUs", 0);
		PlayerPrefs.SetInt("wasGamePlay", 0);
	}

	public void GotoLink()
	{
		Application.OpenURL(hipperlink);
	}

	public void SetDoNotShow(bool value)
	{
		PlayerPrefs.SetInt("doNotShowRateUs", value ? 1 : 0);
	}

	public void SetWasGamePlay(bool value)
	{
		PlayerPrefs.SetInt("wasGamePlay", value ? 1 : 0);
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
		anim.SetTrigger("Show");
		blocker.SetActive(false);
	}

	public void Hide()
	{
		anim.SetTrigger("Hide");
		blocker.SetActive(false);
	}

	public void OnShow()
	{
	}

	public void OnHide()
	{
		base.gameObject.SetActive(false);
	}
}
