using UnityEngine;

public class Level3RevealAfterBreak : MonoBehaviour
{
    [Header("האובייקט שצריך להישבר")]
    [SerializeField] private GameObject objectToWatch;

    [Header("האובייקט שיופיע אחרי השבירה")]
    [SerializeField] private GameObject objectToReveal;

    private bool revealed = false;

    private void Start()
    {
        if (objectToReveal != null)
            objectToReveal.SetActive(false);
    }

    private void Update()
    {
        if (revealed) return;

        if (objectToWatch == null || !objectToWatch.activeInHierarchy)
        {
            revealed = true;

            Debug.Log("Level 3: Revealing puzzle piece");

            if (objectToReveal != null)
            {
                objectToReveal.SetActive(true);

                Level3PuzzlePiecePickup pickup =
                    objectToReveal.GetComponent<Level3PuzzlePiecePickup>();

                if (pickup != null)
                    pickup.UnlockAfterStoneBroken();
            }
        }
    }
}