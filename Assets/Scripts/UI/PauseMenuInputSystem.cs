using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenuInputSystem : MonoBehaviour
{
    public GameObject pausePanel;

    private bool isPaused = false;
    private PlayerInput playerInput;
    private InputAction pauseAction;

    private void Awake()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("PauseMenuInputSystem: No PlayerInput found in scene.");
            return;
        }

        pauseAction = playerInput.actions["Pause"];
        if (pauseAction == null)
        {
            Debug.LogError("PauseMenuInputSystem: No action named 'Pause' found.");
        }
    }

    private void OnEnable()
    {
        if (pauseAction != null)
            pauseAction.performed += OnPausePerformed;
    }

    private void OnDisable()
    {
        if (pauseAction != null)
            pauseAction.performed -= OnPausePerformed;
    }

    private void OnPausePerformed(InputAction.CallbackContext ctx)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
