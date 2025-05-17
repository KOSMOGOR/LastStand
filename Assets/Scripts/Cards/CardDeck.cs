using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDeck", menuName = "Scriptable Objects/CardDeck")]
public class CardDeck : ScriptableObject
{
    public List<CardData> cards;
}
