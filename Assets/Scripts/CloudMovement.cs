using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 0.5f;   // מהירות התנועה
    [SerializeField] private float distance = 2f;  // כמה רחוק לזוז

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * speed) * distance;
        transform.position = startPos + new Vector3(offset, 0f, 0f);
    }
}