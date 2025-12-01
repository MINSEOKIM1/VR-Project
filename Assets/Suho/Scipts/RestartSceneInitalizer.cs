using UnityEngine;

public class RestartSceneInitializer : MonoBehaviour
{
    public EndingUIController endingUI;

    void Start()
    {
        if (endingUI == null)
        {
            Debug.LogError("[RestartSceneInitializer] EndingUIController reference is missing!");
            return;
        }

        RunResult result = null;

        if (GameManager.Instance != null)
        {
            result = GameManager.Instance.BuildRunResult();
        }
        else
        {
            Debug.LogWarning("[RestartSceneInitializer] No GameManager result, using dummy.");
        }

        endingUI.ShowEnding(result);
    }
}
