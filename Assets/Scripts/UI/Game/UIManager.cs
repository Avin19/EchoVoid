using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private Label levelLabel;
    private Label scoreLabel;
    private VisualElement energyFill;
    private VisualElement root;

    private int currentEnergy = 10;
    private int maxEnergy = 10;

    public int CurrentScore { get; private set; } = 0;
    public int CurrentLevel { get; private set; } = 1;

    public int CurrentEnergy => currentEnergy;
    public int MaxEnergy => maxEnergy;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        levelLabel = root.Q<Label>("level-label");
        scoreLabel = root.Q<Label>("score-label");
        energyFill = root.Q<VisualElement>("energy-bar-fill");

        UpdateLevel(CurrentLevel);
        UpdateScore(0);
        UpdateEnergy(currentEnergy, maxEnergy);
    }

    // 🧭 LEVEL
    public void UpdateLevel(int level)
    {
        CurrentLevel = level;
        if (levelLabel != null)
            levelLabel.text = $"LEVEL {level}";
    }

    // 💯 SCORE
    public void UpdateScore(int newScore)
    {
        CurrentScore = newScore;
        if (scoreLabel != null)
            scoreLabel.text = CurrentScore.ToString("0000");
    }

    public void AddScore(int amount) => UpdateScore(CurrentScore + amount);

    // ⚡ ENERGY
    public void UpdateEnergy(int current, int max)
    {
        currentEnergy = current;
        maxEnergy = max;

        if (energyFill == null)
            return;

        float pct = Mathf.Clamp01((float)current / max);
        energyFill.style.width = new Length(pct * 100, LengthUnit.Percent);

        // Dynamic color feedback
        if (pct < 0.2f)
            energyFill.style.backgroundColor = new StyleColor(Color.red);
        else
            energyFill.style.backgroundColor = new StyleColor(new Color(0f, 1f, 1f)); // cyan
    }

    public void ResetUIForNewLevel()
    {
        UpdateLevel(CurrentLevel);
        UpdateEnergy(maxEnergy, maxEnergy);
    }

    // 🔄 HUD Visibility
    public void ShowHUD()
    {
        if (root == null) root = GetComponent<UIDocument>().rootVisualElement;
        root.style.display = DisplayStyle.Flex;
        root.style.opacity = 1f;
    }

    public void HideHUD()
    {
        if (root == null) root = GetComponent<UIDocument>().rootVisualElement;
        root.style.display = DisplayStyle.None;
        root.style.opacity = 0f;
    }
}
