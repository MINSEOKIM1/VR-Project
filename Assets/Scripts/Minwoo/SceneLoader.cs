using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;

    void Start()
    {
        fadeImage.color = new Color(0, 0, 0, 1);
        StartCoroutine(FadeIn());
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeOut(sceneName));
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = 1 - (t / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 0);
    }

    IEnumerator FadeOut(string sceneName)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = t / fadeDuration;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }
}
