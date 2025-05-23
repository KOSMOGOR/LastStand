using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/CardData")]
public class CardData : ScriptableObject
{
    public string cardName;
    public int aggressionCost;
    [TextArea] public string cardDescription;
    public CardType cardType;
    public Sprite cardSprite;
    public GridSelectionType gridSelectionType = GridSelectionType.None;
    public List<CardPlayCondition> playConditions = new();
    public List<CardEffect> effects = new();
}

public enum CardType {
    Damage,
    Summon,
    Extra
}
