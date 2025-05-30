using UnityEngine;

public class TimerManager : MonoBehaviour
{

    // Manages the timer and timer-related bonuses

    // we use two variables to prevent timer issues when pausing and unpausing the game
    private bool levelStarted = false;
    private bool timerRunning = false;
    private float currentTime = 0f;

    public static TimerManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // load saved time?
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (levelStarted && timerRunning)
            currentTime += Time.deltaTime;
    }

    public void StartTimer()
    {
        levelStarted = true;
        timerRunning = true;
    }

    public void StopTimer()
    {
        levelStarted = false;
        timerRunning = false;
    }

    public void PauseTimer()
    {
        timerRunning = false;
    }

    public void ResumeTimer()
    {
        timerRunning = true;
    }

    public void ResetTimer()
    {
        levelStarted = false;
        timerRunning = false;
        currentTime = 0f;
    }

    public float GetTime()
    {
        return currentTime;
    }

    public int GetMultiplier()
    {
        if (!levelStarted) // if level is not active, skip
            return 0;

        if (currentTime < 300f) // 5 minutes
            return 10;

        else if (currentTime < 480f) // 8 minutes
            return 5;

        else if (currentTime < 780f) // 13 minutes
            return 3;

        else if (currentTime < 1200f) // 20 minutes
            return 2;

        else if (currentTime < 1800f) // 30 minutes
            return 1;

        else
            return 0;
    }

    public bool IsLevelStarted()
    {
        return levelStarted;
    }
}
