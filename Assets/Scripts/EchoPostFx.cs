// Example snippet to control post effects from code
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class EchoPostFx : MonoBehaviour
{
    public Volume volume;
    private Bloom bloom;
    private Vignette vignette;
    private ChromaticAberration chroma;

    void Start()
    {
        if (volume == null || volume.profile == null)
        {
            Debug.LogError("EchoPostFx: Volume or profile is null. Post-processing effects disabled.");
            return;
        }

        volume.profile.TryGet(out bloom);
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out chroma);

        if (bloom == null)
            Debug.LogWarning("EchoPostFx: Bloom effect not found in volume profile.");
        if (chroma == null)
            Debug.LogWarning("EchoPostFx: Chromatic Aberration effect not found in volume profile.");
    }

    public void PulseFX()
    {
        if (bloom != null || chroma != null)
            StartCoroutine(AnimatePulse());
    }

    IEnumerator AnimatePulse()
    {
        if (bloom != null)
            bloom.intensity.value = 3f;
        if (chroma != null)
            chroma.intensity.value = 0.4f;
        
        yield return new WaitForSeconds(0.3f);
        
        if (bloom != null)
            bloom.intensity.value = 2f;
        if (chroma != null)
            chroma.intensity.value = 0.15f;
    }
}
