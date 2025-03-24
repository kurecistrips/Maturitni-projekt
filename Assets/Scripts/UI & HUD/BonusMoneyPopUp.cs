using System.Collections;
using UnityEngine;
using TMPro;

public class BonusMoneyPopUp : MonoBehaviour
{
    public static BonusMoneyPopUp main;
    public GameObject bonusPopGO;
    public TextMeshProUGUI bonusText;
    //public bool popedUp = false;
    [SerializeField] private float hideAfter;


    private void Awake()
    {
        main = this;
    }

    private void Start()
    {
        bonusPopGO.SetActive(false);
    }

    public IEnumerator hide()
    {
        yield return new WaitForSeconds(hideAfter);
        bonusPopGO.SetActive(false);
    }
    public void MoneyPerWave(int amount)
    {
        bonusPopGO.SetActive(true);
        bonusText.text = $"Wave Bonus +{amount}$";
        StartCoroutine(hide());
    }
}
