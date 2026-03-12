using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System.Reflection;

public enum CardMatchingPlay
{
    PlayerQuestion,
    PlayerAnswer,
    RivalQuestion,
    RivalAnswer,
    GameEnd
}

public class CardMatchingMinigame : MonoBehaviour
{
    public List<string> Questions;
    public List<string> Answers;
    public List<CardForMatching> Questioncards;
    public List<CardForMatching> Answercards;
    public List<GameObject> qshadows;
    public List<GameObject> ashadows;

    public GameObject cardMatching;
    public GameObject playAgainButton;
    public Sprite CardFront;
    public Sprite QuestionCardBack;
    public Sprite AnswerCardBack;
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
        RandomizeCardAppearance();
    }
    public void StartCardMatching()
    {
        GameController.Instance.StartMinigame();
        cardMatching.SetActive(true);
    }

    public void CloseWindow()
    {
        GameController.Instance.EndMinigame();
        cardMatching.SetActive(false);
    }

    void RandomizeCardAppearance()
    {
        for(int i = 0; i < Questioncards.Count; i++)
        {
            float rand = Random.Range(-5, 5);
            Questioncards[i].gameObject.transform.eulerAngles = new Vector3(0f, 0f, rand);
            qshadows[i].transform.eulerAngles = new Vector3(0f, 0f, rand);

            Questioncards[i].gameObject.GetComponent<Image>().color = new Color(1 - Random.Range(0,0.05f),1 - Random.Range(0,0.05f),1 - Random.Range(0,0.15f),1f);

            rand = Random.Range(-5, 5);
            Answercards[i].gameObject.transform.eulerAngles = new Vector3(0f, 0f, rand);
            ashadows[i].transform.eulerAngles = new Vector3(0f, 0f, rand);

            Answercards[i].gameObject.GetComponent<Image>().color = new Color(1 - Random.Range(0,0.05f),1 - Random.Range(0,0.05f),1 - Random.Range(0,0.15f),1f);

        }
    }
    void RevealAll()  //after someone wins, show all cards (in the future indicate which match which, but only after this func)
    {
        for (int i = 0; i < Questioncards.Count; i++)
        {
            if(Questioncards[i].shown == false)
            {
                Questioncards[i].Reveal();
            }

            if(Answercards[i].shown == false)
            {
                Answercards[i].Reveal();
            }
        }
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

        LoadCards();
        playerScore = 0;
        rivalScore = 0;
        PlayerScoreReader.text = "0";
        RivalScoreReader.text = "0";
        awaiting = CardMatchingPlay.PlayerQuestion;
        questionCard = null;
        answerCard = null;
        playAgainButton.SetActive(false);
        RandomizeCardAppearance();
    }
    //Compare Players cards
    public async Task CalculatePlay(string who)
    {
        bool wonRound = false;
        if(questionCard.index == answerCard.index)  //cards match
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

            //await Awaitable.WaitForSecondsAsync(1f);

            if(playerScore == 5)
            {
                rivalPlayReader.text = "";
                playerPlayReader.text = "Player Wins!!!";
                playAgainButton.SetActive(true);

                awaiting = CardMatchingPlay.GameEnd;
                RevealAll();
                return;
            }
            else if(rivalScore == 5)
            {
                rivalPlayReader.text = "Opponent Wins!!!";
                playerPlayReader.text = "";
                playAgainButton.SetActive(true);

                awaiting = CardMatchingPlay.GameEnd;
                RevealAll();
                return;
            }
            else if(playerScore == 4 && rivalScore == 4)
            {
                //draw
                rivalPlayReader.text = "It's a Draw!!!";
                playerPlayReader.text = "It's a Draw!!!";
                playAgainButton.SetActive(true);

                awaiting = CardMatchingPlay.GameEnd;
                RevealAll();
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
                questionCard = null;
                answerCard = null;
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
                questionCard = null;
                answerCard = null;
                rivalPlayReader.text = "";
                playerPlayReader.text = "Selecting a Question Card...";
            }
        }
    }

    async Task RivalPlay()
    {
        awaiting = CardMatchingPlay.RivalQuestion;
        questionCard = null;
        answerCard = null;
        await Awaitable.WaitForSecondsAsync(1f);

        int num = Random.Range(0, Questioncards.Count);
        questionCard = Questioncards[num];
        int failsafe = 0;
        while(questionCard.shown == true && failsafe < 1000) //has not already been revealed an correctly guessed (is a valid card)
        {
            num = Random.Range(0, Questioncards.Count);
            questionCard = Questioncards[num];
            failsafe++;
        }
        


        //these couple of lines should remain, as well as the last couple.
        //the rest can be changed to properly emulate a humans memory (rn it just randomly selects two cards)
        questionCard.Reveal();
        awaiting = CardMatchingPlay.RivalAnswer;
        rivalPlayReader.text = "Selecting an Answer Card...";
        await Awaitable.WaitForSecondsAsync(2f);
        failsafe = 0;

        num = Random.Range(0, Answercards.Count);
        answerCard = Answercards[num];
        while(answerCard.shown == true && failsafe < 1000) //has not already been revealed an correctly guessed (is a valid card)
        {
            num = Random.Range(0, Answercards.Count);
            answerCard = Answercards[num];
            failsafe++;
        }


        answerCard.Reveal();
        CalculatePlay("Rival");
    }


    void LoadCards()
    {
        var outlist = GetComponentsInChildren<CardForMatching>(true); //give true to access disabled gameobjects
        
        int count = Questioncards.Count;
        List<int> remaining = new List<int>();
        int num = -1;
        
        for(int i = 0; i < count; i++)
        {
            remaining.Add(i);
        }
        for(int i = 0; i < count; i++)
        {   
            num = Random.Range(0, remaining.Count);
            Questioncards[i].info = Questions[remaining[num]];
            Questioncards[i].index = remaining[num];
            remaining.RemoveAt(num);
            
        }

        //again for answers
        for(int i =0; i < count; i++)
        {
            remaining.Add(i);
        }
        for(int i = 0; i < count; i++)
        {   
            num = Random.Range(0, remaining.Count);
            Answercards[i].info = Answers[remaining[num]];
            Answercards[i].index = remaining[num];
            remaining.RemoveAt(num);
        }
    }

}
