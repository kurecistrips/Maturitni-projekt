using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    public GameObject currencyUIElement;
    public TextMeshProUGUI textUI;

    private void Update()
    {
        textUI.text = $"{CurrencyManager.main.currency}$";
    }
}
