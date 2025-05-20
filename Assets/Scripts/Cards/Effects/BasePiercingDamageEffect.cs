using System;
using System.Collections.Generic;
using System.Linq;

public class BasePiercingDamageEffect : CardEffect
{
    public int damage;

    public static void ApplyPiercingDamage(Tile tile, int dmg, int startingY = 0) {
        List<Tile> tiles = GridManager.I.GetAllTilesInColumn(tile.xy.x).Where(t => t.xy.y >= startingY).ToList();
        foreach (Tile t in tiles) {
            foreach (BaseObstacle obstacle in new List<BaseObstacle>(t.obstacles)) {
                if (obstacle.blockingProjectiles) {
                    int dealtDamage = (int)Math.Ceiling((double)dmg / 2);
                    obstacle.TakeDamage(dealtDamage, DamageType.Piercing);
                    dmg -= dealtDamage;
                    if (dmg == 0) return;
                }
            }
            foreach (BaseZombie zombie in new List<BaseZombie>(t.zombies)) {
                int dealtDamage = (int)Math.Ceiling((double)dmg / 2);
                zombie.TakeDamage(dealtDamage, DamageType.Piercing);
                dmg -= dealtDamage;
                if (dmg == 0) return;
            }
        }
    }

    public override void Activate() {
        Tile selectedTile = GridManager.I.currentSelectedTile;
        ApplyPiercingDamage(selectedTile, damage);
        AudioManager.I.PlaySound(SoundType.Riffle);
    }
}
