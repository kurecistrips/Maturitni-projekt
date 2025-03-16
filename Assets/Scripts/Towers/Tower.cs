using TMPro;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
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
    public float explosionRadius; //projectile towers only
    public float projectileSpeed; //projectile towers only
    public Sprite projectilePrefab; //projectile towers only
    public int moneyPerWave;
    public int timesToShoot;
    public bool hiddenDetection;
    private bool isDisplaying = false;

    public SpriteRenderer baseRenderer;
    public SpriteRenderer turretRendere;
    public Sprite towerImage;
    public TextMeshProUGUI lvlText;
    [SerializeField] private GameObject towerUI;
    
    public TowerUpgrades[] upgrades;

    private void Awake()
    {
        main = this;
    }

    private void Start()
    {
        lvlText.text = $"Level: {level}";
        totalCost += costToPlace;
        baseRenderer = transform.Find("Base").GetComponent<SpriteRenderer>();
        if (turretRendere == null)
        {
            return;
        }
        else
        {
            turretRendere = transform.Find("Rotating Point/Turret").GetComponent<SpriteRenderer>();
        }
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
                moneyPerWave = upgrade.moneyPerWave;
                timesToShoot = upgrade.timesToShoot;

                totalCost += upgrade.cost;

                if (upgrade.newBaseSprite) baseRenderer.sprite = upgrade.newBaseSprite;
                if (upgrade.newTurretSprite) turretRendere.sprite = upgrade.newTurretSprite;
                
                if (GetComponent<ProjectileTower>() != null)
                {
                    if (upgrade.newPorjectilePrefab != null) projectilePrefab = upgrade.newPorjectilePrefab; 
                    projectileSpeed = upgrade.newProjectileSpeed;
                    explosionRadius = upgrade.newExplosionRadius;
                }
                
                LevelManager.main.SpendCurrency(upgrade.cost);
                Debug.Log($"{level}, {attackRange}, {attackSpeed}, {damage}, {hiddenDetection}");
            }
            else{
                Debug.Log($"Not enough money {upgrade.cost - LevelManager.main.currency}");
                WarningPopUp.main.PopUp(upgrade.cost);
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
