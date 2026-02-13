using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;


[System.Serializable]
public class MurderCase
{
    //Scene Build Index for murder mystery is 16, level number is -1
    public string name; //Only for development, makes it easy to identify when looking at this data

    public List<NPCController> evidenceObjects = new List<NPCController>();
    public List<string> options = new List<string>(); //multiple choice of answers for murder
    public int correctOption;

    //For now body is separate from other evidence objects in case philmus wants a lot of arch for it, however this can be easily changed if not
    public NPCController body; //multiple choice for seeing dialoge? long sequence of dialogue?


    public int level; //PlayerPrefs.GetInt("CurrentLevel");
    public int murderID; //since multiple options of murders for each level, this differentiates them
    public Transform SpawnLocation; //location in Murder Mystery scene to warp to
}

[System.Serializable]
public class CasesForLevel
{
    public List<MurderCase> cases = new List<MurderCase>();
}

public class MurderMystery : MonoBehaviour
{
    public static MurderMystery Instance { get; private set; }

    [SerializeField]
    public List<CasesForLevel> murders = new List<CasesForLevel>();
    public int currentCase = 0;
    public int currentLevel = 0;

    public void Awake()
    {
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
        if(currentLevel < murders.Count)
        {
            int numCasesForLevel = murders[currentLevel].cases.Count;
            currentCase = Random.Range(0,numCasesForLevel);
            //currentCase = 0;  //used for testing

            GameObject player = GameObject.FindGameObjectWithTag("Player"); 
            player.transform.position = murders[currentLevel].cases[currentCase].SpawnLocation.position;
        }
        else
        {
            Debug.Log("TOO HIGH OF LEVEL, NO MURDERS FOR CURRENT LEVEL");
        }
    }


}
