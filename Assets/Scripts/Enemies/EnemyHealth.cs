using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public static EnemyHealth main;
    private EnemyMovement enemyMovement;
    public int maxHealth;
    private int health;
    public int healthInfo;
    public string enemyName;
    [SerializeField] private int currencyWorth;
    public bool isHidden;
    public bool isBoss;

    private void Awake()
    {
        main = this;

    }
    private void Start()
    {
        health = maxHealth;   
        enemyMovement = GetComponent<EnemyMovement>();
        if (isBoss == true)
        {
            BossUI.main.SetOn();
            BossUI.main.SetBoss(this);
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
                WaveManagerTest.onEnemyDestroy.Invoke();
            }
            LevelManager.main.IncreaseCurrency(currencyWorth);
            Destroy(gameObject);
        }
    }
    
}
