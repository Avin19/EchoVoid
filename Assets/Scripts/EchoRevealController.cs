using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EchoRevealController : MonoBehaviour
{
    private Material mat;
    private float fadeValue = 0f;
    public float fadeSpeed = 2f;

    void Awake()
    {
        // IMPORTANT: get instance copy of material so per-sprite properties won't be shared incorrectly
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        mat = sr.material; // this gives an instance in Play mode
    }

    // call from SoundPulse
    public void Reveal(Vector3 origin, float radius)
    {
        if (mat == null) return;
        mat.SetVector("_ObjectPosition", new Vector4(origin.x, origin.y, origin.z, 0f));
        mat.SetFloat("_RevealRadius", radius);
        fadeValue = 1f;
        mat.SetFloat("_Fade", fadeValue);
    }

    void Update()
    {
        if (fadeValue > 0f)
        {
            fadeValue -= Time.deltaTime * fadeSpeed;
            fadeValue = Mathf.Clamp01(fadeValue);
            if (mat != null) mat.SetFloat("_Fade", fadeValue);
        }
    }

    // debug helper: press G to make this wall glow for testing
#if UNITY_EDITOR
    void OnGUI()
    {
        // optional runtime debug UI
    }
#endif
}
