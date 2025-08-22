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
        }
        
}
