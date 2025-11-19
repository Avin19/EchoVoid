using UnityEngine;
using System.Collections;

public class SoundPulse : MonoBehaviour
{
    [Header("Pulse Settings")]
    public float pulseSpeed = 25f;
    public float maxRadius = 15f;
    public LayerMask wallMask;

    [Header("Visual & Audio Effects")]
    public GameObject pulseVisualPrefab;  // 👈 assign PulseRing prefab here
    public AudioSource pulseAudioSource;  // optional ping sound

    private bool isPulsing = false;

    public void EmitPulse()
    {
        if (!isPulsing)
            StartCoroutine(PulseWave());
    }

    IEnumerator PulseWave()
    {
        isPulsing = true;
        float radius = 0f;

        // Play sound
        if (pulseAudioSource) pulseAudioSource.Play();

        // Spawn visual ring (use pooling if available, otherwise instantiate)
        GameObject ring = null;
        Material ringMat = null;

        if (pulseVisualPrefab)
        {
            if (ObjectPool.Instance != null)
            {
                ring = ObjectPool.Instance.SpawnFromPool("PulseRing", transform.position, Quaternion.identity);
            }
            else
            {
                ring = Instantiate(pulseVisualPrefab, transform.position, Quaternion.identity);
            }
            
            if (ring != null)
            {
                var spriteRenderer = ring.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                    ringMat = spriteRenderer.material;
            }
        }

        while (radius < maxRadius)
        {
            radius += Time.deltaTime * pulseSpeed;

            // Animate ring size & fade
            if (ring)
            {
                ring.transform.localScale = Vector3.one * (radius * 2f);

                if (ringMat)
                {
                    float fade = Mathf.Lerp(1f, 0f, radius / maxRadius);
                    ringMat.SetFloat("_Fade", fade);
                }
            }

            // Send echo to walls
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, wallMask);
            foreach (var hit in hits)
            {
                EchoRevealController echo = hit.GetComponent<EchoRevealController>();
                if (echo != null)
                    echo.Reveal(transform.position, radius);
            }

            yield return null;
        }

        // Return to pool or destroy
        if (ring)
        {
            if (ObjectPool.Instance != null)
                ObjectPool.Instance.ReturnToPool(ring);
            else
                Destroy(ring);
        }

        isPulsing = false;
    }

}
