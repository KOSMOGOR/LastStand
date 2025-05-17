public class GetPlayOpportunities : CardEffect
{
    public int playOpportunities = 1;

    public override void Activate() {
        Player.I.playOpportunities += playOpportunities;
    }
}
