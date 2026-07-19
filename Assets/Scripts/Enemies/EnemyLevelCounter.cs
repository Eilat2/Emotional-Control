using UnityEngine;

public class EnemyLevelCounter : MonoBehaviour
{
    [Header("Door")]
    [SerializeField] private GameObject door;

    private int _enemiesAlive;

    private void Start()
    {
        _enemiesAlive = FindObjectsByType<KillableEnemy>(FindObjectsSortMode.None).Length;

        StateLogger.Log(nameof(EnemyLevelCounter), $"Enemies alive: {_enemiesAlive}");

        if (door != null)
            door.SetActive(false);
    }

    public void EnemyDied()
    {
        _enemiesAlive--;

        StateLogger.Log(nameof(EnemyLevelCounter), $"Enemy killed. Remaining: {_enemiesAlive}");

        if (_enemiesAlive <= 0)
            OpenDoor();
    }

    private void OpenDoor()
    {
        StateLogger.Log(nameof(EnemyLevelCounter), "All enemies defeated. Door opened!");

        if (door != null)
            door.SetActive(true);
    }
}
