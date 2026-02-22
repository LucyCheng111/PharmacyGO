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
using UnityEngine.UI;

public class LeaderBoardManager : MonoBehaviour
{
    [HideInInspector] public ScoreManager scoreManager;
    [SerializeField] private Transform leaderboardContentParent;
    [SerializeField] private Transform leaderboardItemPrefab;
    [SerializeField] private ScrollRect scrollView;
    private const int LEADERBOARD_SCENE_INDEX=16;
    private bool isDestroyed;

    //name of the leaderboard in the unity game services backend
    private string leaderboardID = "pharmacy-go-2-leaderboard";

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        Debug.Log("leaderboard object started");

        //delete the placeholder leaderboard entries that are part of the leaderboard scene when the leaderboard object is loaded into the game
        foreach (Transform t in leaderboardContentParent)
        {
            Destroy(t.gameObject);
        }
    }

    private void Awake()
    {
        //subscribe to the OnSceneLoader event from the scene manager in order to trigger an action when the user opens the leaderboard scene
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

        //create a LeaderboardScoresPage object to contain the leaderboard data that is fetched from the unity game services server
        LeaderboardScoresPage leaderboardScoresPage;

        //make a request to unity games services ansd attempt to populate the leaderboard
        try
        {
            leaderboardScoresPage =
                await LeaderboardsService.Instance.GetScoresAsync(leaderboardID);
        }
        //throw an error if that fails for some reason or another
        catch (System.Exception e)
        {
            Debug.LogError("Fetch failed: " + e);
            return;
        }

        // Scene changed while waiting
        if (isDestroyed || leaderboardContentParent == null)
            return;

        // Clear old items safely (primarily for the placeholder leaderboard entry objects that are already in the leaderboard scene)
        for (int i = leaderboardContentParent.childCount - 1; i >= 0; i--)
        {
            Destroy(leaderboardContentParent.GetChild(i).gameObject);
        }

        //make a new leaderboard entry for each player in the leaderboard scores
        foreach (LeaderboardEntry entry in leaderboardScoresPage.Results)
        {
            if (isDestroyed) return;

            //make a new leaderboard entry for this player
            Transform leaderboardItem =
                Instantiate(leaderboardItemPrefab, leaderboardContentParent);

            //add one to the rank since the leaderboard service indexes leaderboard ranks at 0
            int rank = entry.Rank +1;
            leaderboardItem.GetChild(0).GetComponent<TextMeshProUGUI>().text = rank.ToString();

            //get the player's username and chop off the last 5 characters of the string, the "tag".
            string Playername = TruncateString(entry.PlayerName);
            leaderboardItem.GetChild(1).GetComponent<TextMeshProUGUI>().text = Playername;

            //get the player's score
            leaderboardItem.GetChild(2).GetComponent<TextMeshProUGUI>().text = entry.Score.ToString();
            
        }
        
        //force the scrollbar to the top of the conent field
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(leaderboardContentParent.GetComponent<RectTransform>());
        scrollView.verticalNormalizedPosition = 1f;

    }

    string TruncateString(string username)
    //helper funcion to cut off the "tag" at the end of the usernames fetched from the leaderboard
    {
        int charactersToRemove = 5;

        if(username.Length >= charactersToRemove)
        {
            string editedUsername = username.Remove(username.Length - charactersToRemove);
            return editedUsername;
        }
        else
        {
            return username;
        }
    }

}