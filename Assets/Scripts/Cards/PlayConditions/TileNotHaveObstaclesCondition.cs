public class TileNotHaveObstaclesCondition : CardPlayCondition
{
    public override bool CanPlay() {
        if (GridManager.I.currentSelectedTile == null) return true;
        return GridManager.I.currentSelectedTile.obstacles.Count == 0;
    }
}