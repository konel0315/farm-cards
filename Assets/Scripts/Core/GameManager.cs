using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{ 
        public static GameManager Instance { get; private set; }
        public ManagerContainer Container{get; private set;}

        public void Awake()
        {
                if (Instance != null && Instance != this)
                { 
                    Destroy(gameObject); 
                }
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                Container = new ManagerContainer();
                Container.Register(new CardManager());
                Container.Register(new RoundManager());
                
                
                Container.InitializeAll();
        }
        public void StartGame()
        {
            Debug.Log("게임 시작!");
            Container.GetManager<RoundManager>().StartRound();
        }
}
