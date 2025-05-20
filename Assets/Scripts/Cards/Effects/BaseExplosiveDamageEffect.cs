using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseExplosiveDamageEffect : CardEffect
{
    public int damage;
    public float secondsToDisplay = 1;

    public static void ApplyExplosiveDamage(Tile tile, int dmg, float secondsToDisplay = 1) {
        List<BaseZombie> zombies = new(tile.zombies);
        zombies.ForEach(z => {
            if (dmg == 0) return;
            z.TakeDamage(dmg, DamageType.Explosive);
            dmg -= 1;
        });
        if (secondsToDisplay == 0) return;
        GameObject explotionEffectPrefab = Resources.Load<GameObject>("CardAdditional/ExplosionEffect");
        GameObject explotionEffect = Instantiate(explotionEffectPrefab, tile.transform);
        IEnumerator DestroyEffect() {
            yield return new WaitForSeconds(secondsToDisplay);
            Destroy(explotionEffect);
        }
        GridManager.I.StartCoroutine(DestroyEffect());
        AudioManager.I.PlaySound(SoundType.Explosion);
    }

    public override void Activate() {
        Tile selectedTile = GridManager.I.currentSelectedTile;
        ApplyExplosiveDamage(selectedTile, damage);
    }
}
