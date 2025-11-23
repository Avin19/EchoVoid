using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;

    void OnEnable()
    {

        startButton.onClick.AddListener(OnStartClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
        exitButton.onClick.AddListener(OnExitClicked);
    }
    void Start()
    {
        AdManager.Instance.ShowBanner();
    }

    void OnStartClicked()
    {
        Debug.Log("Start clicked");
        SceneManager.LoadScene("GameScene"); // Change to your actual scene name
    }

    void OnSettingsClicked()
    {
        Debug.Log("Settings clicked");
        SceneManager.LoadScene("SettingsMenu");
        // TODO: open settings UI or overlay
    }

    private void Osable()
    {
        startButton.onClick.RemoveListener(OnStartClicked);
        settingsButton.onClick.RemoveListener(OnSettingsClicked);
        exitButton.onClick.RemoveListener(OnExitClicked);
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
