using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        // מחפש את השחקנית גם אם הקוליידר נמצא על Child
        PlayerEmotionContext player = other.GetComponentInParent<PlayerEmotionContext>();

        if (player == null)
            return;

        triggered = true;

        Debug.Log("Player fell into DeathZone - Game Over");

        // מוריד את כל הסטאמינות ל-0
        Stamina[] staminaSystems = player.GetComponents<Stamina>();

        foreach (Stamina stamina in staminaSystems)
        {
            stamina.ForceDeplete();
        }

        // גיבוי למקרה שהשחקנית בניטרלי ואין סטאמינה פעילה
        player.OnStaminaDepleted();
    }
}