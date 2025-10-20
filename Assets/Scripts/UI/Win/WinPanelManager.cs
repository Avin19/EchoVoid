using UnityEngine;
using UnityEngine.UIElements;

public class WinPanelManager : MonoBehaviour
{
    public static WinPanelManager Instance;

    private VisualElement root;
    private Label winScoreLabel;
    private Button nextLevelButton;
    private Button rewardButton;
    private Label adStatusLabel;

    private int lastScore;

    void Awake() => Instance = this;

    void Start()
    {
        var uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement.Q<VisualElement>("win-root");
        winScoreLabel = root.Q<Label>("win-score");
        nextLevelButton = root.Q<Button>("next-level-button");
        rewardButton = root.Q<Button>("reward-button");

        // ✅ Make sure the panel can receive clicks
        root.pickingMode = PickingMode.Position;

        root.style.display = DisplayStyle.None;

        nextLevelButton.clicked += OnNextLevelClicked;
        rewardButton.clicked += OnRewardClicked;
    }

    public void ShowWinPanel(int score)
    {
        lastScore = score;
        winScoreLabel.text = $"SCORE: {score:0000}";
        root.style.display = DisplayStyle.Flex;
        Time.timeScale = 0f;
        UpdateAdButtonState();
    }

    public void HideWinPanel()
    {
        root.style.display = DisplayStyle.None;
        Time.timeScale = 1f;
    }

    private void OnNextLevelClicked()
    {
        HideWinPanel();
        GameManager.Instance?.NextLevel();
    }

    private void OnRewardClicked()
    {



        // adStatusLabel.text = "Ad not ready yet. Try again soon.";
        rewardButton.SetEnabled(false);
        Invoke(nameof(UpdateAdButtonState), 3f);

    }

    // 🎁 This is triggered once the player finishes watching a rewarded ad
    private void OnAdRewardGranted()
    {
        Debug.Log("🎉 Player earned win bonus from ad!");

        int bonus = Mathf.RoundToInt(lastScore * 0.5f);
        UIManager.Instance?.AddScore(bonus);
        UIManager.Instance?.UpdateScore(UIManager.Instance.CurrentScore);

        HideWinPanel();

        // ✅ Restart or go to next level properly
        GameManager.Instance?.NextLevel();
    }


    private void UpdateAdButtonState()
    {


        rewardButton.SetEnabled(false);
        rewardButton.text = "Loading Ad...";
        adStatusLabel.text = "Preparing ad...";

    }
}
