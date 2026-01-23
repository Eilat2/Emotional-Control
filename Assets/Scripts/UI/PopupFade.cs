using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;       // ✅ New Input System
using UnityEngine.SceneManagement;   // ✅ כדי לבדוק באיזו סצנה אנחנו

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
    // ⚠️ חייב להיות בדיוק שם הסצנה שלך
    public string tutorialSceneName = "Level1_Tutorial";

    // מפתח שנשמר במחשב כדי לזכור שהפופ-אפ כבר הוצג (ספציפית לטוטוריאל)
    private const string POPUP_SHOWN_KEY = "TutorialPopupShown_Level1";

    // CanvasGroup מאפשר לשלוט על שקיפות (alpha)
    private CanvasGroup cg;

    // דגל שמונע הרצה כפולה של Fade Out
    private bool hiding;

    void Awake()
    {
        // מנסה לקבל CanvasGroup קיים
        cg = GetComponent<CanvasGroup>();

        // אם אין — מוסיף אחד
        if (cg == null)
            cg = gameObject.AddComponent<CanvasGroup>();

        // הפופ-אפ הוא ויזואלי בלבד — לא קולט קלט
        cg.interactable = false;
        cg.blocksRaycasts = false;

        // מתחיל במצב שקוף (לפני Fade In)
        cg.alpha = 0f;
    }

    void OnEnable()
    {
        // ✅ אם אנחנו לא בסצנת הטוטוריאל — לא מציגים בכלל
        if (SceneManager.GetActiveScene().name != tutorialSceneName)
        {
            gameObject.SetActive(false);
            return;
        }

        // אם ביקשנו "רק פעם אחת" וכבר הראינו בעבר — מכבים ולא מציגים שוב
        if (showOnlyOnce && PlayerPrefs.GetInt(POPUP_SHOWN_KEY, 0) == 1)
        {
            gameObject.SetActive(false);
            return;
        }

        // אם זה אמור להיות "רק פעם אחת" — מסמנים עכשיו שכבר הוצג
        if (showOnlyOnce)
        {
            PlayerPrefs.SetInt(POPUP_SHOWN_KEY, 1);
            PlayerPrefs.Save();
        }

        // עוצרים קורוטינות ישנות ומריצים פופ-אפ מחדש
        StopAllCoroutines();
        StartCoroutine(RunPopup());
    }

    void Update()
    {
        // אם לא רוצים להעלים בלחיצה או שכבר נעלמים — יוצאים
        if (!hideOn123Press || hiding)
            return;

        // ✅ New Input System: בדיקת מקשים 1/2/3
        if (Keyboard.current != null &&
            (Keyboard.current.digit1Key.wasPressedThisFrame ||
             Keyboard.current.digit2Key.wasPressedThisFrame ||
             Keyboard.current.digit3Key.wasPressedThisFrame))
        {
            // עוצרים הכול ומתחילים Fade Out
            StopAllCoroutines();
            StartCoroutine(Hide());
        }
    }

    /// <summary>
    /// רצף מלא של הפופ-אפ:
    /// Fade In → המתנה → Fade Out
    /// </summary>
    IEnumerator RunPopup()
    {
        // מאפסים דגל כדי לאפשר העלמה שוב
        hiding = false;

        // Fade In
        yield return Fade(0f, 1f, fadeDuration);

        // נשאר על המסך בלי קשר ל-TimeScale
        yield return new WaitForSecondsRealtime(showDuration);

        // Fade Out + כיבוי האובייקט
        yield return Hide();
    }

    /// <summary>
    /// מעלים את הפופ-אפ בצורה חלקה
    /// </summary>
    IEnumerator Hide()
    {
        // מונעים הרצה כפולה
        if (hiding) yield break;
        hiding = true;

        // Fade Out
        yield return Fade(cg.alpha, 0f, fadeDuration);

        // מכבים את האובייקט לגמרי
        gameObject.SetActive(false);
    }

    /// <summary>
    /// מבצע Fade בין ערכי alpha
    /// </summary>
    IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;

        while (t < duration)
        {
            // משתמשים ב-Unscaled כדי שלא יושפע מ-Pause
            t += Time.unscaledDeltaTime;

            // מעבר חלק בין הערכים
            cg.alpha = Mathf.Lerp(from, to, t / duration);

            yield return null;
        }

        // מוודאים ערך סופי מדויק
        cg.alpha = to;
    }

    // ---------------------------
    // עזר לפיתוח (אופציונלי):
    // כדי לבדוק שוב ושוב בטוטוריאל, אפשר לקרוא לזה פעם אחת ואז למחוק.
    // ---------------------------
    public static void ResetPopupShown()
    {
        PlayerPrefs.DeleteKey(POPUP_SHOWN_KEY);
        PlayerPrefs.Save();
    }
}
