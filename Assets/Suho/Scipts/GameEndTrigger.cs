using UnityEngine;

public class GameEndTrigger : MonoBehaviour
{
    public EndingUIController endingController;
    bool _ended = false;

    void OnTriggerEnter(Collider other)
    {
        if (_ended) return;

        //if (!other.CompareTag("Player")) return;

        _ended = true;

        RunResult result;

        if (GameManager.Instance != null)
        {
            result = GameManager.Instance.BuildRunResult();
        }
        else
        {
            Debug.LogWarning("[GameEndTrigger] No GameManager.Instance exists");
            result = new RunResult
            {
                rank = "B",
                collectedEntries = 3,
                totalEntries = 8,
                clearTime = 250f,
                enemyKills = 5,
                remainingHP = 30,
                maxHP = 100
            };
        }

        if (endingController != null)
        {
            endingController.ShowEnding(result);
        }
        else
        {
            Debug.LogWarning("[GameEndTrigger] No endingController Inspector exists");
        }

    }
}
