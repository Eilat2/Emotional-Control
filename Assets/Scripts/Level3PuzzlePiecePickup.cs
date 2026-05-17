using UnityEngine;

public class Level3PuzzlePiecePickup : MonoBehaviour
{
    [Header("החלק שיופיע על לוח הפאזל")]
    [SerializeField] private GameObject pieceVisualOnBoard;

    [Header("מנהל הפאזל של שלב 3")]
    [SerializeField] private Level3PuzzleManager puzzleManager;

    private bool collected = false;

    private void Start()
    {
        if (puzzleManager == null)
            puzzleManager = FindFirstObjectByType<Level3PuzzleManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;
        if (!other.CompareTag("Player")) return;

        collected = true;

        Debug.Log("Collected puzzle piece: " + gameObject.name);

        if (pieceVisualOnBoard != null)
        {
            Debug.Log("Activating board visual: " + pieceVisualOnBoard.name);
            pieceVisualOnBoard.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No Piece Visual connected on: " + gameObject.name);
        }

        if (puzzleManager != null)
        {
            puzzleManager.AddPiece();
        }
        else
        {
            Debug.LogWarning("Level3PuzzleManager not found for: " + gameObject.name);
        }

        gameObject.SetActive(false);
    }
}