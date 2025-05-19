using UnityEngine;

[CreateAssetMenu(fileName = "BaseZombieSpawnInfo", menuName = "Scriptable Objects/BaseZombieSpawnInfo")]
public class BaseZombieSpawnInfo : ScriptableObject
{
    public BaseZombie prefab;

    public virtual bool CanSpawn() { return true; }
}
