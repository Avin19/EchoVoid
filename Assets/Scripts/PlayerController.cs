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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentEnergy = maxEnergy;
        UIManager.Instance?.UpdateEnergy(currentEnergy, maxEnergy);
    }
    private void OnEnable()
    {
        // Subscribe to tap events (mobile)
        // MobileInputManager.OnTap += HandleTap;
    }

    private void OnDisable()
    {
        // Unsubscribe on disable to prevent leaks
        // MobileInputManager.OnTap -= HandleTap;
    }
    void Update()
    {
        // 🚫 Stop all input when not in Playing state
        if (!GameManager.Instance || !GameManager.Instance.IsPlaying())
            return;

        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryEmitPulse();
        }
    }

    void FixedUpdate()
    {
        if (!GameManager.Instance || !GameManager.Instance.IsPlaying())
            return;

        rb.MovePosition(rb.position + input * moveSpeed * Time.fixedDeltaTime);
    }

    void TryEmitPulse()
    {
        if (Time.time < nextPulseTime) return;

        if (currentEnergy > 0)
        {
            // ✅ Instantiate a pulse at player position
            if (pulsePrefab != null)
                pulsePrefab?.EmitPulse();

            currentEnergy--;
            UIManager.Instance?.UpdateEnergy(currentEnergy, maxEnergy);
            nextPulseTime = Time.time + pulseCooldown;

            // 💀 Notify GameManager when depleted
            if (currentEnergy <= 0)
                GameManager.Instance?.OnPlayerEnergyDepleted();
        }
        else
        {
            Debug.Log("⚠️ Out of energy!");
        }
    }

    // ⚡ Called when player continues via ad or collects a powerup
    public void RestoreFullEnergy()
    {
        currentEnergy = maxEnergy;
        UIManager.Instance?.UpdateEnergy(currentEnergy, maxEnergy);
    }

    // ⚡ Called when player gets energy pickup
    public void RestoreEnergy(int amount)
    {
        currentEnergy = Mathf.Min(maxEnergy, currentEnergy + amount);
        UIManager.Instance?.UpdateEnergy(currentEnergy, maxEnergy);
    }

    // 💡 Optional: expose for debugging or other systems
    public int GetEnergy() => currentEnergy;
    public bool HasEnergy => currentEnergy > 0;
}
