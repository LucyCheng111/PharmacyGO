using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HubIntro : MonoBehaviour
{
    [SerializeField] Dialog introDialog;

    private const string PREF_KEY = "HubIntroPlayed";

    // if want to see intro, go to debug->reset hubintro
#if UNITY_EDITOR
    [MenuItem("Debug/Reset Hub Intro")]
    private static void ResetHubIntro()
    {
        PlayerPrefs.DeleteKey(PREF_KEY);
        PlayerPrefs.Save();
        Debug.Log("Hub intro reset! It will play on next game start.");
    }
#endif

    private void Start()
    {


        // Check PlayerPrefs
        if (PlayerPrefs.GetInt(PREF_KEY, 0) == 0)
        {
            StartCoroutine(PlayIntro());
        }
    }

    private IEnumerator PlayIntro()
    {
        // Mark as played
        PlayerPrefs.SetInt(PREF_KEY, 1);
        PlayerPrefs.Save();

        // Wait a frame to avoid race conditions
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => DialogManager.Instance != null);

        // Play dialog
        yield return DialogManager.Instance.ShowDialog(introDialog);
    }
}
