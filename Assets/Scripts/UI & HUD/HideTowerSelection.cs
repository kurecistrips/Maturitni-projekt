using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HideTowerSelection : MonoBehaviour
{
    public Button uiTowerSlotBtn;
    public GameObject uiTwoerSlotPanel;
    public TextMeshProUGUI btnTxt;
    private bool isON = true;

    private void Start()
    {
        uiTowerSlotBtn.onClick.AddListener(PanelOnClick);
        btnTxt.text = "v";
    }
    private void PanelOnClick()
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
