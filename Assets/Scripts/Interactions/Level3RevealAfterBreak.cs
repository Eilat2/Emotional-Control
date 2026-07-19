using UnityEngine;

public class Level3RevealAfterBreak : MonoBehaviour
{
    [Header("האובייקט שצריך להישבר")]
    [SerializeField] private GameObject objectToWatch;

    [Header("האובייקט שיופיע אחרי השבירה")]
    [SerializeField] private GameObject objectToReveal;

    private bool _revealed;

    private void Start()
    {
        if (objectToReveal != null)
            objectToReveal.SetActive(false);
    }

    private void Update()
    {
        if (_revealed)
            return;

        if (objectToWatch != null && objectToWatch.activeInHierarchy)
            return;

        Reveal();
    }

    private void Reveal()
    {
        _revealed = true;
        StateLogger.Log(nameof(Level3RevealAfterBreak), "Revealing puzzle piece.");

        if (objectToReveal == null)
            return;

        objectToReveal.SetActive(true);

        Level3PuzzlePiecePickup pickup = objectToReveal.GetComponent<Level3PuzzlePiecePickup>();
        pickup?.UnlockAfterStoneBroken();
    }
}
