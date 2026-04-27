using UnityEngine;

public class WaterExtinguishFire : MonoBehaviour
{
    [SerializeField] private PuzzleManager puzzleManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Fire"))
        {
            puzzleManager.ExtinguishFireAndRevealDoor();
        }
    }
}