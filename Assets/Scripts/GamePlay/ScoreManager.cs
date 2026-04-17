using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{

    // Manages score and points

    /*
     * To ensure only one ScoreManager in the entire game
     * get --- allow other scripts to read the Instance
     * private set --- Only ScoreManager can assign value
     */
    public static ScoreManager Instance { get; private set; }
    public static event Action<int> OnScoreChanged;

    private int scoreCount;
    private int questionValue = 100;
    private int difficultyBonus;
    private int aiRivalScore;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Load saved score
            scoreCount = PlayerPrefs.GetInt("ScoreCount", 0);
            aiRivalScore = PlayerPrefs.GetInt("AiRivalScore", 0);  // Load AI score
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(bool question, int bonusValue = 0)
    {
        if (!question)
        {
            scoreCount += bonusValue * TimerManager.Instance.GetMultiplier();
        }

        else
        {
            int difficulty = MapArea.i.GetDifficulty();

            if (difficulty <= 20)
            {
                difficultyBonus = 1;
            }
            else if (difficulty <= 40)
            {
                difficultyBonus = 2;
            }
            else if (difficulty <= 60)
            {
                difficultyBonus = 3;
            }
            else if (difficulty <= 80)
            {
                difficultyBonus = 4;
            }
            else
            {
                difficultyBonus = 5;
            }
            // make sure it's at least 1
            int multiplier = Mathf.Max(1, TimerManager.Instance.GetMultiplier());
            Debug.Log($"Scoring breakdown — difficulty: {difficulty}, difficultyBonus: {difficultyBonus}, multiplier: {multiplier}, streak: {MapArea.i.GetCorrectStreak() + 1}");


            {
                scoreCount += questionValue * (MapArea.i.GetCorrectStreak() + 1) * difficultyBonus * multiplier;
            }
        }
        
        Debug.Log("AddScore called. Question=" + question + " Bonus=" + bonusValue);
            // Save scores
            PlayerPrefs.SetInt("ScoreCount", scoreCount);
            OnScoreChanged?.Invoke(scoreCount);
            PlayerPrefs.Save();
        Debug.Log("Score: " + scoreCount);
    }

    public int GetScoreCount()
    {
        return scoreCount;
    }

    public int AddAiScore()
    {
        int previous = aiRivalScore;

        int difficulty = MapArea.i.GetDifficulty();
        int diffBonus = difficulty <= 20 ? 1 :
                        difficulty <= 40 ? 2 :
                        difficulty <= 60 ? 3 :
                        difficulty <= 80 ? 4 : 5;

        // make sure it's at least 1
        int multiplier = Mathf.Max(1, TimerManager.Instance.GetMultiplier());

        aiRivalScore += questionValue * (MapArea.i.GetCorrectStreak() + 1) * diffBonus * multiplier;

        PlayerPrefs.SetInt("AiRivalScore", aiRivalScore);

        return aiRivalScore - previous; // return points earned this round
    }

    //adds hard value of score
    public void AddMinigameScore(int score)
    {
        scoreCount += score;
        PlayerPrefs.SetInt("ScoreCount", scoreCount);
        OnScoreChanged?.Invoke(scoreCount);
        PlayerPrefs.Save();
    }

    public int GetAiRivalScore()
    {
        return aiRivalScore;
    }

    public void ResetAiRivalScore()
    {
        aiRivalScore = 0;
        PlayerPrefs.SetInt("AiRivalScore", 0);
        PlayerPrefs.Save();
    }
}

