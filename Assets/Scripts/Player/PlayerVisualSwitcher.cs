using UnityEngine;

public class PlayerVisualSwitcher : MonoBehaviour
{
    [Header("אובייקטים של הוויזואל לכל רגש")]
    [SerializeField] private GameObject neutralVisual;
    [SerializeField] private GameObject joyVisual;
    [SerializeField] private GameObject rageVisual;

    // שומר את הכיוון האחרון של הדמות
    // 1 = ימינה, -1 = שמאלה
    private int facingDirection = 1;

    public void ShowNeutral()
    {
        neutralVisual.SetActive(true);
        joyVisual.SetActive(false);
        rageVisual.SetActive(false);

        ApplyDirection();
    }

    public void ShowJoy()
    {
        neutralVisual.SetActive(false);
        joyVisual.SetActive(true);
        rageVisual.SetActive(false);

        ApplyDirection();
    }

    public void ShowRage()
    {
        neutralVisual.SetActive(false);
        joyVisual.SetActive(false);
        rageVisual.SetActive(true);

        ApplyDirection();
    }

    // נקרא מהתנועה של השחקן ומעדכן לאיזה כיוון הוא מסתכל
    public void SetDirection(float moveX)
    {
        if (moveX > 0.01f)
        {
            facingDirection = 1;
            ApplyDirection();
        }
        else if (moveX < -0.01f)
        {
            facingDirection = -1;
            ApplyDirection();
        }
    }

    // הופך את כל הוויזואל לפי הכיוון
    private void ApplyDirection()
    {
        SetVisualDirection(neutralVisual);
        SetVisualDirection(joyVisual);
        SetVisualDirection(rageVisual);
    }

    private void SetVisualDirection(GameObject visual)
    {
        if (visual == null) return;

        Vector3 scale = visual.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * facingDirection;
        visual.transform.localScale = scale;
    }
}