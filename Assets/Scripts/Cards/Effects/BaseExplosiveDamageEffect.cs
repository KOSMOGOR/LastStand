using System.Collections.Generic;

public class BaseExplosiveDamageEffect : CardEffect
{
    public int damage;

    public static void ApplyExplosiveDamage(Tile tile, int dmg) {
        List<BaseZombie> zombies = new(tile.zombies);
        zombies.ForEach(z => {
            if (dmg == 0) return;
            z.TakeDamage(dmg, DamageType.Explosive);
            dmg -= 1;
        });
    }

    public override void Activate() {
        Tile selectedTile = GridManager.I.currentSelectedTile;
        ApplyExplosiveDamage(selectedTile, damage);
    }
}
