public class TileNotHaveZombiesCondition : CardPlayCondition
{
    public override bool CanPlay() {
        if (GridManager.I.currentSelectedTile == null) return true;
        return GridManager.I.currentSelectedTile.zombies.Count == 0;
    }
}