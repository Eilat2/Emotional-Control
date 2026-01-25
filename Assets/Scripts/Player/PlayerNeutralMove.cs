using UnityEngine;
using UnityEngine.InputSystem; // מערכת הקלט החדשה של Unity

// סקריפט תנועה בסיסית לשחקן במצב ניטרלי
// מאפשר תזוזה ימינה ושמאלה בלבד
public class PlayerNeutralMove : MonoBehaviour
{
    // מהירות התנועה האופקית של השחקן
    [SerializeField] float moveSpeed = 6f;

    // רפרנס ל־Rigidbody2D של השחקן
    private Rigidbody2D rb;

    // קלט תנועה שמגיע מה־Input System (ציר X = שמאלה/ימינה)
    private Vector2 moveInput;

    // רפרנס ל־HurtLock (נועל תנועה בזמן פגיעה)
    private PlayerHurtLock hurtLock;

    // נקרא פעם אחת כשהאובייקט נטען
    void Awake()
    {
        // קבלת ה־Rigidbody2D שמחובר לשחקן
        rb = GetComponent<Rigidbody2D>();

        // קבלת PlayerHurtLock אם קיים
        hurtLock = GetComponent<PlayerHurtLock>();
    }

    // נקרא כל פריים
    void Update()
    {
        // אם השחקן כרגע בפגיעה – לא מזיזים אותו
        if (hurtLock != null && hurtLock.IsLocked)
            return;

        // מזיז את השחקן ימינה/שמאלה לפי הקלט
        // שומר על המהירות האנכית (קפיצה/נפילה) ללא שינוי
        rb.linearVelocity = new Vector2(
            moveInput.x * moveSpeed,
            rb.linearVelocity.y
        );
    }

    // פונקציה שנקראת אוטומטית ע"י Input System
    // חייבת להיקרא OnMove כדי להתאים ל־Action בשם "Move"
    public void OnMove(InputValue value)
    {
        // קריאת ערך הקלט (Vector2)
        // x = תנועה שמאלה/ימינה
        // y לא בשימוש כאן
        moveInput = value.Get<Vector2>();
    }
}
