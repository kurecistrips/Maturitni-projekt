using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public static Tower main;
    public string TowerName;
    public int level = 1;
    public int costToPlace;
    public float attackRange;
    public float attackSpeed;
    public int damage;
    public bool hiddenDetection;

    public SpriteRenderer baseRenderer;
    public SpriteRenderer turretRendere;
    public Sprite towerImage;
    
    public TowerUpgrades[] upgrades;

    void Awake()
    {
        main = this;
    }

    void Start()
    {
        baseRenderer = transform.Find("Base").GetComponent<SpriteRenderer>();
        turretRendere = transform.Find("Rotating Point/Turret").GetComponent<SpriteRenderer>();
    }

    public void Upgrade()
    {
        if (level-1 < upgrades.Length)
        {
            TowerUpgrades upgrade = upgrades[level-1];
            level++;
            attackRange = upgrade.newRange;
            attackSpeed = upgrade.newAttackSpeed;
            damage = upgrade.newDamage;
            hiddenDetection = upgrade.hiddenDetection;

            if (upgrade.newBaseSprite) baseRenderer.sprite = upgrade.newBaseSprite;
            if (upgrade.newTurretSprite) turretRendere.sprite = upgrade.newTurretSprite;
            Debug.Log($"{level}, {attackRange}, {attackSpeed}, {damage}, {hiddenDetection}");
        }
        
    }
}
