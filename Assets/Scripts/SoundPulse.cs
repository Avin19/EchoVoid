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

        // Spawn visual ring
        GameObject ring = null;
        Material ringMat = null;

        if (pulseVisualPrefab)
        {
            ring = Instantiate(pulseVisualPrefab, transform.position, Quaternion.identity);
            ringMat = ring.GetComponent<SpriteRenderer>().material;
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

        if (ring)
            Destroy(ring);

        isPulsing = false;
    }

}
