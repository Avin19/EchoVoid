using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GoalPulse : MonoBehaviour
{
    [Header("Pulse Settings")]
    public float pulseSpeed = 2f;
    public float scaleMultiplier = 1.15f;
    public float alphaRange = 0.4f;
    public Color pulseColor = new Color(0f, 1f, 1f, 1f);

    private SpriteRenderer sr;
    private Vector3 baseScale;
    private float timeOffset;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        baseScale = transform.localScale;
        timeOffset = Random.Range(0f, 10f); // each goal pulse desyncs slightly
        sr.color = pulseColor;
    }

    void Update()
    {
        float t = Mathf.Sin((Time.time + timeOffset) * pulseSpeed) * 0.5f + 0.5f;

        // Scale pulse
        float scale = Mathf.Lerp(1f, scaleMultiplier, t);
        transform.localScale = baseScale * scale;

        // Alpha pulse
        Color c = pulseColor;
        c.a = Mathf.Lerp(1f - alphaRange, 1f, t);
        sr.color = c;
    }
}
