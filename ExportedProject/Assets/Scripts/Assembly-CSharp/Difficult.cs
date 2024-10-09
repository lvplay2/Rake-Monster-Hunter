using System.Collections;
using UnityEngine;

public class Difficult
{
	private ArrayList diffLvls = new ArrayList();

	private const string maxLvlPref = "PlayerMaxLevel";

	private const string selLvlPref = "SelectedLevel";

	private static readonly Difficult instance = new Difficult();

	public static Difficult Instance
	{
		get
		{
			return instance;
		}
	}

	public int playerMaxLevelNum
	{
		get
		{
			return PlayerPrefs.GetInt("PlayerMaxLevel");
		}
		set
		{
			PlayerPrefs.SetInt("PlayerMaxLevel", value);
		}
	}

	public int selectedLevelNum
	{
		get
		{
			int @int = PlayerPrefs.GetInt("SelectedLevel");
			return (@int == 0) ? 1 : @int;
		}
		set
		{
			PlayerPrefs.SetInt("SelectedLevel", value);
		}
	}

	private Difficult()
	{
		diffLvls.Add(new Normal());
		diffLvls.Add(new Hard());
		diffLvls.Add(new Hell());
	}

	public DifficultLevel GetSelectedLevel()
	{
		int num = selectedLevelNum;
		foreach (DifficultLevel diffLvl in diffLvls)
		{
			if (diffLvl.lvl == num)
			{
				return diffLvl;
			}
		}
		return null;
	}

	public DifficultLevel getLevelByNum(int num)
	{
		foreach (DifficultLevel diffLvl in diffLvls)
		{
			if (diffLvl.lvl == num)
			{
				return diffLvl;
			}
		}
		return null;
	}
}
