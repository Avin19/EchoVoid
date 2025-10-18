using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class LevelTransitionManager : MonoBehaviour
{
    public static LevelTransitionManager Instance;

    private VisualElement fadeOverlay;
    private bool isTransitioning = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        fadeOverlay = root.Q<VisualElement>("fade-overlay");

        // Start transparent
        fadeOverlay.style.opacity = 0;
        fadeOverlay.style.display = DisplayStyle.None;
    }

    public void FadeToNextLevel(System.Action onFadeComplete)
    {
        if (isTransitioning) return;
        StartCoroutine(FadeTransitionRoutine(onFadeComplete));
    }

    private IEnumerator FadeTransitionRoutine(System.Action onFadeComplete)
    {
        isTransitioning = true;
        fadeOverlay.style.display = DisplayStyle.Flex;

        // Fade Out
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 1.5f;
            fadeOverlay.style.opacity = t;
            yield return null;
        }

        onFadeComplete?.Invoke(); // Do the map regeneration here

        yield return new WaitForSeconds(0.3f);

        // Fade In
        t = 1f;
        while (t > 0f)
        {
            t -= Time.deltaTime * 1.5f;
            fadeOverlay.style.opacity = t;
            yield return null;
        }

        fadeOverlay.style.display = DisplayStyle.None;
        isTransitioning = false;
    }
}
