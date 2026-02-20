using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.Leaderboards;
using TMPro;
using Unity.Services.Leaderboards.Models;
using UnityEngine.SceneManagement;
using Unity.Services.Leaderboards.Exceptions;
using Unity.Services.Authentication;

public class LeaderBoardManager : MonoBehaviour
{
    [HideInInspector] public ScoreManager scoreManager;
    [SerializeField] private Transform leaderboardContentParent;
    [SerializeField] private Transform leaderboardItemPrefab;
    private bool isDestroyed;
    private string leaderboardID = "pharmacy-go-2-leaderboard";

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        Debug.Log("leaderboard object started");


        foreach (Transform t in leaderboardContentParent)
        {
            Destroy(t.gameObject);
        }
    }

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "Leaderboard")
        {
            Debug.Log("leaderboard scene loaded");
            UpdateLeaderboard();
        }
    }
    private void OnDestroy()
    {
        isDestroyed = true;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private async void UpdateLeaderboard()
    {

        Debug.Log("-- Fetching data");
        // Wait for services
        while (!ServicesInitializer.IsReady)
        {
            if (isDestroyed) return;
            await Task.Yield();
        }

        while (!AuthenticationService.Instance.IsSignedIn)
        {
            if (isDestroyed) return;
            await Task.Yield();
        }

        LeaderboardScoresPage leaderboardScoresPage;

        try
        {
            leaderboardScoresPage =
                await LeaderboardsService.Instance.GetScoresAsync(leaderboardID);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Fetch failed: " + e);
            return;
        }

        // Scene changed while waiting
        if (isDestroyed || leaderboardContentParent == null)
            return;

        // Clear old items safely
        for (int i = leaderboardContentParent.childCount - 1; i >= 0; i--)
        {
            Destroy(leaderboardContentParent.GetChild(i).gameObject);
        }

        foreach (LeaderboardEntry entry in leaderboardScoresPage.Results)
        {
            if (isDestroyed) return;

            Transform leaderboardItem =
                Instantiate(leaderboardItemPrefab, leaderboardContentParent);

            leaderboardItem.GetChild(0)
                .GetComponent<TextMeshProUGUI>().text =
                entry.Rank.ToString();

            leaderboardItem.GetChild(1)
                .GetComponent<TextMeshProUGUI>().text =
                entry.PlayerName;

            leaderboardItem.GetChild(2)
                .GetComponent<TextMeshProUGUI>().text =
                entry.Score.ToString();
        }
    }

}