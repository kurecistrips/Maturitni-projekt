using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCurrentMoneyUI : MonoBehaviour
{
    public TextMeshProUGUI moneyText;
    public GameObject moneyGO;

    private int money;

    public void Start()
    {
        money = LevelManager.main.currency;
    }

    public void Update(){
        money = LevelManager.main.currency;
        moneyText.text = $"{money}$";
    }

}
