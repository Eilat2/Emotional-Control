using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// מנהל פאוז + ריסטארט + GAME OVER
public class PauseMenuInputSystem : MonoBehaviour
{
    [Header("Pause UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject firstSelectedButton;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameOverFirstSelectedButton;

    [Header("Game Over Button")]
    [SerializeField] private Button gameOverRestartButton;

    private void OnEnable()
    {
        // מאזין ל-Events
        GameEvents.OnGameOver += GameOver;
        GameEvents.OnRestartRequested += Restart;
    }

    private void OnDisable()
    {
        // מפסיק להאזין
        GameEvents.OnGameOver -= GameOver;
        GameEvents.OnRestartRequested -= Restart;
    }

    private void Start()
    {
        // סוגרים פאנלים בהתחלה
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        Time.timeScale = 1f;

        // מחברים את כפתור הריסטארט דרך קוד
        SetupRestartButton();
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        // ESC פותח/סוגר Pause
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            bool panelIsOpen = pausePanel != null && pausePanel.activeSelf;

            if (panelIsOpen)
                Resume();
            else
                Pause();
        }
    }

    // =======================
    // 🔥 חיבור כפתור ריסטארט
    // =======================
    private void SetupRestartButton()
    {
        if (gameOverRestartButton == null && gameOverFirstSelectedButton != null)
            gameOverRestartButton = gameOverFirstSelectedButton.GetComponent<Button>();

        if (gameOverRestartButton != null)
        {
            gameOverRestartButton.onClick.RemoveAllListeners();

            // במקום לקרוא ישירות ל-Restart
            // הכפתור רק שולח Event
            gameOverRestartButton.onClick.AddListener(() =>
            {
                GameEvents.RaiseRestartRequested();
            });
        }
    }

    // =======================
    // ⏸️ PAUSE
    // =======================
    public void Pause()
    {
        if (pausePanel == null) return;

        pausePanel.SetActive(true);
        Time.timeScale = 0f;

        StopPlayerMovement();

        if (EventSystem.current != null && firstSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
    }

    // ▶️ RESUME
    public void Resume()
    {
        if (pausePanel == null) return;

        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // =======================
    // 🔄 RESTART
    // =======================
    public void Restart()
    {
        Debug.Log("GAME OVER RESTART CLICKED");

        // מחזירים זמן רגיל
        Time.timeScale = 1f;

        // מנקים UI
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        // סוגרים פאנלים
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // טוענים מחדש את הסצנה
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // =======================
    // 💀 GAME OVER
    // =======================
    public void GameOver()
    {
        if (gameOverPanel == null)
            gameOverPanel = FindInactiveObjectByName("GameOverPanel");

        if (gameOverPanel == null)
        {
            Debug.LogWarning("Game Over panel is not assigned.");
            return;
        }

        gameOverPanel.SetActive(true);

        Time.timeScale = 0f;

        StopPlayerMovement();

        if (EventSystem.current != null && gameOverFirstSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(gameOverFirstSelectedButton);
        }

        Debug.Log("Game Over menu opened.");
    }

    // =======================
    // 🧠 עצירת שחקן
    // =======================
    private void StopPlayerMovement()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player == null) return;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    // =======================
    // 🔍 חיפוש אובייקט גם אם כבוי
    // =======================
    private GameObject FindInactiveObjectByName(string objectName)
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name == objectName && obj.scene.IsValid())
                return obj;
        }

        return null;
    }
}