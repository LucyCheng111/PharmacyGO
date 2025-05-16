using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PriorityList<T>
{
    //A class that associates a value with a priority.
    [System.Serializable]
    public class ListValue : IComparable<ListValue>
    {
        public T value;
        public float priority;

        public ListValue (T value, float priority)
        {
            this.value = value;
            this.priority = priority;
        }

        public int CompareTo (ListValue other)
        {
            return priority.CompareTo (other.priority);
        }
    }

    [Tooltip ("The priority list.")]
    public List<ListValue> list = new List<ListValue> ();

    // Returns index
    public int BinarySearch (float targetPriority, int minBoundsIndex, int maxBoundsIndex)
    {
        int boundsDistance = maxBoundsIndex - minBoundsIndex;
        if (boundsDistance <= 0)
        {
            return minBoundsIndex;
        }
        int midpointIndex = boundsDistance / 2 + minBoundsIndex;
        if (list[midpointIndex].priority == targetPriority)
        {
            return midpointIndex;
        }
        else if (list[midpointIndex].priority < targetPriority)
        {
            // Check upper half
            minBoundsIndex = midpointIndex + 1;
            return BinarySearch (targetPriority, minBoundsIndex, maxBoundsIndex);
        }
        else
        {
            // Check lower half
            maxBoundsIndex = midpointIndex;
            return BinarySearch (targetPriority, minBoundsIndex, maxBoundsIndex);
        }
    }

    public void AddToList (T value, float priority)
    {
        ListValue newValue = new ListValue (value, priority);
        if (list.Count == 0)
        {
            list.Add (newValue);
        }
        else
        {
            list.Insert (BinarySearch (priority, 0, list.Count), newValue);
        }
    }
    public void RemoveFromList (int index)
    {
        list.RemoveAt (index);
    }

    public T GetHighestPriority ()
    {
        if (list.Count == 0)
            return default(T);

        return list[list.Count - 1].value;
    }
    public T GetLowestPriority ()
    {
        if (list.Count == 0)
            return default (T);

        return list[0].value;
    }
}
