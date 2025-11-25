using UnityEngine;


public class GameUIManager : MonoBehaviour
{

    [Header("Panel")]
    [SerializeField]
    private GameObject tutorial, transtional, hud, levelCompleted;
    private bool isPaused = false;



    void Start()
    {
        SetAllPanel();
        tutorial.SetActive(true);

    }
    public void Restart()
    {
        TransitionPanel();
    }
    public void StartLevel()
    {
        Start();
    }

    public void SetAllPanel()
    {
        tutorial.SetActive(false);
        transtional.SetActive(false);
        hud.SetActive(false);
        levelCompleted.SetActive(false);
    }
    public void TransitionPanel()
    {
        isPaused = false;
        Time.timeScale = 1f;
        SetAllPanel();
        transtional.SetActive(true);
        Invoke(nameof(GameStart), 2f);

    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        SetAllPanel();
        levelCompleted.SetActive(true);
        levelCompleted.GetComponent<LevelCompleted>().Setup("PAUSE", " RESUME ", "WATCH AD ");
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        SetAllPanel();
        hud.SetActive(true);

    }
    public void HUD(int level)
    {
        hud.GetComponent<HUD>().UpdateLevel(level);
    }
    public void HudScore(int score)
    {
        hud.GetComponent<HUD>().UpdateScore(score);
    }

    public void ShowWinPanel(int score)
    {
        SetAllPanel();
        levelCompleted.SetActive(true);
        levelCompleted.GetComponent<LevelCompleted>().Setup("Level Completed", score, " NEXT LEVEL ", "EXTRA LIFE");
    }
    public void ShowLossPanel(string _message)
    {
        SetAllPanel();
        levelCompleted.SetActive(true);
        levelCompleted.GetComponent<LevelCompleted>().Setup(_message, " RESTART LEVEL ", "WATCH AD");
    }
    public void GameStart()
    {
        SetAllPanel();
        hud.SetActive(true);

    }


}
