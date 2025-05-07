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

    public Database()
    {

    }

    public IEnumerator load()
    {
        UnityWebRequest request = UnityWebRequest.Get (DATABASE_PATH);

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
        None,
        Beginner,
        Novice,
        Intermediate,
        Advanced,
        Expert,
    };

    [Flags]
    public enum LocationFlags 
    {
        Bladder = 1,
        Brain = 2,
        Eyes = 4,
        GI_Tract = 8,
        Heart = 16,
        Lungs = 32,
        Smooth_Muscle = 64,
        Other = 128,
    }

    public DifficultyIndex difficulty;

    public int locations; 

    public struct LocationData
    {
        public int module;
        public LocationFlags location;
    };

    public LocationData locationData;

    // Returns (Module, Location)
    public (int, int) readBin(int locations)
    {
        string bin = Convert.ToString(locations, 2);
        while (bin.Length < 13)
        {
            bin = "0" + bin;
        }
        string binModule = bin.Substring(0, 5);
        string binLocations = bin.Substring(5);
        int module = -1;
        char[] charArray = binModule.ToCharArray();
        Array.Reverse(charArray);
        binModule = new string(charArray);
        for (int i = 0; i < binModule.Length; i++)
        {
            if (binModule[i] == '1')
            {
                module = i;
                break;
            }
        }
        int location = Convert.ToInt32(binLocations, 2);

        return (module, location);
    }

    public void loadLocationData()
    {
        this.locationData = new LocationData();
        (int, int) ld = readBin(this.locations);
        this.locationData.module = ld.Item1;
        this.locationData.location = (LocationFlags) ld.Item2;
    }

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