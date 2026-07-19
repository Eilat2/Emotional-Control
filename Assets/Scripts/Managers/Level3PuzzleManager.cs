using UnityEngine;
using UnityEngine.Serialization;

public class Level3PuzzleManager : MonoBehaviour
{
    [Header("Puzzle Board")]
    [SerializeField] private SpriteRenderer puzzleRenderer;

    [Header("Puzzle States")]
    [SerializeField] private Sprite emptyPuzzle;

    [SerializeField] private Sprite triangleOnly;
    [SerializeField] private Sprite squareOnly;
    [SerializeField] private Sprite circleOnly;
    [FormerlySerializedAs("eclipseOnly")]
    [SerializeField] private Sprite ellipseOnly;

    [SerializeField] private Sprite squareTriangle;
    [SerializeField] private Sprite circleTriangle;
    [FormerlySerializedAs("eclipseTriangle")]
    [SerializeField] private Sprite ellipseTriangle;

    [SerializeField] private Sprite squareCircle;
    [FormerlySerializedAs("eclipseSquare")]
    [SerializeField] private Sprite ellipseSquare;
    [FormerlySerializedAs("eclipseCircle")]
    [SerializeField] private Sprite ellipseCircle;

    [SerializeField] private Sprite squareCircleTriangle;
    [FormerlySerializedAs("circleTriangleEclipse")]
    [SerializeField] private Sprite circleTriangleEllipse;

    [SerializeField] private Sprite allTogether;
    [SerializeField] private Sprite allTogetherArrow;

    [Header("המעלית הסודית של שלב 3")]
    [SerializeField] private Level3SecretElevator secretElevator;

    [Header("Glow שיופיע כשהפאזל הושלם")]
    [SerializeField] private GameObject puzzleGlow;

    private bool _hasTriangle;
    private bool _hasSquare;
    private bool _hasCircle;
    private bool _hasEllipse;

    private bool _puzzleCompleted;

    private void Start()
    {
        if (puzzleGlow != null)
            puzzleGlow.SetActive(false);

        if (puzzleRenderer != null)
            puzzleRenderer.sprite = emptyPuzzle;
    }

    public void AddPiece(PuzzlePieceType pieceType)
    {
        if (_puzzleCompleted)
            return;

        switch (pieceType)
        {
            case PuzzlePieceType.Triangle:
                _hasTriangle = true;
                break;

            case PuzzlePieceType.Square:
                _hasSquare = true;
                break;

            case PuzzlePieceType.Circle:
                _hasCircle = true;
                break;

            case PuzzlePieceType.Ellipse:
                _hasEllipse = true;
                break;
        }

        StateLogger.Log(nameof(Level3PuzzleManager), $"Piece added: {pieceType}");

        UpdatePuzzleSprite();

        if (_hasTriangle && _hasSquare && _hasCircle && _hasEllipse)
            PuzzleComplete();
    }

    private void UpdatePuzzleSprite()
    {
        if (puzzleRenderer == null)
        {
            Debug.LogError("Level3PuzzleManager: Puzzle Renderer is not assigned.");
            return;
        }

        puzzleRenderer.sprite = ResolveSprite();
    }

    private Sprite ResolveSprite()
    {
        if (_hasTriangle && _hasSquare && _hasCircle && _hasEllipse) return allTogetherArrow;
        if (_hasTriangle && _hasSquare && _hasCircle) return squareCircleTriangle;
        if (_hasTriangle && _hasCircle && _hasEllipse) return circleTriangleEllipse;
        if (_hasTriangle && _hasSquare) return squareTriangle;
        if (_hasTriangle && _hasCircle) return circleTriangle;
        if (_hasTriangle && _hasEllipse) return ellipseTriangle;
        if (_hasSquare && _hasCircle) return squareCircle;
        if (_hasSquare && _hasEllipse) return ellipseSquare;
        if (_hasCircle && _hasEllipse) return ellipseCircle;
        if (_hasTriangle) return triangleOnly;
        if (_hasSquare) return squareOnly;
        if (_hasCircle) return circleOnly;
        if (_hasEllipse) return ellipseOnly;

        return emptyPuzzle;
    }

    private void PuzzleComplete()
    {
        _puzzleCompleted = true;

        StateLogger.Log(nameof(Level3PuzzleManager), "Puzzle complete - elevator can now activate.");

        if (puzzleGlow != null)
            puzzleGlow.SetActive(true);

        if (secretElevator != null)
            secretElevator.UnlockElevator();
        else
            Debug.LogWarning("Level3PuzzleManager: Secret Elevator is not connected.");
    }
}
