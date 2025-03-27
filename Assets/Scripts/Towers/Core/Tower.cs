using TMPro;
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
    public bool stunResistance;
    private bool isDisplaying = false;

    public bool isStunned = false;
    private float stunDuration;
    public bool isPlaced = false;

    public SpriteRenderer baseRenderer;
    public SpriteRenderer turretRendere;
    public Sprite towerImage;
    public TextMeshProUGUI lvlText;
    [SerializeField] private GameObject towerUI;
    public TextMeshProUGUI targetingText;
    public TextMeshProUGUI upgradeText;
    public TextMeshProUGUI sellText;
    
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

    public void Update()
    {
        if (stunResistance == true && isStunned == true)
        {
            isStunned = false;
            stunDuration = 0;
        }

        if (stunDuration > 0)
        {
            stunDuration -= Time.deltaTime;
        }
        else if (stunDuration <= 0)
        {
            stunDuration = 0;
            isStunned = false;
        }
        sellText.text = $"Sell: {totalCost/2}$";
        
        if (level-1 < upgrades.Length) 
        {
            TowerUpgrades upgrade = upgrades[level-1];
            upgradeText.text = $"Upgrade: {upgrade.cost}$";
        }
        else
        {
            upgradeText.text = $"Max lvl";
        }
        

    }

    public bool Placed(bool placed)
    {
        isPlaced = placed;
        return isPlaced;
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
                stunResistance = upgrade.stunResistance;
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
                WarningPopUp.main.PopUpWarning(upgrade.cost - LevelManager.main.currency);
            }
            
        }

    }

    public void Stun(float stun)
    {
        stunDuration = stun; 
        isStunned = true;
    }

    public void Sell()
    {
        LevelManager.main.IncreaseCurrency(totalCost/2);

        Destroy(gameObject);
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
