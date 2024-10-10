using System;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class AdverController : MonoBehaviour
{
	public delegate void SimpleCallback();

	public SimpleCallback rewardCallback;

	public static AdverController con = null;

	public static bool noAds = false;

	public static bool wasFirst5sec = false;

	public const string gameId = "1783588";

	public const string rewardedVideoPref = "rewardedVideoShowned";

	public float showTimerBefore = 5f;

	public float showAdverAfter = 20f;

	public bool disable5SecAfterRewarded = true;

	public GameObject timer;

	public FP_Joystick[] resetJoysticks;

	public GameObject[] disabeObjects;

	public Player player;

	private float startTime;

	private Text text;

	private int shownedSec;

	private bool adverShowned;

	private static readonly AdverController instance = new AdverController();

	public static AdverController Instance
	{
		get
		{
			return instance;
		}
	}

	private AdverController()
	{
	}

	private void Start()
	{
		if (!con)
		{
			con = this;
		}
		if (Advertisement.isSupported)
		{
			Advertisement.Initialize("1783588");
		}
		startTime = Time.time;
		text = timer.GetComponent<Text>();
		timer.SetActive(false);
	}

	public void WaitSeconds(float seconds)
	{
		float num = Time.time - (startTime + showAdverAfter - seconds);
		if (num > 0f)
		{
			startTime += num;
		}
	}

	public static void ResetRewardedVideo()
	{
		PlayerPrefs.SetString("rewardedVideoShowned", string.Empty);
	}

	private void ResetJoysticks()
	{
		if (resetJoysticks == null)
		{
			return;
		}
		FP_Joystick[] array = resetJoysticks;
		foreach (FP_Joystick fP_Joystick in array)
		{
			if ((bool)fP_Joystick)
			{
				fP_Joystick.Reset();
			}
		}
	}

	private void SetObjcetsActive(bool active)
	{
		if (disabeObjects == null)
		{
			return;
		}
		GameObject[] array = disabeObjects;
		foreach (GameObject gameObject in array)
		{
			if ((bool)gameObject && gameObject.activeSelf != active)
			{
				gameObject.SetActive(active);
			}
		}
	}

	public void Show5SecVideo()
	{
		wasFirst5sec = true;
		ResetJoysticks();
		ShowOptions showOptions = new ShowOptions();
		showOptions.resultCallback = Result5SecVideo;
		Advertisement.Show();
	}

	public void Result5SecVideo(ShowResult result)
	{
	}

	public void ShowRewardedVideo(string placementId, SimpleCallback clbk)
	{
		rewardCallback = clbk;
		ShowOptions showOptions = new ShowOptions();
		showOptions.resultCallback = ResultRewardedVideo;
		Advertisement.Show(placementId, showOptions);
	}

	public string CurrDateStr()
	{
		return DateTime.Now.ToString("dd/MM/yyyy");
	}

	public void SetRewardedVideoWatchedToday()
	{
		PlayerPrefs.SetString("rewardedVideoShowned", CurrDateStr());
	}

	public bool IsRewardedVideoWatchedToday()
	{
		return CurrDateStr() == PlayerPrefs.GetString("rewardedVideoShowned");
	}

	public void ResultRewardedVideo(ShowResult result)
	{
		if (result != ShowResult.Skipped || result != 0)
		{
			if (rewardCallback != null)
			{
				rewardCallback();
			}
			if (disable5SecAfterRewarded)
			{
				SetRewardedVideoWatchedToday();
			}
		}
	}

	private void FixedUpdate()
	{
		bool flag = IsRewardedVideoWatchedToday();
		if (!noAds && !flag && Time.time > startTime + showAdverAfter - showTimerBefore && Advertisement.IsReady())
		{
			if (!timer.activeSelf)
			{
				timer.SetActive(true);
			}
			int num = Mathf.RoundToInt(startTime + showAdverAfter - Time.time);
			if (num != shownedSec)
			{
				shownedSec = num;
				text.text = "Advertisement after " + shownedSec + " seconds.";
			}
		}
		else if (timer.activeSelf)
		{
			timer.SetActive(false);
		}
		if (!noAds && !flag && Time.time >= startTime + showAdverAfter && Advertisement.IsReady())
		{
			startTime = Time.time;
			Show5SecVideo();
		}
	}
}
