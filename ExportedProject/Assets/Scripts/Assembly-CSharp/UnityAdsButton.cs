using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UnityAdsButton : MonoBehaviour
{
	private Button m_Button;

	public string placementId = "rewardedVideo";

	public UnityEvent onRewardEvent = new UnityEvent();

	private void Start()
	{
		m_Button = GetComponent<Button>();
		if ((bool)m_Button)
		{
			m_Button.onClick.AddListener(ShowAd);
		}
	}

	private void Update()
	{
		if ((bool)m_Button)
		{
			m_Button.interactable = Advertisement.IsReady(placementId);
		}
	}

	private void ShowAd()
	{
		AdverController.SimpleCallback clbk = delegate
		{
			onRewardEvent.Invoke();
		};
		AdverController.con.ShowRewardedVideo(placementId, clbk);
	}
}
