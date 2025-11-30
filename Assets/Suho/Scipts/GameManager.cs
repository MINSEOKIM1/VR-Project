using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public RunResult CurrentRunResult { get; private set; }

    public int defaultTotalEntries = 12;
    public int defaultMaxHP = 100;

    public int collectedEntries;
    public int totalEntries;

    public int enemyKills;

    public int maxHP;
    public int currentHP;

    float _startTime;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        StartRun(defaultTotalEntries, defaultMaxHP);
        Debug.Log($"[GameManager] StartRun: totalEntries={defaultTotalEntries}, maxHP={defaultMaxHP}");
    }

    public void StartRun(int totalEntries, int maxHP)
    {
        this.totalEntries = totalEntries;
        this.collectedEntries = 0;

        this.enemyKills = 0;

        this.maxHP = maxHP;
        this.currentHP = maxHP;

        _startTime = Time.time;
    }

    public void AddKill()
    {
        enemyKills++;
    }

    public void AddEntry()
    {
        collectedEntries++;
    }

    public void SetHP(int hp)
    {
        currentHP = Mathf.Clamp(hp, 0, maxHP);
    }


    public void SetRunResult(RunResult result)
    {
        CurrentRunResult = result;
    }

    public RunResult BuildRunResult()
    {
        float clearTime = Time.time - _startTime;

        int score = 0;

        float entryRate = totalEntries > 0
            ? (float)collectedEntries / totalEntries
            : 0f;

        if (collectedEntries == totalEntries) score += 40;
        else if (entryRate >= 0.7f) score += 30;
        else if (entryRate >= 0.5f) score += 20;
        else score += 10;

        score += Mathf.Clamp(enemyKills * 2, 0, 30);

        float hpRate = maxHP > 0 ? (float)currentHP / maxHP : 0f;
        if (currentHP == maxHP) score += 20;
        else if (hpRate >= 0.7f) score += 15;
        else if (hpRate >= 0.4f) score += 10;
        else score += 5;

        if (clearTime <= 300f) score += 20;
        else if (clearTime <= 600f) score += 10;
        else score += 5;

        score += enemyKills * 10;

        string rank;
        if (score >= 100) rank = "S";
        else if (score >= 70) rank = "A";
        else if (score >= 50) rank = "B";
        else if (score >= 30) rank = "C";
        else rank = "D";

        var result = new RunResult
        {
            rank = rank,
            collectedEntries = collectedEntries,
            totalEntries = totalEntries,
            clearTime = clearTime,
            enemyKills = enemyKills,
            remainingHP = currentHP,
            maxHP = maxHP,
            score = score
        };

        CurrentRunResult = result;
        return result;
    }


    [ContextMenu("Set Dummy Result")]
    public void SetDummyResult()
    {
        CurrentRunResult = new RunResult
        {
            rank = "B",
            collectedEntries = 3,
            totalEntries = 10,
            clearTime = 523f,
            enemyKills = 7,
            remainingHP = 25,
            maxHP = 100
        };
    }
}
