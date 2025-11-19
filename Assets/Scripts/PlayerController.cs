using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 input;

    [Header("Energy")]
    public int maxEnergy = 10;
    private int currentEnergy;

    [Header("Pulse Settings")]
    public SoundPulse pulsePrefab;
    public float pulseCooldown = 0.5f;
    private float nextPulseTime;
    
    [Header("Input Buffering")]
    public float inputBufferTime = 0.2f;
    private float pulseBufferTimer = -1f;

    [Header("References")]
    private Joystick joystick;

    [Header("Debug")]
    public bool showDebugInfo = false;

    public void AssignJoystick(Joystick joy)
    {
        joystick = joy;
        if (showDebugInfo)
            Debug.Log($"✅ Joystick assigned to player: {joy != null}");
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentEnergy = maxEnergy;
        UIManager.Instance?.UpdateEnergy(currentEnergy, maxEnergy);
    }

    void Update()
    {
        if (!GameManager.Instance.IsPlaying())
            return;

        // ✅ Support both joystick and keyboard input simultaneously
        Vector2 joystickInput = Vector2.zero;
        Vector2 keyboardInput = Vector2.zero;

        // Get joystick input (works in editor and on mobile)
        if (joystick != null)
        {
            joystickInput = joystick.Direction;
        }

        // Get keyboard input (WASD/Arrow keys)
        keyboardInput.x = Input.GetAxisRaw("Horizontal");
        keyboardInput.y = Input.GetAxisRaw("Vertical");

        // Use whichever input is stronger (allows testing with both)
        if (joystickInput.sqrMagnitude > keyboardInput.sqrMagnitude)
        {
            input = joystickInput;
        }
        else
        {
            input = keyboardInput;
        }

        // ✅ Input buffering for pulse
        if (Input.GetKeyDown(KeyCode.Space))
        {
            pulseBufferTimer = inputBufferTime;
        }

        // Try to consume buffered input
        if (pulseBufferTimer > 0)
        {
            if (TryEmitPulse())
            {
                pulseBufferTimer = -1f; // Clear buffer on success
            }
            else
            {
                pulseBufferTimer -= Time.deltaTime; // Decay buffer timer
            }
        }
    }

    void FixedUpdate()
    {
        if (!GameManager.Instance.IsPlaying())
            return;

        Vector2 movement = input * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        // Debug logging
        if (showDebugInfo && input.sqrMagnitude > 0.01f)
        {
            Debug.Log($"🎮 Input: {input} | Movement: {movement} | Position: {rb.position}");
        }
    }

    public bool TryEmitPulse()
    {
        if (Time.time < nextPulseTime) return false;

        if (currentEnergy > 0)
        {
            // ✅ Instantiate a pulse at player position
            if (pulsePrefab != null)
                pulsePrefab.EmitPulse();

            currentEnergy--;
            UIManager.Instance?.UpdateEnergy(currentEnergy, maxEnergy);
            nextPulseTime = Time.time + pulseCooldown;

            if (currentEnergy <= 0)
                GameManager.Instance?.OnPlayerEnergyDepleted();
            
            return true; // Pulse emitted successfully
        }
        else
        {
            Debug.Log("⚠️ Out of energy!");
            return false; // Failed to emit pulse
        }
    }

    // ⚡ Restore energy fully or partially
    public void RestoreFullEnergy()
    {
        currentEnergy = maxEnergy;
        UIManager.Instance?.UpdateEnergy(currentEnergy, maxEnergy);
    }

    public void RestoreEnergy(int amount)
    {
        currentEnergy = Mathf.Min(maxEnergy, currentEnergy + amount);
        UIManager.Instance?.UpdateEnergy(currentEnergy, maxEnergy);
    }

    public int GetEnergy() => currentEnergy;
    public bool HasEnergy => currentEnergy > 0;
}
