using UnityEngine;

public class WiretrapObstacle : BaseObstacle
{
    public int returnDamage;

    void OnEnable() {
        Messenger<BaseZombie, int, BaseObstacle>.AddListener(EventMessages.ON_ZOMBIE_DEAL_DAMAGE_TO_OBSTACLE, OnZombieDamagesObstacle);
    }

    void OnDisable() {
        Messenger<BaseZombie, int, BaseObstacle>.RemoveListener(EventMessages.ON_ZOMBIE_DEAL_DAMAGE_TO_OBSTACLE, OnZombieDamagesObstacle);
    }

    void OnZombieDamagesObstacle(BaseZombie zombie, int dmg, BaseObstacle obstacle) {
        if (obstacle != this) return;
        zombie.TakeDamage(returnDamage, DamageType.Obstacle);
    }
}
