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

    [Header("Door")]
    public GameObject doorObject;
    public DoorController doorController;

    [Header("Stone")]
    public GameObject unbreakableStone;
    public GameObject breakableStone;

    [Header("Camera Sequence")]
    [SerializeField] private CameraFocusSequence cameraSequence;
    [SerializeField] private Transform focusPoint;

    private bool _puzzleSolved;
    private bool _fireExtinguished;

    // רק אם הפאזל נפתר חוסמים לחיצות
    public bool PuzzleSolved => _puzzleSolved;

    private void Start()
    {
        SetActiveOrWarn(doorObject, false, "Door object");
        if (waterSpray != null)
            waterSpray.SetActive(false);

        SetActiveOrWarn(unbreakableStone, true, "Unbreakable stone");
        if (breakableStone != null)
            breakableStone.SetActive(false);
    }

    public void RegisterButtonPress(PuzzleButton button, EmotionType pressedEmotion, bool pressedCorrectly)
    {
        if (_puzzleSolved)
            return;

        // אין בדיקת סדר - כל כפתור בודק רק אם לחצו עליו עם הרגש הנכון שלו.
        StateLogger.Log(nameof(PuzzleManager),
            pressedCorrectly
                ? $"Button pressed correctly: {button.name}"
                : "Button pressed with wrong emotion, but no Game Over.");

        CheckPuzzleState();
    }

    public void CheckPuzzleState()
    {
        if (neutralButton == null || joyButton == null || rageButton == null)
        {
            Debug.LogWarning("PuzzleManager: One or more buttons are not assigned.");
            return;
        }

        bool allPressed =
            neutralButton.WasPressed &&
            joyButton.WasPressed &&
            rageButton.WasPressed;

        if (!allPressed)
            return;

        bool allCorrect =
            neutralButton.PressedCorrectly &&
            joyButton.PressedCorrectly &&
            rageButton.PressedCorrectly;

        // הצלחה רק אם כל הכפתורים נלחצו עם הרגש הנכון שלהם.
        // אם לא - כל הכפתורים נלחצו אבל לפחות אחד לא נכון: פשוט לא פותרים
        // את הפאזל (אין Game Over).
        if (allCorrect && !_puzzleSolved)
            SolvePuzzle();
    }

    private void SolvePuzzle()
    {
        _puzzleSolved = true;
        StateLogger.Log(nameof(PuzzleManager), "Puzzle solved - all buttons pressed correctly.");

        ActivateWaterSpray();

        if (cameraSequence != null && focusPoint != null)
            cameraSequence.PlayFocusSequence(focusPoint);
        else
            Debug.LogWarning("PuzzleManager: Camera sequence or focus point is not assigned.");
    }

    private void ActivateWaterSpray()
    {
        if (waterSpray == null)
        {
            Debug.LogWarning("PuzzleManager: Water spray is not assigned.");
            return;
        }

        waterSpray.SetActive(true);

        ParticleSystem ps = waterSpray.GetComponentInChildren<ParticleSystem>();
        if (ps != null)
        {
            ps.Clear();
            ps.Play();
        }
        else
        {
            Debug.LogWarning("PuzzleManager: No ParticleSystem found on WaterSpray or its children.");
        }

        StartCoroutine(StopWaterAfterTime());
    }

    private IEnumerator StopWaterAfterTime()
    {
        yield return new WaitForSeconds(waterDuration);

        if (waterSpray != null)
            waterSpray.SetActive(false);
    }

    public void ExtinguishFireAndRevealDoor()
    {
        if (_fireExtinguished)
            return;

        _fireExtinguished = true;

        SetActiveOrWarn(fireObject, false, "Fire object");
        SetActiveOrWarn(unbreakableStone, false, "Unbreakable stone");
        SetActiveOrWarn(breakableStone, true, "Breakable stone");
        SetActiveOrWarn(doorObject, true, "Door object");

        if (doorController != null)
            doorController.OpenDoor();
        else
            Debug.LogWarning("PuzzleManager: Door controller is not assigned.");
    }

    /// <summary>
    /// עוטף את הדפוס החוזר: אם ה-GameObject משוייך - להפעיל/לכבות אותו,
    /// אחרת להזהיר בקונסול שהוא לא משוייך ב-Inspector.
    /// </summary>
    private void SetActiveOrWarn(GameObject obj, bool active, string label)
    {
        if (obj == null)
        {
            Debug.LogWarning($"PuzzleManager: {label} is not assigned.");
            return;
        }

        obj.SetActive(active);
    }
}
