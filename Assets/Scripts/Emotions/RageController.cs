using UnityEngine;

public class RageController : MonoBehaviour
{
    public EmotionType currentEmotion = EmotionType.Neutral;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentEmotion = EmotionType.Rage;
            Debug.Log("RAGE ON");
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            currentEmotion = EmotionType.Neutral;
            Debug.Log("RAGE OFF");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // בדיקה: האם אנחנו בזעם?
        if (currentEmotion != EmotionType.Rage)
            return;

        // בדיקה: האם נגעתי באובייקט שביר?
        if (collision.gameObject.CompareTag("Breakable"))
        {
            Debug.Log("BROKE OBJECT");
            Destroy(collision.gameObject);
        }
    }
}

