using UnityEngine;

public class PlayerVisualSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject neutralVisual;
    [SerializeField] private GameObject joyVisual;
    [SerializeField] private GameObject rageVisual;

    public void ShowNeutral()
    {
        neutralVisual.SetActive(true);
        joyVisual.SetActive(false);
        rageVisual.SetActive(false);
    }

    public void ShowJoy()
    {
        neutralVisual.SetActive(false);
        joyVisual.SetActive(true);
        rageVisual.SetActive(false);
    }

    public void ShowRage()
    {
        neutralVisual.SetActive(false);
        joyVisual.SetActive(false);
        rageVisual.SetActive(true);
    }
}