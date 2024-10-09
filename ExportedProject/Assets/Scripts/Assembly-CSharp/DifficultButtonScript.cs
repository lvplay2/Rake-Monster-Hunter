using UnityEngine;
using UnityEngine.UI;

public class DifficultButtonScript : MonoBehaviour
{
	public int level;

	public int unlockAfter;

	public GameObject lockObj;

	public Button button;

	public Text nameText;

	public GameObject marker;

	private Difficult diffCon = Difficult.Instance;

	private bool IsPrefUnlocked()
	{
		int playerMaxLevelNum = diffCon.playerMaxLevelNum;
		if (unlockAfter <= playerMaxLevelNum)
		{
			return true;
		}
		return false;
	}

	public void CheckUnlock()
	{
		if ((bool)lockObj)
		{
			if (IsPrefUnlocked())
			{
				lockObj.SetActive(false);
			}
			else
			{
				lockObj.SetActive(true);
			}
		}
	}

	private void SetButtonToggled(bool toggled)
	{
		button.interactable = !toggled;
		if ((bool)marker)
		{
			marker.SetActive(toggled);
		}
	}

	public void CheckActiveDifficult()
	{
		if ((bool)button)
		{
			int selectedLevelNum = diffCon.selectedLevelNum;
			if (selectedLevelNum == level)
			{
				SetButtonToggled(true);
			}
			else
			{
				SetButtonToggled(false);
			}
		}
	}

	public void ApplyDifficult()
	{
		diffCon.selectedLevelNum = level;
		Refresh();
	}

	public void ResetDifficultPrefs()
	{
		diffCon.playerMaxLevelNum = 0;
		diffCon.selectedLevelNum = 0;
	}

	public void SetTextName()
	{
		DifficultLevel levelByNum = diffCon.getLevelByNum(level);
		if ((bool)nameText && levelByNum != null)
		{
			nameText.text = levelByNum.name;
		}
	}

	public void Refresh()
	{
		CheckUnlock();
		CheckActiveDifficult();
		SetTextName();
	}

	private void OnEnable()
	{
		Refresh();
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
