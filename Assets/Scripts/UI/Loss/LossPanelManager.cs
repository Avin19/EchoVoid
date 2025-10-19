using UnityEngine;
using UnityEngine.UIElements;

public class LossPanelManager : MonoBehaviour
{
    public static LossPanelManager Instance;

    private VisualElement root;
    private Button watchAdButton;
    private Button restartButton;
    private Label adStatusLabel;

    void Awake() => Instance = this;

    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("loss-root");
        watchAdButton = root.Q<Button>("watch-ad-button");
        restartButton = root.Q<Button>("restart-button");

        // Optional: Add a label under the buttons for status updates
        adStatusLabel = new Label();
        adStatusLabel.text = "";
        adStatusLabel.style.color = new Color(0.8f, 0.9f, 1f);
        adStatusLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        adStatusLabel.style.marginTop = 5;
        root.Add(adStatusLabel);

        root.style.display = DisplayStyle.None;

        watchAdButton.clicked += OnWatchAdClicked;
        restartButton.clicked += OnRestartClicked;

        InvokeRepeating(nameof(UpdateAdButtonState), 1f, 2f);
    }

    public void ShowLossPanel()
    {
        root.style.display = DisplayStyle.Flex;
        Time.timeScale = 0f; // Pause game when loss panel is shown
        UpdateAdButtonState();
    }

    public void HideLossPanel()
    {
        root.style.display = DisplayStyle.None;
        Time.timeScale = 1f;
    }

    private void OnWatchAdClicked()
    {
        if (AdManager.Instance == null)
        {
            Debug.LogWarning("⚠️ AdManager not initialized yet!");
            adStatusLabel.text = "Ad system not ready.";
            return;
        }

        // Try showing ad
        if (AdManager.Instance.IsRewardedReady())
        {
            Debug.Log("🎥 Showing rewarded ad...");
            adStatusLabel.text = "Loading ad...";
            HideLossPanel();
            AdManager.Instance.ShowRewardedAd();
        }
        else
        {
            Debug.Log("⚠️ Rewarded ad not ready!");
            adStatusLabel.text = "Ad not ready yet. Try again soon.";
            watchAdButton.SetEnabled(false);
            Invoke(nameof(UpdateAdButtonState), 3f);
        }
    }

    private void OnRestartClicked()
    {
        Debug.Log("🔄 Restart Level clicked");
        HideLossPanel();
        GameManager.Instance?.RestartLevel();
    }

    private void UpdateAdButtonState()
    {
        if (AdManager.Instance != null && AdManager.Instance.IsRewardedReady())
        {
            watchAdButton.SetEnabled(true);
            watchAdButton.text = "▶ Watch Ad to Continue";
            adStatusLabel.text = "";
        }
        else
        {
            watchAdButton.SetEnabled(false);
            watchAdButton.text = "Loading Ad...";
            adStatusLabel.text = "Preparing ad...";
        }
    }
}
