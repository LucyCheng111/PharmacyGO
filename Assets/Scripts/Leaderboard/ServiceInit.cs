using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;

public class ServicesInitializer : MonoBehaviour
{
    public static bool IsReady { get; private set; }

    //signs user into the unity leaderboard services when they start the program for the first time
    private async void Awake()
    {
        if (IsReady) return;

        DontDestroyOnLoad(gameObject);

        await InitializeServices();
    }

    private async Task InitializeServices()
    {
        try
        {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            IsReady = true;

            Debug.Log("Unity Services Initialized");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Services Init Failed: " + e);
        }
    }
}