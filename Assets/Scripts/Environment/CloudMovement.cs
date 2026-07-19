using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 0.5f;   // מהירות התנועה
    [SerializeField] private float distance = 2f;  // כמה רחוק לזוז

    private Vector3 _startPosition;

    private void Start()
    {
        _startPosition = transform.position;
    }

    private void Update()
    {
        float offset = Mathf.Sin(Time.time * speed) * distance;
        transform.position = _startPosition + new Vector3(offset, 0f, 0f);
    }
}
