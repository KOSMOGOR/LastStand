using UnityEngine;

public class BaseObstacle : BaseTileEntity
{
    public bool blocking = false;
    public bool blockingProjectiles = false;

    public override void SetTile(Tile newTile) {
        if (tile != null) tile.obstacles.Remove(this);
        tile = newTile;
        tile.obstacles.Add(this);
        transform.position = tile.transform.position + tile.zombieOffset;
    }

    public override void TakeDamage(int dmg, DamageType damageType, bool sendEventTakeDamage = true) {
        if (hp <= 0) return;
        hp -= dmg;
        if (sendEventTakeDamage) Messenger<BaseObstacle, int, DamageType>.Broadcast(EventMessages.ON_OBSTACLE_TAKE_DAMAGE, this, dmg, damageType);
        if (hp <= 0) Die();
    }

    public override void Die() {
        Messenger<BaseObstacle>.Broadcast(EventMessages.ON_OBSTACLE_DIE, this);
        SetTile(null);
        Destroy(gameObject);
    }

    public static void SpawnObstacle(BaseObstacle prefab, Tile tile) {
        Instantiate(prefab).SetTile(tile);
    }
}
