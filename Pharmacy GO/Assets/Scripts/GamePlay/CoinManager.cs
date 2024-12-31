using UnityEngine;

public class CoinManager : MonoBehaviour
{
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
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCoin(int amount)
    {
        coinCount += amount;
        Debug.Log("Coins: " + coinCount);
    }

    public int GetCoinCount()
    {
        return coinCount;
    }
}

