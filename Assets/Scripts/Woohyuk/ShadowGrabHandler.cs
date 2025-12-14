using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ShadowGrabHandler : MonoBehaviour
{
    public XRGrabInteractable grab;
    public DogamManager dogamManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        dogamManager.CollectItem("crystal");
        GuidanceSystem.Instance.DoneWithItem(5);
    }
    private void OnRelease(SelectExitEventArgs args) {}
}
