using System.Collections.Generic;
using UnityEngine;

public class ProgressionState : MonoBehaviour
{

    // Keeps track of the current player progression and obtained items

    public static ProgressionState Instance;

    private List<string> collectedItems = new List<string>();

    private void Awake()
    {
        // Only have one instance of the tracker

        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        } 
        
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void CollectItem(string itemName)
    {
        // The check prevents the item getting added again to the list if it reappears for some reason

        if (!collectedItems.Contains(itemName))
        {
            collectedItems.Add(itemName);
        }
    }

    public bool HasItem(string itemName)
    {
        return collectedItems.Contains(itemName);
    }

    public string ReturnNameForItem(string itemname)
    {
        if(itemname == "ShepardsGlasses")
        {
            return "Dr. Shepards Glasses";
        }
        else if(itemname == "MedicalSupplies")
        {
            return "Dr. Shepards Medical Supplies";
        }
        else if(itemname == "IDCard")
        {
            return "Dr. Shepards ID card";
        }

        return "";
    }

    //returns what item is needed to unlock the output level
    public int ReturnLevelForItem(string itemname)
    {
        if(itemname == "ShepardsGlasses")
        {
            return 2;
        }
        else if(itemname == "MedicalSupplies")
        {
            return 3;
        }
        else if(itemname == "IDCard")
        {
            return 4;
        }

        return 0;
    }
}
