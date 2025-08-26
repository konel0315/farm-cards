using UnityEngine;

public class RoundManager : IManager
{
    private CardManager cardManager;
    
    public void Initialize(ManagerContainer container)
    {
        cardManager = container.GetManager<CardManager>();
    }
    
    public void StartRound()
    {
        Debug.Log("1라운드!");
        
        cardManager.InitializeRoundCopy();
        
        cardManager.StartRoundFlowQueue();
    }
}