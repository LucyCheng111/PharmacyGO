using UnityEngine;


public interface Interactable
{

    // Handles calls to interact with NPCs and objects

    void Interact(Transform initiator);
    void ShowPrompt();
    void HidePrompt();

}
