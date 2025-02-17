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

    private void OnGUI()
    {
        playerHealthBarText.text = LevelManager.main.baseHealth.ToString() + "/" + LevelManager.main.maxHealth;
    }
    private void Update()
    {
        lerpSpeed = 3f * Time.deltaTime;

        HealthBarFiller();
        ColorChanger();
    }
    
    void HealthBarFiller()
    {
        playerHealtBarGO.fillAmount = Mathf.Lerp(playerHealtBarGO.fillAmount, LevelManager.main.maxHealth, lerpSpeed);
    }
    
    void ColorChanger()
    {
        Color healthColor = Color.Lerp(Color.red, Color.green, (LevelManager.main.baseHealth / LevelManager.main.maxHealth));

        playerHealtBarGO.color = healthColor;
    }

}
