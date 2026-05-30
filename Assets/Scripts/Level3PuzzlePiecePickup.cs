using UnityEngine;

public class Level3PuzzlePiecePickup : MonoBehaviour
{
    [Header("סוג חלק הפאזל")]
    [SerializeField] private PuzzlePieceType pieceType;

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

        canBeCollected = !lockedUntilStoneBroken;
    }

    public void UnlockAfterStoneBroken()
    {
        canBeCollected = true;

        Debug.Log("Puzzle piece unlocked: " + pieceType);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;
        if (!other.CompareTag("Player")) return;
        if (!canBeCollected) return;

        collected = true;

        Debug.Log("Collected: " + pieceType);

        if (puzzleManager != null)
        {
            puzzleManager.AddPiece(pieceType);
        }
        else
        {
            Debug.LogWarning("Puzzle Manager is NULL!");
        }

        gameObject.SetActive(false);
    }
}