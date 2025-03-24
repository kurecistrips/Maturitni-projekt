using TMPro;
using UnityEngine;

public class WarningPopUp : MonoBehaviour
{
    public static WarningPopUp main;
    public GameObject warningGO;
    public TextMeshProUGUI warningText;
    private bool clicked = false;
    private float timeSinceClick = 1f;
    private float counter;

    private void Awake()
    {
        main = this;
    }
    private void Start()
    {
        warningGO.SetActive(false);
    }
    private void Update()
    {
        if (clicked == true || (counter < timeSinceClick && counter > 0))
        {
            counter -= Time.deltaTime;
            clicked = false;
        }
        else if (counter <= 0 && clicked == false)
        {
            warningGO.SetActive(false);
            counter += 0;
        }
    }
    public void PopUpWarning(int amount){
        clicked = true;
        counter = timeSinceClick;
        warningGO.SetActive(true);
        warningText.text = $"Not enough money: {amount}$ needed";
        
    }

    

}
