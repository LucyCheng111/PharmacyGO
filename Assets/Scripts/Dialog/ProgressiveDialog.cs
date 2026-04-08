using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPCController))]
public class ProgressiveDialog : MonoBehaviour
{

    // This changes the dialogue of an NPC depending on whether the player has acquired a specific item

    [Serializable]
    public class DialogEntry
    {
        // Pair an item with a set of dialog

        public string itemName;
        public Dialog dialog;
    }

    [SerializeField] private List<DialogEntry> dialogEntries = new List<DialogEntry>();

    private NPCController npcController;

    private void Start()
    {
        npcController = GetComponent<NPCController>();

        CheckAndUpdateDialog();
    }

    private void Update()
    {
        CheckAndUpdateDialog();
    }

    private void CheckAndUpdateDialog()
    {
        // Go through list from top to bottom
        // If the player has one of the items listed, change to that dialog
        // If the player has multiple items, then the dialog chosen will be the most recent one (closest to bottom)

        foreach (DialogEntry entry in dialogEntries)
        {
            if (ProgressionState.Instance.HasItem(entry.itemName))
            {
                if (entry.dialog != null)
                {
                    npcController.SetDialog(entry.dialog);
                }    
            }
        }
    }
}
