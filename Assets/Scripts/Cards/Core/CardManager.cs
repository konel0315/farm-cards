using System.Collections.Generic;
using UnityEngine;

public class CardManager : IManager
{
    private CardGroupsSO cardGroupsSO;
    private CardFactory cardFactory;
    private Card card;

    private int cardsRemaining = 0; // 현재 Flow에서 남은 카드 수

    public void Initialize(ManagerContainer container)
    {
        cardGroupsSO = container.GetManager<CardGroupsSO>();
        if (cardGroupsSO == null) return;

        var soCopy = cardGroupsSO.getSoCopy();
        cardFactory = new CardFactory(soCopy);

        card = GameObject.FindObjectOfType<Card>();
        if (card == null) return;

        // 카드 선택 완료 이벤트 구독
        card.OnCardChoiceCompleted += OnCardSelected;
    }

    // -----------------------------
    // Flow 시작
    // -----------------------------
    public void StartFlow() => BeginFlow(new int[] {0}, 2);
    public void SpringFlow() => BeginFlow(new int[] {1,2}, 5);
    public void SummerFlow() => BeginFlow(new int[] {1,3}, 5);
    public void AutumnFlow() => BeginFlow(new int[] {1,4}, 5);
    public void WinterFlow() => BeginFlow(new int[] {1,5}, 5);
    public void MarketFlow() => BeginFlow(new int[] {6}, 2);

    private void BeginFlow(int[] seasons, int numCards)
    {
        cardFactory.InitializeAvailablePool();
        foreach(var season in seasons)
            cardFactory.AddAvailablePool(season);

        cardsRemaining = numCards;
        ShowRandomCard();
    }

    // -----------------------------
    // 카드 선택 완료 이벤트
    // -----------------------------
    private void OnCardSelected()
    {
        cardsRemaining--;

        if (cardsRemaining > 0)
        {
            ShowRandomCard();
        }
        else
        {
            Debug.Log("이번 Flow 카드 모두 선택 완료!");
        }
    }

    // -----------------------------
    // 랜덤 카드 출력
    // -----------------------------
    private void ShowRandomCard()
    {
        var randomGroup = cardFactory.GetRandomCard();
        if (randomGroup != null)
        {
            card.CardSetup(randomGroup);
            card.OpenCardWithAnimation();
        }
        else
        {
            Debug.Log("선택된 풀에서 더 이상 카드를 가져올 수 없습니다.");
        }
    }
}

