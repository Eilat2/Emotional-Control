using UnityEngine;

public class LightningVFX : MonoBehaviour
{
    [SerializeField] private LineRenderer line;
    [SerializeField] private float duration = 0.1f;
    [SerializeField] private float length = 1f;

    private float timer;
    private bool isPlaying;

    void Update()
    {
        if (!isPlaying) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            line.enabled = false;
            isPlaying = false;
        }
    }

    public void PlayLightning()
    {
        if (line == null) return;

        // נקודת התחלה = הדמות
        Vector3 start = transform.position;

        // נקודת סוף = כיוון אקראי
        Vector3 end = start + new Vector3(
            Random.Range(-length, length),
            Random.Range(-length, length),
            0f
        );

        line.SetPosition(0, start);
        line.SetPosition(1, end);

        line.enabled = true;

        timer = duration;
        isPlaying = true;
    }
}