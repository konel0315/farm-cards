using System.Collections.Generic;
using UnityEngine;

public class CardFactory
{
    private Dictionary<int, Dictionary<int, CardGroup>> cardPools;
    public List<CardGroup> AvailablePool = new List<CardGroup>();
    public CardFactory(Dictionary<int,CardGroup> Dic)
    {
        cardPools = new Dictionary<int, Dictionary<int, CardGroup>>();
        
        Dictionary<int, int> seasonCounters = new Dictionary<int, int>();
        
        foreach (var kvp in Dic.Values)
        {
            int season = kvp.SeasonType; // 엑셀에서 넣어둔 값 (0=시작,1=범용,2=봄…)

            if (!cardPools.ContainsKey(season))
            {
                cardPools[season] = new Dictionary<int, CardGroup>();
                seasonCounters[season] = 0;
            }

            int index = seasonCounters[season];
            cardPools[season].Add(index, kvp);
            seasonCounters[season]++;
        }
    }
    
    public void AddAvailablePool(int season)
    {
        if (cardPools.ContainsKey(season))
            AvailablePool.AddRange(cardPools[season].Values);
    }

    public void RemoveAvailablePool(int season)
    {
        AvailablePool.RemoveAll(card => card.SeasonType == season);
    }

    public void InitializeAvailablePool()
    {
        AvailablePool.Clear();
    }

    public CardGroup GetRandomCard()
    {
        var selectablePool = AvailablePool.FindAll(c => c.OneTime != false);
        if (selectablePool.Count == 0)
            return null;
        
        int minPriority = int.MaxValue;
        foreach (var card in selectablePool)
        {
            int priority = card.Priority;
            if (priority < minPriority)
                minPriority = priority;
        }
        
        var priorityPool = selectablePool.FindAll(c => c.Priority == minPriority);
        
        int totalWeight = 0;
        foreach (var card in priorityPool)
            totalWeight += card.Weight;

        int rand = Random.Range(0, totalWeight);
        int cumulative = 0;

        foreach (var card in priorityPool)
        {
            cumulative += card.Weight;
            if (rand < cumulative)
            {
                if (card.OneTime == true)
                    card.OneTime = false;

                return card;
            }
        }

        return null;
    }

}