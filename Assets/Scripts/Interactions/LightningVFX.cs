using UnityEngine;

public class LightningVFX : MonoBehaviour
{
    [SerializeField] private LineRenderer line;
    [SerializeField] private float duration = 0.1f;
    [SerializeField] private float length = 1f;

    private float _timer;
    private bool _isPlaying;

    private void Update()
    {
        if (!_isPlaying)
            return;

        _timer -= Time.deltaTime;

        if (_timer <= 0f)
        {
            line.enabled = false;
            _isPlaying = false;
        }
    }

    public void PlayLightning()
    {
        if (line == null)
            return;

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

        _timer = duration;
        _isPlaying = true;
    }
}
