using UnityEngine;

public class Level3PuzzleManager : MonoBehaviour
{
    [Header("Puzzle Board")]
    [SerializeField] private SpriteRenderer puzzleRenderer;

    [Header("Puzzle States")]
    [SerializeField] private Sprite emptyPuzzle;

    [SerializeField] private Sprite triangleOnly;
    [SerializeField] private Sprite squareOnly;
    [SerializeField] private Sprite circleOnly;
    [SerializeField] private Sprite eclipseOnly;

    [SerializeField] private Sprite squareTriangle;
    [SerializeField] private Sprite circleTriangle;
    [SerializeField] private Sprite eclipseTriangle;

    [SerializeField] private Sprite squareCircle;
    [SerializeField] private Sprite eclipseSquare;
    [SerializeField] private Sprite eclipseCircle;

    [SerializeField] private Sprite squareCircleTriangle;
    [SerializeField] private Sprite circleTriangleEclipse;

    [SerializeField] private Sprite allTogether;
    [SerializeField] private Sprite allTogetherArrow;

    [Header("המעלית הסודית של שלב 3")]
    [SerializeField] private Level3SecretElevator secretElevator;

    [Header("Glow שיופיע כשהפאזל הושלם")]
    [SerializeField] private GameObject puzzleGlow;

    private bool hasTriangle;
    private bool hasSquare;
    private bool hasCircle;
    private bool hasEclipse;

    private bool puzzleCompleted = false;

    private void Start()
    {
        if (puzzleGlow != null)
            puzzleGlow.SetActive(false);

        if (puzzleRenderer != null)
            puzzleRenderer.sprite = emptyPuzzle;
    }

    public void AddPiece(PuzzlePieceType pieceType)
    {
        Debug.Log("Manager received piece: " + pieceType);

        if (puzzleCompleted)
            return;

        switch (pieceType)
        {
            case PuzzlePieceType.Triangle:
                hasTriangle = true;
                Debug.Log("Triangle added");
                break;

            case PuzzlePieceType.Square:
                hasSquare = true;
                Debug.Log("Square added");
                break;

            case PuzzlePieceType.Circle:
                hasCircle = true;
                Debug.Log("Circle added");
                break;

            case PuzzlePieceType.Eclipse:
                hasEclipse = true;
                Debug.Log("Eclipse added");
                break;
        }

        UpdatePuzzleSprite();

        if (hasTriangle && hasSquare && hasCircle && hasEclipse)
            PuzzleComplete();
    }

    private void UpdatePuzzleSprite()
    {
        if (puzzleRenderer == null)
        {
            Debug.LogError("Puzzle Renderer is NULL!");
            return;
        }

        Debug.Log(
            "UpdatePuzzleSprite -> " +
            "Triangle=" + hasTriangle +
            " Square=" + hasSquare +
            " Circle=" + hasCircle +
            " Eclipse=" + hasEclipse);

        if (hasTriangle && hasSquare && hasCircle && hasEclipse)
        {
            Debug.Log("Setting sprite: allTogetherArrow");
            puzzleRenderer.sprite = allTogetherArrow;
        }
        else if (hasTriangle && hasSquare && hasCircle)
        {
            Debug.Log("Setting sprite: squareCircleTriangle");
            puzzleRenderer.sprite = squareCircleTriangle;
        }
        else if (hasTriangle && hasCircle && hasEclipse)
        {
            Debug.Log("Setting sprite: circleTriangleEclipse");
            puzzleRenderer.sprite = circleTriangleEclipse;
        }
        else if (hasTriangle && hasSquare)
        {
            Debug.Log("Setting sprite: squareTriangle");
            puzzleRenderer.sprite = squareTriangle;
        }
        else if (hasTriangle && hasCircle)
        {
            Debug.Log("Setting sprite: circleTriangle");
            puzzleRenderer.sprite = circleTriangle;
        }
        else if (hasTriangle && hasEclipse)
        {
            Debug.Log("Setting sprite: eclipseTriangle");
            puzzleRenderer.sprite = eclipseTriangle;
        }
        else if (hasSquare && hasCircle)
        {
            Debug.Log("Setting sprite: squareCircle");
            puzzleRenderer.sprite = squareCircle;
        }
        else if (hasSquare && hasEclipse)
        {
            Debug.Log("Setting sprite: eclipseSquare");
            puzzleRenderer.sprite = eclipseSquare;
        }
        else if (hasCircle && hasEclipse)
        {
            Debug.Log("Setting sprite: eclipseCircle");
            puzzleRenderer.sprite = eclipseCircle;
        }
        else if (hasTriangle)
        {
            Debug.Log("Setting sprite: triangleOnly");
            puzzleRenderer.sprite = triangleOnly;
        }
        else if (hasSquare)
        {
            Debug.Log("Setting sprite: squareOnly");
            puzzleRenderer.sprite = squareOnly;
        }
        else if (hasCircle)
        {
            Debug.Log("Setting sprite: circleOnly");
            puzzleRenderer.sprite = circleOnly;
        }
        else if (hasEclipse)
        {
            Debug.Log("Setting sprite: eclipseOnly");
            puzzleRenderer.sprite = eclipseOnly;
        }
        else
        {
            Debug.Log("Setting sprite: emptyPuzzle");
            puzzleRenderer.sprite = emptyPuzzle;
        }
    }

    private void PuzzleComplete()
    {
        puzzleCompleted = true;

        Debug.Log("Level 3 Puzzle Complete! Elevator can now activate.");

        if (puzzleGlow != null)
            puzzleGlow.SetActive(true);

        if (secretElevator != null)
            secretElevator.UnlockElevator();
        else
            Debug.LogWarning("Secret Elevator is not connected to Level3PuzzleManager");
    }
}