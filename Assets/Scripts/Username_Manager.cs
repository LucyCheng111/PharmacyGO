using UnityEngine;
using System.IO;
using System.Linq;
public class Username_Manager : MonoBehaviour
{
    public static Username_Manager Instance { get; private set; }
    public string username;
    public TextAsset adjectivesFile; //all text files for this are in "PharmacyGo/Assets/Data Files"
    public TextAsset nounsFile;
    public TextAsset numbersFile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate Username Manager found! Destroying extra instance.");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }



        //GET USERNAME

        username = PlayerPrefs.GetString("Username");
        if(username == string.Empty)
        {
            //User does not have a saved Username on this computer/ browser
            username = Generate_new_username();    
            PlayerPrefs.SetString("Username", username);
        }
        else
        {
            Debug.Log("Aquired Username: " + username);

        }

        /*
        //generates lots of example usernames
        for(int i =0; i < 1000; i++){
            Generate_new_username();
        }
        */
    }

    public string Generate_new_username()
    {
        string generated = "";
        string noun;
        string adjective = "";
        string number = "";
        // a username is made up of 3 parts:
        //ADJECTIVE, NOUN, NUMBER
        string Bulk = adjectivesFile.text;
        string[] adjectives = Bulk.Split('\n');
        int a_n = adjectives.Length;
        //random range's max when returning an int is exclusive, and so that why theres no a_n - 1 
        adjective = new string(adjectives[Random.Range(0, a_n)].Where(char.IsLetterOrDigit).ToArray());

        //noun
        Bulk = nounsFile.text;
        string[] nouns = Bulk.Split('\n');
        int no_n = nouns.Length;

        noun = new string(nouns[Random.Range(0,no_n)].Where(char.IsLetterOrDigit).ToArray());

        //number
        Bulk = numbersFile.text;
        string[] numbers = Bulk.Split('\n');
        int nu_n = numbers.Length;
        
        number = new string(numbers[Random.Range(0,nu_n)].Where(char.IsLetterOrDigit).ToArray());

        generated = adjective + noun + number;
        Debug.Log("Username generated: " + generated);

        return generated;
    }
}