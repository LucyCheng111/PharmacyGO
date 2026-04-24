using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net;

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
    public GameObject sentenceArea;
    public Transform sentenceOrigin; //starting point for sentence objects
    public List<Question> questions;
    public WordBankPlay awaiting = WordBankPlay.Arrange;

    public int numQuestions = 5;

    public Question currentQuestion;
    public List<WordFromBank> words = new List<WordFromBank>();
    public List<WordFromBank> answerSentence = new List<WordFromBank>(); //where you're putting them
    public List<GameObject> placeholderObjects = new List<GameObject>(); //template squares to show how many words
    public GameObject placeholderObject; //prefab for above
    public GameObject WordObject; //physical word to move around prefab
    public GameObject WordInHand; //object to lock to mouse/ finger position
    public int minigameDifficulty = 0; //separate from question difficulty (below)
    //database info
    private int difficulty = 0; 

    int numCoinsToGain = 0;
    int incorrectAnswers = 0;
    private int module = 0;

    //information screens/ popups
    public GameObject infoscreen;
    public GameObject firsttimeinfopopup; //information that only appears in the info screen the first time the player opens the minigame
    public GameObject gameendScreen;
    public TextMeshProUGUI finalcointext; //coins got this round
    public TextMeshProUGUI totalcointext; //coins got since opening the minigame
    public TextMeshProUGUI finalscoretext; //leaderboard for this score (2 questions correct, etc.)

    public TextMeshProUGUI lastquestiontext;
    public TextMeshProUGUI itscorrectanswertext;
    public TextMeshProUGUI yougavetext;
    public TextMeshProUGUI confusedtext;
    public GameObject lastQuestionInfo; //in info screen

    public TextMeshProUGUI scorereader; //constant score reader
    public GameObject exitbutton; //closes game
    public GameObject infobutton;
    public GameObject guessbutton;
    public GameObject QuestionReader;

    private int totalgainedcoins =0;
    private int totalgainedpoints =0;

    public void StartWordBank(int d, int level) //entrypoint, called from npcminigameplayer
    {
        module = level;
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
    public void RestartPlay()
    {
        numCoinsToGain = 0;
        incorrectAnswers = 0;

        DeleteWords();
        StartCoroutine(getQuestionList());
        StartCoroutine(LoadWords());

        scorereader.text = "Correct Solutions: " + numCoinsToGain + "\nIncorrect Solutions: "
            + incorrectAnswers + "\nQuestions Left: " + 5;
    }

    //used for resetting
    public void DeleteWords()
    {
        for(int i = 0; i < words.Count; i++)
        {
            Destroy(words[i].gameObject);
        }
        for(int i = 0; i < answerSentence.Count; i++)
        {
            Destroy(answerSentence[i].gameObject);
        }
        for(int i = 0; i < placeholderObjects.Count; i++)
        {
            Destroy(placeholderObjects[i]);
        }

        words.Clear();
        answerSentence.Clear();
        placeholderObjects.Clear();
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
    public void updateLastQuestionTexts(string yougave, string correctanswer, string q)
    {
        yougavetext.text = "You gave: " + yougave;
        lastquestiontext.text = "Last question: " + q;
        itscorrectanswertext.text = "Its correct answer: " + correctanswer;

    }
    public void CheckSentence()
    {
        string sentence = "";

        for(int i = 0; i <answerSentence.Count; i++)
        {
            sentence += answerSentence[i].text.text;
            if(i != answerSentence.Count - 1)
            {
                sentence += " "; //spacer not on last word
            }
        }
        if (currentQuestion.options[currentQuestion.answerIndex].text == sentence)
        {
            confusedtext.gameObject.SetActive(false);
            lastQuestionInfo.SetActive(false);
            numCoinsToGain++; //give a coin
            scorereader.text = "Correct Solutions: " + numCoinsToGain + "\nIncorrect Solutions: "
            + incorrectAnswers + "\nQuestions Left: " + questions.Count;

        }
        else
        {
            confusedtext.gameObject.SetActive(true);
            lastQuestionInfo.SetActive(true);
            incorrectAnswers++;
            scorereader.text = "Correct Solutions: " + numCoinsToGain + "\nIncorrect Solutions: "
            + incorrectAnswers + "\nQuestions Left: " + questions.Count;
        }

        updateLastQuestionTexts(sentence, currentQuestion.options[currentQuestion.answerIndex].text, currentQuestion.question);
        DeleteWords();
        SelectNextQuestion();
        StartCoroutine(LoadWords());

    }
    public IEnumerator getQuestionList()
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
    }

    public IEnumerator LoadWords()
    {
        yield return new WaitUntil(() => pilot.gotQuestions);
        List<string> tempwords = new List<string>();
        string currentWord = "";
        string rightAnswer = currentQuestion.options[currentQuestion.answerIndex].text;
        //iterate through to actually make the word objects and assign their values
        //correct words
        for(int i = 0; i < rightAnswer.Length; i++) //size of string
        {
            if(rightAnswer[i] == ' ' || rightAnswer[i]  == '\n' || rightAnswer[i]  == '\r' || i == rightAnswer.Length - 1)
            {
                if(i == rightAnswer.Length - 1)
                {
                    currentWord += rightAnswer[i]; //get last letter, otherwise would be culled
                }
                tempwords.Add(currentWord);
                currentWord = "";

                //make a placeholder in the sentence
                int n = placeholderObjects.Count;
                Vector2 position = new Vector2((sentenceOrigin.position.x + ((n * 140) % 840)) , 
                (sentenceOrigin.position.y - ((n * 140) / 840) * 90)  );
                GameObject placeholder = Instantiate(placeholderObject, position, Quaternion.identity);
                placeholder.transform.SetParent(sentenceArea.transform, true);
                placeholderObjects.Add(placeholder);
            }
            else
            {
                currentWord += rightAnswer[i] ;
            }
        }

        int qtotal = pilot.randomQuestions.Count;
        Question randQ;
        List<string> randWords = new List<string>();

        //get other random words to make it a challenge
        int numWordsInRightAnswer = tempwords.Count;
        for(int i = 0; i < minigameDifficulty * 3; i++)
        {
            //will get the # of words in the correct answer + (the difficulty * 3)
            randQ = pilot.randomQuestions[Random.Range(0,qtotal)]; //random Question
            int randO = Random.Range(0,randQ.options.Count);

            string randS = randQ.options[Random.Range(0,randO)].text; //random answer (doesnt matter if right or not)
            string word = "";
            
            if(randS == "")
            {
                //if the answer is an image link, then there isn't text here
                //instead of getting another, we'll skip, and the player will get 1 less word
                //this makes the challenge a little randomized too, and so there won't always be the same number-
                //of words for a given sentence
                continue;
            }
            //get words from string
            for(int l = 0; l < randS.Length; l++)
            {
                if(randS[l] == ' ' || randS[l] == '\n' || randS[l] == '\r' || l == randS.Length - 1)
                {
                    if(l == randS.Length - 1)
                    {
                        word += randS[l];
                    }
                    randWords.Add(word);
                    word = "";
                }
                else
                {
                    word += randS[l];
                }
            }

            word = randWords[Random.Range(0, randWords.Count)];

            for(int t = 0; t < 3; t++)
            {
                word = randWords[Random.Range(0, randWords.Count)];
                if (!tempwords.Contains(word))
                {
                    tempwords.Add(word); //add to list
                }
            }
            

            randWords.Clear();
        }

        Debug.Log("ANSWER IS: " + rightAnswer);
        for(int i = 0; i < tempwords.Count; i++)
        {
            GameObject newWordObject = Instantiate(WordObject, getRandomGameboardPosition() , Quaternion.identity);
            newWordObject.transform.SetParent(GameBoard.transform, false);
            words.Add(newWordObject.GetComponent<WordFromBank>());

            words[i].word = tempwords[i];
            words[i].text.text = words[i].word;
            words[i].gameObject.name = "Word- " + words[i].word;
            words[i].indexInWords = i;
            
        }

        RandomizeWordAppearance();
    }

    public void SelectNextQuestion()
    {
        if(questions.Count > 0)
        {
            currentQuestion = questions[0];
            questions.RemoveAt(0);

            QuestionReader.GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.question;
        }
        else //game over
        {
            OpenEndGamePopup();
            HandleCoinsandScore();
        }
    }
    public void HandleCoinsandScore()
    {

        int scoreToGain = numCoinsToGain * 1000;
        ScoreManager.Instance.AddMinigameScore(scoreToGain);
        CoinManager.Instance.AddCoin(numCoinsToGain);

        totalgainedcoins += numCoinsToGain;
        totalgainedpoints += scoreToGain;
        finalscoretext.text = "Correct Solutions: " + numCoinsToGain + "\nIncorrect Solutions: " + incorrectAnswers;

        if(numCoinsToGain == 1)
        {
            finalcointext.text = "You got:\n" + numCoinsToGain + " Coin and +" + scoreToGain + " Points"; 
        }
        else
        {
            finalcointext.text = "You got:\n" + numCoinsToGain + " Coins and +" + scoreToGain + " Points"; 
        }
        totalcointext.text = "That makes a total of:\n" + totalgainedcoins + " Coins and +" + totalgainedpoints + " Points"; 
    }
    public Vector2 getRandomGameboardPosition()
    {
        return new Vector2(Random.Range(-650f,650f), Random.Range(-325f,325f));
    }

    public void ReprintSentence()
    {
        for(int i = 0; i < answerSentence.Count; i++)
        {
            answerSentence[i].transform.SetParent(sentenceArea.transform, true);
            answerSentence[i].gameObject.transform.position = new Vector2((sentenceOrigin.position.x + ((i * 140) % 840)) , 
                (sentenceOrigin.position.y - ((i * 140) / 840) * 90)  );
            answerSentence[i].indexInSentence = i; //index in sentence
            answerSentence[i].indexInWords = -1;
        }
    }

    //exchange words from sentence and board
    public void exchangeBoardandSentence(Transform word)
    {
        GameObject identifier = FindNearestObjectToWord(word);
        if (identifier)
        {
            if(identifier == sentenceArea)
            {
                if(word.gameObject.GetComponent<WordFromBank>().indexInWords != -1)
                {
                    //append to sentence
                    words.RemoveAt(word.gameObject.GetComponent<WordFromBank>().indexInWords);
                    answerSentence.Add(word.gameObject.GetComponent<WordFromBank>());
                }
                else //was already in sentence, just reappend
                {
                    answerSentence.RemoveAt(word.gameObject.GetComponent<WordFromBank>().indexInSentence);
                    answerSentence.Add(word.gameObject.GetComponent<WordFromBank>());
                }
            }
        }
        else //remove from sentence
        {
            if(word.gameObject.GetComponent<WordFromBank>().indexInSentence != -1)
            {
                answerSentence.RemoveAt(word.gameObject.GetComponent<WordFromBank>().indexInSentence);
                words.Add(word.gameObject.GetComponent<WordFromBank>());
            }
            // else already on game board, just place it down again
    
        }
        reIndex();
        ReprintSentence();
    }

    public void reIndex()
    {
        for(int i = 0; i < answerSentence.Count; i++)
        {
            answerSentence[i].indexInSentence = i;
            answerSentence[i].indexInWords = -1;
        }

        for(int i = 0; i < words.Count; i++)
        {
            words[i].indexInSentence = -1;
            words[i].indexInWords = i;
        }
    }
    //in sentence
    public GameObject FindNearestObjectToWord(Transform word)
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        Vector3 raystart = new Vector3(word.position.x, word.position.y, word.position.z - 15);
        if (Physics.Raycast(raystart, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            if(hit.transform.gameObject.GetComponent<SentenceSpacer>())
            {
                Debug.Log("OVER SPACER");   
                return hit.transform.gameObject;
            }
            else if(hit.transform == sentenceArea.transform )
            {
                //append to end of sentence
                return sentenceArea;
            }
            else
            {
                Debug.Log("NOT IN SENTENCE"); 
                return null;
            }
        }
        return null;
    }

    //actually need this to constantly attach current word the player has "picked up" to be near their mouse/finger
    //called in GameControl.cs
    public void HandleUpdate() 
    {
        if(WordInHand != null)
        {
            if (Application.isMobilePlatform) 
            {
                WordInHand.transform.position = Input.GetTouch(0).position;
            }
            else
            {
                WordInHand.transform.position = Input.mousePosition;
            }
        }
    }

}
