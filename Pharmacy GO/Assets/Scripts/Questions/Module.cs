using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module
{

    //  Selects the appropriate question based on module and difficulty

    private int number_of_modules = 6;
    private int number_of_dificulties = 6;
    public List<Question>[,] questions = new List<Question>[6,6];
    private Queue<Question> queue = new Queue<Question>();

    public Module(List<Question> list)
    {
        for (int i = 0; i < number_of_modules; i++)
        {
            for (int j = 0; j < number_of_dificulties; j++)
            {
                questions[i,j] = new List<Question>();
            }
        }
        foreach (Question q in list)
        {   
            q.loadLocationData();
            questions[q.locationData.module, ((int)q.difficulty)].Add(q);
        }
            
    }

    

    public Question GetRandomQuestion(int module, Question.DifficultyIndex difficulty)
    {
        //Debug.Log("Trying mod: " + module + " | dif: " + ((int)difficulty));
        PriorityList<Question.DifficultyIndex> pl = new PriorityList<Question.DifficultyIndex>();
        pl.AddToList(Question.DifficultyIndex.Beginner, questions[module, 1].Count);
        pl.AddToList(Question.DifficultyIndex.Novice, questions[module, 2].Count);
        pl.AddToList(Question.DifficultyIndex.Intermediate, questions[module, 3].Count);
        pl.AddToList(Question.DifficultyIndex.Advanced, questions[module, 4].Count);
        pl.AddToList(Question.DifficultyIndex.Expert, questions[module, 5].Count);


        if (questions[module,((int)difficulty)].Count <= 0)
        {
            difficulty = pl.GetHighestPriority();
        }

        //Debug.Log("Selecting from mod: " + module + " | dif: " + ((int)difficulty) + " potential q's: " + questions[module,((int)difficulty)].Count);

        int index = UnityEngine.Random.Range(0, questions[module,((int)difficulty)].Count);
        Question q = questions[module, ((int)difficulty)][index];
        questions[module, ((int)difficulty)].Remove(q);
        queue.Enqueue(q);
        if (queue.Count > 4)
        {
            Question unqueued = queue.Dequeue();
            questions[unqueued.locationData.module, ((int)unqueued.difficulty)].Add(unqueued);
        }
        return q;
    }
}

