using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

// מנהל את חלון הפאוז, המשך משחק וריסטארט של השלב
public class PauseMenuInputSystem : MonoBehaviour
{
    // הפאנל של הפאוז
    [SerializeField] private GameObject pausePanel;

    // האם המשחק כרגע בפאוז
    private bool isPaused = false;

    private void Start()
    {
        // בתחילת המשחק סוגרים את חלון הפאוז
        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
    }

    private void Update()
    {
        // אם אין מקלדת לא עושים כלום
        if (Keyboard.current == null)
            return;

        // לחיצה אחת על ESC פותחת/סוגרת
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    // פותח את חלון הפאוז
    public void Pause()
    {
        if (pausePanel == null)
        {
            Debug.LogWarning("Pause panel is not assigned.");
            return;
        }

        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        Debug.Log("Pause menu opened.");
    }

    // סוגר את חלון הפאוז
    public void Resume()
    {
        if (pausePanel == null)
        {
            Debug.LogWarning("Pause panel is not assigned.");
            return;
        }

        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        Debug.Log("Pause menu closed.");
    }

    // עושה ריסטארט לשלב הנוכחי
    public void Restart()
    {
        Time.timeScale = 1f;
        isPaused = false;

        Debug.Log("Restarting current level.");

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}