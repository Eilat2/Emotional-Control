using UnityEngine;

public class PlayerVisualSwitcher : MonoBehaviour
{
    [Header("אובייקטים של הוויזואל לכל רגש")]
    [SerializeField] private GameObject neutralVisual;
    [SerializeField] private GameObject joyVisual;
    [SerializeField] private GameObject rageVisual;

    private int facingDirection = 1;

    private void OnEnable()
    {
        GameEvents.OnEmotionChanged += HandleEmotionChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnEmotionChanged -= HandleEmotionChanged;
    }

    private void HandleEmotionChanged(EmotionController.Emotion emotion)
    {
        if (emotion == EmotionController.Emotion.Joy)
            ShowJoy();
        else if (emotion == EmotionController.Emotion.Rage)
            ShowRage();
        else
            ShowNeutral();
    }

    public void ShowNeutral()
    {
        if (neutralVisual) neutralVisual.SetActive(true);
        if (joyVisual) joyVisual.SetActive(false);
        if (rageVisual) rageVisual.SetActive(false);

        ApplyDirection();
    }

    public void ShowJoy()
    {
        if (neutralVisual) neutralVisual.SetActive(false);
        if (joyVisual) joyVisual.SetActive(true);
        if (rageVisual) rageVisual.SetActive(false);

        ApplyDirection();
    }

    public void ShowRage()
    {
        if (neutralVisual) neutralVisual.SetActive(false);
        if (joyVisual) joyVisual.SetActive(false);
        if (rageVisual) rageVisual.SetActive(true);

        ApplyDirection();
    }

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