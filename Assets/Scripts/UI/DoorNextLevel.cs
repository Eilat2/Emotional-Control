using UnityEngine;

public class DoorNextLevel : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    [SerializeField] private SceneFader sceneFader;

    private bool canLoad = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canLoad) return;
        if (!other.CompareTag("Player")) return;

        canLoad = false;

        if (sceneFader != null)
        {
            sceneFader.FadeToScene(nextSceneName);
        }
    }
}