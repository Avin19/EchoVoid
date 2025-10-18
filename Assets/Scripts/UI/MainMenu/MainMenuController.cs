using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    private Button startButton;
    private Button settingsButton;
    private Button exitButton;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        startButton = root.Q<Button>("start-button");
        settingsButton = root.Q<Button>("settings-button");
        exitButton = root.Q<Button>("exit-button");

        startButton.clicked += OnStartClicked;
        settingsButton.clicked += OnSettingsClicked;
        exitButton.clicked += OnExitClicked;
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

    void OnExitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
