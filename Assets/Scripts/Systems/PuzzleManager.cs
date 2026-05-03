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

    private bool puzzleSolved = false;
    private bool fireExtinguished = false;

    private void Start()
    {
        if (doorObject != null)
        {
            doorObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Door object is not assigned.");
        }

        if (waterSpray != null)
        {
            waterSpray.SetActive(false);
        }

        // בתחילת השלב האבן הלא שבירה פעילה, והשבירה כבויה
        if (unbreakableStone != null)
        {
            unbreakableStone.SetActive(true);
        }

        if (breakableStone != null)
        {
            breakableStone.SetActive(false);
        }
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

        if (allCorrect && !puzzleSolved)
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

        // מחליפים מאבן לא שבירה לאבן שבירה
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