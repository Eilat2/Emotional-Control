using UnityEngine;

public class Level3PuzzlePiecePickup : MonoBehaviour
{
    [Header("סוג חלק הפאזל")]
    [SerializeField] private PuzzlePieceType pieceType;

    [Header("מנהל הפאזל של שלב 3")]
    [SerializeField] private Level3PuzzleManager puzzleManager;

    [Header("לסמן רק לחלק שמאחורי האבן")]
    [SerializeField] private bool lockedUntilStoneBroken = false;

    private bool _collected;
    private bool _canBeCollected = true;

    private void Start()
    {
        if (puzzleManager == null)
            puzzleManager = FindFirstObjectByType<Level3PuzzleManager>();

        _canBeCollected = !lockedUntilStoneBroken;
    }

    public void UnlockAfterStoneBroken()
    {
        _canBeCollected = true;
        StateLogger.Log(nameof(Level3PuzzlePiecePickup), $"Puzzle piece unlocked: {pieceType}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_collected || !_canBeCollected)
            return;

        if (!other.CompareTag("Player"))
            return;

        _collected = true;
        StateLogger.Log(nameof(Level3PuzzlePiecePickup), $"Collected: {pieceType}");

        if (puzzleManager != null)
            puzzleManager.AddPiece(pieceType);
        else
            Debug.LogWarning("Level3PuzzlePiecePickup: Puzzle Manager is not assigned.");

        gameObject.SetActive(false);
    }
}
