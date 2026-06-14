using UnityEngine;

public class BreakableWall : MonoBehaviour, IBreakable
{
    [Header("Debris Settings")]
    [SerializeField] GameObject debrisPiecePrefab;
    [SerializeField] int debrisCount = 12;
    [SerializeField] float debrisForce = 4f;
    [SerializeField] float debrisTorque = 200f;
    [SerializeField] float debrisLifeTime = 1.5f;

    [Header("Reveal")]
    [SerializeField] GameObject hiddenButton;

    [Header("Tutorial")]
    [SerializeField] GameObject tutorialPopup;

    [Header("Puzzle Piece Unlock")]
    [SerializeField] private Level3PuzzlePiecePickup puzzlePieceToUnlock;

    private bool isBroken = false;

    public void OnBreak()
    {
        if (isBroken) return;

        isBroken = true;

        SpawnDebris();

        TutorialPopupTrigger tutorial = GetComponent<TutorialPopupTrigger>();

        if (tutorial != null)
        {
            tutorial.HidePopupForever();
        }
        else if (tutorialPopup != null)
        {
            tutorialPopup.SetActive(false);
        }

        if (hiddenButton != null)
        {
            hiddenButton.SetActive(true);
        }

        if (puzzlePieceToUnlock != null)
        {
            puzzlePieceToUnlock.UnlockAfterStoneBroken();
        }

        Destroy(gameObject);
    }

    private void SpawnDebris()
    {
        if (debrisPiecePrefab == null) return;

        for (int i = 0; i < debrisCount; i++)
        {
            Vector3 offset = Random.insideUnitCircle * 0.15f;

            GameObject piece = Instantiate(
                debrisPiecePrefab,
                transform.position + offset,
                Quaternion.identity
            );

            Destroy(piece, debrisLifeTime);

            Rigidbody2D rb = piece.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.AddForce(Random.insideUnitCircle.normalized * debrisForce, ForceMode2D.Impulse);
                rb.AddTorque(Random.Range(-debrisTorque, debrisTorque));
            }
        }
    }
}