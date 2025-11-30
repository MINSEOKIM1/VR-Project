using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EndingUIController : MonoBehaviour
{
    [Header("Root")]
    public GameObject rootPanel;

    [Header("Texts")]
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI entriesText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI killsText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI scoreText;

    [Header("Buttons")]
    public Button restartButton;

    [Header("Scene Names")]
    // Restart Scene
    public string explorationSceneName = "SuhoScene";

    void Awake()
    {
        if (rootPanel != null)
            rootPanel.SetActive(false);
    }

    public void ShowEnding(RunResult result)
    {
        if (rootPanel != null)
            rootPanel.SetActive(true);

        if (result == null)
        {
            rankText.text = "-";
            entriesText.text = "COLLECTION: 0 / 0";
            timeText.text = "TIME: 00:00";
            killsText.text = "ENEMIES KILLED: 0";
            hpText.text = "HP: 0 / 0";
            scoreText.text = "SCORE: 0";
        }
        else
        {
            rankText.text = $"{result.rank}";
            entriesText.text = $"COLLECTIONS: {result.collectedEntries} / {result.totalEntries}";
            timeText.text = $"TIME: {FormatTime(result.clearTime)}";
            killsText.text = $"ENEMIES KILLED: {result.enemyKills}";
            hpText.text = $"HP: {result.remainingHP} / {result.maxHP}";
            scoreText.text = $"SCORE: {result.score}";
        }

        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(OnClickRestart);
    }

    string FormatTime(float t)
    {
        int totalSeconds = Mathf.FloorToInt(t);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        return $"{minutes:00}:{seconds:00}";
    }

    void OnClickRestart()
    {
        Debug.Log("[EndingUI] Restart button clicked!");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartRun(
                GameManager.Instance.defaultTotalEntries,
                GameManager.Instance.defaultMaxHP
            );
        }

        SceneManager.LoadScene(explorationSceneName);
    }
}
