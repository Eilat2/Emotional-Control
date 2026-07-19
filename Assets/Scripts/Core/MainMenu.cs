using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string firstLevelName = "Level1";

    [Header("Panels")]
    [SerializeField] private GameObject buttonsHolder;
    [SerializeField] private GameObject optionsPanel;

    public void StartGame()
    {
        SceneManager.LoadScene(firstLevelName);
    }

    public void OpenOptions()
    {
        SetPanels(buttonsActive: false, optionsActive: true);
    }

    public void CloseOptions()
    {
        SetPanels(buttonsActive: true, optionsActive: false);
    }

    private void SetPanels(bool buttonsActive, bool optionsActive)
    {
        if (buttonsHolder != null)
            buttonsHolder.SetActive(buttonsActive);
        else
            Debug.LogWarning("MainMenu: Buttons holder is not assigned.");

        if (optionsPanel != null)
            optionsPanel.SetActive(optionsActive);
        else
            Debug.LogWarning("MainMenu: Options panel is not assigned.");
    }

    public void QuitGame()
    {
        StateLogger.Log(nameof(MainMenu), "Quit Game requested.");
        Application.Quit();
    }
}
