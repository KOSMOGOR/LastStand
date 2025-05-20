using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseShootingDamageEffect : CardEffect
{
    public int damage;
    public GameObject shootingEffectPrefab;
    public float secondsToDisplay = 0.8f;

    public static void ApplyShootingDamage(Tile tile, int dmg, int startingY = 0, float secondsToDisplay = 0.8f) {
        List<Tile> tiles = GridManager.I.GetAllTilesInColumn(tile.xy.x).Where(t => t.xy.y >= startingY).ToList();
        List<Tile> checkedTiles = new();
        bool wasHit = false;
        foreach (Tile t in tiles) {
            checkedTiles.Add(t);
            BaseObstacle obstacle = t.GetFirstBlockingProjectilesObstacle();
            if (obstacle) { obstacle.TakeDamage(dmg, DamageType.Shooting); wasHit = true; break; }
            BaseZombie zombie = t.GetFirstZombie();
            if (zombie) { zombie.TakeDamage(dmg, DamageType.Shooting); wasHit = true; break; }
        }
        if (checkedTiles.Count == 0 || secondsToDisplay == 0) return;
        GameObject shootingEffectPrefab = Resources.Load<GameObject>("CardAdditional/ShootingDamageEffect");
        List<GameObject> shootingEffects = new();
        for (int i = 0; i < checkedTiles.Count; ++i) {
            shootingEffects.Add(Instantiate(shootingEffectPrefab));
            shootingEffects[i].transform.position = tiles[i].transform.position;
            if (wasHit && i == checkedTiles.Count - 1) shootingEffects[i].GetComponent<Animator>().SetBool("hit", true);
        }
        IEnumerator DestroyEffects() {
            yield return new WaitForSeconds(secondsToDisplay);
            shootingEffects.ForEach(se => Destroy(se));
        }
        GridManager.I.StartCoroutine(DestroyEffects());
    }

    public override void Activate() {
        Tile selectedTile = GridManager.I.currentSelectedTile;
        ApplyShootingDamage(selectedTile, damage, secondsToDisplay: secondsToDisplay);
        AudioManager.I.PlaySound(SoundType.Riffle);
    }
}
