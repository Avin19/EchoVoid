using UnityEngine;

/// <summary>
/// Displays an arrow on the player pointing towards the goal
/// Helps players navigate in the darkness
/// </summary>
public class GoalDirectionArrow : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform goal;

    [Header("Arrow Settings")]
    public float distanceFromPlayer = 1.5f;
    public float arrowScale = 2f;
    public Color arrowColor = new Color(1f, 0.3f, 0.3f, 0.8f); // Red with transparency

    [Header("Visibility Settings")]
    public bool showOnlyWhenFar = false;
    public float minDistanceToShow = 5f; // Only show when goal is far
    public float maxDistanceToShow = 50f; // Hide when too far (out of range)

    [Header("Animation")]
    public bool enablePulse = true;
    public float pulseSpeed = 3f;
    public float pulseAmount = 0.2f;

    private GameObject arrowObject;
    private SpriteRenderer arrowRenderer;
    private Vector3 baseScale;

    void Start()
    {
        CreateArrow();
    }

    void Update()
    {
        if (player == null || goal == null)
        {
            if (arrowObject != null)
                arrowObject.SetActive(false);
            return;
        }

        float distance = Vector2.Distance(player.position, goal.position);

        // Check visibility conditions
        bool shouldShow = true;
        if (showOnlyWhenFar)
        {
            shouldShow = distance >= minDistanceToShow && distance <= maxDistanceToShow;
        }

        if (arrowObject != null)
        {
            arrowObject.SetActive(shouldShow);

            if (shouldShow)
            {
                UpdateArrowPosition();
                UpdateArrowRotation();

                if (enablePulse)
                {
                    AnimatePulse();
                }
            }
        }
    }

    void CreateArrow()
    {
        arrowObject = new GameObject("GoalDirectionArrow");
        arrowObject.transform.SetParent(transform);
        arrowObject.transform.localScale = Vector3.one * 2;

        arrowRenderer = arrowObject.AddComponent<SpriteRenderer>();
        arrowRenderer.sprite = CreateArrowSprite();
        arrowRenderer.material = new Material(Shader.Find("EchoVoid/PlayerGlow"));
        arrowRenderer.color = arrowColor;
        arrowRenderer.sortingOrder = 100; // Render on top

        baseScale = Vector3.one * arrowScale;
        arrowObject.transform.localScale = baseScale;
    }

    void UpdateArrowPosition()
    {
        if (player == null) return;

        // Position arrow at fixed distance from player
        Vector2 direction = (goal.position - player.position).normalized;
        Vector3 arrowPos = player.position + (Vector3)(direction * distanceFromPlayer);
        arrowObject.transform.position = arrowPos;
    }

    void UpdateArrowRotation()
    {
        if (player == null || goal == null) return;

        // Point arrow towards goal
        Vector2 direction = goal.position - player.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrowObject.transform.rotation = Quaternion.Euler(0, 0, angle - 90f); // -90 because sprite points up
    }

    void AnimatePulse()
    {
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        arrowObject.transform.localScale = baseScale * pulse;

        // Also pulse the alpha
        Color c = arrowColor;
        c.a = arrowColor.a * (0.7f + Mathf.Sin(Time.time * pulseSpeed) * 0.3f);
        arrowRenderer.color = c;
    }

    Sprite CreateArrowSprite()
    {
        // Create a simple arrow texture
        int size = 32;
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];

        // Clear to transparent
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }

        // Draw arrow shape (pointing up)
        int centerX = size / 2;

        // Arrow head (triangle)
        for (int y = size - 8; y < size; y++)
        {
            int width = (size - y) * 2;
            for (int x = centerX - width / 2; x < centerX + width / 2; x++)
            {
                if (x >= 0 && x < size)
                {
                    pixels[y * size + x] = Color.white;
                }
            }
        }

        // Arrow shaft
        int shaftWidth = 4;
        for (int y = 0; y < size - 8; y++)
        {
            for (int x = centerX - shaftWidth / 2; x < centerX + shaftWidth / 2; x++)
            {
                if (x >= 0 && x < size)
                {
                    pixels[y * size + x] = Color.white;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        texture.filterMode = FilterMode.Point; // Crisp pixels

        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    public void SetGoal(Transform goalTransform)
    {
        goal = goalTransform;
    }

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    void OnDestroy()
    {
        if (arrowObject != null)
        {
            Destroy(arrowObject);
        }
    }
}
