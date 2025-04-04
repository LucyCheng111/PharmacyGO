using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

public class Database
{
    public bool loaded = false;
    public QuestionSet questionSet;

    public const string FILE_PATH = "https://api.github.com/repos/AlecDuval/gitDatabaseTest/contents";
    public const string DATABASE_PATH = FILE_PATH + "/jsonTest.json";
    public const string IMAGES_PATH = FILE_PATH + "/images";
    public const string MAIN_AUTH_TOKEN = "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";

    public Database()
    {

    }

    public IEnumerator load()
    {
        UnityWebRequest request = UnityWebRequest.Get (DATABASE_PATH);
        request.SetRequestHeader ("authorization", MAIN_AUTH_TOKEN);

        yield return request.SendWebRequest ();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseText = request.downloadHandler.text;
            Debug.Log (responseText);
            JObject responseJson = JObject.Parse (responseText);                    // Convert text to json object
            string base64Content = responseJson.GetValue ("content").ToString ();   // Get base64 from json
            byte[] byteArrayContent = Convert.FromBase64String (base64Content);     // Convert base64 to byte array
            string databaseContent = Encoding.UTF8.GetString (byteArrayContent);    // Convert byte array to string
            Debug.Log (databaseContent);
            string formattedContent = (string)JsonConvert.DeserializeObject (databaseContent);
            JObject databaseJson = JObject.Parse (formattedContent);
            questionSet = databaseJson.ToObject<QuestionSet> ();
            Debug.Log (questionSet);
            loaded = true;
        }
        else
        {
            Debug.LogError ("Database Error: " + request.error);
        }
    }
}

[System.Serializable]
public class QuestionSet
{
    public List<Question> questions = new List<Question> ();

    public override string ToString ()
    {
        string output = "";
        foreach (Question question in questions)
        {
            output += question.ToString () + "\n";
        }
        return output;
    }
}

[System.Serializable]
public class Question
{
    public string question;
    public string imageLink; //url for image
    public List<Option> options = new List<Option>();
    public int answerIndex;
    public enum DifficultyIndex
    {
        easy,
        medium,
        hard
    };

    public DifficultyIndex difficulty;

    public int locations; 

    public override string ToString ()
    {
        return string.Format ("{0}:{1}:{2}:{3}:{4}", question, options[4], answerIndex, difficulty, locations);
    }
}

[System.Serializable]
public class Option
{
    public enum OptionType
    {
        None,
        String,
        Image
    };
    public string text;
    public string imageLink;

    public (OptionType, string) grabOption()
    {
        if (text.Length == 0 && imageLink.Length == 0) // Has no image or string
        {
            return (OptionType.None, "Failed to load option");
        }
        if (text.Length != 0 && imageLink.Length == 0) // Has no image and has 1 string
        {
            return (OptionType.String, text);
        }
        if (text.Length == 0 && imageLink.Length != 0) // Has 1 image and has no string
        {
            return (OptionType.Image, imageLink);
        }
        return (OptionType.None, "Option has both Text and Images");
    }

    public override string ToString()
    {
        return text.Length == 0 ? text : imageLink;
    }
}