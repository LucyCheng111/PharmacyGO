
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceBox : MonoBehaviour
{

    // Manager for the choice box

    [SerializeField] ChoiceText choiceTextPrefab;

    bool choiceSelected = false;

    List<ChoiceText> choiceTexts;
    int currentChoice;

    public IEnumerator ShowChoices(List<string> choices, Action<int> onChoiceSelected)
    {
        choiceSelected = false;
        currentChoice = 0;

        gameObject.SetActive(true);

        // Delete previous choices
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        choiceTexts = new List<ChoiceText>();
        for (int i = 0; i < choices.Count; i++)
        {
            var choiceTextObj = Instantiate(choiceTextPrefab, transform);
            choiceTextObj.TextField.text = choices[i];
            choiceTextObj.Initialize(i, OnChoiceClicked);
            choiceTexts.Add(choiceTextObj);
        }
        // Set initial selection
        UpdateChoiceSelection();

        yield return new WaitUntil(() => choiceSelected == true);

        onChoiceSelected?.Invoke(currentChoice);
        gameObject.SetActive(false);
    }

    private void OnChoiceClicked(int index)
    {
        Debug.Log($"OnChoiceClicked called with index: {index}");
        currentChoice = index;
        UpdateChoiceSelection();
        choiceSelected = true;
    }

    private void UpdateChoiceSelection()
    {
        for (int i = 0; i < choiceTexts.Count; i++)
        {
            choiceTexts[i].SetSelected(i == currentChoice);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            ++currentChoice;
            currentChoice = Mathf.Clamp(currentChoice, 0, choiceTexts.Count - 1);
            UpdateChoiceSelection();
        }
            
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            --currentChoice;
            currentChoice = Mathf.Clamp(currentChoice, 0, choiceTexts.Count - 1);
            UpdateChoiceSelection();
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            choiceSelected = true;
        }
    }
}



