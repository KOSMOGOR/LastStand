public class BasePlaceObstacleEffect : CardEffect
{
    public BaseObstacle obstacle;

    public override void Activate() {
        Tile selectedTile = GridManager.I.currentSelectedTile;
        if (selectedTile.zombies.Count > 0 || selectedTile.obstacles.Count > 0) return;
        BaseObstacle.SpawnObstacle(obstacle, selectedTile);
        AudioManager.I.PlaySound(SoundType.Summon);
    }
}
