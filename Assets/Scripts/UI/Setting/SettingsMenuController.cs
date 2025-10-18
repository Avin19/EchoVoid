using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class SettingsMenuController : MonoBehaviour
{
    private Slider volumeSlider;
    private DropdownField graphicsDropdown;
    private Toggle vibrationToggle;
    private Button backButton;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        volumeSlider = root.Q<Slider>("volume-slider");
        graphicsDropdown = root.Q<DropdownField>("graphics-dropdown");
        vibrationToggle = root.Q<Toggle>("vibration-toggle");
        backButton = root.Q<Button>("back-button");

        volumeSlider.RegisterValueChangedCallback(OnVolumeChanged);
        graphicsDropdown.RegisterValueChangedCallback(OnGraphicsChanged);
        vibrationToggle.RegisterValueChangedCallback(OnVibrationToggled);
        backButton.clicked += OnBackClicked;

        // Load saved settings
        LoadSettings();
    }

    void OnVolumeChanged(ChangeEvent<float> evt)
    {
        AudioListener.volume = evt.newValue;
        PlayerPrefs.SetFloat("volume", evt.newValue);
    }

    void OnGraphicsChanged(ChangeEvent<string> evt)
    {
        switch (evt.newValue)
        {
            case "Low": QualitySettings.SetQualityLevel(0); break;
            case "Medium": QualitySettings.SetQualityLevel(2); break;
            case "High": QualitySettings.SetQualityLevel(5); break;
        }
        PlayerPrefs.SetString("graphics", evt.newValue);
    }

    void OnVibrationToggled(ChangeEvent<bool> evt)
    {
#if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
#endif
        PlayerPrefs.SetInt("vibration", evt.newValue ? 1 : 0);
    }

    void OnBackClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void LoadSettings()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("volume", 0.8f);
        graphicsDropdown.value = PlayerPrefs.GetString("graphics", "Medium");
        vibrationToggle.value = PlayerPrefs.GetInt("vibration", 1) == 1;
    }
}
