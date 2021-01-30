using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum Minigame
{
    SymbolMatch,
    Maze,
}

public class ScoreManager : NetworkBehaviour
{
    public int maxTimeSeconds = 120;
    [SerializeField] float currentTimeSeconds = 0;

    public bool isCountdownActive { get; private set; }
    private float CurrentTimeMinutesOnly { get { return Mathf.Floor(currentTimeSeconds / 60); } }
    private float CurrentTimeSecondsOnly { get { return Mathf.Floor(currentTimeSeconds % 60); } }


    private float levelStartTime;
    public int levelsCleared { get; private set; }
    public int score { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        ResetGame();

        //testing (example use of functions)
        StartMinigame(); 
        StartCountdown();
        InvokeRepeating("GetTimeRemaining", 0, 1);

        Invoke("TestGameComplete", 7);
    }

    //TempTest function
    void TestGameComplete()
    {
        MinigameComplete(Minigame.SymbolMatch);
    }

    // Update is called once per frame
    void Update()
    {
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

    public void AddTimeSeconds(float seconds)
    {
        currentTimeSeconds += seconds;
        if (currentTimeSeconds > maxTimeSeconds) currentTimeSeconds = maxTimeSeconds;
    }

    public void StartCountdown()
    {
        if (!isCountdownActive)
        {
            ResetCountdown();
            ResumeCountdown();
        }
    }

    public void PauseCountdown()
    {
        isCountdownActive = false;
        if (IsInvoking("DecrementTimer"))
        {
            CancelInvoke("DecrementTimer");
        }
    }

    public void ResumeCountdown()
    {
        isCountdownActive = true;
    }

    void ResetCountdown()
    {
        PauseCountdown();
        currentTimeSeconds = maxTimeSeconds;
    }

    //Call at the start of every minigame
    public void StartMinigame()
    {
        levelStartTime = currentTimeSeconds;
    }

    //Call when a minigame is completed
    public void MinigameComplete(Minigame gameType)
    {
        int baseScore;
        int extraTimeSeconds;
        float maxScoreClearTime;
        float minScoreClearTime;
        float TimeBonusMax;
        switch (gameType)
        {
            case Minigame.SymbolMatch:
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
        Debug.Log(str);
        return str;
    }
}
