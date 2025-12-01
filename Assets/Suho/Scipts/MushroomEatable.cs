using UnityEngine;


public class MushroomEatable : MonoBehaviour
{
    UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    bool _eaten = false;

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

        //TODO: Eating Sound

        Destroy(gameObject);
    }
}
