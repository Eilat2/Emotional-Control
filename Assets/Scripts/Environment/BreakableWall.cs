using UnityEngine;

public class BreakableWall : MonoBehaviour, IBreakable
{
    [Header("Debris Settings")]
    [SerializeField] private GameObject debrisPiecePrefab;
    [SerializeField] private int debrisCount = 12;
    [SerializeField] private float debrisForce = 4f;
    [SerializeField] private float debrisTorque = 200f;
    [SerializeField] private float debrisLifeTime = 1.5f;
    [SerializeField] private float debrisPositionJitter = 0.15f;

    [Header("Reveal")]
    [SerializeField] private GameObject hiddenButton;

    [Header("Tutorial")]
    [SerializeField] private GameObject tutorialPopup;

    [Header("Puzzle Piece Unlock")]
    [SerializeField] private Level3PuzzlePiecePickup puzzlePieceToUnlock;

    private bool _isBroken;

    public void OnBreak()
    {
        if (_isBroken)
            return;

        _isBroken = true;

        DebrisSpawner.SpawnRandomDebris(
            debrisPiecePrefab,
            transform.position,
            debrisCount,
            debrisForce,
            debrisTorque,
            positionJitter: debrisPositionJitter,
            lifeTime: debrisLifeTime
        );

        HideTutorialPopup();

        if (hiddenButton != null)
            hiddenButton.SetActive(true);

        if (puzzlePieceToUnlock != null)
            puzzlePieceToUnlock.UnlockAfterStoneBroken();

        Destroy(gameObject);
    }

    private void HideTutorialPopup()
    {
        TutorialPopupTrigger tutorial = GetComponent<TutorialPopupTrigger>();

        if (tutorial != null)
            tutorial.HidePopupForever();
        else if (tutorialPopup != null)
            tutorialPopup.SetActive(false);
    }
}
