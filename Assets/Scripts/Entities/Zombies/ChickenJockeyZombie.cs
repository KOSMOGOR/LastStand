using UnityEngine;

public class ChickenJockeyZombie : BaseZombie
{
    public BaseZombie zombieSpawnOnDeathPrefab;

    public override void Die() {
        Tile t = tile;
        base.Die();
        SpawnZombie(zombieSpawnOnDeathPrefab, t);
    }
}
