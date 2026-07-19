using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorNextLevel : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    [SerializeField] private SceneFader sceneFader;
    [SerializeField] private DoorGlowController glow;
    [SerializeField] private Transform portalCenter;
    [SerializeField] private float suckDuration = 0.5f;

    private bool _canLoad = true;

    private void Start()
    {
        // מחפש SceneFader אוטומטית אם לא חובר ידנית
        if (sceneFader == null)
            sceneFader = FindFirstObjectByType<SceneFader>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_canLoad || !other.CompareTag("Player"))
            return;

        _canLoad = false;

        ApplyGlowForCurrentEmotion(other.gameObject);

        StartCoroutine(EnterDoorAndLoad(other.gameObject));
    }

    private void ApplyGlowForCurrentEmotion(GameObject player)
    {
        if (glow == null)
            return;

        EmotionController emotion = player.GetComponent<EmotionController>();
        if (emotion == null)
            return;

        switch (emotion.GetCurrentEmotion())
        {
            case EmotionType.Joy:
                glow.SetJoy();
                break;

            case EmotionType.Rage:
                glow.SetRage();
                break;

            default:
                glow.SetNeutral();
                break;
        }
    }

    private IEnumerator EnterDoorAndLoad(GameObject player)
    {
        PlayerDoorTransition transition = player.GetComponent<PlayerDoorTransition>();

        Vector3 endPos = portalCenter != null ? portalCenter.position : transform.position;
        Vector3 endScale = player.transform.localScale * 0.2f;

        if (transition != null)
        {
            // הדלת לא יודעת איך מקפיאים את השחקן.
            // היא רק מבקשת מהשחקן לבצע אנימציית כניסה לדלת.
            yield return transition.EnterDoor(endPos, endScale, suckDuration);
        }
        else
        {
            Debug.LogWarning("DoorNextLevel: PlayerDoorTransition is missing on Player.");
        }

        if (sceneFader != null)
            sceneFader.FadeToScene(nextSceneName);
        else
            SceneManager.LoadScene(nextSceneName);
    }
}
