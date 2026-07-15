using UnityEngine;

public class PlayerVisualSwitcher : MonoBehaviour
{
    [Header("אובייקטים של הוויזואל לכל רגש")]
    [SerializeField] private GameObject neutralVisual;
    [SerializeField] private GameObject joyVisual;
    [SerializeField] private GameObject rageVisual;

    private int _facingDirection = 1;

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
        SetActiveVisual(neutralVisual);
    }

    public void ShowJoy()
    {
        SetActiveVisual(joyVisual);
    }

    public void ShowRage()
    {
        SetActiveVisual(rageVisual);
    }

    private void SetActiveVisual(GameObject active)
    {
        if (neutralVisual) neutralVisual.SetActive(neutralVisual == active);
        if (joyVisual) joyVisual.SetActive(joyVisual == active);
        if (rageVisual) rageVisual.SetActive(rageVisual == active);

        ApplyDirection();
    }

    public void SetDirection(float moveX)
    {
        if (moveX > 0.01f)
        {
            _facingDirection = 1;
            ApplyDirection();
        }
        else if (moveX < -0.01f)
        {
            _facingDirection = -1;
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
        scale.x = Mathf.Abs(scale.x) * _facingDirection;
        visual.transform.localScale = scale;
    }
}
