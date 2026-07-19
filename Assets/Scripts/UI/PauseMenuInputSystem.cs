using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// מנהל Pause, Restart ו-Game Over
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

    private bool _isPaused;
    private bool _isGameOver;

    private void Awake()
    {
        EnsurePausePanel();
        EnsureGameOverPanel();
    }

    private void OnEnable()
    {
        GameEvents.OnGameOver += GameOver;
        GameEvents.OnRestartRequested += Restart;
    }

    private void OnDisable()
    {
        GameEvents.OnGameOver -= GameOver;
        GameEvents.OnRestartRequested -= Restart;
    }

    private void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
        else
            Debug.LogWarning("PauseMenuInputSystem: PausePanel לא מחובר.");

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        _isPaused = false;
        _isGameOver = false;
        Time.timeScale = 1f;

        SetupRestartButton();
    }

    private void Update()
    {
        if (_isGameOver)
            return;

        bool escapePressed =
            Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame;

        bool pPressed =
            Keyboard.current != null &&
            Keyboard.current.pKey.wasPressedThisFrame;

        if (escapePressed || pPressed)
            TogglePause();
    }

    private void TogglePause()
    {
        if (_isPaused)
            Resume();
        else
            Pause();
    }

    private void SetupRestartButton()
    {
        if (gameOverRestartButton == null && gameOverFirstSelectedButton != null)
        {
            gameOverRestartButton =
                gameOverFirstSelectedButton.GetComponent<Button>();
        }

        if (gameOverRestartButton == null)
            return;

        gameOverRestartButton.onClick.RemoveAllListeners();
        gameOverRestartButton.onClick.AddListener(
            GameEvents.RaiseRestartRequested
        );
    }

    public void Pause()
    {
        if (!EnsurePausePanel())
        {
            Debug.LogWarning(
                "PauseMenuInputSystem: אי אפשר לפתוח Pause כי PausePanel לא מחובר."
            );
            return;
        }

        _isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;

        StopPlayerMovement();
        SelectUiButton(firstSelectedButton);

        StateLogger.Log(
            nameof(PauseMenuInputSystem),
            "Pause opened"
        );
    }

    public void Resume()
    {
        if (!EnsurePausePanel())
        {
            Debug.LogWarning(
                "PauseMenuInputSystem: אי אפשר לסגור Pause כי PausePanel לא מחובר."
            );
            return;
        }

        _isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;

        SelectUiButton(null);

        StateLogger.Log(
            nameof(PauseMenuInputSystem),
            "Pause closed"
        );
    }

    public void Restart()
    {
        _isGameOver = false;
        _isPaused = false;
        Time.timeScale = 1f;

        SelectUiButton(null);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        StateLogger.Log(
            nameof(PauseMenuInputSystem),
            "Restart requested - reloading scene"
        );

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    public void GameOver()
    {
        if (!EnsureGameOverPanel())
        {
            Debug.LogWarning(
                "PauseMenuInputSystem: Game Over panel is not assigned."
            );
            return;
        }

        _isGameOver = true;
        _isPaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;

        StopPlayerMovement();
        SelectUiButton(gameOverFirstSelectedButton);

        StateLogger.Log(
            nameof(PauseMenuInputSystem),
            "Game Over menu opened."
        );
    }

    private void StopPlayerMovement()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player == null)
            return;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (rb == null)
            return;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    private void SelectUiButton(GameObject buttonToSelect)
    {
        if (EventSystem.current == null)
            return;

        EventSystem.current.SetSelectedGameObject(null);

        if (buttonToSelect != null)
            EventSystem.current.SetSelectedGameObject(buttonToSelect);
    }

    private bool EnsurePausePanel()
    {
        if (pausePanel == null)
            pausePanel = FindInactiveObjectByName("PausePanel");

        return pausePanel != null;
    }

    private bool EnsureGameOverPanel()
    {
        if (gameOverPanel == null)
            gameOverPanel = FindInactiveObjectByName("GameOverPanel");

        return gameOverPanel != null;
    }

    private GameObject FindInactiveObjectByName(string objectName)
    {
        GameObject[] allObjects =
            Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name == objectName && obj.scene.IsValid())
                return obj;
        }

        return null;
    }
}