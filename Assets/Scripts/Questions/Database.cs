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
    // Loads questions and images from the database

    public bool loaded = false;
    public QuestionSet questionSet;

    // Updated path to test database
    public const string FILE_PATH = "https://api.github.com/repos/kyofyufufufufufufufu/test_database1/contents";
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
            JObject responseJson = JObject.Parse(responseText);
            string base64Content = responseJson.GetValue("content").ToString();
            byte[] byteArrayContent = Convert.FromBase64String(base64Content);
            string databaseContent = Encoding.UTF8.GetString(byteArrayContent);
            
            // Parses databaseContent directly
            // Originally had deserialization now it just reads it
            questionSet = JsonConvert.DeserializeObject<QuestionSet>(databaseContent);
            
            loaded = true;
            // Log to confirm successful load
            Debug.Log("Database successfully loaded and parsed!");
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
    public string imageLink;
    public List<Option> options = new List<Option> ();
    public int answerIndex;
    
    public int difficulty; 

    // Added minigameType (DOES NOT DO ANYTHING YET)
    public string minigameType = "MultipleChoice";

    public int locations;

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

    public struct LocationData
    {
        public int module;
        public LocationFlags location;
    };

    public LocationData locationData;

    // Uses bitwise logic so that it's matching the WinForms packing logic
    //
    // RIGHT NOW THIS DOES NOT DETERMINE WHAT LOCATION THE QUESTION IS SET IN, FIXED IN NEXT SPRINT
    //
    public void loadLocationData()
    {
        this.locationData = new LocationData();
        
        // Extract bottom 8 bits for flags
        int bodyPartSum = locations & 0xFF;
        // Extract bits above 8 for module
        int moduleValue = locations >> 8;

        // Determine which bit was set for the module index
        int modIdx = 0;
        if (moduleValue > 0)
        {
            int temp = moduleValue;
            while (temp > 1)
            {
                temp >>= 1;
                modIdx++;
            }
        }

        this.locationData.module = modIdx;
        this.locationData.location = (LocationFlags)bodyPartSum;
    }

    public override string ToString ()
    {
        return string.Format ("{0}:{1}:{2}:{3}:{4}", question, options.Count > 4 ? options[4].text : "No Option 5", answerIndex, difficulty, locations);
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

    public bool useImage;

    public (OptionType, string) grabOption()
    {
        if (useImage && !string.IsNullOrEmpty(imageLink))
        {
            return (OptionType.Image, imageLink);
        }
        if (!string.IsNullOrEmpty(text))
        {
            return (OptionType.String, text);
        }
        
        return (OptionType.None, "Failed to load option");
    }

    public override string ToString ()
    {
        return string.IsNullOrEmpty(text) ? imageLink : text;
    }
}