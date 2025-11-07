using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudController : MonoBehaviour
{

    // Handles all HUD elements in one place

    [SerializeField] GameObject coinCounter;
    [SerializeField] GameObject scoreCounter;
    [SerializeField] GameObject toolUICanvas;
    [SerializeField] GameObject joyStickCanvas;
    [SerializeField] GameObject timerDisplay;
    [SerializeField] GameObject multiplierDisplay;

    [SerializeField] GameObject Counters; //coin, score, etc...

    public void TurnHudOn()
    {
        Counters.SetActive(true);
        // toolUICanvas.SetActive(true);
        // if (Application.isMobilePlatform)
        // {
        //     joyStickCanvas.SetActive(true);
        // }

        if (toolUICanvas != null) toolUICanvas.SetActive(true);
        if (Application.isMobilePlatform && joyStickCanvas != null)
            joyStickCanvas.SetActive(true);
    }

    public void TurnHudOff()
    {
        toolUICanvas.SetActive(false);
        joyStickCanvas.SetActive(false); 
        Counters.SetActive(false);
    }

    public void EnteringBattle()
    {
        coinCounter.GetComponent<TMP_Text>().color = Color.black;
        scoreCounter.GetComponent<TMP_Text>().color = Color.black;
        timerDisplay.GetComponent<TMP_Text>().color = Color.black;
        multiplierDisplay.GetComponent<TMP_Text>().color = Color.black;
        toolUICanvas.SetActive(false);
        joyStickCanvas.SetActive(false);
    }

    public void ExitingBattle()
    {
        coinCounter.GetComponent<TMP_Text>().color = Color.white;
        scoreCounter.GetComponent<TMP_Text>().color = Color.white;
        timerDisplay.GetComponent<TMP_Text>().color = Color.white;
        multiplierDisplay.GetComponent<TMP_Text>().color = Color.white;
        if (toolUICanvas != null) toolUICanvas.SetActive(true);
        if (joyStickCanvas != null)
            joyStickCanvas.SetActive(true);
    }
}