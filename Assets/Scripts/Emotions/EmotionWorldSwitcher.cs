using UnityEngine;

public class EmotionWorldSwitcher : MonoBehaviour
{
    [Header("Joy Objects")]
    [SerializeField] private GameObject[] joyObjects;

    [Header("Rage Objects")]
    [SerializeField] private GameObject[] rageObjects;

    [Header("Neutral Objects")]
    [SerializeField] private GameObject[] neutralObjects;

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
            ShowJoyWorld();
        else if (emotion == EmotionController.Emotion.Rage)
            ShowRageWorld();
        else
            ShowNeutralWorld();
    }

    public void ShowJoyWorld()
    {
        SetObjects(joyObjects, true);
        SetObjects(rageObjects, false);
        SetObjects(neutralObjects, false);
    }

    public void ShowRageWorld()
    {
        SetObjects(joyObjects, false);
        SetObjects(rageObjects, true);
        SetObjects(neutralObjects, false);
    }

    public void ShowNeutralWorld()
    {
        SetObjects(joyObjects, false);
        SetObjects(rageObjects, false);
        SetObjects(neutralObjects, true);
    }

    private void SetObjects(GameObject[] objects, bool active)
    {
        foreach (GameObject obj in objects)
        {
            if (obj != null)
                obj.SetActive(active);
        }
    }
}