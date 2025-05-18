using System;
using UnityEngine;

public class BaseZombie : BaseTileEntity
{
    public float speed;
    public int damage;
    [SerializeField] float progression = 0;
    public float progressionThreshold = 1;

    public bool stunned = false;

    SpriteRenderer sprite;

    void Awake() {
        sprite = GetComponent<SpriteRenderer>();
    }

    public override void SetTile(Tile newTile) {
        if (tile != null) tile.zombies.Remove(this);
        tile = newTile;
        if (tile != null) {
            tile.zombies.Add(this);
            transform.position = tile.transform.position + tile.zombieOffset;
        }
    }

    public void Progress() {
        float currentSpeed = speed;
        Messenger<BaseZombie>.Broadcast<float>(EventMessages.EVALUATE_ZOMBIE_SPEED, this, ds => currentSpeed += ds);
        currentSpeed = Mathf.Max(currentSpeed, 0.25f);
        progression += currentSpeed;
    }

    public bool CanTakeTurn() {
        return progression >= progressionThreshold;
    }

    public void TakeTurn() {
        if (!CanTakeTurn()) return;
        progression -= progressionThreshold;
        Tile newTile = GridManager.I.GetTileSafe(tile.xy + MovementDirection.DOWN);
        BaseObstacle obstacle = newTile.GetFirstBlockingObstacle();
        int currentDamage = damage;
        Messenger<BaseZombie>.Broadcast<int>(EventMessages.EVALUATE_ZOMBIE_DAMAGE, this, dd => currentDamage += dd);
        currentDamage = Math.Max(currentDamage, 1);
        if (tile.xy.y == 0 && newTile.xy.y == 0) Player.I.TakeDamage(currentDamage);
        else if (obstacle != null) {
            obstacle.TakeDamage(currentDamage, DamageType.Zombie);
            Messenger<BaseZombie, int, BaseObstacle>.Broadcast(EventMessages.ON_ZOMBIE_DEAL_DAMAGE_TO_OBSTACLE, this, currentDamage, obstacle);
        }
        else TryMove(newTile);
    }
    
    public void TryMove(Tile newTile) {
        if (stunned) {
            stunned = false;
            return;
        }
        bool canMove = true;
        Messenger<BaseZombie, Tile>.Broadcast<bool>(EventMessages.ON_ZOMBIE_TRY_MOVE, this, newTile, x => canMove &= x);
        if (canMove) {
            SetTile(newTile);
            Messenger<BaseZombie, Tile>.Broadcast(EventMessages.ON_ZOMBIE_MOVE, this, newTile);
        }
    }

    public override void TakeDamage(int dmg, DamageType damageType, bool sendEventTakeDamage = true) {
        if (hp <= 0) return;
        hp -= dmg;
        if (sendEventTakeDamage) Messenger<BaseZombie, int, DamageType>.Broadcast(EventMessages.ON_ZOMBIE_TAKE_DAMAGE, this, dmg, damageType);
        if (hp <= 0) Die();
    }

    public override void Die() {
        Messenger<BaseZombie>.Broadcast(EventMessages.ON_ZOMBIE_DIE, this);
        SetTile(null);
        Destroy(gameObject);
    }

    public static void SpawnZombie(BaseZombie prefab, Tile tile) {
        Instantiate(prefab).SetTile(tile);
    }

    public void SetVisible(bool visible) {
        sprite.enabled = visible;
    }
}
