using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine.Networking;

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
    public MinigamePilot pilot; //separate script to only request from github once, reducing risk of 403
    public List<Question> questions;
    public List<CardForMatching> Questioncards;
    public List<CardForMatching> Answercards;
    public List<GameObject> qshadows;
    public List<GameObject> ashadows;

    public GameObject cardMatching; //main object
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
    //used in database collection
    
    private int difficulty = 0; 
    private int correctAnswer = 8; //used in map area
    private int wrongAnswer = -5;
    private int correctStreak = 0;
    private int module = 0;

    //information screens/ popups
    public GameObject infoscreen;
    public GameObject firsttimeinfopopup; //information that only appears in the info screen the first time the player opens the minigame
    public GameObject gameendScreen;
    public TextMeshProUGUI finalcointext;
    public TextMeshProUGUI totalcointext;
    public TextMeshProUGUI finalscoretext;

    public GameObject exitbutton;
    public GameObject infobutton;

    private int totalgainedcoins =0;
    private int totalgainedpoints =0;
    public char[] symbolsformatching = {'☺', '☼', '♯', '♠', '♣','♥','♦','♪'}; //could use numbers too


    public void StartCardMatching() //entrypoint, called from npcminigameplayer
    {
        GameController.Instance.StartMinigame();
        cardMatching.SetActive(true);
        totalgainedcoins = 0; //reset session
        totalgainedpoints = 0;
    }
    
    public void CloseWindow()
    {
        GameController.Instance.EndMinigame();
        cardMatching.SetActive(false);
    }

    public void OpenInfoPopup()
    {
        infoscreen.SetActive(true);

        exitbutton.SetActive(false);
        infobutton.SetActive(false);
    }
    public void CloseInfoPopup()
    {
        infoscreen.SetActive(false);
        firsttimeinfopopup.SetActive(false);
        exitbutton.SetActive(true);
        infobutton.SetActive(true);
    }


    public void HandleCoinsandScore()
    {
        int numCoinsToGain = playerScore - rivalScore;
        
        
        int scoreToGain = playerScore * 1000;
        ScoreManager.Instance.AddMinigameScore(scoreToGain);

        finalscoretext.text = "Final score:\nYou: " + playerScore + "   Opponent: " + rivalScore;

        if(numCoinsToGain < 0)
        {
            numCoinsToGain = -1; //only lose a max of 1
            finalcointext.text = "You lost:\n" + 1 + " Coin and got " + scoreToGain + " Points";
            CoinManager.Instance.RemoveCoin(1); //take coins
        }
        else
        {
            CoinManager.Instance.AddCoin(numCoinsToGain); //give coins
            if(numCoinsToGain == 1)
            {
                finalcointext.text = "You got:\n" + numCoinsToGain + " Coin and +" + scoreToGain + " Points"; 
            }
            else
            {
                finalcointext.text = "You got:\n" + numCoinsToGain + " Coins and +" + scoreToGain + " Points"; 
            }
        }
        totalgainedcoins += numCoinsToGain; //subtraction will still apply
        totalgainedpoints += scoreToGain;

        totalcointext.text = "That makes a total of:\n" + totalgainedcoins + " Coins and +" + totalgainedpoints + " Points"; 
    }
    public void OpenEndGamePopup()
    {
        gameendScreen.SetActive(true);

        exitbutton.SetActive(false);
        infobutton.SetActive(false);

    }
    public void CloseEndGamePopup()
    {
        gameendScreen.SetActive(false);
        Debug.Log("JKOLADSF");
        exitbutton.SetActive(true);
        infobutton.SetActive(true);
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
                Questioncards[i].Reveal(false);
            }

            if(Answercards[i].shown == false)
            {
                Answercards[i].Reveal(false);
            }

            Questioncards[i].indexidentifier.SetActive(true);
            Questioncards[i].indexidentifier.GetComponent<TextMeshProUGUI>().text = "" + symbolsformatching[Questioncards[i].index];

            Answercards[i].indexidentifier.SetActive(true);
            Answercards[i].indexidentifier.GetComponent<TextMeshProUGUI>().text = "" + symbolsformatching[Answercards[i].index];
        }
    }

    public void RestartPlay()
    {
        playerPlayReader.text = "Selecting a Question Card...";
        rivalPlayReader.text = "";
        for(int i = 0; i < questions.Count; i++)
        {
            Answercards[i].gameObject.GetComponent<Button>().interactable = true;
            Questioncards[i].gameObject.GetComponent<Button>().interactable = true;

            Answercards[i].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "";
            Questioncards[i].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "";

            Answercards[i].Hide();
            Questioncards[i].Hide();
        }

        StartCoroutine(LoadCards());
        playerScore = 0;
        rivalScore = 0;
        PlayerScoreReader.text = "0";
        RivalScoreReader.text = "0";
        awaiting = CardMatchingPlay.PlayerQuestion;
        questionCard = null;
        answerCard = null;
        RandomizeCardAppearance();
    }
    //Compare Players cards
    public void IncrementDifficulty(bool correct)
    {
        if (!correct)//got question wrong
        {
            correctStreak = 0;
            difficulty += wrongAnswer;
            if (difficulty < 0)
            {
                difficulty = 0;
            }
        }
        else
        {
            correctStreak += 1;
            difficulty += correctAnswer;
            if (difficulty > 100)
            {
                difficulty = 100;
            }
        }
    }
    public async Task CalculatePlay(string who)
    {
        bool wonRound = false;
        if(questionCard.index == answerCard.index)  //cards match
        {
            wonRound = true;

            if(who == "Player")
            {
                //chose correctly
                IncrementDifficulty(true);
                playerScore++;
                PlayerScoreReader.text = playerScore.ToString();
            }
            else
            {
                IncrementDifficulty(false);
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
                OpenEndGamePopup();
                HandleCoinsandScore();

                awaiting = CardMatchingPlay.GameEnd;
                RevealAll();
                return;
            }
            else if(rivalScore == 5)
            {
                rivalPlayReader.text = "Opponent Wins!!!";
                playerPlayReader.text = "";
                OpenEndGamePopup();
                HandleCoinsandScore();

                awaiting = CardMatchingPlay.GameEnd;
                RevealAll();
                return;
            }
            else if(playerScore == 4 && rivalScore == 4)
            {
                //draw
                rivalPlayReader.text = "It's a Draw!!!";
                playerPlayReader.text = "It's a Draw!!!";
                OpenEndGamePopup();
                HandleCoinsandScore();

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
        questionCard.Reveal(true);

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


        answerCard.Reveal(true);
        CalculatePlay("Rival");
    }

    IEnumerator LoadCards()
    {
        yield return new WaitUntil(() => pilot.gotQuestions);
        int count = Questioncards.Count;
        
        
        for(int i = 0; i < count; i++) //get questions and answers from database
        {
            Question q = pilot.moduleManager.GetRandomQuestion(module, (int) (difficulty / 20));

            questions[i] = q;
        }

        List<int> remaining = new List<int>();
        int num = -1;
        for(int i = 0; i < count; i++)
        {
            remaining.Add(i);
        }
        for(int i = 0; i < count; i++)
        {   
            num = Random.Range(0, remaining.Count);
            Questioncards[i].info = questions[remaining[num]].question;
            Questioncards[i].index = remaining[num];
            if(!string.IsNullOrEmpty(questions[remaining[num]].imageLink))
            {
                StartCoroutine(pilot.DownloadImage(questions[remaining[num]].imageLink, Questioncards[i].gameObject));
            }
            else
            {
                Questioncards[i].image.texture = null;
            }

            remaining.RemoveAt(num);
            //Questioncards[i].Hide();
        }

        //again for answers
        for(int i =0; i < count; i++)
        {
            remaining.Add(i);
        }
        for(int i = 0; i < count; i++)
        {   
            num = Random.Range(0, remaining.Count);
            Answercards[i].info = questions[remaining[num]].options[questions[remaining[num]].answerIndex].text;
            Answercards[i].index = remaining[num];
            if (!string.IsNullOrEmpty(questions[remaining[num]].options[questions[remaining[num]].answerIndex].imageLink))
            {
                StartCoroutine(pilot.DownloadImage(questions[remaining[num]].options[questions[remaining[num]].answerIndex].imageLink, Answercards[i].gameObject));
            }
            else
            {
                Answercards[i].image.texture = null;
            }

            remaining.RemoveAt(num);
            //Answercards[i].Hide();
        }
    }

}
