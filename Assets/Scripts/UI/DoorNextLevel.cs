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

    private bool canLoad = true;

    private void Start()
    {
        // מחפש SceneFader אוטומטית אם לא חובר ידנית
        if (sceneFader == null)
        {
            sceneFader = FindFirstObjectByType<SceneFader>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canLoad) return;
        if (!other.CompareTag("Player")) return;

        canLoad = false;

        EmotionController emotion = other.GetComponent<EmotionController>();

        if (emotion != null && glow != null)
        {
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

        StartCoroutine(EnterDoorAndLoad(other.gameObject));
    }

    private IEnumerator EnterDoorAndLoad(GameObject player)
    {
        PlayerDoorTransition transition =
            player.GetComponent<PlayerDoorTransition>();

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
            Debug.LogWarning("PlayerDoorTransition is missing on Player.");
        }

        // מעבר סצנה
        if (sceneFader != null)
        {
            sceneFader.FadeToScene(nextSceneName);
        }
        else
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}