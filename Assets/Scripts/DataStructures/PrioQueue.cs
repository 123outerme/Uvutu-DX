using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NOTE: my shoddy implementation of .NET PriorityQueue. Not as performant in all likelihood

public interface PrioQueueComparer<T>
{
    int Compare(T x, T y);
}

public class DefaultPrioQueueComparer<T> : PrioQueueComparer<T>
{
    public int Compare(T x, T y)
    {
        return 0; //No idea how to cleanly provide a default comparer without IComparable (which doesn't seem to exist in Unity C#???)
    }
}

class PrioQueueItem<TElement, TPriority>
{
    public TElement element;
    public TPriority priority;

    public PrioQueueItem(TElement e, TPriority p)
    {
        element = e;
        priority = p;
    }
}

public class PrioQueue<TElement, TPriority>
{
    private PrioQueueComparer<TPriority> comparer;
    private List<PrioQueueItem<TElement, TPriority>> queueList;

    public PrioQueue()
    {
        comparer = new DefaultPrioQueueComparer<TPriority>();
        queueList = new List<PrioQueueItem<TElement, TPriority>>();
    }

    public PrioQueue(PrioQueueComparer<TPriority> elementComparer)
    {
        comparer = elementComparer;
        queueList = new List<PrioQueueItem<TElement, TPriority>>();
    }

    public void Enqueue(TElement elem, TPriority prio)
    {
        queueList.Add(new PrioQueueItem<TElement, TPriority>(elem, prio));
    }

    public TElement Dequeue()
    {
        if (queueList.Count > 0)
        {
            PrioQueueItem<TElement, TPriority> highestPrio = queueList[0];
            int highestIndex = 0;
            for(int i = 1; i < queueList.Count; i++)
            {
                if (comparer.Compare(highestPrio.priority, queueList[i].priority) < 0)  //if highestPrio isn't the highest priority item
                {
                    highestPrio = queueList[i];
                    highestIndex = i;
                }
            }
            queueList.RemoveAt(highestIndex);  //pop off the list and reutrn the element
            return highestPrio.element;
        }
        else
            return default(TElement);
    }

    public TElement Peek()
    {
        if (queueList.Count > 0)
        {
            PrioQueueItem<TElement, TPriority> highestPrio = queueList[0];
            for(int i = 1; i < queueList.Count; i++)
            {
                if (comparer.Compare(highestPrio.priority, queueList[i].priority) < 0)  //if highestPrio isn't the highest priority item
                    highestPrio = queueList[i];
            }
            return highestPrio.element;
        }
        else
            return default(TElement);
    }

    public void Clear()
    {
        queueList.Clear();
    }

    public int GetCount()
    {
        return queueList.Count;
    }
}
