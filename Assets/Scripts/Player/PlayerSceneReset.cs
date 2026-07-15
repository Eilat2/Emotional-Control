using UnityEngine;
using UnityEngine.SceneManagement;

// ============================================================
//  PlayerSceneReset – מאפס את מצב השחקן בכל טעינת סצנה
//
//  זהו כרגע המנגנון היחיד שאחראי על:
//    - מיקום השחקן ב-spawn point
//    - איפוס פיזיקה / ויזואל / קוליידרים
//    - איפוס מערכת הרגשות וה-State Machine
//    - איפוס Stamina
//
//  (PlayerSceneHandler.cs ו-PlayerSpawnSetter.cs הוסרו –
//   הם היו מנגנונים מקבילים/ישנים שכבר לא היו מחוברים
//   בפועל בפרויקט, וגרמו לכפילות מבלבלת.)
// ============================================================
public class PlayerSceneReset : MonoBehaviour
{
    [Header("Reset Settings")]
    [SerializeField] private float normalGravityScale = 4f;

    private Vector3 _originalScale;

    private void Awake()
    {
        _originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetPosition();
        ResetTransformScale();
        ResetSpriteVisibility();
        ResetColliders();
        ResetPhysics();
        ResetEmotionState();
        ResetStamina();
        ReEnableAllScripts();
        ResetStateMachine();
    }

    private void ResetPosition()
    {
        GameObject respawn = GameObject.Find("PlayerSpawnPoint");
        if (respawn != null)
            transform.position = respawn.transform.position;
        else
            Debug.LogWarning("PlayerSpawnPoint not found in scene.");
    }

    private void ResetTransformScale()
    {
        transform.localScale = _originalScale;
    }

    private void ResetSpriteVisibility()
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);
        foreach (SpriteRenderer sr in renderers)
        {
            Color c = sr.color;
            c.a = 1f;
            sr.color = c;
        }
    }

    private void ResetColliders()
    {
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>(true);
        foreach (Collider2D col in colliders)
            col.enabled = true;
    }

    private void ResetPhysics()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            return;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = normalGravityScale;
        rb.simulated = true;
    }

    private void ResetEmotionState()
    {
        PlayerEmotionContext context = GetComponent<PlayerEmotionContext>();
        if (context != null)
            context.ResetToNeutral();

        EmotionController emotion = GetComponent<EmotionController>();
        if (emotion != null)
        {
            emotion.current = EmotionController.Emotion.Neutral;
            GameEvents.RaiseEmotionChanged(EmotionController.Emotion.Neutral);
        }
    }

    private void ResetStamina()
    {
        Stamina[] staminaComponents = GetComponentsInChildren<Stamina>(true);
        foreach (Stamina stamina in staminaComponents)
            stamina.ResetForNewScene();
    }

    private void ReEnableAllScripts()
    {
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
            script.enabled = true;
    }

    private void ResetStateMachine()
    {
        PlayerStateMachine sm = GetComponent<PlayerStateMachine>();
        if (sm != null)
            sm.ResetMachine();
    }
}
