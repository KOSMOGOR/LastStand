using UnityEngine;

public class BaseZombieSpawnInfo : ScriptableObject
{
    public BaseZombie prefab;
    public int aggressionCost;

    public virtual bool CanSpawn() { return true; }
}
