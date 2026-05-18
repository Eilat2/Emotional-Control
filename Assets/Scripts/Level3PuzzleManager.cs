using UnityEngine;

public class Level3PuzzleManager : MonoBehaviour
{
    [Header("כמה חלקים צריך כדי להשלים את הפאזל")]
    [SerializeField] private int piecesNeeded = 4;

    [Header("המעלית הסודית של שלב 3")]
    [SerializeField] private Level3SecretElevator secretElevator;

    [Header("Glow שיופיע כשהפאזל הושלם")]
    [SerializeField] private GameObject puzzleGlow;

    private int collectedPieces = 0;
    private bool puzzleCompleted = false;

    private void Start()
    {
        // בתחילת השלב ה-Glow כבוי
        if (puzzleGlow != null)
        {
            puzzleGlow.SetActive(false);
        }
    }

    public void AddPiece()
    {
        if (puzzleCompleted) return;

        collectedPieces++;

        Debug.Log("Level 3 Puzzle Pieces: " + collectedPieces + "/" + piecesNeeded);

        if (collectedPieces >= piecesNeeded)
        {
            PuzzleComplete();
        }
    }

    private void PuzzleComplete()
    {
        puzzleCompleted = true;

        Debug.Log("Level 3 Puzzle Complete! Elevator can now activate.");

        // מדליקים Glow מסביב לפאזל כשהוא הושלם
        if (puzzleGlow != null)
        {
            puzzleGlow.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Puzzle Glow is not connected to Level3PuzzleManager");
        }

        if (secretElevator != null)
        {
            secretElevator.UnlockElevator();
        }
        else
        {
            Debug.LogWarning("Secret Elevator is not connected to Level3PuzzleManager");
        }
    }
}