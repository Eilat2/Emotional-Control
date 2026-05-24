using UnityEngine;

public class Level3PuzzlePiecePickup : MonoBehaviour
{
    [Header("החלק שיופיע על לוח הפאזל")]
    [SerializeField] private GameObject pieceVisualOnBoard;

    [Header("מנהל הפאזל של שלב 3")]
    [SerializeField] private Level3PuzzleManager puzzleManager;

    [Header("לסמן רק לחלק שמאחורי האבן")]
    [SerializeField] private bool lockedUntilStoneBroken = false;

    private bool collected = false;
    private bool canBeCollected = true;

    private void Start()
    {
        if (puzzleManager == null)
            puzzleManager = FindFirstObjectByType<Level3PuzzleManager>();

        // אם סימנו שהחלק נעול — אי אפשר לאסוף אותו עד שהאבן נשברת
        canBeCollected = !lockedUntilStoneBroken;
    }

    // נקרא מתוך BreakableWall אחרי שהאבן נשברת
    public void UnlockAfterStoneBroken()
    {
        canBeCollected = true;
        Debug.Log("Puzzle piece unlocked after stone was broken: " + gameObject.name);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;
        if (!other.CompareTag("Player")) return;

        // רק החלק שמאחורי האבן אמור להיחסם פה
        if (!canBeCollected)
        {
            Debug.Log("Cannot collect before breaking the stone: " + gameObject.name);
            return;
        }

        collected = true;

        Debug.Log("Collected puzzle piece: " + gameObject.name);

        if (pieceVisualOnBoard != null)
        {
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