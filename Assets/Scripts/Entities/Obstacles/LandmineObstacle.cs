using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LandmineObstacle : BaseObstacle
{
    public int explosionDamage;

    void OnEnable() {
        Messenger.AddListener(EventMessages.ON_ALL_ZOMBIE_TAKE_TURN, OnAllZombieTakeTurn);
    }

    void OnDisable() {
        Messenger.RemoveListener(EventMessages.ON_ALL_ZOMBIE_TAKE_TURN, OnAllZombieTakeTurn);
    }

    void OnAllZombieTakeTurn() {
        if (tile.zombies.Count > 0) {
            BaseExplosiveDamageEffect.ApplyExplosiveDamage(tile, explosionDamage);
            Die();
        }
    }
}
