using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module
{
    public List<Question>[,] questions = new List<Question>[6,6];
    public Queue<Question> queue = new Queue<Question>();

    public Module(List<Question> list)
    {
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                questions[i,j] = new List<Question>();
            }
        }
        foreach (Question q in list)
        {   
            string bin = Convert.ToString(q.locations, 2);
            while (bin.Length < 13)
            {
                bin = "0" + bin;
            }
            string binModule = bin.Substring(0, 5);
            int module = Convert.ToInt32(binModule, 2);
            // Debug.Log("m " + module + " " + binModule + " " + q.locations);
            // Debug.Log("d " + ((int)q.difficulty));
            questions[module,((int)q.difficulty)].Add(q);
        }
            
    }

    public class ReverseComparer : IComparer
    {
        // Call CaseInsensitiveComparer.Compare with the parameters reversed.
        public int Compare(System.Object x, System.Object y)
        {
            return (new CaseInsensitiveComparer()).Compare(y, x );
        }
    }


    public Question GetRandomQuestion(int module, Question.DifficultyIndex difficulty)
    {
        Debug.Log("Trying mod: " + module + " | dif: " + ((int)difficulty));
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

        Debug.Log("Selecting from mod: " + module + " | dif: " + ((int)difficulty) + " potential q's: " + questions[module,((int)difficulty)].Count);

        int index = UnityEngine.Random.Range(0, questions[module,((int)difficulty)].Count);
        //Debug.Log("index: " + index + "| count" + questions[module,((int)difficulty)].Count);
        Question q = questions[module, ((int)difficulty)][index];
        questions[module, ((int)difficulty)].Remove(q);
        queue.Enqueue(q);
        if (queue.Count > 4)
        {
            Question unqueued = queue.Dequeue();
            string bin = Convert.ToString(unqueued.locations, 2);
            while (bin.Length < 13)
            {
                bin = "0" + bin;
            }
            string binModule = bin.Substring(0, 5);
            int module2 = Convert.ToInt32(binModule, 2);
            questions[module2,((int)unqueued.difficulty)].Add(unqueued);
        }
        return q;
    }
}

