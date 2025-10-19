using UnityEngine;
using UnityEngine.UIElements;

public class PulseButtonController : MonoBehaviour
{
    private Button pulseButton;

    void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        pulseButton = root.Q<Button>("pulse-button");

        if (pulseButton == null)
        {
            Debug.LogError("❌ Pulse button not found in UXML!");
            return;
        }

        pulseButton.clicked += OnPulseClicked;
    }

    void OnPulseClicked()
    {
        var player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.TryEmitPulse();
        }
        else
        {
            Debug.LogWarning("⚠️ Player not found!");
        }
    }
}
