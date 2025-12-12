using UnityEngine;


public class MushroomEatable : MonoBehaviour
{
    UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    bool _eaten = false;

    [Header("Sound")]
    [SerializeField] private AudioClip eatClip;
    [Range(0f, 1f)] public float eatVolume = 1f;

    void Awake()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    public void Eat()
    {
        if (_eaten) return;
        
        DogamManager.Instance.CollectItem("mushroom");

        if (grab != null && !grab.isSelected) return;
        _eaten = true;

        PlayEatAudio();

        Destroy(gameObject);
    }

    void PlayEatAudio()
    {
        if (eatClip == null) return;

        AudioSource.PlayClipAtPoint(eatClip, transform.position, eatVolume);
    }
}
