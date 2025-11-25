using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class TutorialUI : MonoBehaviour
{
    [SerializeField] private Button skipButton;
    [SerializeField] private TextMeshProUGUI buttonTxt;
    [SerializeField] private TextMeshProUGUI informTxt;
    [SerializeField] private int click = 0;


    void Start()
    {
        skipButton.onClick.AddListener(SkipTutorial);
        buttonTxt.text = "SKIP";

    }

    private void SkipTutorial()
    {
        click++;
        if (click == 1)
        {
            informTxt.text = " Use the joystick (or WASD) to move around. ";
        }
        if (click == 2)
        {
            informTxt.text = "Tap the screen or press SPACE to emit sound and reveal obstacles.";
        }
        if (click == 3)
        {
            informTxt.text = " Now find the glowing goal and escape the void. ";
            buttonTxt.text = "MISSION START";
        }
        if (click >= 4)
        {
            GameManager.Instance.Transition();
        }
    }
}
