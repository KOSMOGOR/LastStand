using System.Collections.Generic;

public class ShotgunShootingDamageEffect : BaseShootingDamageEffect
{
    public int damageAdjacent;

    public override void Activate() {
        Tile selectedTile = GridManager.I.currentSelectedTile;
        ApplyShootingDamage(selectedTile, damage, secondsToDisplay: secondsToDisplay);
        List<Tile> adjacentTiles = new() {GridManager.I.GetTile(selectedTile.xy + MovementDirection.LEFT), GridManager.I.GetTile(selectedTile.xy + MovementDirection.RIGHT)};
        adjacentTiles.ForEach(tile => {
            if (tile != null) ApplyShootingDamage(tile, damageAdjacent, secondsToDisplay: secondsToDisplay);
        });
        AudioManager.I.PlaySound(SoundType.Shotgun);
    }
}
