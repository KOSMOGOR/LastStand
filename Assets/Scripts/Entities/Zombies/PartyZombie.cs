using UnityEngine;

public class PartyZombie : BaseZombie
{
    public BaseObstacle obstacleSpawnOnDeathPrefab;

    public override void Die() {
        Tile t = tile;
        base.Die();
        BaseObstacle.SpawnObstacle(obstacleSpawnOnDeathPrefab, t);
    }
}
