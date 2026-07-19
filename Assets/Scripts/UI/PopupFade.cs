using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// סקריפט לפופ-אפ טוטוריאל:
/// מופיע עם Fade In, נשאר כמה שניות,
/// ונעלם עם Fade Out או בלחיצה על 1/2/3.
/// מופיע רק בסצנת הטוטוריאל.
/// בנוסף: אפשר להציג פעם אחת בלבד (PlayerPrefs).
/// </summary>
public class PopupFade : MonoBehaviour
{
    [Header("Timings")]
    // כמה זמן הפופ-אפ נשאר גלוי על המסך
    public float showDuration = 2.5f;

    // כמה זמן לוקח אפקט ה-Fade In / Fade Out
    public float fadeDuration = 0.25f;

    // האם להעלים את הפופ-אפ גם בלחיצה על 1/2/3
    public bool hideOn123Press = true;

    [Header("Show once")]
    // האם להציג את הפופ-אפ רק פעם אחת (רק בטוטוריאל)
    public bool showOnlyOnce = true;

    [Header("Tutorial scene")]
    // חייב להיות בדיוק שם הסצנה שלך
    public string tutorialSceneName = "Level1_Tutorial";

    // מפתח שנשמר במחשב כדי לזכור שהפופ-אפ כבר הוצג (ספציפית לטוטוריאל)
    private const string PopupShownKey = "TutorialPopupShown_Level1";

    // CanvasGroup מאפשר לשלוט על שקיפות (alpha)
    private CanvasGroup _cg;

    // דגל שמונע הרצה כפולה של Fade Out
    private bool _hiding;

    private void Awake()
    {
        _cg = GetComponent<CanvasGroup>();

        if (_cg == null)
            _cg = gameObject.AddComponent<CanvasGroup>();

        // הפופ-אפ הוא ויזואלי בלבד - לא קולט קלט
        _cg.interactable = false;
        _cg.blocksRaycasts = false;

        // מתחיל במצב שקוף (לפני Fade In)
        _cg.alpha = 0f;
    }

    private void OnEnable()
    {
        // אם אנחנו לא בסצנת הטוטוריאל - לא מציגים בכלל
        if (SceneManager.GetActiveScene().name != tutorialSceneName)
        {
            gameObject.SetActive(false);
            return;
        }

        // אם ביקשנו "רק פעם אחת" וכבר הראינו בעבר - מכבים ולא מציגים שוב
        if (showOnlyOnce && PlayerPrefs.GetInt(PopupShownKey, 0) == 1)
        {
            gameObject.SetActive(false);
            return;
        }

        if (showOnlyOnce)
        {
            PlayerPrefs.SetInt(PopupShownKey, 1);
            PlayerPrefs.Save();
        }

        StopAllCoroutines();
        StartCoroutine(RunPopup());
    }

    private void Update()
    {
        if (!hideOn123Press || _hiding)
            return;

        // New Input System: בדיקת מקשים 1/2/3
        if (Keyboard.current != null &&
            (Keyboard.current.digit1Key.wasPressedThisFrame ||
             Keyboard.current.digit2Key.wasPressedThisFrame ||
             Keyboard.current.digit3Key.wasPressedThisFrame))
        {
            StopAllCoroutines();
            StartCoroutine(Hide());
        }
    }

    /// <summary>
    /// רצף מלא של הפופ-אפ: Fade In -> המתנה -> Fade Out
    /// </summary>
    private IEnumerator RunPopup()
    {
        _hiding = false;

        yield return Fade(0f, 1f, fadeDuration);

        // נשאר על המסך בלי קשר ל-TimeScale
        yield return new WaitForSecondsRealtime(showDuration);

        yield return Hide();
    }

    private IEnumerator Hide()
    {
        if (_hiding)
            yield break;

        _hiding = true;

        yield return Fade(_cg.alpha, 0f, fadeDuration);

        gameObject.SetActive(false);
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;

        while (t < duration)
        {
            // Unscaled כדי שלא יושפע מ-Pause
            t += Time.unscaledDeltaTime;
            _cg.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }

        _cg.alpha = to;
    }

    // עזר לפיתוח (אופציונלי):
    // כדי לבדוק שוב ושוב בטוטוריאל, אפשר לקרוא לזה פעם אחת ואז למחוק.
    public static void ResetPopupShown()
    {
        PlayerPrefs.DeleteKey(PopupShownKey);
        PlayerPrefs.Save();
    }
}
