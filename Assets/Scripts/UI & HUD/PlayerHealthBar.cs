using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public TextMeshProUGUI playerHealthBarText;
    public Image playerHealtBarGO;
    private float lerpSpeed;

    private float baseHealth;
    private float maxHealth;

    private void OnGUI()
    {
        playerHealthBarText.text = baseHealth.ToString() + "/" + maxHealth;
    }
    private void Update()
    {
        baseHealth = LevelManager.main.baseHealth;
        maxHealth = LevelManager.main.maxHealth;
        
        lerpSpeed = 3f * Time.deltaTime;

        HealthBarFiller();
        ColorChanger();
    }
    
    void HealthBarFiller()
    {
        playerHealtBarGO.fillAmount = Mathf.Lerp(playerHealtBarGO.fillAmount, baseHealth / maxHealth, lerpSpeed);
    }
    
    void ColorChanger()
    {
        Color healthColor = Color.Lerp(Color.red, Color.green, (baseHealth / maxHealth));

        playerHealtBarGO.color = healthColor;
    }

}
