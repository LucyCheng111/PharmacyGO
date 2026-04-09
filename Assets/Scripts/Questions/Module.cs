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
            // Updated so it uses the new integer difficulty field, matches Database.cs in the database repo
            questions[q.locationData.module, q.difficulty].Add(q);
        }
            
    }

    // Changed parameter from enum to int
    public Question GetRandomQuestion(int module, int difficulty)
    {        
        // Now handles integers
        PriorityList<int> pl = new PriorityList<int>();
        pl.AddToList(1, questions[module, 1].Count);
        pl.AddToList(2, questions[module, 2].Count);
        pl.AddToList(3, questions[module, 3].Count);
        pl.AddToList(4, questions[module, 4].Count);
        pl.AddToList(5, questions[module, 5].Count);

        if (questions[module, difficulty].Count <= 0)
        {
            difficulty = pl.GetHighestPriority();
        }
        
        int index = UnityEngine.Random.Range(0, questions[module, difficulty].Count);
        Question q = questions[module, difficulty][index];
        questions[module, difficulty].Remove(q);
        
        queue.Enqueue(q);
        
        if (queue.Count > 4)
        {
            Question unqueued = queue.Dequeue();
            questions[unqueued.locationData.module, unqueued.difficulty].Add(unqueued);
        }
        
        return q;
    }
}