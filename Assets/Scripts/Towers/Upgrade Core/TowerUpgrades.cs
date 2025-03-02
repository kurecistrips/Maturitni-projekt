using System.Collections;
using System.Collections.Generic;
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
    public GameObject newPorjectilePrefab; // Only for projectile towers
    public float newProjectileSpeed; // Only for projectile towers
    public bool hiddenDetection;
}
