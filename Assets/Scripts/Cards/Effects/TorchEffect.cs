using System.Collections.Generic;
using System.Linq;

public class TorchEffect : CardEffect
{
    public override void Activate() {
        for (int x = 0; x < GridManager.I.gridWidth; ++x) {
            List<Tile> tilesWithZombies = GridManager.I.GetAllTilesInColumn(x).Where(t => t.zombies.Count > 0).ToList();
            if (tilesWithZombies.Count > 0) tilesWithZombies.First().zombies.ForEach(z => z.stunned = true);
        }
        float EvaluateSpeed(BaseZombie zombie) {
            RemoveListeners();
            return -0.25f;
        }
        void RemoveListeners() {
            Messenger<BaseZombie>.RemoveListener<float>(EventMessages.EVALUATE_ZOMBIE_SPEED, EvaluateSpeed);
            Messenger.RemoveListener(EventMessages.ON_PLAYER_END_TURN, RemoveListeners);
        }
        Messenger<BaseZombie>.AddListener<float>(EventMessages.EVALUATE_ZOMBIE_SPEED, EvaluateSpeed);
        Messenger.AddListener(EventMessages.ON_PLAYER_END_TURN, RemoveListeners);
    }
}
