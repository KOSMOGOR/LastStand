public class DealAdditionalDamageEffect : CardEffect
{
    public float multiplier = 1;
    public int numberOfTimesToActivate = 1;
    public int additionalFlatDamage = 0;
    public DamageType damageType;

    public override void Activate() {
        int activationCount = 0;
        void OnTakeDamage(BaseZombie zombie, int dmg, DamageType damageType) {
            if (damageType == this.damageType) {
                activationCount += 1;
                if (dmg * multiplier != 0) zombie.TakeDamage((int)(dmg * multiplier), damageType, false);
                if (additionalFlatDamage != 0) zombie.TakeDamage(additionalFlatDamage, damageType, false);
                if (activationCount == numberOfTimesToActivate) {
                    RemoveListeners();
                }
            }
        }
        void RemoveListeners() {
            Messenger<BaseZombie, int, DamageType>.RemoveListener(EventMessages.ON_ZOMBIE_TAKE_DAMAGE, OnTakeDamage);
            Messenger.RemoveListener(EventMessages.ON_PLAYER_END_TURN, RemoveListeners);
        }
        Messenger<BaseZombie, int, DamageType>.AddListener(EventMessages.ON_ZOMBIE_TAKE_DAMAGE, OnTakeDamage);
        Messenger.AddListener(EventMessages.ON_PLAYER_END_TURN, RemoveListeners);
    }
}
