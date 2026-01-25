using UnityEngine;
using UnityEngine.InputSystem;

// תנועה של השחקן במצב Rage
// מאפשר תזוזה ימינה ושמאלה בלבד
// נעצרת זמנית כשיש פגיעה (HurtLock)
public class PlayerRageMove : MonoBehaviour
{
    // מהירות התנועה האופקית של השחקן
    [SerializeField] float moveSpeed = 6f;

    // רפרנס ל־Rigidbody2D של השחקן
    Rigidbody2D rb;

    // קלט תנועה מה־Input System
    Vector2 moveInput;

    // רפרנס לנעילת פגיעה
    PlayerHurtLock hurtLock;

    void Awake()
    {
        // קבלת Rigidbody2D
        rb = GetComponent<Rigidbody2D>();

        // קבלת HurtLock אם קיים
        hurtLock = GetComponent<PlayerHurtLock>();
    }

    void Update()
    {
        // אם השחקן בפגיעה – לא מזיזים אותו
        if (hurtLock != null && hurtLock.IsLocked)
            return;

        // תנועה אופקית
        rb.linearVelocity = new Vector2(
            moveInput.x * moveSpeed,
            rb.linearVelocity.y
        );
    }

    // חייב להיות Action בשם "Move" כדי שזה יעבוד עם Send Messages
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
}
