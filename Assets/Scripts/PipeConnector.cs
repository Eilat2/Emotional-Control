using UnityEngine;
using UnityEngine.InputSystem;

public class PipeConnector : MonoBehaviour
{
    public EmotionType pipeOwner;
    public Transform pipeEnd;
    public Transform socketTarget;

    public bool flowingCorrectly = false;

    [Header("Visual")]
    public SpriteRenderer pipeRenderer;
    public Color correctFlowColor = Color.white;
    public Color wrongFlowColor = Color.gray;

    private bool playerInside = false;
    private EmotionController playerEmotion;
    private LevelPipeManager manager;

    void Start()
    {
        manager = FindFirstObjectByType<LevelPipeManager>();

        if (pipeRenderer != null)
        {
            pipeRenderer.color = wrongFlowColor;
        }
    }

    void Update()
    {
        if (!playerInside || playerEmotion == null)
            return;

        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            TryConnect();
        }
    }

    void TryConnect()
    {
        if (pipeEnd != null && socketTarget != null)
            pipeEnd.position = socketTarget.position;

        EmotionType current = playerEmotion.GetCurrentEmotion();

        if (current == pipeOwner)
        {
            flowingCorrectly = true;
            Debug.Log(pipeOwner + " נכון");

            if (pipeRenderer != null)
                pipeRenderer.color = correctFlowColor;
        }
        else
        {
            flowingCorrectly = false;
            Debug.Log(pipeOwner + " לא נכון");

            if (pipeRenderer != null)
                pipeRenderer.color = wrongFlowColor;
        }

        if (manager != null)
        {
            manager.CheckPuzzleState();
        }
        else
        {
            Debug.Log("Manager is NULL");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;
        playerEmotion = other.GetComponentInParent<EmotionController>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;
        playerEmotion = null;
    }
}