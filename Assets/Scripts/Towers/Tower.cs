using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public static Tower main;
    public string TowerName;
    public int level = 1;
    public int costToPlace;
    public int totalCost = 0;
    public float attackRange;
    public float attackSpeed;
    public int damage;
    public bool hiddenDetection;
    private bool isDisplaying = false;

    public SpriteRenderer baseRenderer;
    public SpriteRenderer turretRendere;
    public Sprite towerImage;
    public TextMeshProUGUI lvlText;
    [SerializeField] private GameObject towerUI;
    [SerializeField] private CircleCollider2D rangeCircle;
    
    public TowerUpgrades[] upgrades;

    void Awake()
    {
        main = this;
    }

    void Start()
    {
        baseRenderer = transform.Find("Base").GetComponent<SpriteRenderer>();
        turretRendere = transform.Find("Rotating Point/Turret").GetComponent<SpriteRenderer>();
        lvlText.text = $"Level: {level}";
        totalCost += costToPlace;
        //rangeCircle.enabled = true;
    }

    public void Upgrade()
    {
        if (level-1 < upgrades.Length)
        {
            TowerUpgrades upgrade = upgrades[level-1];
            if (upgrade.cost <= LevelManager.main.currency){
                level++;
                lvlText.text = $"Level: {level}";
                attackRange = upgrade.newRange;
                attackSpeed = upgrade.newAttackSpeed;
                damage = upgrade.newDamage;
                hiddenDetection = upgrade.hiddenDetection;

                //rangeCircle.radius = attackRange;

                totalCost += upgrade.cost;

                if (upgrade.newBaseSprite) baseRenderer.sprite = upgrade.newBaseSprite;
                if (upgrade.newTurretSprite) turretRendere.sprite = upgrade.newTurretSprite;
                LevelManager.main.SpendCurrency(upgrade.cost);
                Debug.Log($"{level}, {attackRange}, {attackSpeed}, {damage}, {hiddenDetection}");
            }
            else{
                Debug.Log($"Not enough money {upgrade.cost - LevelManager.main.currency}");
            }
            
        }
        
    }

    public void OpenTowerUI()
    {
        if (isDisplaying == false)
        {
            towerUI.SetActive(true);
            isDisplaying = true;
        }
        else
        {
            CloseTowerUI();
        }
    }
    
    public void CloseTowerUI()
    {
        towerUI.SetActive(false);
        isDisplaying = false;
    }
}
