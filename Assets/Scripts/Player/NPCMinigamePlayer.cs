using UnityEngine;
public class NPCMinigamePlayer : MonoBehaviour
{
    public MinigameType type;
    public int difficulty; //0-10 inclusive
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
        else if(type == MinigameType.WordBank)
        {
            MainCanvas.Instance.GetComponentInChildren<WordBankMinigame>().StartWordBank(difficulty);
        }
        else if(type == MinigameType.Slapjack)
        {
            MainCanvas.Instance.GetComponentInChildren<SlapjackMinigame>().StartSlapjack(difficulty);
        }
    }
}
