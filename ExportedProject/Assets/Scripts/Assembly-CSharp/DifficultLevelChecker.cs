using UnityEngine;
using UnityEngine.UI;

public class DifficultLevelChecker : MonoBehaviour
{
    public Text LevelText;

    private void Start()
    {
        DifficultLevel SelectedLevel = Difficult.Instance.GetSelectedLevel();

        if (SelectedLevel != null)
        {
            string DifficultLevelName = GetDifficultLevelName(SelectedLevel.lvl);

            LevelText.text = "Difficult Level: " + DifficultLevelName;
        }
    }

    private string GetDifficultLevelName(int num)
    {
        switch (num)
        {
            case 1:
                return "Normal";
            case 2:
                return "Hard";
            case 3:
                return "Hell";
            default:
                return "Unknown";
        }
    }
}
