using UnityEngine;

// מנהל הפאזל של הכפתורים
public class LevelPipeManager : MonoBehaviour
{
    // שלושת הכפתורים בפאזל
    public PuzzleButton neutralButton;
    public PuzzleButton joyButton;
    public PuzzleButton rageButton;

    // המים שיופעלו אחרי פתרון נכון
    public GameObject waterSpray;

    // האש שתכבה אחרי פתרון נכון
    public GameObject fireObject;

    // הדלת לשלב הבא
    public DoorController doorController;

    // סקריפט המצלמה שאחראי על מעבר המצלמה לנקודת פוקוס וחזרה
    [Header("Camera Sequence")]
    [SerializeField] private CameraFocusSequence cameraSequence;

    // נקודה באזור המים/אש שאליה המצלמה תזוז
    [SerializeField] private Transform focusPoint;

    // כדי שלא נפתור את הפאזל כמה פעמים
    private bool puzzleSolved = false;

    // בודק את מצב הפאזל
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

        if (allCorrect && !puzzleSolved)
        {
            puzzleSolved = true;
            Debug.Log("Puzzle solved!");

            // מפעילים את המים
            if (waterSpray != null)
            {
                waterSpray.SetActive(true);
                Debug.Log("Water spray activated.");
            }
            else
            {
                Debug.LogWarning("Water spray is not assigned.");
            }

            // מכבים את האש
            if (fireObject != null)
            {
                fireObject.SetActive(false);
                Debug.Log("Fire object deactivated.");
            }
            else
            {
                Debug.LogWarning("Fire object is not assigned.");
            }

            // פותחים את הדלת
            if (doorController != null)
            {
                doorController.OpenDoor();
                Debug.Log("Door opened.");
            }
            else
            {
                Debug.LogWarning("Door controller is not assigned.");
            }

            // מזיזים את המצלמה לאזור המים/אש ואז מחזירים אותה לשחקן
            if (cameraSequence != null && focusPoint != null)
            {
                cameraSequence.PlayFocusSequence(focusPoint);
            }
            else
            {
                Debug.LogWarning("Camera sequence or focus point is not assigned.");
            }
        }
        else if (!allCorrect)
        {
            Debug.Log("Puzzle failed. Not all buttons were pressed with the correct emotion.");
        }
    }
}