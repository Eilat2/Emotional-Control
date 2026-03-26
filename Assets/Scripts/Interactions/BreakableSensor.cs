using UnityEngine;

public class BreakableSensor : MonoBehaviour
{
    public IBreakable current { get; private set; }

    void OnTriggerEnter2D(Collider2D other)
    {
        var b = other.GetComponentInParent<IBreakable>();
        if (b != null) current = b;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var b = other.GetComponentInParent<IBreakable>();
        if (b != null && current == b) current = null;
    }
}
