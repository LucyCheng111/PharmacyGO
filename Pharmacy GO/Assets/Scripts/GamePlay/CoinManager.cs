using UnityEngine;

public class CoinManager : MonoBehaviour
{

    // Manages and tracks coins earned

    /*
     * To ensure only one CoinManager in the entire game
     * get --- allow other scripts to read the Instance
     * private set --- Only CoinManager can assign value
     */
    public static CoinManager Instance { get; private set; }

    private int coinCount;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Load saved coins
            coinCount = PlayerPrefs.GetInt("CoinCount", 0);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCoin(int amount)
    {
        coinCount += amount;

        // Save earned coins
        PlayerPrefs.SetInt("CoinCount", coinCount);
        PlayerPrefs.Save();
    }

    public int GetCoinCount()
    {
        return coinCount;
    }

    public void RemoveCoin(int amount)
    {
        coinCount -= amount;

        // Saved spent coins
        PlayerPrefs.SetInt("CoinCount", coinCount);
        PlayerPrefs.Save();
    }
}

