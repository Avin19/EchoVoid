using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// Dynamic procedural soundtrack generator for "Echo Void"
/// - Procedural ambient tones that evolve over time
/// - Reactive to player energy (bpm + pitch)
/// - Thread-safe audio thread behavior
/// - Random neural pings (visual + audio)
/// - Smooth drifting for base and echo frequencies
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class ThemeSongGenerator : MonoBehaviour
{
    public static ThemeSongGenerator Instance;

    [Header("Base Theme Settings")]
    [Range(40f, 120f)] public float bpm = 70f;
    [Range(0f, 1f)] public float volume = 0.3f;
    [Range(60f, 440f)] public float baseFrequency = 220f;
    [Range(1f, 3f)] public float echoFrequencyRatio = 1.618f;
    [Range(0f, 1f)] public float modulationDepth = 0.25f;
    public int loopSeconds = 4;

    [Header("Reactive Behavior")]
    public bool reactiveToEnergy = true;
    public float lowEnergyPitchBoost = 1.3f;
    public float lowEnergyTempoBoost = 1.5f;

    [Header("Neural Ping Settings")]
    public bool enablePings = true;
    public float pingInterval = 8f;
    public Color pingColor = new Color(0f, 1f, 1f, 1f);

    [Header("Organic Drift Settings")]
    public float driftSpeed = 0.2f;
    public float freqJitter = 10f;
    public float ratioJitter = 0.05f;
    public float bpmJitter = 5f;

    private AudioSource audioSource;
    private int sampleRate;
    private long samplePosition;
    private double phaseMain, phaseEcho;
    private float pingTimer;
    private GameObject pingVisual;

    // Thread-safe cached data
    private float cachedEnergyPercent = 1f;
    private readonly object energyLock = new object();
    private Transform goalTransform;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        sampleRate = AudioSettings.outputSampleRate;

        int lengthSamples = sampleRate * Mathf.Max(1, loopSeconds);
        AudioClip clip = AudioClip.Create("EchoVoidTheme", lengthSamples, 1, sampleRate, true, OnAudioRead);

        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.spatialBlend = 0f;
        audioSource.volume = volume;
        audioSource.Play();
    }

    void Update()
    {
        // 💡 Neural Pings
        if (enablePings)
        {
            pingTimer -= Time.deltaTime;
            if (pingTimer <= 0f)
            {
                PlayPing();
                pingTimer = UnityEngine.Random.Range(pingInterval * 0.8f, pingInterval * 1.2f);
            }
        }

        // 🌊 Organic Drift: frequency and tempo evolution
        float noiseT = Time.time * driftSpeed;

        float baseTarget = baseFrequency + (Mathf.PerlinNoise(noiseT, 0f) - 0.5f) * freqJitter * 2f;
        float echoTarget = 1.618f + (Mathf.PerlinNoise(0f, noiseT) - 0.5f) * ratioJitter * 2f;
        baseFrequency = Mathf.Lerp(baseFrequency, baseTarget, Time.deltaTime * 0.5f);
        echoFrequencyRatio = Mathf.Lerp(echoFrequencyRatio, echoTarget, Time.deltaTime * 0.5f);

        float bpmTarget = bpm + (Mathf.PerlinNoise(noiseT * 0.5f, 1f) - 0.5f) * bpmJitter;
        bpm = Mathf.Lerp(bpm, bpmTarget, Time.deltaTime * 0.2f);

        // ✅ Cache energy safely from main thread
        if (reactiveToEnergy && GameManager.Instance != null)
        {
            var player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                lock (energyLock)
                {
                    cachedEnergyPercent = Mathf.Clamp01((float)player.GetEnergy() / player.maxEnergy);
                }
            }
        }
    }
    public void SetGoal(Transform goal)
    {
        goalTransform = goal;
    }


    // 🎵 Procedural Audio (runs on audio thread)
    private void OnAudioRead(float[] data)
    {
        double sr = sampleRate;
        double bpmNow = bpm;
        double baseFreqNow = baseFrequency;

        // ✅ Thread-safe energy read
        float energyPct;
        lock (energyLock)
        {
            energyPct = cachedEnergyPercent;
        }

        if (reactiveToEnergy)
        {
            bpmNow = Mathf.Lerp(bpm * lowEnergyTempoBoost, bpm, energyPct);
            baseFreqNow = Mathf.Lerp(baseFrequency * lowEnergyPitchBoost, baseFrequency, energyPct);
        }

        double echoFreq = baseFreqNow * echoFrequencyRatio;
        double pulseSpeed = bpmNow / 60.0;

        for (int i = 0; i < data.Length; i++)
        {
            double t = (double)samplePosition / sr;

            // Breathing modulation
            double mod = 0.5 + 0.5 * Math.Sin(2 * Math.PI * pulseSpeed * 0.5 * t);
            mod = 1.0 - (modulationDepth * (1.0 - mod));

            double env = 0.6 + 0.4 * Math.Sin(2 * Math.PI * 0.15 * t);

            // Two sine oscillators
            double s1 = Math.Sin(phaseMain);
            double s2 = Math.Sin(phaseEcho);

            phaseMain += 2 * Math.PI * baseFreqNow / sr;
            phaseEcho += 2 * Math.PI * echoFreq / sr;

            if (phaseMain > Math.PI * 2) phaseMain -= Math.PI * 2;
            if (phaseEcho > Math.PI * 2) phaseEcho -= Math.PI * 2;

            double mixed = (s1 * 0.6 + s2 * 0.4) * mod * env;

            // Gentle deterministic noise
            uint seed = (uint)((samplePosition << 13) ^ samplePosition);
            seed = seed * 1664525u + 1013904223u;
            double noise = ((seed & 0xFFFF) / 32768.0 - 1.0) * 0.001;
            mixed += noise;

            data[i] = (float)(mixed * volume);

            samplePosition++;
            if (samplePosition > (long)sr * 3600L * 24L)
                samplePosition = samplePosition % (long)sr;
        }
    }

    // 💥 Neural ping tone + visual
    void PlayPing()
    {
        if (goalTransform == null) return;
        // short audio blip
        var src = gameObject.AddComponent<AudioSource>();
        src.volume = 0.15f;
        src.spatialBlend = 0f;
        int sampleCount = 44100 / 4; // 0.25 sec
        var clip = AudioClip.Create("Ping", sampleCount, 1, 44100, false);
        float[] data = new float[sampleCount];
        float freq = UnityEngine.Random.Range(400f, 1200f);
        float phase = 0;
        for (int i = 0; i < data.Length; i++)
        {
            float t = i / 44100f;
            float env = Mathf.Exp(-t * 6f);
            data[i] = Mathf.Sin(phase) * env;
            phase += 2 * Mathf.PI * freq / 44100f;
        }
        clip.SetData(data, 0);
        src.clip = clip;
        AudioSource.PlayClipAtPoint(GeneratePing(), goalTransform.position, 0.6f);

        // Start visual pulse at goal
        StartCoroutine(PulseVisual());
        src.Play();
        Destroy(src, 1f);

        // visual pulse
        if (pingVisual == null)
        {
            pingVisual = GameObject.CreatePrimitive(PrimitiveType.Quad);
            pingVisual.transform.localScale = Vector3.one * 6f;
            pingVisual.GetComponent<Renderer>().material = new Material(Shader.Find("Unlit/Color"));
            pingVisual.GetComponent<Renderer>().material.color = pingColor;
            pingVisual.name = "NeuralPingVisual";
            Destroy(pingVisual.GetComponent<Collider>());
        }

        StartCoroutine(PulseVisual());
    }
    private AudioClip GeneratePing()
    {
        int sampleRate = 44100;                // Standard audio rate
        int sampleCount = sampleRate / 4;      // 0.25 second ping
        float[] data = new float[sampleCount];

        // Pick a random frequency for variation (between 400–1200 Hz)
        float freq = UnityEngine.Random.Range(400f, 1200f);
        float phase = 0f;

        for (int i = 0; i < data.Length; i++)
        {
            float t = i / (float)sampleRate;

            // Exponential decay envelope (starts loud, fades fast)
            float envelope = Mathf.Exp(-t * 6f);

            // Pure sine wave modulated by envelope
            data[i] = Mathf.Sin(phase) * envelope;

            // Increment phase for the sine
            phase += 2 * Mathf.PI * freq / sampleRate;
        }

        // Create an AudioClip from generated samples
        AudioClip clip = AudioClip.Create("Ping", sampleCount, 1, sampleRate, false);
        clip.SetData(data, 0);

        return clip;
    }

    private IEnumerator PulseVisual()
    {
        if (goalTransform == null) yield break;

        // Create visual if missing
        if (pingVisual == null)
        {
            pingVisual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            pingVisual.name = "NeuralPingVisual";
            Destroy(pingVisual.GetComponent<Collider>());

            var mat = new Material(Shader.Find("Unlit/Color"));
            mat.color = new Color(pingColor.r, pingColor.g, pingColor.b, 0.8f);
            pingVisual.GetComponent<Renderer>().material = mat;
            pingVisual.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            pingVisual.layer = LayerMask.NameToLayer("UI");
            pingVisual.transform.rotation = Quaternion.Euler(0f, 0f, 0f);// ensure visible
        }

        var player = FindObjectOfType<PlayerController>();
        if (player == null) yield break;

        // Start at goal
        pingVisual.transform.position = goalTransform.position + Vector3.up * 0.05f;
        pingVisual.transform.localScale = Vector3.zero;
        pingVisual.SetActive(true);

        // Distance from player to goal affects scale and fade
        float dist = Vector3.Distance(goalTransform.position, player.transform.position);
        float maxScale = Mathf.Clamp(dist * 2f, 3f, 20f);
        float maxAlpha = Mathf.Lerp(1f, 0.3f, Mathf.InverseLerp(0f, 25f, dist));

        // Smooth expand and fade
        float time = 0f;
        var matRef = pingVisual.GetComponent<Renderer>().material;

        while (time < 1f)
        {
            time += Time.deltaTime * 1.2f;
            float t = Mathf.SmoothStep(0f, 1f, time);

            // Expanding circle
            float scale = Mathf.Lerp(0f, maxScale, t);
            pingVisual.transform.localScale = Vector3.one * scale;

            // Always face camera (billboard)
            if (Camera.main != null)
                pingVisual.transform.LookAt(Camera.main.transform);

            // Fade alpha
            float alpha = Mathf.Lerp(maxAlpha, 0f, t);
            matRef.color = new Color(pingColor.r, pingColor.g, pingColor.b, alpha);


            yield return null;
        }

        pingVisual.SetActive(false);
    }


}
