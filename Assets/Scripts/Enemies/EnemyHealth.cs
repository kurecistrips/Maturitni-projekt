using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public static EnemyHealth main;
    private EnemyMovement enemyMovement;
    private SpriteRenderer spriteRenderer;
    public int maxHealth;
    private int health;
    public int healthInfo;
    public string enemyName;
    [SerializeField] private int currencyWorth;
    [SerializeField] private float hiddenAlpha = 0.5f;
    public bool isHidden;
    public bool isBoss;

    private void Awake()
    {
        main = this;
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyMovement = GetComponent<EnemyMovement>();
    }
    private void Start()
    {
        health = maxHealth;   
        
        if (isBoss == true)
        {
            BossUI.main.SetOn();
            BossUI.main.SetBoss(this);
        }
        if (isHidden == true)
        {
            Color newColor = spriteRenderer.color;
            newColor.a = hiddenAlpha;
            spriteRenderer.color = newColor;
            enemyName = $"Hidden {enemyName}";
        }
    }
    private void Update()
    {
        healthInfo = health;
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;

        if (health <= 0)
        {
            if (enemyMovement.fromLastWave == false)
            {
                WaveManager.onEnemyDestroy.Invoke();
            }
            LevelManager.main.IncreaseCurrency(currencyWorth);
            Destroy(gameObject);
        }
    }
    
}
