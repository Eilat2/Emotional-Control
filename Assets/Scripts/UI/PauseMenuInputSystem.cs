using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

// מנהל את חלון הפאוז (Pause), המשך משחק (Resume) וריסטארט (Restart)
public class PauseMenuInputSystem : MonoBehaviour
{
    [Header("Pause UI")]

    // הפאנל הראשי של הפאוז (כל החלון)
    [SerializeField] private GameObject pausePanel;

    // הכפתור הראשון שייבחר אוטומטית (כדי שהUI יעבוד מיד)
    [SerializeField] private GameObject firstSelectedButton;

    private void Start()
    {
        // בתחילת המשחק סוגרים את חלון הפאוז
        if (pausePanel != null)
            pausePanel.SetActive(false);

        // מבטיחים שהמשחק רץ
        Time.timeScale = 1f;
    }

    private void Update()
    {
        // אם אין מקלדת – לא עושים כלום
        if (Keyboard.current == null)
            return;

        // אם לחצו ESC בפריים הזה
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // בודקים אם הפאנל פתוח בפועל (לא לפי משתנה!)
            bool panelIsOpen = pausePanel != null && pausePanel.activeSelf;

            // אם פתוח → סוגרים
            if (panelIsOpen)
                Resume();
            // אם סגור → פותחים
            else
                Pause();
        }
    }

    // פתיחת חלון פאוז
    public void Pause()
    {
        if (pausePanel == null)
        {
            Debug.LogWarning("Pause panel is not assigned.");
            return;
        }

        // מציגים את הפאנל
        pausePanel.SetActive(true);

        // עוצרים את הזמן במשחק
        Time.timeScale = 0f;

        // מגדירים כפתור ראשון כדי שהUI יגיב (חשוב מאוד!)
        if (EventSystem.current != null && firstSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }

        Debug.Log("Pause menu opened.");
    }

    // חזרה למשחק
    public void Resume()
    {
        if (pausePanel == null)
            return;

        // מסתירים את הפאנל
        pausePanel.SetActive(false);

        // מחזירים את הזמן
        Time.timeScale = 1f;

        Debug.Log("Pause menu closed.");
    }

    // ריסטארט של השלב
    public void Restart()
    {
        // מחזירים זמן לנורמלי
        Time.timeScale = 1f;

        Debug.Log("Restarting current level.");

        // טוענים מחדש את הסצנה הנוכחית
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}