using System.Collections.Generic;
using UnityEngine;

public class Unical : MonoBehaviour
{
	public static SortedDictionary<string, GameObject> all = new SortedDictionary<string, GameObject>();

	public string unical;

	public bool hideAtStartup;

	public bool makePersist;

	public static GameObject Get(string unicalName)
	{
		if (all.ContainsKey(unicalName))
		{
			return all[unicalName];
		}
		return null;
	}

	private void Awake()
	{
		if (makePersist)
		{
			if (unical != string.Empty)
			{
				if (!all.ContainsKey(unical))
				{
					Object.DontDestroyOnLoad(base.gameObject);
					all.Add(unical, base.gameObject);
				}
				else
				{
					Object.Destroy(base.gameObject);
				}
			}
			else
			{
				Debug.LogError("Persist object must have a unical name.");
			}
		}
		else if (unical != string.Empty)
		{
			if (!all.ContainsKey(unical))
			{
				all.Add(unical, base.gameObject);
			}
			else
			{
				Debug.LogError("Unical name '" + unical + "' is alredy used by other object");
			}
		}
		if (hideAtStartup)
		{
			base.gameObject.SetActive(false);
		}
	}

	private void OnDestroy()
	{
		if (unical != string.Empty && all.ContainsKey(unical) && all[unical] == base.gameObject)
		{
			all.Remove(unical);
		}
	}
}
