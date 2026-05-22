using UnityEngine;

// חיישן שבודק אם השחקן נמצא ליד משהו שאפשר לשבור
public class BreakableSensor : MonoBehaviour
{
    // המטרה השבירה הנוכחית
    public IBreakable current { get; private set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IBreakable breakable = other.GetComponentInParent<IBreakable>();

        if (breakable == null)
            return;

        current = breakable;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        IBreakable breakable = other.GetComponentInParent<IBreakable>();

        if (breakable == null)
            return;

        current = breakable;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        IBreakable breakable = other.GetComponentInParent<IBreakable>();

        if (breakable == null)
            return;

        if (current == breakable)
        {
            current = null;
        }
    }
}