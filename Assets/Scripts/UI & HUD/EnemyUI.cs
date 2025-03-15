using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyUI : MonoBehaviour
{
    private EnemyHealth enemyHealth;
    public GameObject enemyCanvas;
    public TextMeshProUGUI healthText, nameText;
    public Image healthBar;

    private float health;
    private float maxHealth;
    private string enemyName;
    private float lerpSpeed;

    private void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        if (EnemyHealth.main == null)
        {
            return;
        }
        enemyCanvas.SetActive(false);
    }

    private void Update()
    {
        if (EnemyHealth.main != null)
        {
            health = enemyHealth.healthInfo;
            maxHealth = enemyHealth.maxHealth;
            enemyName = enemyHealth.enemyName;
        }

        lerpSpeed = 3f * Time.deltaTime;
        healthText.text = $"{health}/{maxHealth}";
        nameText.text = $"{enemyName}";
        HealthBarFiller();
        ColorChange();

    }
    private void OnMouseOver()
    {
        enemyCanvas.SetActive(true);
    }
    public void OnMouseExit()
    {
        enemyCanvas.SetActive(false);
    }

    private void HealthBarFiller()
    {
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, health / maxHealth, lerpSpeed);
    }

    private void ColorChange()
    {
        Color healthColor = Color.Lerp(Color.red, Color.green, (health / maxHealth));

        healthBar.color = healthColor;
    }

    
}
