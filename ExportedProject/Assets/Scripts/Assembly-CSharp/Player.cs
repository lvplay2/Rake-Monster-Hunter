using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : Creature
{
	[Serializable]
	public class ResetInputs
	{
		public FP_Joystick movJoy;

		public FP_Lookpad lookPad;

		public FP_Button shoot;

		public FP_Button reload;

		public void Reset()
		{
			movJoy.Reset();
			lookPad.Reset();
			shoot.Reset();
			reload.Reset();
		}
	}

	public string mainMenuScene = "MainMenu";

	public GameObject headObj;

	public GameObject inGameUI;

	public GameObject gameOverUI;

	public GameObject victoryUI;

	public GameObject headBox;

	public GameObject mainCam;

	public GameObject uiCam;

	public GameObject disableObj;

	public PlaySound voiceSounds;

	public bool invincible;

	public float hpForRessurect = 0.8f;

	public AudioSource scaredBreaze;

	public DamageArrow damageArrow;

	public ResetInputs resetInputs;

	protected float firstPainBreazeAfter;

	protected float repeatPainBreazeAfter;

	[HideInInspector]
	public static int numberOfRessurections;

	protected const int maxRessurections = 1;

	protected float scaredBreazeStart;

	protected float scaredBreazeTime = 3f;

	protected float scaredBreazePreDelay = 3f;

	protected override void Start()
	{
		base.Start();
		Screen.sleepTimeout = -1;
		firstPainBreazeAfter = heals.hpMax / 2f;
		repeatPainBreazeAfter = heals.hpMax / 4f;
	}

	private void Update()
	{
	}

	public void OnHearScareSound()
	{
		scaredBreazeTime = Time.time + scaredBreazePreDelay;
	}

	public override void OnNotLethalStrike(float dmg, Transform damager)
	{
		base.OnNotLethalStrike(dmg, damager);
		Debug.Log("OnNotLethalStrike");
		Invoke("LateTakeHit", 0.25f);
		if (heals.hp < firstPainBreazeAfter)
		{
			Invoke("LateInjuredBreaze", 2f);
		}
	}

	private void LateTakeHit()
	{
		voiceSounds.PlayRand("takeHit");
	}

	private void LateInjuredBreaze()
	{
		CancelInvoke("LateInjuredBreaze");
		if (heals.hp < firstPainBreazeAfter)
		{
			voiceSounds.PlayRand("injuredBreaze");
			if (heals.hp < repeatPainBreazeAfter)
			{
				Invoke("LateInjuredBreaze", 10f);
			}
		}
	}

	public override void TakeDamage(float damage, Transform damager)
	{
		if (!invincible)
		{
			damageArrow.ShowDamageArrow(damager);
			base.TakeDamage(damage, damager);
		}
	}

	public void OnMonsterDie()
	{
		Invoke("PlayerWin", 5f);
	}

	private void PlayerWin()
	{
		CancelInvoke();
		voiceSounds.Play(string.Empty);
		resetInputs.Reset();
		inGameUI.SetActive(false);
		victoryUI.SetActive(true);
		Difficult instance = Difficult.Instance;
		DifficultLevel selectedLevel = instance.GetSelectedLevel();
		if (instance.playerMaxLevelNum < instance.selectedLevelNum)
		{
			instance.playerMaxLevelNum = instance.selectedLevelNum;
		}
		victoryUI.transform.Find("txtWinningText").GetComponent<Text>().text = selectedLevel.winnigText;
	}

	public override void Die()
	{
		CancelInvoke();
		voiceSounds.PlayRand("die");
		resetInputs.Reset();
		inGameUI.SetActive(false);
		SetHeadToFall();
		Invoke("DisablePlayer", 0.2f);
		GameObject gameObject = gameOverUI.transform.Find("buttons").Find("btnRessurection").gameObject;
		GameObject gameObject2 = gameOverUI.transform.Find("WatchVideo").gameObject;
		if (numberOfRessurections < 1)
		{
			gameObject.SetActive(true);
			gameObject2.SetActive(true);
		}
		else
		{
			gameObject.SetActive(false);
			gameObject2.SetActive(false);
		}
		gameOverUI.transform.Find("buttons").Find("btnMainMenu").gameObject.SetActive(false);
		Invoke("ShowGameOverMenu", 5f);
	}

	private void SetHeadToFall()
	{
		headBox.SetActive(true);
		Transform transform = headBox.transform;
		Transform transform2 = mainCam.transform;
		Transform transform3 = uiCam.transform;
		transform.rotation = transform2.rotation;
		transform.position = transform2.position;
		transform2.parent = transform;
		transform2.localPosition = new Vector3(0f, 0f, 0f);
		transform2.localRotation = Quaternion.Euler(0f, 0f, 0f);
		transform3.parent = transform;
		transform3.localPosition = transform2.localPosition;
		transform3.localRotation = transform2.localRotation;
	}

	private void SetHeadToPlayer()
	{
		Transform parent = headObj.transform;
		Transform transform = mainCam.transform;
		Transform transform2 = uiCam.transform;
		transform.parent = parent;
		transform.localPosition = new Vector3(0f, 0f, 0f);
		transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		transform2.parent = parent;
		transform2.localPosition = transform.localPosition;
		transform2.localRotation = transform.localRotation;
		headBox.SetActive(false);
	}

	private void DisablePlayer()
	{
		disableObj.SetActive(false);
	}

	private void ShowGameOverMenu()
	{
		gameOverUI.SetActive(true);
		Invoke("ShowHomeButton", 3f);
	}

	private void ShowHomeButton()
	{
		gameOverUI.transform.Find("buttons").Find("btnMainMenu").gameObject.SetActive(true);
	}

	public void GotoMenu_Settings()
	{
		PlayerPrefs.SetString("GotoMenuItem", "Settings");
		GotoMenu();
	}

	public void GotoMenu()
	{
		SceneManager.LoadScene(mainMenuScene);
	}

	public void Ressurection()
	{
		numberOfRessurections++;
		base.gameObject.SetActive(true);
		SetHeadToPlayer();
		Heal(hpForRessurect);
		gameOverUI.SetActive(false);
		inGameUI.SetActive(true);
	}

	public override void Freeze(float time)
	{
	}
}
