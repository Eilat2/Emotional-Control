using UnityEngine;

public class GameOverRestartButton : MonoBehaviour
{
    public void RestartGame()
    {
        GameEvents.RaiseRestartRequested();
    }
}