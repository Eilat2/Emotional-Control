using UnityEngine;

// אויב משוריין – הולך ימינה/שמאלה בין שני גבולות X שנקבעים בתחילת המשחק
public class ArmoredEnemyPatrol : MonoBehaviour
{
    [Header("נקודות סיור (אפשר גם להיות ילדים)")]
    public Transform leftPoint;
    public Transform rightPoint;

    [Header("תנועה")]
    public float speed = 2f;

    private Rigidbody2D rb;
    private int dir = 1; // 1 = ימינה, -1 = שמאלה

    // נשמור את הגבולות כערכים קבועים בעולם (כדי שלא "יברחו" אם הנקודות זזות)
    private float leftX;
    private float rightX;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // אם לא חיברו נקודות – אין מה לעשות
        if (!leftPoint || !rightPoint)
        {
            Debug.LogError("ArmoredEnemyPatrol: לא חיברת leftPoint/rightPoint ב-Inspector");
            enabled = false;
            return;
        }

        // קיבוע הגבולות בתחילת המשחק
        leftX = leftPoint.position.x;
        rightX = rightPoint.position.x;

        // לוודא שהשמאלי באמת קטן מהימני
        if (leftX > rightX)
        {
            float tmp = leftX;
            leftX = rightX;
            rightX = tmp;
        }
    }

    void FixedUpdate()
    {
        // תנועה אופקית
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);

        // אם עברנו את הגבול הימני – הופכים שמאלה
        if (dir == 1 && transform.position.x >= rightX)
            Flip();

        // אם עברנו את הגבול השמאלי – הופכים ימינה
        if (dir == -1 && transform.position.x <= leftX)
            Flip();
    }

    private void Flip()
    {
        dir *= -1;

        // להפוך את הספייט שיסתכל לכיוון ההליכה
        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * dir;
        transform.localScale = s;
    }
}
