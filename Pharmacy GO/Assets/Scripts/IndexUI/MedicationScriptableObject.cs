using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Medication", menuName = "Scriptable Objects/Medication")]
public class Medication : ScriptableObject
{
    public string medicationName;
    public string level;
    public string treatmentOrganList;
    public Sprite image;
    public string information;
    public string sideEffects;
}
