using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

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
    public IEnumerator getQuestions()
    {   
        database = new Database();
        StartCoroutine(database.load());
        yield return new WaitUntil(() => database.loaded);
        randomQuestions = database.questionSet.questions;
        moduleManager = new Module(randomQuestions);
        gotQuestions = true;

    }
    public IEnumerator DownloadImage(string url, GameObject card)
    {
        // Use UnityWebRequestTexture to download raw image bytes directly
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        CardForMatching matchcard = card.GetComponent<CardForMatching>();
        CardForSlapping slapcard = card.GetComponent<CardForSlapping>();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            if (matchcard)
            {
                matchcard.image.texture = texture;
            }
            if (slapcard)
            {
                slapcard.image.texture = texture;
            }
            
        }
        else
        {
            Debug.LogError("Question image load error: " + request.error);
            Debug.LogError("Failed URL: " + url);
        }
    }
}
