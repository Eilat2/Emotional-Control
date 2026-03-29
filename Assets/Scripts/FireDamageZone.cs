using UnityEngine;

public class FireDamageZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("🔥 נשרפת!");
    }
}