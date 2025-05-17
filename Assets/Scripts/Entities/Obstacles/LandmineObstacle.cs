using UnityEngine;

public class LandmineObstacle : BaseObstacle
{
    public int explosionDamage;

    void OnEnable() {
        Messenger<BaseZombie, Tile>.AddListener(EventMessages.ON_ZOMBIE_MOVE, OnZombieMove);
    }

    void OnDisable() {
        Messenger<BaseZombie, Tile>.RemoveListener(EventMessages.ON_ZOMBIE_MOVE, OnZombieMove);
    }

    void OnZombieMove(BaseZombie zombie, Tile newTile) {
        if (newTile == tile) {
            newTile.zombies.ForEach(z => z.TakeDamage(explosionDamage, DamageType.Obstacle));
            Die();
        }
    }
}
