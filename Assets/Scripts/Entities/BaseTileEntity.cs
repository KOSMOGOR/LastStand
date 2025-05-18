using UnityEngine;

public abstract class BaseTileEntity : MonoBehaviour
{
    public int maxHp;
    public int hp;
    public Tile tile;

    protected void Start() {
        hp = maxHp;
    }

    public abstract void SetTile(Tile newTile);

    public abstract void TakeDamage(int dmg, DamageType damageType, bool sendEventTakeDamage = true);

    public abstract void Die();
}

public enum DamageType {
    Shooting,
    Piercing,
    Explosive,
    Zombie,
    Obstacle
}
