using System.Collections.Generic;
using UnityEngine;


[System.Serializable]

// The reason for this class:
// If we want the NPC to give item to the player in the future, or encounter new event
public class Dialog
{
    [SerializeField] List<string> lines;

    public List<string> Lines
    {
        get { return lines; }
    }
}
