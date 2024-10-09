using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image), typeof(Button))]
public sealed class GameBaner : MonoBehaviour
{
	private class UrlInfo
	{
		public string URL;

		public string MainName;

		public int Version;

		public UrlInfo(string MainName, int Version, string URL)
		{
			this.URL = URL;
			this.MainName = MainName;
			this.Version = Version;
		}
	}

	private static string URLGameList = "http://diamon13dem.h1n.ru/gameList.xml";

	private static List<string> gameIDs = new List<string>();

	private static Dictionary<string, UrlInfo> urlDictionary = new Dictionary<string, UrlInfo>();

	private static Dictionary<string, AssetBundle> resourcesDictionary = new Dictionary<string, AssetBundle>();

	private static bool inited = false;

	private static bool initing = false;

	private static bool shown = false;

	private static bool showing = false;

	private static GameBaner _instance;

	private Image _image;

	private Button _button;

	private static GameBaner Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType<GameBaner>();
				if (_instance == null)
				{
					_instance = new GameObject("Game Baner").AddComponent<GameBaner>();
				}
			}
			return _instance;
		}
		set
		{
			if (_instance == null)
			{
				_instance = value;
			}
		}
	}

	private Image image
	{
		get
		{
			if (_image == null)
			{
				_image = GetComponent<Image>();
			}
			_image.enabled = shown;
			return _image;
		}
		set
		{
			_image = value;
		}
	}

	private Button button
	{
		get
		{
			if (_button == null)
			{
				_button = GetComponent<Button>();
			}
			_button.enabled = shown;
			return _button;
		}
		set
		{
			_button = value;
		}
	}

	private static void Init()
	{
		if (!initing)
		{
			initing = true;
			Instance.StartCoroutine(InitCoroutine());
		}
	}

	private static IEnumerator InitCoroutine()
	{
		yield return Load(URLGameList);
		inited = true;
		initing = false;
	}

	private static IEnumerator Load(string urlGameList)
	{
		using (WWW www = new WWW(urlGameList))
		{
			while (!www.isDone && string.IsNullOrEmpty(www.error))
			{
				yield return new WaitForEndOfFrame();
			}
			if (!string.IsNullOrEmpty(www.error))
			{
				Debug.LogError(www.error);
				yield break;
			}
			Dictionary<string, UrlInfo> tUrlDictionary = GetUrlDictionary(www.text);
			Dictionary<string, AssetBundle> tResourceDictionary = new Dictionary<string, AssetBundle>();
			foreach (string id in tUrlDictionary.Keys)
			{
				using (WWW w = WWW.LoadFromCacheOrDownload(tUrlDictionary[id].URL, tUrlDictionary[id].Version))
				{
					while (!w.isDone && string.IsNullOrEmpty(w.error))
					{
						yield return new WaitForEndOfFrame();
					}
					if (string.IsNullOrEmpty(w.error))
					{
						AssetBundle assetBundle = w.assetBundle;
						if (assetBundle != null)
						{
							tResourceDictionary.Add(id, assetBundle);
						}
					}
				}
			}
			foreach (string key in tResourceDictionary.Keys)
			{
				if (!gameIDs.Contains(key))
				{
					gameIDs.Add(key);
					urlDictionary.Add(key, tUrlDictionary[key]);
					resourcesDictionary.Add(key, tResourceDictionary[key]);
				}
			}
		}
	}

	private static IEnumerator LoadSprite(string urlAdress)
	{
		using (WWW www = new WWW(urlAdress))
		{
			while (!www.isDone && string.IsNullOrEmpty(www.error))
			{
				initing = false;
				yield return new WaitForEndOfFrame();
			}
			if (!string.IsNullOrEmpty(www.error))
			{
				Debug.LogError(www.error);
				initing = false;
			}
		}
	}

	private static Dictionary<string, UrlInfo> GetUrlDictionary(string Text)
	{
		Dictionary<string, UrlInfo> dictionary = new Dictionary<string, UrlInfo>();
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(Text);
		XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("Game");
		if (elementsByTagName.Count > 0)
		{
			foreach (XmlNode item in elementsByTagName)
			{
				if (!dictionary.ContainsKey(item.Attributes["id"].Value))
				{
					try
					{
						dictionary.Add(item.Attributes["id"].Value, new UrlInfo(item.Attributes["name"].Value, int.Parse(item.Attributes["version"].Value), item.InnerText));
					}
					catch
					{
					}
				}
			}
		}
		return dictionary;
	}

	private static void ShowURL(string Identifier)
	{
		Application.OpenURL("market://details?id=" + Identifier);
	}

	public static void Show()
	{
		if (!showing)
		{
			showing = true;
			Instance.StartCoroutine(ShowCoroutine());
		}
	}

	private static IEnumerator ShowCoroutine()
	{
		if (!inited)
		{
			Init();
		}
		while (!inited)
		{
			yield return new WaitForEndOfFrame();
		}
		int randNumber = Random.Range(0, gameIDs.Count);
		if (gameIDs[randNumber] == Application.identifier && gameIDs.Count > 1)
		{
			randNumber++;
			if (randNumber >= gameIDs.Count)
			{
				randNumber = 0;
			}
		}
		BanerStructure baner = resourcesDictionary[gameIDs[randNumber]].LoadAsset<BanerStructure>(urlDictionary[gameIDs[randNumber]].MainName);
		if (baner != null)
		{
			Instance.image.sprite = baner.sprites[Random.Range(0, baner.sprites.Length)];
			Instance.image.enabled = true;
			Instance.button.onClick.RemoveAllListeners();
			Instance.button.onClick.AddListener(delegate
			{
				ShowURL(gameIDs[randNumber]);
			});
			Instance.button.enabled = true;
			shown = true;
			showing = false;
		}
	}

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			image = GetComponent<Image>();
			image.enabled = shown;
			button = GetComponent<Button>();
			button.enabled = shown;
			Show();
		}
		else
		{
			Debug.LogWarning(string.Concat("Component ", GetType().ToString(), "removed, because there must be only one on the stage. \n There is already an object ", _instance.gameObject, " with such a componen"));
			Object.Destroy(this);
		}
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}
}
