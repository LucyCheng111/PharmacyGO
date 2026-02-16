using UnityEngine;
using Unity.Services.Leaderboards;
using System.Threading.Tasks;
using Unity.Services.Authentication;

public class LeaderboardUploader : MonoBehaviour
{
    private const string leaderboardID = "pharmacy-go-2-leaderboard";

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        ScoreManager.OnScoreChanged += UploadScore;
    }

    private void OnDisable()
    {
        ScoreManager.OnScoreChanged -= UploadScore;
    }

    private async void UploadScore(int score)
    {
        //make sure services are initialized
        while (!ServicesInitializer.IsReady)
        {
            await Task.Yield();
        }

        while (!AuthenticationService.Instance.IsSignedIn)
            await Task.Yield();

        try
        {
            await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardID,score);
            Debug.Log("Uploaded: " + score);
        }catch(System.Exception e){
            Debug.LogError(e);
        }
    }
}