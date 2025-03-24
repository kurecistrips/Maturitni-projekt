using UnityEngine;


[System.Serializable]
public class TowerUpgrades
{
    public int level;
    public int cost;
    public float newRange;
    public float newAttackSpeed;
    public int newDamage;
    public Sprite newBaseSprite;
    public Sprite newTurretSprite;
    public Sprite newPorjectilePrefab; // Only for projectile towers
    public float newProjectileSpeed; // Only for projectile towers
    public int moneyPerWave; // For Farm tower but can be used for towers also
    public int timesToShoot; // Only for burst tower
    public float newExplosionRadius;
    public bool hiddenDetection;
    public bool stunResistance;
}
