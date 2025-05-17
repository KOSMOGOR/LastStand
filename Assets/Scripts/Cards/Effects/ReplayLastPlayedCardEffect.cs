using System.Collections.Generic;
using System.Linq;

public class ReplayLastPlayedCardEffect : CardEffect
{
    public List<CardType> cardTypesInclude = new();
    public List<CardData> cardDatasExlude = new();

    public override void Activate() {
        List<Card> cards = Player.I.cardsPlayedThisTurn.Where(c => cardTypesInclude.Contains(c.cardData.cardType) && !cardDatasExlude.Contains(c.cardData)).ToList();
        if (cards.Count > 0) cards.Last().Play();
    }
}
