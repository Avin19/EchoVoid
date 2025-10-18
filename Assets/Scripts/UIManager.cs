using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private Label levelLabel;
    private Label scoreLabel;
    private VisualElement energyFill;

    private int currentEnergy = 10;
    private int maxEnergy = 10;

    public int CurrentScore { get; private set; } = 0;
    public int CurrentLevel { get; private set; } = 1;

    // ✅ Add these read-only properties:
    public int CurrentEnergy => currentEnergy;
    public int MaxEnergy => maxEnergy;

    void Awake()
    {
        // ✅ Singleton setup
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        levelLabel = root.Q<Label>("level-label");
        scoreLabel = root.Q<Label>("score-label");
        energyFill = root.Q<VisualElement>("energy-bar-fill");

        UpdateLevel(CurrentLevel);
        UpdateScore(0);
        UpdateEnergy(currentEnergy, maxEnergy);
    }

    // 🎯 LEVEL
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

    public void AddScore(int amount)
    {
        UpdateScore(CurrentScore + amount);
    }

    // ⚡ ENERGY
    public void UpdateEnergy(int current, int max)
    {
        currentEnergy = current;
        maxEnergy = max;

        if (energyFill == null)
            return;

        float pct = Mathf.Clamp01((float)current / max);
        energyFill.style.width = new Length(pct * 100, LengthUnit.Percent);

        // Color feedback
        if (pct < 0.2f)
            energyFill.style.backgroundColor = new StyleColor(Color.red);
        else
            energyFill.style.backgroundColor = new StyleColor(new Color(0f, 1f, 1f));
    }

    // 🔄 RESET (used by GameManager between levels)
    public void ResetUIForNewLevel()
    {
        UpdateLevel(CurrentLevel);
        UpdateEnergy(maxEnergy, maxEnergy);
    }

    // 💥 OPTIONAL — external getters
    public int GetEnergy() => currentEnergy;
    public int GetMaxEnergy() => maxEnergy;
    // 👇 Add this at the bottom of your UIManager class
    public void ShowHUD()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        if (root != null)
        {
            root.style.display = DisplayStyle.Flex;
            root.style.opacity = 1f;
        }
    }

    public void HideHUD()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        if (root != null)
        {
            root.style.display = DisplayStyle.None;
            root.style.opacity = 0f;
        }
    }

}
