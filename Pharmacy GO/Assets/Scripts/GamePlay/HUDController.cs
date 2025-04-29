using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudController : MonoBehaviour
{

    [SerializeField] GameObject coinCounter;
    [SerializeField] GameObject scoreCounter;
    [SerializeField] GameObject toolUICanvas;
    [SerializeField] GameObject joyStickCanvas;
    [SerializeField] GameObject timerDisplay;
    [SerializeField] GameObject multiplierDisplay;

    public void TurnHudOn()
    {
        coinCounter.SetActive(true);
        scoreCounter.SetActive(true);
        timerDisplay.SetActive(true);
        multiplierDisplay.SetActive(true);
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
        coinCounter.SetActive(false);
        scoreCounter.SetActive(false);
        toolUICanvas.SetActive(false);
        joyStickCanvas.SetActive(false);
        timerDisplay.SetActive(false);
        multiplierDisplay.SetActive(false);
    }

    public void EnteringBattle()
    {
        coinCounter.GetComponent<TMP_Text>().color = Color.black;
        scoreCounter.GetComponent<TMP_Text>().color = Color.black;
        timerDisplay.GetComponent<TMP_Text>().color = Color.black;
        multiplierDisplay.GetComponent<TMP_Text>().color = Color.black;
    }

    public void ExitingBattle()
    {
        coinCounter.GetComponent<TMP_Text>().color = Color.white;
        scoreCounter.GetComponent<TMP_Text>().color = Color.white;
        timerDisplay.GetComponent<TMP_Text>().color = Color.white;
        multiplierDisplay.GetComponent<TMP_Text>().color = Color.white;
    }
}