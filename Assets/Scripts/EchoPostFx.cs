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
        volume.profile.TryGet(out bloom);
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out chroma);
    }

    public void PulseFX()
    {
        StartCoroutine(AnimatePulse());
    }

    IEnumerator AnimatePulse()
    {
        bloom.intensity.value = 3f;
        chroma.intensity.value = 0.4f;
        yield return new WaitForSeconds(0.3f);
        bloom.intensity.value = 2f;
        chroma.intensity.value = 0.15f;
    }
}
