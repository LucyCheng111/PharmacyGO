using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;
using System.Text.RegularExpressions;
using System;

[System.Serializable]
public enum CaseType{
    LivePatient,
    Murder
}

[System.Serializable]
public class MurderCase
{
    //Scene Build Index for murder mystery is 16, level number is -1
    public string name; //Only for development, makes it easy to identify when looking at this data
    public CaseType type;
    public PatientController patient; //OPTIONAL, ONLY USE IF LIVE PATIENT TYPE
    public List<NPCController> evidenceObjects = new List<NPCController>();
    

    public int level; //PlayerPrefs.GetInt("CurrentLevel");
    public int murderID; //since multiple options of murders for each level, this differentiates them
    public Transform SpawnLocation; //location in Murder Mystery scene to warp to
}

[System.Serializable]
public class CasesForLevel
{
    public string name; //Only for development, makes it easy to identify when looking at this data
    public List<MurderCase> cases = new List<MurderCase>();
}

public class MurderMystery : MonoBehaviour
{
    public static MurderMystery Instance { get; private set; }

    [SerializeField]
    public List<CasesForLevel> murders = new List<CasesForLevel>();
    public int currentCase = 0;
    public int currentLevel = 0;
    public string portalFrom = "";

    public void Awake()
    {
        portalFrom = PlayerPrefs.GetString("SpawnPointID", "");
        if(Instance != null & Instance != this )
        {
            Destroy( gameObject );
            return;
        }
        Instance = this;
        
        randomizeMurder();
    }

    void randomizeMurder()
    {
        UnityEngine.Debug.Log(PlayerPrefs.GetString("SpawnPointID", ""));
        string wherefrom = PlayerPrefs.GetString("SpawnPointID", "");
        int levelfrom = convertSpawnPointIDtoLevel(wherefrom) - 1;

        if(levelfrom > murders.Count)
        {
            Debug.Log("TOO HIGH OF LEVEL, NO MURDERS FOR CURRENT LEVEL, DEFAULTING TO LEVEL 1");
            levelfrom = 0;
        }

        int numCasesForLevel = murders[levelfrom].cases.Count;
        //currentCase = Random.Range(0,numCasesForLevel);
        currentCase = 1;  //used for testing

        GameObject player = GameObject.FindGameObjectWithTag("Player"); 
        player.transform.position = murders[levelfrom].cases[currentCase].SpawnLocation.position;
    }

    public int convertSpawnPointIDtoLevel(string input)
    {
        //got this from https://discussions.unity.com/t/extract-number-from-string/4361
        return Convert.ToInt32(Regex.Replace(input, "[^0-9]", ""));
    }

    public string GetSpawnpointFromLevel() //returns SpawnPointID for where the player should get when booted from scene
    {
        string wherefrom = PlayerPrefs.GetString("SpawnPointID", "");
        int levelfrom = convertSpawnPointIDtoLevel(wherefrom);
        return "Lv" + levelfrom + "MMToHub";
    }

}
