using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Medication", menuName = "Scriptable Objects/Medication")]
public class Medication : ScriptableObject
{
    [Header("Basic Info")]
    public string medicationName;
    public Sprite image;
    public string level;
    public string treatmentOrgan;
    public string pharmacologicClass;  

    [Header("Pharmacological Details")]
    
    [TextArea]
    public string mechanismOfAction;

    [TextArea]
    public string therapeuticUses;

    [TextArea]
    public string sideEffects;              

    [TextArea]
    public string majorDrugInteractions;        

    [Header("Others")]
    [TextArea]
    public string funFact;                     

}
