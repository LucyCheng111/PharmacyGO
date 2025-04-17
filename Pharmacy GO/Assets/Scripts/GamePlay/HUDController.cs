using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{

    [SerializeField] GameObject coinCounter;
    [SerializeField] GameObject scoreCounter;
    [SerializeField] GameObject toolUICanvas;
    [SerializeField] GameObject joyStickCanvas;

    public void TurnHudOn()
    {
        coinCounter.SetActive(true);
        scoreCounter.SetActive(true);
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
    }
}