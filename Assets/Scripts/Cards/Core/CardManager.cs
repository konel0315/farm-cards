using System.Collections.Generic;
using UnityEngine;

public class CardManager : IManager
{
    private CardGroupsSO cardGroupsSO;

    public void Initialize(ManagerContainer container)
    {
        cardGroupsSO = container.GetManager<CardGroupsSO>();
        if (cardGroupsSO == null)
        {
            Debug.LogWarning("CardGroupsSO를 찾을 수 없습니다!");
            return;
        }

        Debug.Log("CardManager 초기화 완료!");

        var card = GameObject.FindObjectOfType<Card>();
        if (card == null)
        {
            Debug.LogWarning("씬에 Card 컴포넌트가 없습니다!");
            return;
        }

        var soCopy = cardGroupsSO.getSoCopy();
        if (soCopy == null || soCopy.Count == 0) return;

        int randIndex = Random.Range(0, soCopy.Count);
        var randomGroup = soCopy[randIndex];

        card.CardSetup(randomGroup);
        card.openCard();
    }

}