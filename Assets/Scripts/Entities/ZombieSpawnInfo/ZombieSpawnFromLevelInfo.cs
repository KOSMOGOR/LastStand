using UnityEngine;

[CreateAssetMenu(fileName = "ZombieSpawnFromLevelInfo", menuName = "Scriptable Objects/ZombieSpawnFromLevelInfo")]
public class ZombieSpawnFromLevelInfo : BaseZombieSpawnInfo
{
    public int minimumLevel = 1;

    public override bool CanSpawn() {
        return GameManager.I.currentLevel >= minimumLevel;
    }
}
