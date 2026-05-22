using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    private bool isPaused = false;

    private void Awake()
    {
        if (pausePanel == null)
            pausePanel = FindInactiveObjectByName("PausePanel");

        if (gameOverPanel == null)
            gameOverPanel = FindInactiveObjectByName("GameOverPanel");
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
        Debug.Log("PauseMenuInputSystem STARTED");

        if (pausePanel != null)
            pausePanel.SetActive(false);
        else
            Debug.LogWarning("PauseMenuInputSystem: PausePanel לא מחובר.");

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        isPaused = false;
        Time.timeScale = 1f;

        SetupRestartButton();
    }

    private void Update()
    {
        bool escapePressed = Keyboard.current != null &&
                             Keyboard.current.escapeKey.wasPressedThisFrame;

        bool pPressed = Keyboard.current != null &&
                        Keyboard.current.pKey.wasPressedThisFrame;

        if (escapePressed || pPressed)
        {
            Debug.Log("PauseMenuInputSystem: Pause key pressed");

            if (GameStateMachine.Instance != null)
            {
                if (GameStateMachine.Instance.CurrentState ==
                    GameStateMachine.Instance.PlayingState)
                {
                    GameStateMachine.Instance
                        .TransitionTo(GameStateMachine.Instance.PausedState);
                }
                else if (GameStateMachine.Instance.CurrentState ==
                         GameStateMachine.Instance.PausedState)
                {
                    GameStateMachine.Instance
                        .TransitionTo(GameStateMachine.Instance.PlayingState);
                }
            }
            else
            {
                TogglePause();
            }
        }
    }

    private void TogglePause()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    private void SetupRestartButton()
    {
        if (gameOverRestartButton == null && gameOverFirstSelectedButton != null)
            gameOverRestartButton = gameOverFirstSelectedButton.GetComponent<Button>();

        if (gameOverRestartButton != null)
        {
            gameOverRestartButton.onClick.RemoveAllListeners();

            gameOverRestartButton.onClick.AddListener(() =>
            {
                GameEvents.RaiseRestartRequested();
            });
        }
    }

    public void Pause()
    {
        if (pausePanel == null)
            pausePanel = FindInactiveObjectByName("PausePanel");

        if (pausePanel == null)
        {
            Debug.LogWarning("PauseMenuInputSystem: אי אפשר לפתוח Pause כי PausePanel לא מחובר.");
            return;
        }

        isPaused = true;

        pausePanel.SetActive(true);
        Time.timeScale = 0f;

        StopPlayerMovement();

        if (EventSystem.current != null && firstSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }

        Debug.Log("PauseMenuInputSystem: Pause opened");
    }

    public void Resume()
    {
        if (pausePanel == null)
            pausePanel = FindInactiveObjectByName("PausePanel");

        if (pausePanel == null)
        {
            Debug.LogWarning("PauseMenuInputSystem: אי אפשר לסגור Pause כי PausePanel לא מחובר.");
            return;
        }

        isPaused = false;

        pausePanel.SetActive(false);
        Time.timeScale = 1f;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        Debug.Log("PauseMenuInputSystem: Pause closed");
    }

    public void Restart()
    {
        Debug.Log("GAME OVER RESTART CLICKED");

        Time.timeScale = 1f;
        isPaused = false;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

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

    private void StopPlayerMovement()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player == null)
            return;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

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