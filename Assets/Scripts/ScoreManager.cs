using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Serialization;
public enum Minigame
{
    LetterSelect,
    ColourMatch,
    Maze,
    DiceSorting,
}

public class ScoreManager : NetworkBehaviour
{
    [FormerlySerializedAs("m_ShowDebugMessages")]
    [Tooltip("This will enable verbose debug messages in the Unity Editor console")]
    public bool showDebugMessages;

    [SyncVar]
    public int maxTimeSeconds = 120;

    [SyncVar]
    [SerializeField] 
    float currentTimeSeconds = 0;


    private bool isCountdownActive;
    public float CurrentTimeMinutesOnly { get { return Mathf.Floor(currentTimeSeconds / 60); } }
    public float CurrentTimeSecondsOnly { get { return Mathf.Floor(currentTimeSeconds % 60); } }


    private float levelStartTime;
    private int levelsCleared;

    [SyncVar]
    private int score;
    public int Score { get { return score; } }

    public static ScoreManager singleton { get; private set; }

    public override void OnStartClient()
    {
        base.OnStartClient();
        InitializeSingleton();
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        InitializeSingleton();
    }

    bool InitializeSingleton()
    {
        Debug.Log("Initialising Score manager");
        if (singleton != null && singleton == this) return true;

        LogFilter.Debug = showDebugMessages;
        if (LogFilter.Debug)
        {
            LogFactory.EnableDebugMode();
        }

        if (singleton != null)
        {
            Destroy(gameObject);
            return false;
        }

        singleton = this;
        if (Application.isPlaying) DontDestroyOnLoad(gameObject);

        Debug.Log("Finish initialisation");
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer) {
            if (isCountdownActive) {
                currentTimeSeconds -= Time.deltaTime;
                if (currentTimeSeconds < 0)
                {
                    currentTimeSeconds = 0;
                    PauseCountdown();
                    //Maybe trigger event when this happens?
                }
            }
        }
    }

    [Server]
    //Adds the given amount of time to the timer
    public void AddTimeSeconds(float seconds)
    {
        currentTimeSeconds += seconds;
        if (currentTimeSeconds > maxTimeSeconds) currentTimeSeconds = maxTimeSeconds;
    }

    [Server]
    //Deducts the given value from the current score
    public void ScorePenalty(int scoreToDeduct) 
    {
        score -= scoreToDeduct;
        if (score < 0) score = 0;
    }

    [Server]
    //Call at the start of every minigame
    public void StartMinigame()
    {
        levelStartTime = currentTimeSeconds;
        ResumeCountdown();
    }

    [Server]
    //Call when a minigame is completed
    public void MinigameComplete(Minigame gameType)
    {
        PauseCountdown();
        int baseScore;
        int extraTimeSeconds;
        float maxScoreClearTime;
        float minScoreClearTime;
        float TimeBonusMax;
        switch (gameType)
        {
            case Minigame.LetterSelect:
                baseScore = 200;
                extraTimeSeconds = 5;
                maxScoreClearTime = 7;
                minScoreClearTime = 15;
                TimeBonusMax = 200;
                break;
            default:
                baseScore = 100;
                extraTimeSeconds = 10;
                maxScoreClearTime = 5;
                minScoreClearTime = 10;
                TimeBonusMax = 100;
                break;
        }
        float t = 1-Mathf.Clamp(((levelStartTime - currentTimeSeconds) - maxScoreClearTime) / (minScoreClearTime - maxScoreClearTime), 0, 1);
        //Debug.Log(t);
        float timeBonus = Mathf.Lerp(0, TimeBonusMax, t);
        //Debug.Log("TimeBonus: " + timeBonus);
        score += baseScore+Mathf.FloorToInt(timeBonus);
        //Debug.Log("Score: " + score);
        levelsCleared++;
        AddTimeSeconds(extraTimeSeconds);

    }

    [Server]
    public void ResetGame()
    {
        ResetCountdown();
        levelStartTime = 0;
        levelsCleared = 0;
        score = 0;
    }

    public string GetTimeRemaining()
    {
        string str = string.Format("{0:00}:{1:00}", CurrentTimeMinutesOnly, CurrentTimeSecondsOnly);
        //Debug.Log(str);
        return str;
    }
    
    public float GetTimeRemainingSeconds()
    {
        return currentTimeSeconds;
    }

    [Server]
    void StartCountdown()
    {
        if (!isCountdownActive)
        {
            ResetCountdown();
            ResumeCountdown();
        }
    }
    [Server]
    void PauseCountdown()
    {
        isCountdownActive = false;
    }
    [Server]
    void ResumeCountdown()
    {
        isCountdownActive = true;
    }
    [Server]
    void ResetCountdown()
    {
        PauseCountdown();
        currentTimeSeconds = maxTimeSeconds;
    }
}
