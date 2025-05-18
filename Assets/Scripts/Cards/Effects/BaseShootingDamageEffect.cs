using System.Collections.Generic;
using System.Linq;

public class BaseShootingDamageEffect : CardEffect
{
    public int damage;

    public static void ApplyShootingDamage(Tile tile, int dmg, int startingY = 0) {
        List<Tile> tiles = GridManager.I.GetAllTilesInColumn(tile.xy.x).Where(t => t.xy.y >= startingY).ToList();
        foreach (Tile t in tiles) {
            BaseObstacle obstacle = t.GetFirstBlockingProjectilesObstacle();
            if (obstacle) { obstacle.TakeDamage(dmg, DamageType.Shooting); break; }
            BaseZombie zombie = t.GetFirstZombie();
            if (zombie) { zombie.TakeDamage(dmg, DamageType.Shooting); break; }
        }
    }

    public override void Activate() {
        Tile selectedTile = GridManager.I.currentSelectedTile;
        ApplyShootingDamage(selectedTile, damage);
    }
}
