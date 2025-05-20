using System.Collections.Generic;

public class RePlayTopDiscardCardEffect : CardEffect
{
    public List<CardData> cardDatasToIgnore;

    public override void Activate() {
        if (Player.I.discardPile.Count == 0) return;
        Card card = Player.I.discardPile[^1];
        if (cardDatasToIgnore.Contains(card.cardData)) return;
        card.Play();
    }
}
