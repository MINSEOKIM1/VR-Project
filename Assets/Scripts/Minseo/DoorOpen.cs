using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DoorOpen : MonoBehaviour
{
    public Transform dest;
    public float speed = 0.5f;

    public void OnSocketSelectEntered(SelectEnterEventArgs args)
    {
        var grabbed = args.interactableObject.transform;
        DogamManager.Instance.CollectItem("lock");
        Debug.Log($"소켓에 연결됨: {grabbed.name}");
        StartCoroutine(OpenDoor());
    }

    private IEnumerator OpenDoor()
    {
        while (true)
        {
            transform.position = Vector3.Lerp(transform.position, dest.position, speed * Time.deltaTime);
            yield return null;

            if (Vector3.Distance(transform.position, dest.position) < 0.1f)
            {
                transform.position = dest.position;
                break;
            }
        }
    }
}
