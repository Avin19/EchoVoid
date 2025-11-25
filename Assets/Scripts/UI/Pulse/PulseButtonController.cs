using UnityEngine;
using UnityEngine.UI;

public class PulseButtonController : MonoBehaviour
{
    [SerializeField] private Button pulseButton;

    void Awake()
    {

        pulseButton.onClick.AddListener(OnPulseClicked);
    }

    void OnPulseClicked()
    {
        var player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.TryEmitPulse();
        }

    }
}
