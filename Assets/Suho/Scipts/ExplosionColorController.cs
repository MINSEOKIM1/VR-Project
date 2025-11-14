using UnityEngine;

public class ExplosionColorController : MonoBehaviour
{
    public float colorMultiplier = 1.3f;

    public string colorPropertyName = "_Color";

    public void Initialize(Color baseColor)
    {
        Color explosionColor = baseColor * colorMultiplier;
        explosionColor.a = 1f;
        ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in particleSystems)
        {
            var main = ps.main;
            main.startColor = explosionColor;
        }

        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
        {
            if (r.sharedMaterial != null && r.sharedMaterial.HasProperty(colorPropertyName))
            {
                Material mat = r.material;
                mat.SetColor(colorPropertyName, explosionColor);
            }
        }
    }
}
