using UnityEngine;
public class HideGameOverUI : MonoBehaviour
{
    public GameObject gameOverUI; // Asigna el objeto 'GameOverUI' desde el Inspector

    public void HideUI()
    {
        // Desactiva el objeto 'GameOverUI'
        gameOverUI.SetActive(false);
    }
}
