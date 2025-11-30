using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingCutsceneController : MonoBehaviour
{
    public Animator shipAnimator;
    public string nextSceneName = "RestartScene";
    public float delayAfterLanding = 1.0f;
    public CanvasGroup fadeCanvas; 

    void Start()
    {
        shipAnimator.Play("TakeOff");
    }

    public void OnTakeOffFinished()
    {
        StartCoroutine(WaitAndLoad());
    }

    private IEnumerator WaitAndLoad()
    {
        yield return new WaitForSeconds(delayAfterLanding);

        if (fadeCanvas != null)
        {
            float t = 0f;
            float duration = 1f;
            while (t < duration)
            {
                t += Time.deltaTime;
                fadeCanvas.alpha = t / duration;
                yield return null;
            }
        }

        SceneManager.LoadScene(nextSceneName);
    }
}
