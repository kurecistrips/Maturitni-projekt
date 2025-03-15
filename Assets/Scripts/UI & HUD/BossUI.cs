using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    public static BossUI main;
    public GameObject bossUIElement;
    public TextMeshProUGUI bossName;
    public TextMeshProUGUI bossHealth;
    public Image bossHealthBarGO;
    private float lerpSpeed;
    [SerializeField] private EnemyHealth boss;

    private float baseHealth;
    private float maxHealth;

    private void Awake()
    {
        main = this;
    }
    public void SetOn()
    {
        bossUIElement.SetActive(true);
    }
    public void SetBoss(EnemyHealth bossRef)
    {
        boss = bossRef;
    }
    private void Start()
    {
        bossUIElement.SetActive(false);
    }
    
    private void Update()
    {
        if (boss == null) return;
        baseHealth = boss.healthInfo;
        maxHealth = boss.maxHealth;

        lerpSpeed = 3f * Time.deltaTime;
        bossHealth.text = $"{baseHealth}/{maxHealth}";
        bossName.text = $"{boss.enemyName}";


        HealthBarFiller();
        ColorChanger();
    }

    private void HealthBarFiller()
    {
        bossHealthBarGO.fillAmount = Mathf.Lerp(bossHealthBarGO.fillAmount, baseHealth / maxHealth, lerpSpeed);
    }
    private void ColorChanger()
    {
        Color healthColor = Color.Lerp(Color.red, Color.green, (baseHealth / maxHealth));

        bossHealthBarGO.color = healthColor;
    }

}
