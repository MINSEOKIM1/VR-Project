using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MushroomGlow : MonoBehaviour
{
    [Header("Trigger or Collision")]
    [SerializeField] bool useTrigger = true;
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

    static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");
    Renderer[] rends;
    MaterialPropertyBlock mpb;
    Coroutine pulseCo;
    
    void Start()
    {
        TriggerGlow();
    }

    void Reset()
    {
        var col = GetComponent<Collider>();
        if (col) useTrigger = col.isTrigger;
    }

    void Awake()
    {
        EnsureKinematicRigidbody();

        rends = GetComponentsInChildren<Renderer>(true);
        mpb = new MaterialPropertyBlock();

        foreach (var r in rends)
        {
            if (!r) continue;
            r.GetPropertyBlock(mpb);
            mpb.SetColor(EmissionColorID, baseEmission);
            r.SetPropertyBlock(mpb);
            foreach (var m in r.sharedMaterials.Where(m => m != null))
                m.EnableKeyword("_EMISSION");
        }

        if (pulseLight)
        {
            pulseLight.intensity = 0f;
            pulseLight.range = lightRadius;
        }
    }

    void EnsureKinematicRigidbody()
    {
        var rb = GetComponent<Rigidbody>();
        if (!rb) rb = gameObject.AddComponent<Rigidbody>();

        rb.useGravity = false;
        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    bool IsInReactLayers(GameObject other)
    {
        return (reactLayers.value & (1 << other.layer)) != 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!useTrigger) return;
        TriggerGlow();
    }

    void OnCollisionEnter(Collision other)
    {
        Debug.Log($"[Mushroom] CollisionEnter with {other.gameObject.name}, layer={LayerMask.LayerToName(other.gameObject.layer)}, contacts={other.contactCount}");
        if (useTrigger) return;
        if (IsInReactLayers(other.collider.gameObject))
            TriggerGlow();
    }

    void TriggerGlow()
    {
        if (pulseCo != null) StopCoroutine(pulseCo);
        pulseCo = StartCoroutine(GlowPulse());
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
        }
    }

    void SetLight(float intensity)
    {
        if (pulseLight) pulseLight.intensity = intensity;
    }
}
