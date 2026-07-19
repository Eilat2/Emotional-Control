using UnityEngine;

public class DoorGlowController : MonoBehaviour
{
    [Header("Colors")]
    [SerializeField] private Color neutralColor = new Color(0.7f, 0.7f, 0.7f);
    [SerializeField] private Color joyColor = new Color(1f, 0.85f, 0.3f);
    [SerializeField] private Color rageColor = new Color(1f, 0.3f, 0.2f);

    [Header("Pulse")]
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseAmount = 0.02f;
    [SerializeField] private float alphaPulse = 0.08f;

    private SpriteRenderer _sr;
    private Vector3 _startScale;
    private Color _currentBaseColor;

    private void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        _startScale = transform.localScale;

        // מתחילים בניטרלי
        SetNeutral();
    }

    private void Update()
    {
        // אפקט נשימה (עדין!)
        float t = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = _startScale * t;

        // אפקט alpha קטן
        Color c = _currentBaseColor;
        c.a = Mathf.Clamp01(_currentBaseColor.a + Mathf.Sin(Time.time * pulseSpeed) * alphaPulse);
        _sr.color = c;
    }

    public void SetNeutral() => SetBaseColor(neutralColor);
    public void SetJoy() => SetBaseColor(joyColor);
    public void SetRage() => SetBaseColor(rageColor);

    private void SetBaseColor(Color color)
    {
        _currentBaseColor = color;
        _sr.color = color;
    }
}