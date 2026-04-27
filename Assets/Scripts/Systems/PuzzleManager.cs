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
    // אחרי כמה שניות המים ייכבו לבד
    [SerializeField] private float waterDuration = 3f;

    // האש שתכבה רק כשהמים נוגעים בה
    public GameObject fireObject;

    [Header("Door")]
    // האובייקט של הדלת עצמה שיופיע אחרי כיבוי האש
    public GameObject doorObject;

    // סקריפט הדלת שאחראי על פתיחה / ביטול קוליידר
    public DoorController doorController;

    [Header("Camera Sequence")]
    [SerializeField] private CameraFocusSequence cameraSequence;
    [SerializeField] private Transform focusPoint;

    // כדי שלא נפתור את הפאזל כמה פעמים
    private bool puzzleSolved = false;

    // כדי שלא נכבה את האש ונפתח את הדלת כמה פעמים
    private bool fireExtinguished = false;

    private void Start()
    {
        // בתחילת השלב מסתירים את הדלת עצמה
        if (doorObject != null)
        {
            doorObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Door object is not assigned.");
        }

        // גם המים מתחילים כבויים
        if (waterSpray != null)
        {
            waterSpray.SetActive(false);
        }
    }

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
            // שימי לב: כאן כבר לא מכבים את האש ולא פותחים את הדלת.
            // האש תיכבה רק כשהמים יגעו בה.
            if (waterSpray != null)
            {
                waterSpray.SetActive(true);

                // מפעיל מחדש את ה-Particle System כדי שהמים יתחילו מיד
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

                // מכבים את המים אחרי כמה שניות
                StartCoroutine(StopWaterAfterTime());

                Debug.Log("Water spray activated.");
            }
            else
            {
                Debug.LogWarning("Water spray is not assigned.");
            }

            // מזיזים את המצלמה לאזור ואז מחזירים
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

    // מכבה את המים אחרי זמן שהגדרנו ב-Inspector
    private IEnumerator StopWaterAfterTime()
    {
        yield return new WaitForSeconds(waterDuration);

        if (waterSpray != null)
        {
            waterSpray.SetActive(false);
            Debug.Log("Water spray stopped.");
        }
    }

    // הפונקציה הזאת תיקרא מהסקריפט של המים כשהם נוגעים באש
    public void ExtinguishFireAndRevealDoor()
    {
        if (fireExtinguished) return;

        fireExtinguished = true;

        // מכבים את האש
        if (fireObject != null)
        {
            fireObject.SetActive(false);
            Debug.Log("Fire object deactivated by water.");
        }
        else
        {
            Debug.LogWarning("Fire object is not assigned.");
        }

        // חושפים את הדלת אחרי שהאש נכבית
        if (doorObject != null)
        {
            doorObject.SetActive(true);
            Debug.Log("Door object revealed. Active: " + doorObject.activeSelf);
        }
        else
        {
            Debug.LogWarning("Door object is not assigned.");
        }

        // פותחים את הדלת / מבטלים קוליידר
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