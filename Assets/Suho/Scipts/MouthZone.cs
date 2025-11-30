using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MouthZone : MonoBehaviour
{
    [Header("Filter (Optional)")]
    [SerializeField] bool useLayerFilter = false;
    [SerializeField] LayerMask eatableLayers = ~0;

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    bool IsInEatableLayer(GameObject obj)
    {
        if (!useLayerFilter) return true;
        return (eatableLayers.value & (1 << obj.layer)) != 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsInEatableLayer(other.gameObject)) return;

        var eatable = other.GetComponentInParent<MushroomEatable>();
        if (eatable != null)
        {
            eatable.Eat();
        }
    }
}
