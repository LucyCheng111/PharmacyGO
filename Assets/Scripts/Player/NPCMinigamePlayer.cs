using UnityEngine;

public enum MinigameType
{
    None,
    CardMatching,
    Whackamole,
    Slapjack
}
public class NPCMinigamePlayer : MonoBehaviour
{
    public MinigameType type;
    
    public void StartMinigame()
    {
        if(type == MinigameType.None){
            Debug.Log("MINIGAMENPC NOT ASSIGNED A GAME");
        }
        else if(type == MinigameType.CardMatching)
        {
            MainCanvas.Instance.GetComponentInChildren<CardMatchingMinigame>().StartCardMatching();
            MainCanvas.Instance.GetComponentInChildren<CardMatchingMinigame>().RestartPlay();
        }
        else if(type == MinigameType.Whackamole)
        {
            
        }
        else if(type == MinigameType.Slapjack)
        {
            
        }
    }
}
