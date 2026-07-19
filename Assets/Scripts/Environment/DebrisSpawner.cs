using UnityEngine;

// ============================================================
//  DebrisSpawner – לוגיקת "פזר חתיכות שברים עם פיזיקה אקראית"
//  משותפת. מצאנו את אותה לוגיקה כמעט-זהה בשני מקומות שונים:
//  BreakableWall (כאן) ו-KillableEnemy (בתיקיית Enemies).
//
//  לא MonoBehaviour בכוונה - זה רק helper סטטי, אין לו state
//  או Inspector משלו.
// ============================================================
public static class DebrisSpawner
{
    public static void SpawnRandomDebris(
        GameObject debrisPrefab,
        Vector3 originPosition,
        int count,
        float force,
        float torque,
        float positionJitter = 0f,
        float lifeTime = 0f,
        ForceMode2D torqueMode = ForceMode2D.Force)
    {
        if (debrisPrefab == null)
            return;

        for (int i = 0; i < count; i++)
        {
            Vector3 offset = positionJitter > 0f
                ? (Vector3)(Random.insideUnitCircle * positionJitter)
                : Vector3.zero;

            GameObject piece = Object.Instantiate(debrisPrefab, originPosition + offset, Quaternion.identity);

            if (lifeTime > 0f)
                Object.Destroy(piece, lifeTime);

            Rigidbody2D rb = piece.GetComponent<Rigidbody2D>();
            if (rb == null)
                continue;

            rb.AddForce(Random.insideUnitCircle.normalized * force, ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(-torque, torque), torqueMode);
        }
    }
}
