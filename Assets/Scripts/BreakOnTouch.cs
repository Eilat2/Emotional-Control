using UnityEngine;

public class BreakOnTouch : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Breakable"))
        {
            Debug.Log("Broke breakable!");
            Destroy(collision.gameObject);
        }
    }
}
