using System;

public static class GameEvents
{
    public static event Action<EmotionController.Emotion> OnEmotionChanged;
    public static event Action OnGameOver;
    public static event Action OnRestartRequested;

    public static event Action<Stamina.StaminaType, float, float> OnStaminaChanged;

    public static void RaiseEmotionChanged(EmotionController.Emotion emotion)
    {
        OnEmotionChanged?.Invoke(emotion);
    }

    public static void RaiseGameOver()
    {
        OnGameOver?.Invoke();
    }

    public static void RaiseRestartRequested()
    {
        OnRestartRequested?.Invoke();
    }

    public static void RaiseStaminaChanged(Stamina.StaminaType type, float current, float max)
    {
        OnStaminaChanged?.Invoke(type, current, max);
    }
}