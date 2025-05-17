using UnityEngine;

public class TestEffect : CardEffect
{
    public override void Activate() {
        int n = Random.Range(1, 10);
        bool OnTryMove(BaseZombie zombie, Tile tile) {
            Debug.Log(n);
            n -= 1;
            if (n == 0) Messenger<BaseZombie, Tile>.RemoveListener(EventMessages.ON_ZOMBIE_TRY_MOVE, OnTryMove);
            return true;
        }
        Messenger<BaseZombie, Tile>.AddListener(EventMessages.ON_ZOMBIE_TRY_MOVE, OnTryMove);
    }
}
