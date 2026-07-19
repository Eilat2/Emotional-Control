using UnityEngine;

// חיישן שבודק אם השחקן נמצא ליד משהו שאפשר לשבור
public class BreakableSensor : MonoBehaviour
{
    // המטרה השבירה הנוכחית
    public IBreakable current { get; private set; }

    private void OnTriggerEnter2D(Collider2D other) => TrackAsCurrent(other);
    private void OnTriggerStay2D(Collider2D other) => TrackAsCurrent(other);

    private void OnTriggerExit2D(Collider2D other)
    {
        IBreakable breakable = other.GetComponentInParent<IBreakable>();

        if (breakable != null && current == breakable)
            current = null;
    }

    private void TrackAsCurrent(Collider2D other)
    {
        IBreakable breakable = other.GetComponentInParent<IBreakable>();

        if (breakable != null)
            current = breakable;
    }
}
