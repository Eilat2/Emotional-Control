using UnityEngine;
using UnityEngine.InputSystem;

// כפתור פאזל שאפשר ללחוץ עליו עם כל רגש,
// אבל רק רגש מסוים נחשב ללחיצה נכונה
public class PuzzleButton : MonoBehaviour
{
    // הרגש שצריך ללחוץ עם הכפתור הזה
    public EmotionType requiredEmotion;

    // רפרנס למנהל הפאזל
    public LevelPipeManager levelManager;

    // האם השחקן נמצא ליד הכפתור
    private bool playerInside = false;

    // רפרנס לסקריפט הרגשות של השחקן
    private EmotionController playerEmotion;

    // האם כבר לחצו על הכפתור
    private bool wasPressed = false;

    // האם הלחיצה הייתה נכונה
    private bool pressedCorrectly = false;

    // מאפשר למנהל לקרוא את המידע הזה
    public bool WasPressed => wasPressed;
    public bool PressedCorrectly => pressedCorrectly;

    void Update()
    {
        // אם השחקן לא ליד הכפתור, לא עושים כלום
        if (!playerInside || playerEmotion == null)
            return;

        // לחיצה על F
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            PressButton();
        }
    }

    void PressButton()
    {
        EmotionType currentEmotion = playerEmotion.GetCurrentEmotion();

        wasPressed = true;

        if (currentEmotion == requiredEmotion)
        {
            pressedCorrectly = true;
            Debug.Log("Correct button press.");
        }
        else
        {
            pressedCorrectly = false;
            Debug.Log("Wrong button press.");
        }

        // מודיע למנהל לבדוק את מצב הפאזל
        if (levelManager != null)
        {
            levelManager.CheckPuzzleState();
        }
        else
        {
            Debug.LogWarning("Level manager is not assigned.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;
        playerEmotion = other.GetComponentInParent<EmotionController>();

        Debug.Log("Player entered button area.");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;
        playerEmotion = null;

        Debug.Log("Player left button area.");
    }
}