using UnityEngine;

public class SkongerZombie : BaseZombie
{
    public int minPossibleHp = 6;
    public int maxPossibleHp = 20;
    public int minDamage = 1;
    public int maxDamage = 6;
    public float minSpeed = 0.5f;
    public float maxSpeed = 2;
    public float speedInterval = 0.25f;

    new void Start() {
        base.Start();
        hp = Random.Range(minPossibleHp, maxPossibleHp + 1);
        damage = Random.Range(minDamage, maxPossibleHp + 1);
        speed = minSpeed + Random.Range(0, (int)((maxSpeed - minDamage) / speedInterval) + 1) * speedInterval;
    }
}
