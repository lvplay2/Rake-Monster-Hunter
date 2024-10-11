using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryButton : MonoBehaviour
{
    // Este método se ejecutará cuando se haga clic en el botón
    public void RestartGameplayScene()
    {
        // Cambia a la escena llamada "GamePlayScene"
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}