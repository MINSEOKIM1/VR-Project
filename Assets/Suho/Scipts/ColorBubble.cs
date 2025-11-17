using System.Linq;
using UnityEngine;

public class ColorBubble : MonoBehaviour
{
    [Header("Bubble Settings")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionLifetime = 3f;

    [Header("Inner Particles")]
    [SerializeField] private ParticleSystem innerParticleSystem;
    [SerializeField] private float innerParticleExtraLifetime = 3f;

    [Header("Color Settings (HSV)")]
    [Range(0f, 1f)] public float minHue = 0.0f;
    [Range(0f, 1f)] public float maxHue = 1.0f;
    [Range(0f, 1f)] public float minSaturation = 0.7f;
    [Range(0f, 1f)] public float maxSaturation = 1.0f;
    [Range(0f, 1f)] public float minValue = 0.1f;
    [Range(0f, 1f)] public float maxValue = 0.3f;

    [Header("Float Settings")]
    [SerializeField] private Vector2 bobSpeedRange = new Vector2(0.15f, 0.35f);
    [SerializeField] private Vector2 bobAmpRange = new Vector2(0.03f, 0.07f);
    [SerializeField] private Vector2 driftSpeedRange = new Vector2(0.05f, 0.12f);
    [SerializeField] private Vector2 driftAmpRange = new Vector2(0.01f, 0.03f);
    [SerializeField] private Vector2 rotSpeedRange = new Vector2(2f, 8f);
    [SerializeField] private bool useLocalBase = false;

    private bool popped = false;
    private Color currentInnerColor;

    private static readonly int InnerColorID = Shader.PropertyToID("_InnerColor");
    private MaterialPropertyBlock mpb;
    private Renderer[] bubbleRenderers;

    private Vector3 basePosition;
    private float bobSpeed, bobAmp;
    private float driftSpeed, driftAmp;
    private float rotSpeed;
    private float phaseY;
    private Vector2 noiseSeed;

    private void Start()
    {
        basePosition = useLocalBase ? transform.localPosition : transform.position;

        bubbleRenderers = GetComponentsInChildren<Renderer>(true);

        float h = Random.Range(minHue, maxHue);
        float s = Random.Range(minSaturation, maxSaturation);
        float v = Random.Range(minValue, maxValue);

        currentInnerColor = Color.HSVToRGB(h, s, v);
        currentInnerColor.a = 1f;

        if (mpb == null) mpb = new MaterialPropertyBlock();
        foreach (var r in bubbleRenderers)
        {
            if (r == null) continue;
            r.GetPropertyBlock(mpb);
            if (r.sharedMaterials.Any(m => m != null && m.HasProperty(InnerColorID)))
            {
                mpb.SetColor(InnerColorID, currentInnerColor);
                r.SetPropertyBlock(mpb);
            }
        }

        if (innerParticleSystem != null)
        {
            var main = innerParticleSystem.main;
            main.startColor = currentInnerColor;

            Debug.Log("[Bubble] Inner Particle System initiated.");

            innerParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        else
        {
            Debug.LogWarning("[Bubble] Inner Particle System not assigned.", this);
        }

        bobSpeed = Random.Range(bobSpeedRange.x, bobSpeedRange.y);
        bobAmp = Random.Range(bobAmpRange.x, bobAmpRange.y);
        driftSpeed = Random.Range(driftSpeedRange.x, driftSpeedRange.y);
        driftAmp = Random.Range(driftAmpRange.x, driftAmpRange.y);
        rotSpeed = Random.Range(rotSpeedRange.x, rotSpeedRange.y);
        phaseY = Random.Range(0f, Mathf.PI * 2f);
        noiseSeed = new Vector2(Random.value * 1000f, Random.value * 1000f);
    }

    private void Update()
    {
        if (!popped)
            FloatAnimation();
    }

    private void FloatAnimation()
    {
        float t = Time.time;

        float yOffset = Mathf.Sin(t * bobSpeed + phaseY) * bobAmp;

        float px = (Mathf.PerlinNoise(noiseSeed.x + t * driftSpeed, noiseSeed.y) - 0.5f) * 2f;
        float pz = (Mathf.PerlinNoise(noiseSeed.x, noiseSeed.y + t * driftSpeed) - 0.5f) * 2f;
        float xOffset = px * driftAmp;
        float zOffset = pz * driftAmp;

        Vector3 offset = new Vector3(xOffset, yOffset, zOffset);

        if (useLocalBase)
            transform.localPosition = basePosition + offset;
        else
            transform.position = basePosition + offset;

        transform.Rotate(0f, rotSpeed * Time.deltaTime, 0f, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        Pop();
    }

    private void Pop()
    {
        if (popped) return;
        popped = true;

        bubbleRenderers = GetComponentsInChildren<Renderer>(true)
            .Where(r => r.GetComponent<ParticleSystem>() == null)
            .ToArray();


        var col = GetComponent<Collider>();
        if (col == null) col = GetComponentInChildren<Collider>();
        if (col != null) col.enabled = false;

        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            var colorController = explosion.GetComponent<ExplosionColorController>();
            if (colorController != null)
                colorController.Initialize(currentInnerColor);

            Destroy(explosion, explosionLifetime);
        }

        if (innerParticleSystem != null)
        {
            Color.RGBToHSV(currentInnerColor, out float h, out float s, out float v);

            float desaturateAmount = 0.6f;
            s = Mathf.Lerp(s, 0.3f, desaturateAmount);

            v *= 0.8f;

            Color dustColor = Color.HSVToRGB(h, s, v);
            dustColor.a = currentInnerColor.a * 0.6f;

            var particleInstance = Instantiate(innerParticleSystem, transform.position, Quaternion.identity);
            var main = particleInstance.main;
            main.startColor = dustColor;
            particleInstance.Play(true);

            float life = main.startLifetime.constantMax;
            Destroy(particleInstance.gameObject, life + innerParticleExtraLifetime);
        }
        else
        {
            Debug.LogWarning("[Bubble] Inner Particle System is null — cannot play.", this);
        }

        Destroy(gameObject);
    }
}
