using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button pauseBtn;
    private int currentEnergy = 10;
    private int maxEnergy = 10;

    public int CurrentScore { get; private set; } = 0;
    public int CurrentLevel { get; private set; } = 1;
    private void Start()
    {
        UpdateLevel(CurrentLevel);
        UpdateScore(0);
        UpdateEnergy(currentEnergy, maxEnergy);
    }

    public void UpdateEnergy(int currentEnergy, int maxEnergy)
    {
        throw new NotImplementedException();
    }

    public void UpdateScore(int v)
    {
        scoreText.text = v.ToString("0000");
    }

    public void UpdateLevel(int currentLevel)
    {
        levelText.text = "Level" + currentLevel.ToString();
    }

    void OnEnable()
    {
        pauseBtn.onClick.AddListener(OnPause);
    }
    void OnDisable()
    {
        pauseBtn.onClick.RemoveListener(OnPause);
    }

    private void OnPause()
    {
        GameManager.Instance.Paused();
    }


}
