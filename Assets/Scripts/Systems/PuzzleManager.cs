using System.Collections;
using UnityEngine;

// מנהל הפאזל של הכפתורים
public class PuzzleManager : MonoBehaviour
{
    // שלושת הכפתורים בפאזל
    public PuzzleButton neutralButton;
    public PuzzleButton joyButton;
    public PuzzleButton rageButton;

    // המים שיופעלו אחרי פתרון נכון
    public GameObject waterSpray;

    [Header("Water Settings")]
    [SerializeField] private float waterDuration = 3f;

    // האש שתכבה רק כשהמים נוגעים בה
    public GameObject fireObject;

    [Header("Wrong Order UI")]
    [SerializeField] private GameObject orderNotCorrectText; // טקסט שיופיע כשסדר הכפתורים לא נכון

    [SerializeField] private float wrongOrderMessageDuration = 3f;
    // כמה זמן הטקסט יופיע לפני Game Over

    [Header("Door")]
    public GameObject doorObject;
    public DoorController doorController;

    [Header("Stone")]
    public GameObject unbreakableStone;
    public GameObject breakableStone;

    [Header("Camera Sequence")]
    [SerializeField] private CameraFocusSequence cameraSequence;
    [SerializeField] private Transform focusPoint;

    private bool puzzleSolved = false;
    private bool fireExtinguished = false;
    private bool puzzleFailed = false;

    // האם הייתה טעות כלשהי במהלך הפאזל
    private bool sequenceHasMistake = false;

    private int currentSequenceIndex = 0;

    // רק אם הפאזל נפתר חוסמים לחיצות
    public bool PuzzleSolved => puzzleSolved;

    private void Start()
    {
        if (orderNotCorrectText != null)
            orderNotCorrectText.SetActive(false);

        if (doorObject != null)
            doorObject.SetActive(false);
        else
            Debug.LogWarning("Door object is not assigned.");

        if (waterSpray != null)
            waterSpray.SetActive(false);

        if (unbreakableStone != null)
            unbreakableStone.SetActive(true);

        if (breakableStone != null)
            breakableStone.SetActive(false);
    }

    public void RegisterButtonPress(PuzzleButton button, EmotionType pressedEmotion, bool pressedCorrectly)
    {
        if (puzzleSolved || puzzleFailed)
            return;

        if (currentSequenceIndex >= 3)
            return;

        PuzzleButton expectedButton = null;

        switch (currentSequenceIndex)
        {
            case 0:
                expectedButton = neutralButton;
                break;

            case 1:
                expectedButton = rageButton;
                break;

            case 2:
                expectedButton = joyButton;
                break;
        }

        if (!pressedCorrectly || button != expectedButton)
        {
            sequenceHasMistake = true;
            Debug.Log("Puzzle mistake recorded.");
        }

        currentSequenceIndex++;

        CheckPuzzleState();
    }

    public void CheckPuzzleState()
    {
        Debug.Log("Checking puzzle state...");

        if (neutralButton == null || joyButton == null || rageButton == null)
        {
            Debug.LogWarning("One or more buttons are not assigned.");
            return;
        }

        Debug.Log("Neutral pressed: " + neutralButton.WasPressed + ", correct: " + neutralButton.PressedCorrectly);
        Debug.Log("Joy pressed: " + joyButton.WasPressed + ", correct: " + joyButton.PressedCorrectly);
        Debug.Log("Rage pressed: " + rageButton.WasPressed + ", correct: " + rageButton.PressedCorrectly);

        bool allPressed =
            neutralButton.WasPressed &&
            joyButton.WasPressed &&
            rageButton.WasPressed;

        if (!allPressed)
        {
            Debug.Log("Not all buttons were pressed yet.");
            return;
        }

        bool allCorrect =
            neutralButton.PressedCorrectly &&
            joyButton.PressedCorrectly &&
            rageButton.PressedCorrectly;

        bool sequenceComplete = currentSequenceIndex == 3;

        // הצלחה
        if (allCorrect && sequenceComplete && !sequenceHasMistake && !puzzleSolved)
        {
            puzzleSolved = true;
            Debug.Log("Puzzle solved!");

            if (waterSpray != null)
            {
                waterSpray.SetActive(true);

                ParticleSystem ps = waterSpray.GetComponentInChildren<ParticleSystem>();

                if (ps != null)
                {
                    ps.Clear();
                    ps.Play();
                    Debug.Log("Particle system started.");
                }
                else
                {
                    Debug.LogWarning("No Particle System found on WaterSpray or its children.");
                }

                StartCoroutine(StopWaterAfterTime());
                Debug.Log("Water spray activated.");
            }
            else
            {
                Debug.LogWarning("Water spray is not assigned.");
            }

            if (cameraSequence != null && focusPoint != null)
                cameraSequence.PlayFocusSequence(focusPoint);
            else
                Debug.LogWarning("Camera sequence or focus point is not assigned.");
        }

        // כישלון
        else if (sequenceComplete)
        {
            Debug.Log("Puzzle failed after all buttons were pressed.");
            StartCoroutine(ShowWrongOrderThenGameOver());
        }
    }

    private IEnumerator ShowWrongOrderThenGameOver()
    {
        if (puzzleFailed)
            yield break;

        puzzleFailed = true;

        if (waterSpray != null)
            waterSpray.SetActive(false);

        if (orderNotCorrectText != null)
            orderNotCorrectText.SetActive(true);

        yield return new WaitForSeconds(wrongOrderMessageDuration);

        PauseMenuInputSystem pauseMenu =
            FindFirstObjectByType<PauseMenuInputSystem>(FindObjectsInactive.Include);

        if (pauseMenu != null)
            pauseMenu.GameOver();
        else
            Debug.LogWarning("PauseMenuInputSystem not found in scene.");
    }

    private IEnumerator StopWaterAfterTime()
    {
        yield return new WaitForSeconds(waterDuration);

        if (waterSpray != null)
        {
            waterSpray.SetActive(false);
            Debug.Log("Water spray stopped.");
        }
    }

    public void ExtinguishFireAndRevealDoor()
    {
        if (fireExtinguished) return;

        fireExtinguished = true;

        if (fireObject != null)
        {
            fireObject.SetActive(false);
            Debug.Log("Fire object deactivated by water.");
        }
        else
        {
            Debug.LogWarning("Fire object is not assigned.");
        }

        if (unbreakableStone != null)
        {
            unbreakableStone.SetActive(false);
            Debug.Log("Unbreakable stone deactivated.");
        }
        else
        {
            Debug.LogWarning("Unbreakable stone is not assigned.");
        }

        if (breakableStone != null)
        {
            breakableStone.SetActive(true);
            Debug.Log("Breakable stone activated.");
        }
        else
        {
            Debug.LogWarning("Breakable stone is not assigned.");
        }

        if (doorObject != null)
        {
            doorObject.SetActive(true);
            Debug.Log("Door object revealed. Active: " + doorObject.activeSelf);
        }
        else
        {
            Debug.LogWarning("Door object is not assigned.");
        }

        if (doorController != null)
        {
            doorController.OpenDoor();
            Debug.Log("Door opened.");
        }
        else
        {
            Debug.LogWarning("Door controller is not assigned.");
        }
    }
}