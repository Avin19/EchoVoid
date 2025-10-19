using UnityEngine;
using UnityEngine.UIElements;

public class WinPanelManager : MonoBehaviour
{
    public static WinPanelManager Instance;

    private VisualElement root;
    private Label winScoreLabel;
    private Button nextLevelButton;
    private Button rewardButton;

    void Awake() => Instance = this;

    void Start()
    {
        var uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement.Q<VisualElement>("win-root");
        winScoreLabel = root.Q<Label>("win-score");
        nextLevelButton = root.Q<Button>("next-level-button");
        rewardButton = root.Q<Button>("reward-button");

        root.style.display = DisplayStyle.None;

        nextLevelButton.clicked += OnNextLevelClicked;
        rewardButton.clicked += OnRewardClicked;
    }

    public void ShowWinPanel(int score)
    {
        winScoreLabel.text = $"SCORE: {score:0000}";
        root.style.display = DisplayStyle.Flex;
        Time.timeScale = 0f;
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
        HideWinPanel();
        AdManager.Instance?.ShowRewardedAd();
    }
}
