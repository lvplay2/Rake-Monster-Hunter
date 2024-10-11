using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
	[Serializable]
	public class MenuItem
	{
		public string name;

		public GameObject gameObj;

		private Animator anim;

		private void Init()
		{
			if (!anim)
			{
				anim = gameObj.GetComponent<Animator>();
			}
		}

		public void Show()
		{
			Init();
			gameObj.SetActive(true);
		}

		public void Hide()
		{
			Init();
			gameObj.SetActive(false);
		}
	}

	public string currItemName;

	public MenuItem[] items;


	public string GamePlayScene = "GamePlay";

	private bool isBusy;

	private MenuItem curr;

	public void ResetWatchBF()
	{
		PlayerPrefs.SetInt("WatchBF", 0);
	}

	private MenuItem GetItem(string name)
	{
		MenuItem[] array = items;
		foreach (MenuItem menuItem in array)
		{
			if (menuItem.name == name)
			{
				return menuItem;
			}
		}
		return null;
	}

	private void Start()
	{
		Debug.Log("MainMenu.Start()");
		Invoke("ShowRateUs_Later", 0f);
		MenuItem[] array = items;
		foreach (MenuItem menuItem in array)
		{
			menuItem.Hide();
		}
		string @string = PlayerPrefs.GetString("GotoMenuItem");
		if (@string != string.Empty)
		{
			Debug.Log("GotoMenuItem" + @string);
			PlayerPrefs.SetString("GotoMenuItem", string.Empty);
			curr = GetItem(@string);
		}
		else
		{
			Debug.Log("GetItem(currItemName)");
			curr = GetItem(currItemName);
		}
		curr.Show();
	}

	public void OnClickMenuItem(string name)
	{
		MenuItem[] array = items;
		foreach (MenuItem menuItem in array)
		{
			if (menuItem.name == name)
			{
				curr.Hide();
				curr = menuItem;
				curr.Show();
			}
		}
	}

	public void OnClickPlay()
	{
		if (!isBusy)
		{
			Player.numberOfRessurections = 0;
			OnClickMenuItem("Loading");
			Invoke("LoadGamePlayScene", 0.25f);
			PlayerPrefs.SetInt("RewardedVideoShowned", 0);
		}
	}

	private void LoadGamePlayScene()
	{
		SceneManager.LoadScene(GamePlayScene);
	}

	public void OnClickSettings()
	{
	}
}
