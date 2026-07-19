using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    private bool _isFading;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SetAlpha(0f);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void FadeToScene(string sceneName)
    {
        if (!_isFading)
            StartCoroutine(FadeAndLoad(sceneName));
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        if (fadeImage == null)
        {
            SceneManager.LoadScene(sceneName);
            yield break;
        }

        _isFading = true;

        yield return Fade(0f, 1f);

        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (fadeImage != null)
            StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        if (fadeImage == null)
        {
            _isFading = false;
            yield break;
        }

        yield return Fade(1f, 0f);

        _isFading = false;
    }

    /// <summary>
    /// עוטף את לוגיקת ה-fade המשותפת (הלוך וחזור היו קוד כמעט זהה בעבר).
    /// </summary>
    private IEnumerator Fade(float from, float to)
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            SetAlpha(Mathf.Lerp(from, to, time / fadeDuration));
            yield return null;
        }

        SetAlpha(to);
    }

    private void SetAlpha(float alpha)
    {
        if (fadeImage == null)
            return;

        Color c = fadeImage.color;
        c.a = alpha;
        fadeImage.color = c;
    }
}
