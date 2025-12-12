using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MushroomGlow : MonoBehaviour
{
    [Header("Reaction Filter (Optional)")]
    [SerializeField] bool useLayerFilter = false;
    [SerializeField] LayerMask reactLayers = ~0;

    [Header("Emission")]
    [ColorUsage(true, true)] public Color baseEmission = Color.black;
    [ColorUsage(true, true)] public Color hitEmission = new Color(5f, 3f, 0.5f);
    public float riseTime = 0.08f, holdTime = 0.12f, decayTime = 0.6f;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Optional Light")]
    public Light pulseLight;
    public float lightMaxIntensity = 6f;
    public float lightRadius = 2.5f;

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip glowClip;
    [Range(0f, 1f)] public float glowVolume = 1f;

    static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");
    Renderer[] rends;
    MaterialPropertyBlock mpb;
    Coroutine pulseCo;

    [Header("Trigger Limit")]
    [SerializeField] bool useTriggerCooldown = true;
    [SerializeField] float minTriggerInterval = 1.0f;

    float lastTriggerTime = -999f;

    void Awake()
    {
        rends = GetComponentsInChildren<Renderer>(true);
        mpb = new MaterialPropertyBlock();

        foreach (var r in rends)
        {
            if (!r) continue;

            r.GetPropertyBlock(mpb);
            mpb.SetColor(EmissionColorID, baseEmission);
            r.SetPropertyBlock(mpb);

            var mat = r.material;
            if (mat != null)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor(EmissionColorID, baseEmission);
            }
        }

        if (pulseLight)
        {
            pulseLight.intensity = 0f;
            pulseLight.range = lightRadius;
        }
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
        audioSource.minDistance = 0.5f;
        audioSource.maxDistance = 10f;

    }

    bool IsInReactLayers(GameObject other)
    {
        if (!useLayerFilter) return true;
        return (reactLayers.value & (1 << other.layer)) != 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsInReactLayers(other.gameObject)) return;
        TriggerGlow();
    }

    void OnCollisionEnter(Collision other)
    {
        Debug.Log($"[Mushroom] CollisionEnter with {other.gameObject.name}, " +
                  $"layer={LayerMask.LayerToName(other.gameObject.layer)}, contacts={other.contactCount}");
        if (other.gameObject.layer == LayerMask.NameToLayer("Terrain"))
            return;
        if (!IsInReactLayers(other.collider.gameObject)) return;
        TriggerGlow();
    }

    void TriggerGlow()
    {
        if (useTriggerCooldown)
        {
            if (Time.time - lastTriggerTime < minTriggerInterval)
            {
                return;
            }

            lastTriggerTime = Time.time;
        }

        PlayGlowAudio();

        if (pulseCo != null) StopCoroutine(pulseCo);
        pulseCo = StartCoroutine(GlowPulse());
    }

    void PlayGlowAudio()
    {
        if (glowClip == null) return;

        if (audioSource != null)
            audioSource.PlayOneShot(glowClip, glowVolume);
        else
            AudioSource.PlayClipAtPoint(glowClip, transform.position, glowVolume);
    }

    System.Collections.IEnumerator GlowPulse()
    {
        // Up
        for (float t = 0f; t < riseTime; t += Time.deltaTime)
        {
            float k = curve.Evaluate(t / Mathf.Max(0.0001f, riseTime));
            SetEmission(Color.Lerp(baseEmission, hitEmission, k));
            SetLight(Mathf.Lerp(0f, lightMaxIntensity, k));
            yield return null;
        }
        SetEmission(hitEmission);
        SetLight(lightMaxIntensity);

        yield return new WaitForSeconds(holdTime);

        // Down
        for (float t = 0f; t < decayTime; t += Time.deltaTime)
        {
            float k = curve.Evaluate(t / Mathf.Max(0.0001f, decayTime));
            SetEmission(Color.Lerp(hitEmission, baseEmission, k));
            SetLight(Mathf.Lerp(lightMaxIntensity, 0f, k));
            yield return null;
        }
        SetEmission(baseEmission);
        SetLight(0f);
    }

    void SetEmission(Color c)
    {
        foreach (var r in rends)
        {
            if (!r) continue;

            r.GetPropertyBlock(mpb);
            mpb.SetColor(EmissionColorID, c);
            r.SetPropertyBlock(mpb);

            var mat = r.material;
            if (mat != null)
            {
                mat.SetColor(EmissionColorID, c);
                mat.EnableKeyword("_EMISSION");
            }
        }
    }

    void SetLight(float intensity)
    {
        if (pulseLight) pulseLight.intensity = intensity;
    }
}
