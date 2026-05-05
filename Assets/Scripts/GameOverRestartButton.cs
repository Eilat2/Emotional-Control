using UnityEngine;

// הסקריפט הזה יושב ישירות על כפתור ה-Restart של Game Over
public class GameOverRestartButton : MonoBehaviour
{
    public void RestartGame()
    {
        // מחפשים את מנהל ה-UI גם אם חלק מהאובייקטים כבויים
        PauseMenuInputSystem pauseMenu =
            FindFirstObjectByType<PauseMenuInputSystem>(FindObjectsInactive.Include);

        if (pauseMenu != null)
        {
            pauseMenu.Restart();
        }
        else
        {
            Debug.LogWarning("PauseMenuInputSystem not found when clicking Restart.");
        }
    }
}