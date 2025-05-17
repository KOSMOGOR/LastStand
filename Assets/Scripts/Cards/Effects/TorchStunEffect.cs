using System.Collections.Generic;
using System.Linq;

public class TorchStunEffect : CardEffect
{
    public override void Activate() {
        for (int x = 0; x < GridManager.I.gridWidth; ++x) {
            List<Tile> tilesWithZombies = GridManager.I.GetAllTilesInColumn(x).Where(t => t.zombies.Count > 0).ToList();
            if (tilesWithZombies.Count > 0) tilesWithZombies.First().zombies.ForEach(z => z.stunned = true);
        }
    }
}
