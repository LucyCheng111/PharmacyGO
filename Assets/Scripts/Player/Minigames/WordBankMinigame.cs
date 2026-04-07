using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum WordBankPlay  //only used to determine if you can pick up the words
{
    Arrange,
    GameEnd
}

public class WordBankMinigame : MonoBehaviour
{
    public MinigamePilot pilot; //separate script to only request from github once, reducing risk of 403
    public GameObject WordBank; //main object
    public GameObject GameBoard;
    public List<Question> questions;
    public WordBankPlay awaiting = WordBankPlay.Arrange;
    public int numQuestions = 3;

    public Question currentQuestion;
    public List<WordFromBank> words = new List<WordFromBank>();
    public List<WordFromBank> answerSentence = new List<WordFromBank>(); //where you're putting them
    public GameObject WordObject; //physical word to move around prefab
    public GameObject WordInHand; //object to lock to mouse/ finger position
    public int minigameDifficulty = 0; //separate from question difficulty (below)
    //database info
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

    public GameObject exitbutton; //closes game
    public GameObject infobutton;
    public GameObject guessbutton;
    public GameObject giveUpbutton; //shows answer, gives another question (doesn't close game)
    public GameObject QuestionReader;

    private int totalgainedcoins =0;
    private int totalgainedpoints =0;

    public void StartWordBank(int d) //entrypoint, called from npcminigameplayer
    {
        minigameDifficulty = d;
        GameController.Instance.StartMinigame();
        WordBank.SetActive(true);
        totalgainedcoins = 0; //reset session
        totalgainedpoints = 0;
        RestartPlay();
    }
    
    public void CloseWindow()
    {
        DeleteWords();
        GameController.Instance.EndMinigame();
        WordBank.SetActive(false);
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
    public void RestartPlay()
    {
        DeleteWords();
        StartCoroutine(LoadWords());

        
        
    }

    //used for resetting
    public void DeleteWords()
    {
        for(int i = 0; i < words.Count; i++)
        {
            Destroy(words[i].gameObject);
        }

        words.Clear();
    }
    public void RandomizeWordAppearance()
    {
        for(int i = 0; i < words.Count; i++)
        {
            //color
            words[i].Facade.gameObject.GetComponent<Image>().color = new Color(1 - Random.Range(0,0.05f),1 - Random.Range(0,0.05f),1 - Random.Range(0,0.15f),1f);
            //rotation
            float rand = Random.Range(-5, 5);
            words[i].gameObject.transform.eulerAngles = new Vector3(0f, 0f, rand);
        }
    }
    public IEnumerator LoadWords()
    {
        yield return new WaitUntil(() => pilot.gotQuestions);
        List<string> tempwords = new List<string>();
        string currentWord = "";

        for(int i = 0; i < numQuestions; i++) //get questions and answers from database
        {
            Question q = pilot.moduleManager.GetRandomQuestion(module, (int) (difficulty / 20));
            string answer = q.options[currentQuestion.answerIndex].text;
            if(q.options[q.answerIndex].imageLink == "")
            {
                //iterate through just scanning for banned keywords
                for(int j = 0; j < answer.Length; j++) //size of string
                {
                    if(answer[j] == ' ' || answer[j]  == '\n' || answer[j]  == '\r' || j == answer.Length - 1)
                    {
                        tempwords.Add(currentWord);
                        //Debug.Log("WORD IN ANSWER: " + currentWord);
                    }
                    else
                    {
                        currentWord += answer[j] ;
                    }
                }
                //check for words
                if(tempwords.Contains("all")) //has banned word
                {
                    i--; //skip this question
                }
                else
                {
                    questions.Add(q);
                }
            }
            else //has image
            {
                i--; //skip this question
            }
            tempwords.Clear();
            currentWord = "";
        }

        SelectNextQuestion();

        string rightAnswer = currentQuestion.options[currentQuestion.answerIndex].text;
        //iterate through to actually make the word objects and assign their values
        for(int i = 0; i < rightAnswer.Length; i++) //size of string
        {
            if(rightAnswer[i] == ' ' || rightAnswer[i]  == '\n' || rightAnswer[i]  == '\r' || i == rightAnswer.Length - 1)
            {
                if(i == rightAnswer.Length - 1 && rightAnswer[i] != '.')
                {
                    currentWord += rightAnswer[i]; //get last letter, otherwise would be culled
                }
                tempwords.Add(currentWord);
                currentWord = "";
            }
            else
            {
                currentWord += rightAnswer[i] ;
            }
        }
        Debug.Log("QUESTION IS: " + currentQuestion.question);
        Debug.Log("ANSWER IS: " + rightAnswer);
        for(int i = 0; i < tempwords.Count; i++)
        {
            GameObject newWordObject = Instantiate(WordObject, getRandomGameboardPosition() , Quaternion.identity);
            newWordObject.transform.SetParent(GameBoard.transform, false);
            words.Add(newWordObject.GetComponent<WordFromBank>());

            words[i].word = tempwords[i];
            words[i].text.text = words[i].word;
            words[i].gameObject.name = "Word- " + words[i].word;
            
        }

        RandomizeWordAppearance();
    }

    public void SelectNextQuestion()
    {
        currentQuestion = questions[0];
        questions.RemoveAt(0);

        QuestionReader.GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.question;
    }

    public Vector2 getRandomGameboardPosition()
    {
        return new Vector2(Random.Range(-650f,650f), Random.Range(-325f,325f));
    }

    //actually need this to constantly attach current word the player has "picked up" to be near their mouse/finger
    public void Update() 
    {
        
    }

}
