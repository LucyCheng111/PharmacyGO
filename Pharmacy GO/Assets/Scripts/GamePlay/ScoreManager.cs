using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    /*
     * To ensure only one ScoreManager in the entire game
     * get --- allow other scripts to read the Instance
     * private set --- Only ScoreManager can assign value
     */
    public static ScoreManager Instance { get; private set; }

    private int scoreCount;
    private int questionValue = 100;
    private int difficultyBonus;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Load saved score
            scoreCount = PlayerPrefs.GetInt("ScoreCount", 0);
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

            if (difficulty <= 30)
            {
                difficultyBonus = 1;
            }
            else if (difficulty < 70)
            {
                difficultyBonus = 3;
            }
            else
            {
                difficultyBonus = 5;
            }

            {
                scoreCount += questionValue * (MapArea.i.GetCorrectStreak() + 1) * difficultyBonus * TimerManager.Instance.GetMultiplier();
            }
        }
        

            // Save scores
            PlayerPrefs.SetInt("ScoreCount", scoreCount);
        Debug.Log("Score: " + scoreCount);
    }

    public int GetScoreCount()
    {
        return scoreCount;
    }
}

