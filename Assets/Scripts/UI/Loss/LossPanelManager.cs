using UnityEngine;
using UnityEngine.UIElements;

public class LossPanelManager : MonoBehaviour
{
    public static LossPanelManager Instance;

    private VisualElement root;
    private Button watchAdButton;
    private Button restartButton;

    void Awake() => Instance = this;

    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("loss-root");
        watchAdButton = root.Q<Button>("watch-ad-button");
        restartButton = root.Q<Button>("restart-button");

        root.style.display = DisplayStyle.None;

        watchAdButton.clicked += OnWatchAdClicked;
        restartButton.clicked += OnRestartClicked;
    }

    public void ShowLossPanel()
    {
        root.style.display = DisplayStyle.Flex;
        Time.timeScale = 0f; // pause game
    }

    public void HideLossPanel()
    {
        root.style.display = DisplayStyle.None;
        Time.timeScale = 1f;
    }

    private void OnWatchAdClicked()
    {
        Debug.Log("🎬 Watch Ad to Continue clicked");
        HideLossPanel();
    }

    private void OnRestartClicked()
    {
        Debug.Log("🔄 Restart Level clicked");
        HideLossPanel();
        GameManager.Instance?.RestartLevel();
    }
}
