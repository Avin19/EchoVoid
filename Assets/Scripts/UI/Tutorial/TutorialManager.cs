using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Interactive tutorial for joystick + pulse.
/// - Step 1: requires joystick movement (or keyboard) for a short sustained threshold.
/// - Step 2: requires a pulse (space key or pulse button).
/// - After completion, hides itself and optionally notifies GameManager or other listeners.
/// 
/// Setup:
/// - Put a UIDocument in your scene and assign the TutorialUI.uxml as the source asset.
/// - Attach this script to the same GameObject as the UIDocument (or assign UIDocument manually).
/// - Assign 'joystick' (your Joystick component) and 'player' (PlayerController) via inspector if you have them.
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class TutorialUI : MonoBehaviour
{
    public Joystick joystick; // assign in inspector (optional)
    public PlayerController player; // optional — used to call TryEmitPulse
    public float moveThreshold = 0.4f; // joystick magnitude required
    public float sustainSeconds = 0.35f; // how long movement must be sustained

    UIDocument uiDocument;
    VisualElement root;
    VisualElement stepMove;
    VisualElement stepPulse;
    VisualElement stepDone;
    Button skipButton;

    bool stepMoveCompleted = false;
    bool stepPulseCompleted = false;

    void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
    }

    void Start()
    {
        if (uiDocument == null)
        {
            Debug.LogError("[TutorialUI] No UIDocument found on same GameObject.");
            enabled = false;
            return;
        }

        root = uiDocument.rootVisualElement.Q<VisualElement>("tutorial-root");
        stepMove = root.Q<VisualElement>("step-move");
        stepPulse = root.Q<VisualElement>("step-pulse");
        stepDone = root.Q<VisualElement>("step-done");
        skipButton = root.Q<Button>("skip-button");

        if (skipButton != null)
            skipButton.clicked += SkipTutorial;

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
        // show pulse step, hide move step
        if (stepMove != null) stepMove.style.display = DisplayStyle.None;
        if (stepPulse != null) stepPulse.style.display = DisplayStyle.Flex;

        // Listen for pulse action
        // If you have a pulse button in UI, we could query it; instead we listen for SPACE or call TryEmitPulse.
        StartCoroutine(MonitorPulseStep());
    }

    IEnumerator MonitorPulseStep()
    {
        // If they tap pulse button (UI) you'd wire that button's clicked to TriggerPulse() from elsewhere.
        // We'll monitor both SPACE and a call to TriggerPulseFromPlayer() (set by player when pulse is emitted)

        while (!stepPulseCompleted)
        {
            // Check for SPACE
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // If PlayerController exists, call TryEmitPulse on it to keep behavior consistent
                player?.TryEmitPulse();
                StepPulseSucceeded();
                break;
            }

            // Also check for touch: tapping anywhere on right-side of screen could be considered a pulse.
#if UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 0)
            {
                Touch t = Input.GetTouch(0);
                // right half screen tap counts as pulse
                if (t.phase == TouchPhase.Began && t.position.x > Screen.width * 0.5f)
                {
                    player?.TryEmitPulse();
                    StepPulseSucceeded();
                    break;
                }
            }
#endif

            yield return null;
        }
    }

    void StepPulseSucceeded()
    {
        stepPulseCompleted = true;
        if (stepPulse != null) stepPulse.style.display = DisplayStyle.None;
        if (stepDone != null) stepDone.style.display = DisplayStyle.Flex;

        // short delay then hide
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
        if (root != null)
            root.style.display = DisplayStyle.None;

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
