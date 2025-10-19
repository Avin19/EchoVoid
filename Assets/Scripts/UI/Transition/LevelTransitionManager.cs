using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class LevelTransitionManager : MonoBehaviour
{
    public static LevelTransitionManager Instance;

    [Header("Transition Settings")]
    [Tooltip("Fade duration in seconds (unscaled time).")]
    public float fadeDuration = 1f;

    [Tooltip("Optional background color for transition.")]
    public Color fadeColor = Color.black;

    private VisualElement fadeOverlay;
    private bool isFading;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Find UI Document and create fade overlay
        var uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null)
        {
            Debug.LogWarning("⚠️ No UIDocument attached to LevelTransitionManager!");
            return;
        }

        var root = uiDoc.rootVisualElement;
        fadeOverlay = new VisualElement();
        fadeOverlay.style.position = Position.Absolute;
        fadeOverlay.style.top = 0;
        fadeOverlay.style.left = 0;
        fadeOverlay.style.right = 0;
        fadeOverlay.style.bottom = 0;
        fadeOverlay.style.backgroundColor = new StyleColor(fadeColor);
        fadeOverlay.style.opacity = 0f;
        fadeOverlay.style.display = DisplayStyle.None;

        root.Add(fadeOverlay);
    }

    /// <summary>
    /// Fades out, executes callback, then fades back in.
    /// Works even when Time.timeScale = 0 (uses unscaled time).
    /// </summary>
    public void FadeToNextLevel(System.Action onFadeComplete)
    {
        if (isFading)
        {
            Debug.Log("⏳ Transition already in progress...");
            return;
        }

        StartCoroutine(FadeRoutine(onFadeComplete));
    }

    private IEnumerator FadeRoutine(System.Action onFadeComplete)
    {
        isFading = true;

        fadeOverlay.style.display = DisplayStyle.Flex;
        float t = 0f;

        // Fade in (screen goes black)
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float alpha = Mathf.Clamp01(t / fadeDuration);
            fadeOverlay.style.opacity = alpha;
            yield return null;
        }

        fadeOverlay.style.opacity = 1f;

        // Small unscaled delay for clarity
        yield return new WaitForSecondsRealtime(0.2f);

        // Run callback (e.g. load next level, restart)
        onFadeComplete?.Invoke();

        // Fade out (reveal new scene)
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float alpha = Mathf.Clamp01(1f - (t / fadeDuration));
            fadeOverlay.style.opacity = alpha;
            yield return null;
        }

        fadeOverlay.style.opacity = 0f;
        fadeOverlay.style.display = DisplayStyle.None;

        isFading = false;
    }

    /// <summary>
    /// Instantly fade to black (for immediate transitions).
    /// </summary>
    public void ForceFadeIn()
    {
        fadeOverlay.style.display = DisplayStyle.Flex;
        fadeOverlay.style.opacity = 1f;
    }

    /// <summary>
    /// Instantly fade out (fully transparent).
    /// </summary>
    public void ForceFadeOut()
    {
        fadeOverlay.style.opacity = 0f;
        fadeOverlay.style.display = DisplayStyle.None;
    }
}
