using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WordBankPlay  //only used to determine if you can pick up the words
{
    Arrange,
    GameEnd
}

public class WordBankMinigame : MonoBehaviour
{
    public MinigamePilot pilot; //separate script to only request from github once, reducing risk of 403
    public List<Question> questions;


}
