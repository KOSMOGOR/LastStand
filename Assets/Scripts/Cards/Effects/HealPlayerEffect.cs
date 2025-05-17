public class HealPlayerEffect : CardEffect
{
    public int healAmount;

    public override void Activate() {
        Player.I.playerHp += healAmount;
    }
}
