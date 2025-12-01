using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndTrigger : MonoBehaviour
{
    public string endingSceneName = "EndingScene";
    public GameObject endingCanvas;

    public bool _ended = false;

    void OnTriggerEnter(Collider other)
    {
        if (_ended) return;

        if (!other.CompareTag("Player")) return;

        endingCanvas.SetActive(true);
    }
}
