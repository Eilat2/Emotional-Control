using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

// מנהל את חלון הפאוז (Pause), המשך משחק (Resume) וריסטארט (Restart)
public class PauseMenuInputSystem : MonoBehaviour
{
    [Header("Pause UI")]

    // הפאנל הראשי של הפאוז
    [SerializeField] private GameObject pausePanel;

    // הכפתור הראשון שייבחר אוטומטית
    [SerializeField] private GameObject firstSelectedButton;

    private void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            bool panelIsOpen = pausePanel != null && pausePanel.activeSelf;

            if (panelIsOpen)
                Resume();
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

        pausePanel.SetActive(true);

        // עוצרים את המשחק
        Time.timeScale = 0f;

        // עוצרים פיזית את השחקן כדי שלא ימשיך לזוז בזמן פאוז
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }

        // בוחרים כפתור ראשון כדי שה-UI יהיה מוכן לקלט
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

        pausePanel.SetActive(false);

        Time.timeScale = 1f;

        Debug.Log("Pause menu closed.");
    }

    // ריסטארט של השלב הנוכחי
    public void Restart()
    {
        Debug.Log("RESTART BUTTON CLICKED!!! Scene: " + SceneManager.GetActiveScene().name);

        // מחזירים את הזמן לפני טעינת הסצנה
        Time.timeScale = 1f;

        // סוגרים את חלון הפאוז
        if (pausePanel != null)
            pausePanel.SetActive(false);

        // טוענים מחדש את הסצנה הנוכחית בלבד
        // לא מוחקים את ה-Player, כי הוא Persistent ועובר בין שלבים
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}