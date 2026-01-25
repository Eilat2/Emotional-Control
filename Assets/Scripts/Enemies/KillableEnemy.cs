using UnityEngine;

// אויב שניתן להרוג ע"י Rage
public class KillableEnemy : MonoBehaviour, IBreakable
{
    // פונקציה שנקראת כשהשחקן פוגע בו
    public void OnBreak()
    {
        Die();
    }

    // מוות של האויב
    void Die()
    {
        // פה בעתיד:
        // אנימציה / סאונד / פוף / ניקוד
        Destroy(gameObject);
    }
}
