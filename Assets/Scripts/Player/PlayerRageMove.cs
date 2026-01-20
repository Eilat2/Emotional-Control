using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRageMove : MonoBehaviour
{
    [SerializeField] float moveSpeed = 6f;

    Rigidbody2D rb;
    Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    // חייב להיות Action בשם "Move" כדי שזה יעבוד עם Send Messages
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
}
