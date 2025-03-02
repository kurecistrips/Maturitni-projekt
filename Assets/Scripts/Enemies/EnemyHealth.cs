using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public static EnemyHealth main;
    public int maxHealth;
    private int health;
    public int healthInfo;
    [SerializeField] private int currencyWorth;
    public bool isHidden;

    private void Awake()
    {
        main = this;

    }
    private void Start()
    {
        health = maxHealth;
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
            WaveManagerTest.onEnemyDestroy.Invoke();
            LevelManager.main.IncreaseCurrency(currencyWorth);
            Destroy(gameObject);
        }
    }
    
}
