using UnityEngine;

// חיישן שבודק אם השחקן נמצא ליד משהו שאפשר לשבור
// למשל: אבן שבירה או אויב שניתן להרוג בזעם
public class BreakableSensor : MonoBehaviour
{
    // המטרה השבירה הנוכחית שהשחקן עומד לידה
    public IBreakable current { get; private set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // מחפשים על האובייקט שנכנס לטריגר או על ההורה שלו
        // רכיב שמממש את IBreakable
        IBreakable breakable = other.GetComponentInParent<IBreakable>();

        // אם זה לא משהו שביר - מתעלמים
        if (breakable == null)
            return;

        // שומרים את המטרה השבירה הנוכחית
        current = breakable;

        Debug.Log("Breakable target detected: " + other.name);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // גם בזמן שהשחקן עדיין בתוך הטריגר,
        // נמשיך לעדכן את המטרה השבירה.
        // זה עוזר אם השחקן כבר עומד ליד האבן ואז מחליף לזעם.
        IBreakable breakable = other.GetComponentInParent<IBreakable>();

        if (breakable == null)
            return;

        current = breakable;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // בודקים אם האובייקט שיצא הוא משהו שביר
        IBreakable breakable = other.GetComponentInParent<IBreakable>();

        if (breakable == null)
            return;

        // מנקים את המטרה רק אם זו באמת המטרה ששמורה כרגע
        if (current == breakable)
        {
            current = null;
            Debug.Log("Breakable target lost: " + other.name);
        }
    }
}