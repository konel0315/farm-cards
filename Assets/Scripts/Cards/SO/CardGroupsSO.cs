using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "CardGroupsSO", menuName = "Cards/CardGroupsSO")]
public class CardGroupsSO : ScriptableObject
{
    [SerializedDictionary]
    public SerializedDictionary<int, CardGroup> CardGroupsByID = new SerializedDictionary<int, CardGroup>();
}