using UnityEngine;

public class ArmoredEnemyPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] private Transform[] patrolPoints;
    // מערך של נקודות שהאויב ילך ביניהן

    [Header("Movement")]
    [SerializeField] private float speed = 2f;
    // מהירות התנועה של האויב

    [SerializeField] private float reachDistance = 0.2f;
    // מרחק שממנו נחשב שהאויב "הגיע" לנקודה (לא חייב להיות בדיוק עליה)

    private Rigidbody2D rb;
    private int currentPointIndex = 0;
    // אינדקס הנקודה הנוכחית במערך

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // אם אין נקודות – לעצור את הסקריפט ולתת שגיאה
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogError("No patrol points assigned!");
            enabled = false;
        }
    }

    void FixedUpdate()
    {
        // היעד הנוכחי
        Transform target = patrolPoints[currentPointIndex];

        // מחשבים מרחק רק בציר X (כי התנועה אופקית)
        float distanceX = Mathf.Abs(target.position.x - transform.position.x);

        // אם הגענו מספיק קרוב לנקודה
        if (distanceX <= reachDistance)
        {
            // עוברים לנקודה הבאה (וחוזרים להתחלה בסוף)
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;

            // מעדכנים יעד חדש אחרי ההחלפה
            target = patrolPoints[currentPointIndex];
        }

        // מחשבים כיוון תנועה (ימינה או שמאלה)
        float direction = Mathf.Sign(target.position.x - transform.position.x);

        // מזיזים את האויב רק בציר X
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

        // הופכים את הדמות רק אם יש כיוון (לא 0)
        if (direction != 0)
        {
            Flip(direction);
        }
    }

    private void Flip(float direction)
    {
        // הופך את הספייט לפי כיוון התנועה
        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * direction;
        transform.localScale = s;
    }
}