using UnityEngine;

public class user_scoreboard : MonoBehaviour
{
    public static user_scoreboard Instance { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate User Scoreboard found! Destroying extra instance.");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
