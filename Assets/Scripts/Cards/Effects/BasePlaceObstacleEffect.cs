public class BasePlaceObstacleEffect : CardEffect
{
    public BaseObstacle obstacle;

    public override void Activate() {
        Tile selectedTile = GridManager.I.currentSelectedTile;
        BaseObstacle.SpawnObstacle(obstacle, selectedTile);
        AudioManager.I.PlaySound(SoundType.Summon);
    }
}
