using UnityEngine;
using UnityEngine.UIElements;

public class TutorialManager : MonoBehaviour
{
    private UIDocument uiDoc;
    private Label tutorialText;
    private VisualElement moveIcon, pulseIcon;
    private Label hintLabel;

    private int currentStep = 0;
    private bool moveDetected = false;
    private bool pulseDetected = false;

    void Start()
    {
        uiDoc = GetComponent<UIDocument>();
        var root = uiDoc.rootVisualElement;

        tutorialText = root.Q<Label>("tutorial-text");
        moveIcon = root.Q<VisualElement>("move-icon");
        pulseIcon = root.Q<VisualElement>("pulse-icon");
        hintLabel = root.Q<Label>("hint-label");

        ShowStep(0);
    }

    void Update()
    {
        switch (currentStep)
        {
            case 0: // Movement
                if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
                {
                    moveDetected = true;
                    AdvanceStep();
                }
                break;

            case 1: // Pulse
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    pulseDetected = true;
                    AdvanceStep();
                }
                break;

            case 2: // Energy awareness
                if (UIManager.Instance != null && UIManager.Instance.CurrentEnergy <= 7)
                {
                    AdvanceStep();
                }
                break;

            case 3: // Goal
                if (GameManager.Instance != null && GameManager.Instance.HasReachedGoal)
                {
                    AdvanceStep();
                }
                break;
        }
    }

    void ShowStep(int step)
    {
        currentStep = step;

        switch (step)
        {
            case 0:
                tutorialText.text = "Move using WASD or Arrow Keys";
                moveIcon.style.display = DisplayStyle.Flex;
                pulseIcon.style.display = DisplayStyle.None;
                break;

            case 1:
                tutorialText.text = "Press SPACE to emit a sound pulse";
                moveIcon.style.display = DisplayStyle.None;
                pulseIcon.style.display = DisplayStyle.Flex;
                break;

            case 2:
                tutorialText.text = "Each pulse drains energy — use them wisely";
                moveIcon.style.display = DisplayStyle.None;
                pulseIcon.style.display = DisplayStyle.Flex;
                break;

            case 3:
                tutorialText.text = "Find the glowing echo to escape the void";
                moveIcon.style.display = DisplayStyle.None;
                pulseIcon.style.display = DisplayStyle.None;
                break;

            case 4:
                tutorialText.text = "Good... you remember how to resonate.";
                hintLabel.text = "";
                Invoke(nameof(EndTutorial), 3f);
                break;
        }
    }

    void AdvanceStep()
    {
        currentStep++;
        ShowStep(currentStep);
    }

    void EndTutorial()
    {
        gameObject.SetActive(false); // hide tutorial
        UIManager.Instance?.ShowHUD(); // bring HUD back if hidden
        GameManager.Instance?.StartLevel();
    }
}
