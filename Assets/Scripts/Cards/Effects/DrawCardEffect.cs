public class DrawCardEffect : CardEffect
{
    public int cardsToDraw;

    public override void Activate() {
        for (int i = 0; i < cardsToDraw; ++i) Player.I.DrawCardFromDeck();
    }
}
