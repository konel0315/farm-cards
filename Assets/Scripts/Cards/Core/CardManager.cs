using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CardManager : IManager
{
    private CardGroupsSO cardGroupsSO;
    private CardFactory cardFactory;
    private Card card;

    private int cardsRemaining = 0; // 현재 Flow에서 남은 카드 수
    private Queue<System.Action> flowQueue;

    // -----------------------------
    // 초기화 (DI 사용)
    // -----------------------------
    public void Initialize(ManagerContainer container)
    {
        cardGroupsSO = container.GetManager<CardGroupsSO>();
        card = container.GetManager<Card>();

        if (cardGroupsSO == null || card == null)
        {
            Debug.LogWarning("CardManager 초기화 실패: CardGroupsSO 또는 Card가 없습니다.");
            return;
        }

        // 이벤트 구독 (중복 방지)
        card.OnCardChoiceCompleted -= OnCardSelected;
        card.OnCardChoiceCompleted += OnCardSelected;

        // CardFactory 초기화
        var soCopy = cardGroupsSO.getSoCopy();
        cardFactory = new CardFactory(soCopy);
    }

    // -----------------------------
    // Round용 카드 복사본 초기화
    // -----------------------------
    public void InitializeRoundCopy()
    {
        var originalData = cardGroupsSO.getSoCopy();
        var copyData = new Dictionary<int, CardGroup>();

        foreach (var kvp in originalData)
        {
            copyData[kvp.Key] = new CardGroup
            {
                Title = kvp.Value.Title,
                Name = kvp.Value.Name,
                TitleImage = kvp.Value.TitleImage,
                Description = kvp.Value.Description,
                Priority = kvp.Value.Priority,
                Weight = kvp.Value.Weight,
                OneTime = kvp.Value.OneTime,
                SeasonType = kvp.Value.SeasonType,
                Options = kvp.Value.Options
            };
        }

        cardFactory = new CardFactory(copyData);
    }

    // -----------------------------
    // Flow 시작
    // -----------------------------
    public void StartFlow() => BeginFlow(new int[] { 0 }, 2);
    public void SpringFlow() => BeginFlow(new int[] { 1, 2 }, 5);
    public void SummerFlow() => BeginFlow(new int[] { 1, 3 }, 5);
    public void AutumnFlow() => BeginFlow(new int[] { 1, 4 }, 5);
    public void WinterFlow() => BeginFlow(new int[] { 1, 5 }, 5);
    public void MarketFlow() => BeginFlow(new int[] { 6 }, 2);

    // -----------------------------
    // Round 전체 흐름 순차 실행
    // -----------------------------
    public void StartRoundFlowQueue()
    {
        flowQueue = new Queue<System.Action>();
        flowQueue.Enqueue(StartFlow);
        flowQueue.Enqueue(SpringFlow);
        flowQueue.Enqueue(SummerFlow);
        flowQueue.Enqueue(AutumnFlow);
        flowQueue.Enqueue(WinterFlow);
        flowQueue.Enqueue(MarketFlow);

        StartNextFlow();
    }

    private void StartNextFlow()
    {
        if (flowQueue == null || flowQueue.Count == 0)
        {
            Debug.Log("모든 Flow 종료!");
            return;
        }

        var flow = flowQueue.Dequeue();
        flow.Invoke();
    }

    // -----------------------------
    // 카드 Flow 실행
    // -----------------------------
    private void BeginFlow(int[] seasons, int numCards)
    {
        Debug.Log($"Flow 시작: Seasons={string.Join(",", seasons)}, NumCards={numCards}");

        cardFactory.InitializeAvailablePool();
        foreach (var season in seasons)
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
            StartNextFlow(); // 남은 흐름 진행
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
            Debug.LogWarning("선택된 풀에서 더 이상 카드를 가져올 수 없습니다.");
        }
    }
}
