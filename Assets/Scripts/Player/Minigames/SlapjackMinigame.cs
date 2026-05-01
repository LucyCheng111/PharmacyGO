using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public enum SlapjackPlay
{
    PlayerTurn, //play inputs a card
    OpponentTurn, 
    GameEnd
}

public class SlapjackMinigame : MonoBehaviour
{
    public MinigamePilot pilot; //separate script to only request from github once, reducing risk of 403
    public List<Question> questions;
    public GameObject slapjack; //main object
    public Sprite CardFront;
    public Sprite QuestionCardBack;
    public Sprite AnswerCardBack;
    public SlapjackPlay awaiting = SlapjackPlay.PlayerTurn;
    public List<CardForSlapping> playerCards = new List<CardForSlapping>();
    public List<CardForSlapping> opponentCards = new List<CardForSlapping>();
    public List<CardForSlapping> centerCards = new List<CardForSlapping>();
    public List<CardForSlapping> questionCards = new List<CardForSlapping>();
    public CardForSlapping currentcentercard;
    public Question currentQuestion;
    public int currentQuestionCardIndex = 0;
    public int minigameDifficulty = 0; //separate from question difficulty (below)
    public GameObject card_prefab;
    //main objects
    public GameObject centerpile;
    public GameObject playerpile;
    public GameObject opponentpile;

    public GameObject questionpile;
    public GameObject currentquestioncardlocation; //stack to the left of the question pile

    public GameObject opponentcardnumreader;
    public GameObject playercardnumreader;
    public GameObject whoselectedreader;
    public GameObject correctnessreader;
    public GameObject playerTurnreader;
    public GameObject opponentTurnreader;
    //information screens/ popups
    public GameObject infoscreen;
    public GameObject firsttimeinfopopup; //information that only appears in the info screen the first time the player opens the minigame
    public GameObject gameendScreen;
    public TextMeshProUGUI finalcointext;
    public TextMeshProUGUI totalcointext;
    public TextMeshProUGUI finalscoretext;
    public TextMeshProUGUI totalscoretext;
    public GameObject exitbutton;
    public GameObject infobutton;

    private int totalgainedcoins = 0;
    private int totalgainedpoints = 0;
    private int currentgainedpoints = 0;
    private int currentgainedcoins = 0;
    
    public TextMeshProUGUI statstext; //for 4 vals below
    private int playercorrectquestions = 0;  //slapped center and got it right
    private int playerincorrectquestions = 0; //slapped center and got it wrong
    private int opponentcorrectquestions = 0;
    private int opponentincorrectquestions = 0;

    //used in database collection
    private int correctAnswer = 8; //used in map area
    private int wrongAnswer = -5;
    private int correctStreak = 0;
    private int module = 0;
    [SerializeField] List<Question> randomQuestions;
    private int difficulty = 0; //difficulty of the questions
    private IEnumerator currentOpponentAction;
    //opponent intelligence
    bool opponentDecidedNo = false;
    bool opponentActionCanceled = false;
    public bool cardisMoving = false; //used for opponent to wait until all cards have been locked into place before continuing (lock)

    //delays
    float cardstackdelay = .1f; //time between every card when a whole stack moves
    float correctnessreadingdelay = 2f; //time given to player to read text before it goes away (correct/ incorrect text)
    float cardreadingdelay = 2f; //guaranteed time given to player to read a card before opponent reacts

    public void CloseWindow()
    {
        GameController.Instance.EndMinigame();
        slapjack.SetActive(false);
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

        firsttimeinfopopup.SetActive(false); //won't open again, but is open through the scene by default
        exitbutton.SetActive(true);
        infobutton.SetActive(true);
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

        exitbutton.SetActive(true);
        infobutton.SetActive(true);
    }

    void RandomizeCardAppearance()
    {
        
    }

    public void StartSlapjack(int difficult, int level) //entrypoint, called from npcminigameplayer
    {
        module = level;
        minigameDifficulty = difficult;
        GameController.Instance.StartMinigame();
        slapjack.SetActive(true);
        totalgainedcoins = 0; //reset session
        totalgainedpoints = 0;
        RestartPlay();
    }

    public void RestartPlay()
    {
        playerTurnreader.SetActive(true);
        opponentTurnreader.SetActive(false);
        opponentDecidedNo = false;
        opponentActionCanceled = false;
        for(int i = 0; i < questionCards.Count; i++) //delete old question cards
        {
            Destroy(questionCards[i].gameObject);
        }
        for(int i = 0; i < playerCards.Count; i++)
        {
            Destroy(playerCards[i].gameObject);
        }
        for(int i = 0; i < opponentCards.Count; i++)
        {
            Destroy(opponentCards[i].gameObject);
        }
        for(int i = 0; i < centerCards.Count; i++)
        {
            Destroy(centerCards[i].gameObject);
        }
        playerCards.Clear();
        opponentCards.Clear();
        questionCards.Clear();
        centerCards.Clear();
        questions.Clear();

        StartCoroutine(LoadCards());
        awaiting = SlapjackPlay.PlayerTurn;
        RandomizeCardAppearance();
    }
    public void DrawNextQuestionCard()
    {
        if(currentQuestionCardIndex <= 9)
        {
            StartCoroutine(questionCards[currentQuestionCardIndex].selectThisQuestionCard());
            currentQuestionCardIndex++;
        }
        else
        {
            //out of question cards, game end
            EndGame(true);
        }

    }
    public void HandleCoinsandScore()
    {
        statstext.text = "Game Stats:\n" + 
        "Correct guesses (Player): " + playercorrectquestions + " \n" +
        "Incorrect guesses (Player): " + playerincorrectquestions + " \n" +
        "Correct guesses (Opponent): " + opponentcorrectquestions + " \n" +
        "Incorrect guesses (Opponent): " + opponentincorrectquestions + " \n";

        int numCoinsToGain = (playercorrectquestions - opponentcorrectquestions) - (playerincorrectquestions - opponentincorrectquestions);
        if(numCoinsToGain < 0){numCoinsToGain = 0;}
        
        int scoreToGain = numCoinsToGain * 1000;
        ScoreManager.Instance.AddMinigameScore(scoreToGain);

        finalscoretext.text = "Final score:\nYou: " + playercorrectquestions + "   Opponent: " + opponentcorrectquestions;

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
    
    
    public void reIndexCards(int list) //0- player, 1- center, 2- opponent
    {
        if(list == 0) //player
        {
            for(int i = 0; i < playerCards.Count; i++)
            {
                playerCards[i].index = i;
            }
        }
        else if(list == 1) //center
        {
            for(int i = 0; i < centerCards.Count; i++)
            {
                centerCards[i].index = i;
            }
        }
        else  //opponent
        {
            for(int i = 0; i < opponentCards.Count; i++)
            {
                opponentCards[i].index = i;
            }
        }
    } 
    IEnumerator LoadCards()
    {
        yield return new WaitUntil(() => pilot.gotQuestions);
        
        int Questioncount = 10;
        
    
        for(int i = 0; i < Questioncount; i++) //get questions and answers from database
        {
            Question q = pilot.moduleManager.GetRandomQuestion(module, (int) (difficulty / 20));
            questions.Add(q);
        }

        for(int i = 0; i < Questioncount; i++) //make new question cards and assign new questions
        {   
            GameObject newcard_object = Instantiate(card_prefab,questionpile.transform.position , Quaternion.identity); //make new question card and assign values
            newcard_object.name = "Questioncard_" + i;
            newcard_object.transform.SetParent(questionpile.transform, true);

            CardForSlapping newcard = newcard_object.GetComponent<CardForSlapping>();
            newcard.isQuestion = true;
            newcard.index = i;
            newcard.controller = this;
            newcard.type = SlapjackCardType.Other;
            newcard.Hide();

            questionCards.Add(newcard);
            
            questionCards[i].info = questions[i].question;


            if(!string.IsNullOrEmpty(questions[i].imageLink))
            {
                StartCoroutine(pilot.DownloadImage(questions[i].imageLink, questionCards[i].gameObject));
            }
            else
            {
                questionCards[i].image.texture = null;
            }

        }
        DrawNextQuestionCard();

        int cardcount = 0; 
        for(int i =0; i < Questioncount; i++)
        {
            cardcount += questions[i].options.Count;//get every answer to have more cards
        }

        for(int i = 0; i < cardcount; i++) //create answer cards and distribute them
        {   
            GameObject newcard_object = Instantiate(card_prefab,centerpile.transform.position , Quaternion.identity); //make new question card and assign values
            //newcard_object.name = "othercard_" + i;
            newcard_object.transform.SetParent(centerpile.transform, true);

            CardForSlapping newcard = newcard_object.GetComponent<CardForSlapping>();

            newcard.index = i;
            newcard.controller = this;
            newcard.type = SlapjackCardType.Other;
            newcard.Hide();

            centerCards.Add(newcard);

        }

        //manually go down every question and add the info to each card
        int qnum = 0;
        int cardnum = 0;
        while(qnum < questions.Count)
        {
            for(int i = 0; i< questions[qnum].options.Count; i++)
            {
                centerCards[cardnum].info = questions[qnum].options[i].text;
                

                if(!string.IsNullOrEmpty(questions[qnum].imageLink))
                {
                    StartCoroutine(pilot.DownloadImage(questions[qnum].imageLink, centerCards[cardnum].gameObject));
                }
                else
                {
                    centerCards[cardnum].image.texture = null;
                }
                cardnum++;
            }
            qnum++;
        }

        ArrangeCards();
    }

    //at end of LoadCards, all cards are in order in the center pile, this randomizes them and splits them up to the player and the opponent
    public void ArrangeCards()
    {
        int numcards = centerCards.Count;
        Debug.Log("Slapjack loaded " + numcards + " cards");

        //fisher yates shuffle
        for(int i = numcards - 1; i > 0; i--)
        {
            CardForSlapping juggler;
            int j = Random.Range(0,i + 1);
            juggler = centerCards[i];
            centerCards[i] = centerCards[j];
            centerCards[j] = juggler;
        }

        for(int i = 0; i < numcards / 2; i++)
        {
            centerCards[i].index = i; //reindex
            StartCoroutine(centerCards[i].moveFromCenter(false)); //give to opp
        }

        numcards = centerCards.Count; //recount
        for(int i = numcards - 1; i > -1; i--) // i can be 0, index 0 is used
        {
            centerCards[i].index = i; //reindex
            StartCoroutine(centerCards[i].moveFromCenter(true)); //player
        }
        UpdateCardNumReaders();
    }

    public void UpdateCardNumReaders()
    {
        playercardnumreader.GetComponent<TextMeshProUGUI> ().text = ""+playerCards.Count;
        opponentcardnumreader.GetComponent<TextMeshProUGUI> ().text = ""+opponentCards.Count;
    }

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
    public void CardDrawn(bool isPlayer, CardForSlapping card) //called when a new answer card is moved to the center 
    {
        
        //Debug.Log("CARD PLAYER: " + isPlayer);
        opponentDecidedNo = false;
        opponentActionCanceled = false;
        currentcentercard = card;
        if (isPlayer)
        {
            awaiting = SlapjackPlay.OpponentTurn;
            playerTurnreader.SetActive(false);
            opponentTurnreader.SetActive(true);
        }
        else
        {
            awaiting = SlapjackPlay.PlayerTurn;
            playerTurnreader.SetActive(true);
            opponentTurnreader.SetActive(false);
        }

        //opponent decide if to slap
        CalculatePlay();
    }
    public void StopAllActions()
    {
        StopAllCoroutines();
    }
    
    public IEnumerator PlayerSelectedCenter()
    {
        HaltOpponentAction();
        Debug.Log("PLAYER SELECTED CENTER");
        if (currentcentercard.info == currentQuestion.options[currentQuestion.answerIndex].text)
        {
            whoselectedreader.GetComponent<TextMeshProUGUI> ().text = "Player Selected!";
            correctnessreader.GetComponent<TextMeshProUGUI> ().text = "Correct!";
            playercorrectquestions++;
            int n = centerCards.Count;
            for(int i = 0; i < n; i++) //give all cards in center to the player
            {
                StartCoroutine(centerCards[0].moveFromCenter(true));
                yield return new WaitForSeconds(cardstackdelay);
            }
            DrawNextQuestionCard();
            UpdateCardNumReaders();
        }
        else
        {
            if(playerCards.Count == 0)
            {
                EndGame(false); // out of cards
                yield break; //return
            }
            StartCoroutine(playerCards[0].SwapSides(false));
            whoselectedreader.GetComponent<TextMeshProUGUI> ().text = "Player Selected!";
            correctnessreader.GetComponent<TextMeshProUGUI> ().text = "Incorrect!";
            playerincorrectquestions++;
            
        }
    }

    
    public void HaltOpponentAction()
    {
        //stops opponent from selecting the center card, as the player already has, or has drawn the next card
        if (currentOpponentAction != null)
        {
            StopCoroutine(currentOpponentAction);    
        }
        opponentActionCanceled = true;   
    }
    public void CalculatePlay()
    {
        float chancetoPress = 0; //dif 6 = 60
        float timetopress = 0;
        if(currentcentercard.info == currentQuestion.options[currentQuestion.answerIndex].text)
        { //the current center card is the correct answer

            Debug.Log("THIS IS CORRECT"); //for us to know when its correct (we are not pharmacy majors)
            chancetoPress = (1 + minigameDifficulty) * .10f; //ie dif 6 = 60%

            timetopress = .1f + ((10 - minigameDifficulty) * .05f);; 
            currentOpponentAction = OpponentAction(timetopress, chancetoPress, true);  
            StartCoroutine(currentOpponentAction);
            StartCoroutine(OpponentContinue());
        }
        else
        { //not the correct answer
            chancetoPress = (10 - minigameDifficulty) * .025f; //ranges from .25 to 0

            timetopress = .1f + ((10 - minigameDifficulty) * .05f); // starts at .5, decreases by .05 until level 10 = .1 second delay
            currentOpponentAction = OpponentAction(timetopress, chancetoPress, false);
            StartCoroutine(currentOpponentAction);
            StartCoroutine(OpponentContinue());
        }
    }

    //patch for stopping coroutines which causes visual bug with displacement of cards
    public void MoveCardsToCorrectPile()
    {
        for(int i = 0; i < playerCards.Count; i++)
        {
            if(playerCards[i].gameObject.transform.position != playerpile.transform.position)
            {
                Debug.Log("PLAYER CARD DISPLACED");
                playerCards[i].gameObject.transform.position = playerpile.transform.position;
            }
        }
        for(int i = 0; i < opponentCards.Count; i++)
        {
            if(opponentCards[i].gameObject.transform.position != opponentpile.transform.position)
            {
                Debug.Log("OPPONENT CARD DISPLACED");
                opponentCards[i].gameObject.transform.position = opponentpile.transform.position;
            }
        }
        for(int i = 0; i < centerCards.Count; i++)
        {
            if(centerCards[i].gameObject.transform.position != centerpile.transform.position)
            {
                Debug.Log("CENTER CARD DISPLACED");
                centerCards[i].gameObject.transform.position = centerpile.transform.position;
            }
        }
    }

    public IEnumerator OpponentAction(float waittime, float chance, bool iscorrect) //opponent waits proper amount of time before reacting (stopped if player input)
    {
        yield return new WaitForSeconds(waittime);
        yield return new WaitUntil(() => awaiting == SlapjackPlay.OpponentTurn);

        int percent = (int) (chance * 100);

        int roll = Random.Range(0, 100);
        if(roll <= percent)
        {
            //select
            //Debug.Log("OPPONENT ROLLED TO PRESS, " + "CHANCE: " + percent);
            currentcentercard.Reveal(true); //outline card


            if (iscorrect)
            {
                correctnessreader.GetComponent<TextMeshProUGUI> ().text = "Correct!";
                whoselectedreader.GetComponent<TextMeshProUGUI> ().text = "Opponent Selected!";
                opponentcorrectquestions++;

                int n = centerCards.Count;
                for(int i = 0; i < n; i++) //give all cards in center to the opponent
                {
                    StartCoroutine(centerCards[0].moveFromCenter(false));
                    yield return new WaitForSeconds(cardstackdelay);
                    UpdateCardNumReaders();
                }
                DrawNextQuestionCard();
                UpdateCardNumReaders();
            }
            else
            {
                if(opponentCards.Count == 0)
                {
                    EndGame(false); // out of cards
                    yield break; //return
                }
                StartCoroutine(opponentCards[opponentCards.Count - 1].SwapSides(true));
                correctnessreader.GetComponent<TextMeshProUGUI> ().text = "Incorrect!";
                whoselectedreader.GetComponent<TextMeshProUGUI> ().text = "Opponent Selected!";
                opponentincorrectquestions++;
                opponentActionCanceled = true; //end turn
            }
        } 
        else
        {
            //Debug.Log("OPPONENT ROLLED NO, " + "CHANCE: " + percent);
            opponentDecidedNo = true;
        }
        yield return null;
    }
    
    public IEnumerator OpponentContinue() //opponent draws card 
    {
        opponentDecidedNo = false;
        opponentActionCanceled = false;
        yield return new WaitUntil(() => opponentActionCanceled || opponentDecidedNo);
        //yield return new WaitForSeconds(correctnessreadingdelay); // have delay between giving player a card and then drawing a new one (looks weird otherwise)
        yield return new WaitUntil(() => cardisMoving == false && awaiting == SlapjackPlay.OpponentTurn);
        yield return new WaitForSeconds(correctnessreadingdelay); // have delay between player drawing a card and opponent drawing a card
        MoveCardsToCorrectPile();
        Debug.Log("OPPONENT CONTINUES");

        if(opponentCards.Count == 0)
        {
            EndGame(false);
            yield break; //return
        }
        opponentCards[0].Press();
        
        correctnessreader.GetComponent<TextMeshProUGUI> ().text = "";
        whoselectedreader.GetComponent<TextMeshProUGUI> ().text = "";

    }

    public void EndGame(bool outofQuestions) //if not outofquestions, then someone ran out of cards
    {
        awaiting = SlapjackPlay.GameEnd;
        OpenEndGamePopup();
        HandleCoinsandScore();
    }
}
