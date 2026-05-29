using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public enum MinigameType
{
    None,
    CardMatching,
    WordBank,
    Slapjack
}
public class MinigamePilot : MonoBehaviour
{
    public bool gotQuestions = false;
    public Database database;
    public List<Question> randomQuestions;
    public Module moduleManager;
    void Start()
    {
        StartCoroutine(getQuestions());
    }
    public IEnumerator getQuestions()
    {   
        database = new Database();
        StartCoroutine(database.load());
        yield return new WaitUntil(() => database.loaded);
        randomQuestions = database.questionSet.questions;
        moduleManager = new Module(randomQuestions);
        gotQuestions = true;

    }
    public IEnumerator DownloadImage(string url, GameObject obj)
    {
        // Use UnityWebRequestTexture to download raw image bytes directly
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        CardForMatching matchcard = obj.GetComponent<CardForMatching>();
        CardForSlapping slapcard = obj.GetComponent<CardForSlapping>();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            if (matchcard != null)
            {
                matchcard.image.texture = texture;
            }
            else if (slapcard != null)
            {
                slapcard.image.texture = texture;
            }
            else //readerboard for wordbank
            {
                obj.GetComponent<RawImage>().texture = texture;
            }
        }
        else
        {
            Debug.LogError("Question image load error: " + request.error);
            Debug.LogError("Failed URL: " + url);
        }
    }
}