using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{

    [SerializeField] GameObject coinCounter;
    [SerializeField] GameObject scoreCounter;

    public void TurnHudOn()
    {
        coinCounter.SetActive(true);
        scoreCounter.SetActive(true);
    }

    public void TurnHudOff()
    {
        coinCounter.SetActive(false);
        scoreCounter.SetActive(false);
    }
}