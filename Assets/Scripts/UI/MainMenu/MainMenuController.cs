using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton, settingBackBtn;
    [SerializeField] private GameObject settingPanel;

    void OnEnable()
    {
        startButton.onClick.AddListener(OnStartClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
        exitButton.onClick.AddListener(OnExitClicked);
        settingBackBtn.onClick.AddListener(OnSettingBackClicked);
    }

    private void OnSettingBackClicked()
    {
        settingPanel.SetActive(false);
    }

    void Start()
    {
        AdManager.Instance.ShowBanner();
    }

    void OnStartClicked()
    {

        SceneManager.LoadScene("GameScene"); // Change to your actual scene name
    }

    void OnSettingsClicked()
    {

        settingPanel.SetActive(true);
        // TODO: open settings UI or overlay
    }

    private void OnDisable()
    {
        startButton.onClick.RemoveListener(OnStartClicked);
        settingsButton.onClick.RemoveListener(OnSettingsClicked);
        exitButton.onClick.RemoveListener(OnExitClicked);
        settingBackBtn.onClick.RemoveListener(OnSettingBackClicked);
    }
    void OnExitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
