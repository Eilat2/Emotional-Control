using UnityEngine;

public class DoorGlowController : MonoBehaviour
{
    [Header("Colors")]
    [SerializeField] Color neutralColor = new Color(0.7f, 0.7f, 0.7f);
    [SerializeField] Color joyColor = new Color(1f, 0.85f, 0.3f);
    [SerializeField] Color rageColor = new Color(1f, 0.3f, 0.2f);

    [Header("Pulse")]
    [SerializeField] float pulseSpeed = 2f;
    [SerializeField] float pulseAmount = 0.02f;
    [SerializeField] float alphaPulse = 0.08f;

    SpriteRenderer sr;
    Vector3 startScale;
    Color currentBaseColor;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        startScale = transform.localScale;

        // מתחילים בניטרלי
        SetNeutral();
    }

    void Update()
    {
        // אפקט נשימה (עדין!)
        float t = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = startScale * t;

        // אפקט alpha קטן
        Color c = currentBaseColor;
        c.a = Mathf.Clamp01(currentBaseColor.a + Mathf.Sin(Time.time * pulseSpeed) * alphaPulse);
        sr.color = c;
    }

    // פונקציות לשינוי צבע
    public void SetNeutral()
    {
        currentBaseColor = neutralColor;
        sr.color = neutralColor;
    }

    public void SetJoy()
    {
        currentBaseColor = joyColor;
        sr.color = joyColor;
    }

    public void SetRage()
    {
        currentBaseColor = rageColor;
        sr.color = rageColor;
    }
}