using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Database
{
    public bool loaded = false;
    public QuestionSet questionSet;

    public Database()
    {

    }

    public IEnumerator load()
    {
        string url = "https://tornquisterik.github.io/Webpage/test.json";
        UnityWebRequest request = UnityWebRequest.Get (url);

        yield return request.SendWebRequest ();

        if (request.result == UnityWebRequest.Result.Success)
        {
            //request.downloadHandler
            string responseText = request.downloadHandler.text;
            Debug.Log (responseText);
            questionSet = JsonUtility.FromJson<QuestionSet> (responseText);
            Debug.Log (questionSet);
            loaded = true;
        }
        else
        {
            Debug.Log ("Error: " + request.error);
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
    public string image; //url for image
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
    public string @string;
    public string image;

    public (OptionType, string) grabOption()
    {
        if (@string.Length == 0 && image.Length == 0) // Has no image or string
        {
            return (OptionType.None, "Failed to load option");
        }
        if (@string.Length != 0 && image.Length == 0) // Has no image and has 1 string
        {
            return (OptionType.String, @string);
        }
        if (@string.Length == 0 && image.Length != 0) // Has 1 image and has no string
        {
            return (OptionType.Image, image);
        }
        return (OptionType.None, "Option has both Text and Images");
    }

    public override string ToString()
    {
        return @string.Length == 0 ? @string : image;
    }
}