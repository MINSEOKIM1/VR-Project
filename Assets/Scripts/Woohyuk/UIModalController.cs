using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UIModalController : MonoBehaviour
{
    [SerializeField] Button closeButton;
    [SerializeField] CanvasGroup canvasGroup; // optional
    public UnityEvent buttonEvent;
    public string endingSceneName;
    
    public void OnClickClose()
    {
        // 중복 클릭 방지
        closeButton.interactable = false;

        if (!string.IsNullOrEmpty(endingSceneName))
        {
            SceneManager.LoadScene(endingSceneName);
        }
    }

    private void OnDestroy()
    {
        // 메모리 누수 방지
        closeButton.onClick.RemoveListener(OnClickClose);
    }
}
