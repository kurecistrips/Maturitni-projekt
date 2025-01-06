using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HideTowerSelection : MonoBehaviour
{
    public Button uiTowerSlotBtn;
    public GameObject uiTwoerSlotPanel;
    public TextMeshProUGUI btnTxt;
    private bool isON = true;

    void Start()
    {
        uiTowerSlotBtn.onClick.AddListener(PanelOnClick);
        btnTxt.text = "v";
    }
    void PanelOnClick()
    {
        if (isON == true)
        {
            uiTwoerSlotPanel.SetActive(false);
            isON = false;
            btnTxt.text = "ʌ";
        }
        else{
            uiTwoerSlotPanel.SetActive(true);
            isON = true;
            btnTxt.text = "v";

        }
    }



}
