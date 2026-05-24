using UnityEngine;

public class ElevatorDeathZone : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Transform elevator;

    [Header("Safety Check")]
    [SerializeField] private float safeDistanceFromElevator = 1.2f;
    [SerializeField] private float fallingSpeedThreshold = -1f;

    private bool triggered = false;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        bool isFalling = rb.linearVelocity.y < fallingSpeedThreshold;

        bool isNearElevator =
            Mathf.Abs(other.transform.position.y - elevator.position.y) < safeDistanceFromElevator;

        if (isFalling && !isNearElevator)
        {
            triggered = true;
            ShowGameOver();
        }
    }

    private void ShowGameOver()
    {
        Time.timeScale = 0f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Debug.Log("Game Over: Player fell in elevator shaft");
    }
}