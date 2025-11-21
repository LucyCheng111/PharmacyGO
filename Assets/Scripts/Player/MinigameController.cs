using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public enum CardMatchingPlay
{
    PlayerQuestion,
    PlayerAnswer,
    RivalQuestion,
    RivalAnswer,
    GameEnd
}

public class MinigameController : MonoBehaviour
{
    public List<string> Questions;
    public List<string> Answers;
    public List<CardForMatching> Questioncards;
    public List<CardForMatching> Answercards;

    public GameObject cardMatching;
    public GameObject playAgainButton;
    public TextMeshProUGUI playerPlayReader;
    public TextMeshProUGUI rivalPlayReader;
    public TextMeshProUGUI PlayerScoreReader;
    public TextMeshProUGUI RivalScoreReader;

    public CardMatchingPlay awaiting = CardMatchingPlay.PlayerQuestion;

    public CardForMatching questionCard;
    public CardForMatching answerCard;
    public int playerScore;
    public int rivalScore;

    void Start()
    {
        playerPlayReader.text = "Selecting a Question Card...";
        LoadCards();
    }
    public void StartCardMatching()
    {
        cardMatching.SetActive(true);
    }

    public void CloseWindow()
    {
        cardMatching.SetActive(false);
    }

    public void RestartPlay()
    {
        playerPlayReader.text = "Selecting a Question Card...";
        rivalPlayReader.text = "";
        for(int i = 0; i < Answers.Count; i++)
        {
            Answercards[i].gameObject.GetComponent<Button>().interactable = true;
            Questioncards[i].gameObject.GetComponent<Button>().interactable = true;

            Answercards[i].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "";
            Questioncards[i].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "";

            Answercards[i].shown = false;
            Questioncards[i].shown = false;
        }

        Answercards.Clear();
        Questioncards.Clear();
        LoadCards();
        playerScore = 0;
        rivalScore = 0;
        PlayerScoreReader.text = "0";
        RivalScoreReader.text = "0";
        awaiting = CardMatchingPlay.PlayerQuestion;
        playAgainButton.SetActive(false);
    }
    //Compare Players cards
    public async Task CalculatePlay(string who)
    {
        bool wonRound = false;
        if(questionCard.index == answerCard.index)
        {
            wonRound = true;

            if(who == "Player")
            {
                //chose correctly
                playerScore++;
                PlayerScoreReader.text = playerScore.ToString();
            }
            else
            {
                rivalScore++;
                RivalScoreReader.text = rivalScore.ToString();
            }
            questionCard.GetComponent<Button>().interactable = false;  //disable the buttons that are matches
            answerCard.GetComponent<Button>().interactable = false;

            await Awaitable.WaitForSecondsAsync(1f);

            if(playerScore == 5)
            {
                rivalPlayReader.text = "";
                playerPlayReader.text = "Player Wins!!!";
                playAgainButton.SetActive(true);
                return;
            }
            else if(rivalScore == 5)
            {
                rivalPlayReader.text = "Rival Wins!!!";
                playerPlayReader.text = "";
                playAgainButton.SetActive(true);
                return;
            }
            else if(playerScore == 4 && rivalScore == 4)
            {
                //draw
                rivalPlayReader.text = "It's a Draw!!!";
                playerPlayReader.text = "It's a Draw!!!";
                playAgainButton.SetActive(true);
                return;
            }
        }
        else
        {
            await Awaitable.WaitForSecondsAsync(2f);
            answerCard.Hide();
            questionCard.Hide();
        }

        playerPlayReader.text = "";
        rivalPlayReader.text = "Selecting a Question Card...";

        // proceed next play 
        if(who == "Player")
        {
            if (wonRound)
            {
                awaiting = CardMatchingPlay.PlayerQuestion;
                rivalPlayReader.text = "";
                playerPlayReader.text = "Selecting a Question Card...";
            }
            else
            {
                RivalPlay();
            }
        }
        else //calced rival score
        {
            if (wonRound)
            {
                RivalPlay();
            }
            else
            {
                awaiting = CardMatchingPlay.PlayerQuestion;
                rivalPlayReader.text = "";
                playerPlayReader.text = "Selecting a Question Card...";
            }
        }


    }

    async Task RivalPlay()
    {
        await Awaitable.WaitForSecondsAsync(1f);

        int num = Random.Range(0, Questioncards.Count - 1);
        questionCard = Questioncards[num];
        while(questionCard.shown == true) //has not already been revealed an correctly guessed (is a valid card)
        {
            num = Random.Range(0, Questioncards.Count - 1);
            questionCard = Questioncards[num];
        }
        


        //these couple of lines should remain, as well as the last couple.
        //the rest can be changed to properly emulate a humans memory (rn it just randomly selects two cards)
        questionCard.Reveal();
        rivalPlayReader.text = "Selecting an Answer Card...";
        await Awaitable.WaitForSecondsAsync(2f);



        num = Random.Range(0, Answercards.Count - 1);
        answerCard = Answercards[num];
        while(answerCard.shown == true) //has not already been revealed an correctly guessed (is a valid card)
        {
            num = Random.Range(0, Answercards.Count - 1);
            answerCard = Answercards[num];
        }


        answerCard.Reveal();
        CalculatePlay("Rival");
    }

    void LoadCards()
    {
        var outlist = GetComponentsInChildren<CardForMatching>();
        foreach (var c in outlist)
        {
            if (!c.isQuestion)
            {
                Answercards.Add(c);
            }
            else
            {
                Questioncards.Add(c);
            }
        }
        int count = Questioncards.Count;
        List<int> taken = new List<int>();
        int num = -1;  //cant be valid index at start

        for(int i = 0; i < count; i++)
        {   
            while(true){
                num = Random.Range(0, Questions.Count);
                if (taken.Contains(num))
                {
                    num = Random.Range(0, Questions.Count);
                }
                else
                {
                    break;
                }
            }
            taken.Add(num);
            Questioncards[i].info = Questions[num];
            Questioncards[i].index = num;
        }

        taken.Clear();
        num = -1;

        for(int i = 0; i < count; i++)
        {
            while(true){  // find an entry not already selected for a prev card
                num = Random.Range(0, Answers.Count);
                if (taken.Contains(num))
                {
                    num = Random.Range(0, Answers.Count);
                }
                else
                {
                    break;
                }
            }
            taken.Add(num);
            Answercards[i].info = Answers[num]; //give string (Question wording, Answer Wording...)
            Answercards[i].index = num; //assign the checker (if A.index == Q.index, this is the right answer)
        }

    }


}
