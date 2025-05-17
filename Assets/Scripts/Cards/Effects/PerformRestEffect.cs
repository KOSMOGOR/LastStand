public class PerformRestEffect : CardEffect
{
    public override void Activate() {
        Player.I.MakeRest();
    }
}
