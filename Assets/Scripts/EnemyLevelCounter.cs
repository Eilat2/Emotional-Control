using UnityEngine;

public class EnemyLevelCounter : MonoBehaviour
{
    [Header("Door")]
    [SerializeField] private GameObject door;

    private int enemiesAlive;

    private void Start()
    {
        enemiesAlive = FindObjectsOfType<KillableEnemy>().Length;

        Debug.Log("Enemies alive: " + enemiesAlive);

        // מתחילים עם דלת כבויה
        if (door != null)
        {
            door.SetActive(false);
        }
    }

    public void EnemyDied()
    {
        enemiesAlive--;

        Debug.Log("Enemy killed. Remaining: " + enemiesAlive);

        if (enemiesAlive <= 0)
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        Debug.Log("All enemies defeated. Door opened!");

        if (door != null)
        {
            door.SetActive(true);
        }
    }
}