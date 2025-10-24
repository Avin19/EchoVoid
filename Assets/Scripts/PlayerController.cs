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

    [Header("References")]
    private Joystick joystick;

    public void AssignJoystick(Joystick joy)
    {
        joystick = joy;
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

#if UNITY_ANDROID || UNITY_IOS
        // ✅ Use joystick on mobile
        input = joystick != null ? joystick.Direction : Vector2.zero;
#else
        // ✅ Use keyboard on PC
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
#endif

        // Debug joystick values

        // ✅ Emit pulse
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryEmitPulse();
        }
    }

    void FixedUpdate()
    {
        if (!GameManager.Instance.IsPlaying())
            return;

        rb.MovePosition(rb.position + input * moveSpeed * Time.fixedDeltaTime);
    }

    public void TryEmitPulse()
    {
        if (Time.time < nextPulseTime) return;

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
        }
        else
        {
            Debug.Log("⚠️ Out of energy!");
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
