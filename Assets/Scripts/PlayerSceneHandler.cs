using UnityEngine;
using UnityEngine.SceneManagement;

// מטפל במעבר בין סצנות:
// 1. מעביר את השחקן לנקודת התחלה
// 2. ממלא מחדש את הסטאמינה
public class PlayerSceneHandler : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // מחפש נקודת התחלה בסצנה החדשה
        GameObject spawnPoint = GameObject.Find("PlayerSpawnPoint");

        if (spawnPoint != null)
        {
            transform.position = spawnPoint.transform.position;
            Debug.Log("Player moved to spawn point.");
        }
        else
        {
            Debug.LogWarning("PlayerSpawnPoint was not found.");
        }

        // ממלא מחדש את כל הסטאמינות של השחקן
        Stamina[] staminas = GetComponents<Stamina>();

        foreach (Stamina stamina in staminas)
        {
            stamina.Refill();
        }

        Debug.Log("Player stamina refilled.");
    }
}