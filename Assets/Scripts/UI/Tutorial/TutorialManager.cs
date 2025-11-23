using System.Collections;
using UnityEngine;
using UnityEngine.UI;



public class TutorialUI : MonoBehaviour
{
    public Joystick joystick; // assign in inspector (optional)
    public PlayerController player; // optional — used to call TryEmitPulse
    public float moveThreshold = 0.4f; // joystick magnitude required
    public float sustainSeconds = 0.35f; // how long movement must be sustained


    [SerializeField] private Button skipButton;

    bool stepMoveCompleted = false;
    bool stepPulseCompleted = false;

    void Awake()
    {

    }

    void Start()
    {


        // Hook up skip button
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipTutorial);
            Debug.Log("[TutorialUI] Skip button hooked up successfully.");
        }
        else
        {
            Debug.LogWarning("[TutorialUI] Skip button not found in UI!");
        }

        // Ensure player/joystick references if not assigned
        if (joystick == null)
            joystick = FindObjectOfType<Joystick>();
        if (player == null)
            player = FindObjectOfType<PlayerController>();

        // Start monitoring
        StartCoroutine(MonitorMoveStep());
    }

    IEnumerator MonitorMoveStep()
    {
        float timer = 0f;
        while (!stepMoveCompleted)
        {
            Vector2 dir = Vector2.zero;

            if (joystick != null)
                dir = joystick.Direction;
            else
            {
                // fallback to keyboard
                dir.x = Input.GetAxisRaw("Horizontal");
                dir.y = Input.GetAxisRaw("Vertical");
            }

            float mag = dir.magnitude;

            if (mag >= moveThreshold)
            {
                timer += Time.unscaledDeltaTime;
                if (timer >= sustainSeconds)
                {
                    // move step completed
                    stepMoveCompleted = true;
                    OnMoveStepCompleted();
                    break;
                }
            }
            else
            {
                timer = 0f;
            }

            yield return null;
        }
    }

    void OnMoveStepCompleted()
    {

        Invoke(nameof(StepPulseSucceeded), 2f);
    }



    void StepPulseSucceeded()
    {
        stepPulseCompleted = true;

        StartCoroutine(FinishAndHide(0.9f));
    }

    IEnumerator FinishAndHide(float wait)
    {
        yield return new WaitForSecondsRealtime(wait);
        HideTutorial();
    }

    public void SkipTutorial()
    {
        HideTutorial();
    }

    void HideTutorial()
    {

        // Optionally notify GameManager or other systems (if needed)
        // Example: GameManager.Instance?.OnTutorialCompleted();
    }

    // Optional: public called by other code if the player emits a pulse not via space (for example UI pulse button)
    public void NotifyPulseEmittedExternally()
    {
        if (!stepMoveCompleted) return; // only accept if move step done
        if (stepPulseCompleted) return;

        StepPulseSucceeded();
    }
}
