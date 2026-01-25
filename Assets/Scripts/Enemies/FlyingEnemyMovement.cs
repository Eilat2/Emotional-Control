using UnityEngine;

// תנועה פשוטה של אויב מעופף: זז ימינה-שמאלה סביב נקודת ההתחלה
public class FlyingEnemyMovement : MonoBehaviour
{
    [SerializeField] float speed = 2f;        // מהירות
    [SerializeField] float moveDistance = 2f; // טווח לכל צד

    private Vector3 startPos;
    private int dir = 1;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.Translate(Vector2.right * dir * speed * Time.deltaTime);

        float dx = transform.position.x - startPos.x;
        if (Mathf.Abs(dx) >= moveDistance)
            dir *= -1;
    }
}
