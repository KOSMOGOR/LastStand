using System.Collections.Generic;
using System.Linq;

public class RocketLauncherShootEffect : CardEffect
{
    public int shootingDamage;
    public int explosionDamage;

    public override void Activate() {
        Tile selectedTile = GridManager.I.currentSelectedTile;
        List<Tile> tiles = GridManager.I.GetAllTilesInColumn(selectedTile.xy.x);
        BaseObstacle foundObstacle = null;
        BaseZombie foundZombie = null;
        Tile centerExplosion = null;
        foreach (Tile t in tiles) {
            BaseObstacle obstacle = t.GetFirstBlockingProjectilesObstacle();
            if (obstacle) { foundObstacle = obstacle; break; }
            BaseZombie zombie = t.GetFirstZombie();
            if (zombie) { foundZombie = zombie; break; }
        }
        BaseShootingDamageEffect.ApplyShootingDamage(selectedTile, shootingDamage);
        AudioManager.I.PlaySound(SoundType.GrenadeLauncher);
        if (foundObstacle != null) centerExplosion = foundObstacle.tile;
        else if (foundZombie != null) centerExplosion = foundZombie.tile;
        if (centerExplosion == null) return;
        List<Tile> adjacentTiles = new List<Tile>{
            GridManager.I.GetTile(centerExplosion.xy + MovementDirection.UP),
            GridManager.I.GetTile(centerExplosion.xy + MovementDirection.DOWN),
            GridManager.I.GetTile(centerExplosion.xy + MovementDirection.LEFT),
            GridManager.I.GetTile(centerExplosion.xy + MovementDirection.RIGHT)
        };
        BaseExplosiveDamageEffect.ApplyExplosiveDamage(centerExplosion, explosionDamage);
        adjacentTiles.ForEach(t => { if (t) BaseExplosiveDamageEffect.ApplyExplosiveDamage(t, explosionDamage); });
    }
}
