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
        buttonsHolder.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        buttonsHolder.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}