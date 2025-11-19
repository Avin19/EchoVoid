using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class GoalPulse : MonoBehaviour
{
    [Header("Core Pulse Settings")]
    public float pulseSpeed = 2f;
    public float scaleMultiplier = 1.15f;
    public float alphaRange = 0.4f;
    public Color pulseColor = new Color(1f, 0.2f, 0.2f, 1f); // Red color

    [Header("Expanding Ring Settings")]
    public bool enableExpandingRing = true;
    public GameObject ringPrefab; // Assign pulse ring prefab
    public float ringInterval = 2f; // Time between rings
    public float ringSpeed = 8f;
    public float ringMaxRadius = 10f;
    public Color ringColor = new Color(1f, 0.3f, 0.3f, 1f); // Red ring

    private SpriteRenderer sr;
    private Vector3 baseScale;
    private float timeOffset;
    private float nextRingTime;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        baseScale = transform.localScale;
        timeOffset = Random.Range(0f, 10f);
        sr.color = pulseColor;
        nextRingTime = Time.time + ringInterval;
    }

    void Update()
    {
        // Core pulse animation
        float t = Mathf.Sin((Time.time + timeOffset) * pulseSpeed) * 0.5f + 0.5f;

        // Scale pulse
        float scale = Mathf.Lerp(1f, scaleMultiplier, t);
        transform.localScale = baseScale * scale;

        // Alpha pulse
        Color c = pulseColor;
        c.a = Mathf.Lerp(1f - alphaRange, 1f, t);
        sr.color = c;

        // Expanding ring effect
        if (enableExpandingRing && Time.time >= nextRingTime)
        {
            EmitRing();
            nextRingTime = Time.time + ringInterval;
        }
    }

    void EmitRing()
    {
        if (ringPrefab != null)
        {
            // Use object pool if available
            GameObject ring;
            if (ObjectPool.Instance != null)
            {
                ring = ObjectPool.Instance.SpawnFromPool("GoalRing", transform.position, Quaternion.identity);
            }
            else
            {
                ring = Instantiate(ringPrefab, transform.position, Quaternion.identity);
            }

            if (ring != null)
            {
                StartCoroutine(AnimateRing(ring));
            }
        }
        else
        {
            // Fallback: Create simple ring without prefab
            StartCoroutine(CreateAndAnimateSimpleRing());
        }
    }

    IEnumerator AnimateRing(GameObject ring)
    {
        float radius = 0f;
        SpriteRenderer ringRenderer = ring.GetComponent<SpriteRenderer>();
        Material ringMat = ringRenderer != null ? ringRenderer.material : null;

        // Set initial color
        if (ringRenderer != null)
        {
            ringRenderer.color = ringColor;
        }

        while (radius < ringMaxRadius)
        {
            radius += Time.deltaTime * ringSpeed;
            float progress = radius / ringMaxRadius;

            // Scale ring
            ring.transform.localScale = Vector3.one * (radius * 2f);

            // Fade out
            if (ringMat != null)
            {
                float fade = 1f - progress;
                Color c = ringColor;
                c.a = fade;
                if (ringRenderer != null)
                    ringRenderer.color = c;
            }

            yield return null;
        }

        // Return to pool or destroy
        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.ReturnToPool(ring);
        }
        else
        {
            Destroy(ring);
        }
    }

    IEnumerator CreateAndAnimateSimpleRing()
    {
        // Create a simple circle sprite for the ring
        GameObject ring = new GameObject("GoalRing");
        ring.transform.position = transform.position;
        
        SpriteRenderer ringRenderer = ring.AddComponent<SpriteRenderer>();
        ringRenderer.sprite = CreateCircleSprite();
        ringRenderer.color = ringColor;
        ringRenderer.sortingOrder = sr.sortingOrder - 1;

        float radius = 0f;
        while (radius < ringMaxRadius)
        {
            radius += Time.deltaTime * ringSpeed;
            float progress = radius / ringMaxRadius;

            ring.transform.localScale = Vector3.one * (radius * 0.5f);
            
            Color c = ringColor;
            c.a = 1f - progress;
            ringRenderer.color = c;

            yield return null;
        }

        Destroy(ring);
    }

    Sprite CreateCircleSprite()
    {
        // Create a simple circle texture
        int size = 64;
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];

        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f - 2f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                float alpha = 0f;

                if (dist > radius - 2f && dist < radius)
                {
                    alpha = 1f;
                }

                pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}
