using UnityEngine;

public class ArmoredEnemyPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] private Transform[] patrolPoints; // מערך של נקודות שהאויב יעבור ביניהן

    [Header("Movement")]
    [SerializeField] private float speed = 2f; // מהירות תנועה
    [SerializeField] private float reachDistance = 0.1f; // מרחק שממנו נחשב שהגענו לנקודה

    private Rigidbody2D rb;
    private int currentPointIndex = 0; // אינדקס הנקודה הנוכחית

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // אם אין נקודות מסלול – לעצור את הסקריפט
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogError("No patrol points assigned!");
            enabled = false;
        }
    }

    void FixedUpdate()
    {
        Transform target = patrolPoints[currentPointIndex]; // היעד הנוכחי

        // חישוב כיוון תנועה (ימינה או שמאלה)
        float direction = Mathf.Sign(target.position.x - transform.position.x);

        // תנועה אופקית בלבד
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

        // להפוך את הדמות לפי כיוון ההליכה
        Flip(direction);

        // בדיקה אם הגענו לנקודה
        if (Vector2.Distance(transform.position, target.position) < reachDistance)
        {
            // מעבר לנקודה הבאה במסלול (חוזר להתחלה בסוף)
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
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