using UnityEngine;

public class FlyingEnemyMovement : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] private Transform[] patrolPoints; // נקודות מסלול (אפשר משולש / ריבוע וכו')

    [Header("Movement")]
    [SerializeField] private float speed = 2f; // מהירות תנועה
    [SerializeField] private float reachDistance = 0.1f; // מרחק הגעה לנקודה

    private int currentPointIndex = 0; // אינדקס הנקודה הנוכחית

    void Update()
    {
        // אם אין נקודות – לא עושים כלום
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Transform target = patrolPoints[currentPointIndex]; // היעד הבא

        // תנועה חלקה לנקודה
        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        // בדיקה אם הגענו לנקודה
        if (Vector2.Distance(transform.position, target.position) < reachDistance)
        {
            // מעבר לנקודה הבאה
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }
}