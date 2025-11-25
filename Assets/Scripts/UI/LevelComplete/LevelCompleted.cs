using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompleted : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI leveltxt;
    [SerializeField] private TextMeshProUGUI scoretxt;
    [SerializeField] private TextMeshProUGUI restartBtnText;
    [SerializeField] private TextMeshProUGUI watchBtnText;

    [SerializeField] private Button restartbtn;
    [SerializeField] private Button watchBtn;

    public void Setup(string _levelTxt, int score, string _restartBtnText, string _watchBtnText)
    {
        leveltxt.text = _levelTxt;
        scoretxt.text = score.ToString();
        restartBtnText.text = _restartBtnText;
        watchBtnText.text = _watchBtnText;

    }
    public void Setup(string _levelTxt, string _restartBtnText, string _watchBtnText)
    {
        leveltxt.text = _levelTxt;
        restartBtnText.text = _restartBtnText;
        watchBtnText.text = _watchBtnText;

    }

    void OnEnable()
    {
        restartbtn.onClick.AddListener(RestartButton);
        watchBtn.onClick.AddListener(WatchButton);

    }



    private void WatchButton()
    {
        GameManager.Instance.ContinueAfterAd();
    }

    private void RestartButton()
    {
        GameManager.Instance.RestartGame();
        Debug.Log("Restarting the game ");
    }
    void OnDisable()
    {
        restartbtn.onClick?.RemoveListener(RestartButton);
        watchBtn.onClick?.RemoveListener(WatchButton);
    }
}
