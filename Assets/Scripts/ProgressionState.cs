using System.Collections.Generic;
using UnityEngine;

public class ProgressionState : MonoBehaviour
{

    // Keeps track of the current player progression and obtained items

    public static ProgressionState Instance;

    private List<string> collectedItems = new List<string>();
    public List<string> allItems = new List<string>();

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

        LoadSavedItems();
    }

    public void LoadSavedItems()
    {
        for (int i = 0; i < allItems.Count; i++)
        {
            int check = PlayerPrefs.GetInt("has" + allItems[i]);

            if(check == 1)
            {
                CollectItem(allItems[i]);
            }
        }
    }

    public void CollectItem(string itemName)
    {
        // The check prevents the item getting added again to the list if it reappears for some reason

        if (!collectedItems.Contains(itemName))
        {
            collectedItems.Add(itemName);

            PlayerPrefs.SetInt("has" + itemName, 1);
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
        else if (itemname == "Postcard")
        {
            return "Dr. Shepards Postcard";
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
        else if (itemname == "Postcard")
        {
            return 5;
        }

        return 0;
    }
}
