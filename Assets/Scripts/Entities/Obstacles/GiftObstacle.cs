using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GiftObstacle : BaseObstacle
{
    public string spawnerZombieName = "PartyZombie";

    public override void Die() {
        Tile t = tile;
        base.Die();
        List<BaseZombie> allZombieCanSpawnInfos = GameManager.I.zombieSpawnInfos.Where(zsi => zsi.CanSpawn()).Select(zsi => zsi.prefab).ToList();
        BaseZombie zombieToSpawn = allZombieCanSpawnInfos.Where(z => z.name == spawnerZombieName).First();
        allZombieCanSpawnInfos.Remove(zombieToSpawn);
        if (Random.value > 0.01) zombieToSpawn = allZombieCanSpawnInfos[Random.Range(0, allZombieCanSpawnInfos.Count)];
        BaseZombie.SpawnZombie(zombieToSpawn, t);
    }
}
